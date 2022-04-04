using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float movementSpeed = 8;
    public float rotationSpeed = 160f;
    public float scopingSensitivity = 160f;
    [SerializeField] float cameraRotationLimit = 85f;
    private float currentCameraRotationX = 0f;
    [SerializeField] GameObject cam;
    [SerializeField] float thrusterFuelBurnSpeed = 1.1f;
    [SerializeField] float thrusterFuelRegenSpeed = 0.2f;
    private float thrusterFuelAmount = 1f;
    [SerializeField] LayerMask environmentMask;
    [Header("Spring Configure:")]
    [SerializeField] float thrusterForce = 4000f;
    [SerializeField] float jointSpring = 10f;
    [SerializeField] float jointMaxForce = 20f;
    [SerializeField] float jointDamper = 5f;
    private int countEnterSpace;
    private bool isFirstEnterSpace;

    private Rigidbody rb;
    private ConfigurableJoint joint;
    private Animator animator;
    private PhotonView PV;
    private PlayerSetup playerSetup;
    private PlayerShoot playershoot;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        joint = GetComponent<ConfigurableJoint>();
        PV = GetComponent<PhotonView>();
        playerSetup = GetComponent<PlayerSetup>();
        playershoot = GetComponent<PlayerShoot>();
        SetJointSettings(jointSpring, jointDamper);
        countEnterSpace = 0;
        isFirstEnterSpace = true;
    }

    public float GetThrusterFuelAmount()
    {
        return thrusterFuelAmount;
    }

    void FixedUpdate()
    {
        // setting target position for spring
        // apply gravity when flying over objects
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.down, out hit, 100f, environmentMask))
        {
            joint.targetPosition = new Vector3(0f, -hit.point.y, 0f);
        }
        else
        {
            joint.targetPosition = Vector3.zero;
        }
        // if pause menu active
        if(playerSetup.playerUI != null)
        {
            if (playerSetup.playerUI.pauseMenu.activeSelf == true)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                return;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
        // move object throught keyboard shortcut
        BasicMovement();
        // rotate object throught position mouse
        BasicRotation();
        if(Input.GetButton("Jump") && thrusterFuelAmount > 0)
        {
            thrusterFuelAmount -= thrusterFuelBurnSpeed * Time.fixedDeltaTime;
            if(thrusterFuelAmount >= 0.01f)
            {
                if (isFirstEnterSpace)
                {
                    countEnterSpace = 0;
                }
                isFirstEnterSpace = false;
                rb.AddForce(new Vector3(0f, thrusterForce * Time.fixedDeltaTime, 0f), ForceMode.Acceleration);
                countEnterSpace++;
                SetJointSettings(0, jointDamper);
            }
        }
        else
        {
            thrusterFuelAmount += thrusterFuelRegenSpeed * Time.fixedDeltaTime;
            if(countEnterSpace <= 17)
            {
                SetJointSettings(jointSpring, 2);
            }
            else
            {
                SetJointSettings(jointSpring, jointDamper);
            }
            isFirstEnterSpace = true;
        }
        thrusterFuelAmount = Mathf.Clamp(thrusterFuelAmount, 0f, 1f);
    }

    void BasicMovement()
    {
        float _xMov = Input.GetAxis("Horizontal");
        float _zMov = Input.GetAxis("Vertical");
        if(_zMov > 0)
        {
            if(_xMov > 0)
            {
                animator.SetInteger("Vertical", 1);
                animator.SetInteger("Horizontal", 1);
            }
            else if(_xMov == 0)
            {
                animator.SetInteger("Vertical", 1);
                animator.SetInteger("Horizontal", 0);
            }
            else
            {
                animator.SetInteger("Vertical", 1);
                animator.SetInteger("Horizontal", -1);
            }
        }
        else if(_zMov == 0)
        {
            if (_xMov > 0)
            {
                animator.SetInteger("Vertical", 0);
                animator.SetInteger("Horizontal", 1);
            }
            else if (_xMov == 0)
            {
                animator.SetInteger("Vertical", 0);
                animator.SetInteger("Horizontal", 0);
            }
            else
            {
                animator.SetInteger("Vertical", 0);
                animator.SetInteger("Horizontal", -1);
            }
        }
        else
        {
            if (_xMov > 0)
            {
                animator.SetInteger("Vertical", -1);
                animator.SetInteger("Horizontal", 1);
            }
            else if (_xMov == 0)
            {
                animator.SetInteger("Vertical", -1);
                animator.SetInteger("Horizontal", 0);
            }
            else
            {
                animator.SetInteger("Vertical", -1);
                animator.SetInteger("Horizontal", -1);
            }
        }
        Vector3 movHorizontal = transform.right * _xMov;
        Vector3 movVertical = transform.forward * _zMov;
        Vector3 velocity = (movHorizontal + movVertical) * movementSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + velocity);
    }

    void BasicRotation()
    {
        float yRot = 0f;
        float xRot = 0f;
        if (playershoot.currentWeapon.sniper && playershoot.isScoped)
        {
            yRot = Input.GetAxis("Mouse X") * scopingSensitivity * Time.fixedDeltaTime;
            xRot = Input.GetAxis("Mouse Y") * scopingSensitivity * Time.fixedDeltaTime;
        }
        else
        {
            yRot = Input.GetAxis("Mouse X") * rotationSpeed * Time.fixedDeltaTime;
            xRot = Input.GetAxis("Mouse Y") * rotationSpeed * Time.fixedDeltaTime;
        }

        rb.MoveRotation(rb.rotation * Quaternion.Euler(new Vector3(0f, yRot, 0f)));
        if(cam != null)
        {
            currentCameraRotationX -= xRot;
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);
            animator.SetFloat("RotationX", (-currentCameraRotationX / cameraRotationLimit));
            if((-currentCameraRotationX / cameraRotationLimit) < -0.1f)
            {
                PV.RPC("RPC_SetLayerWeight", RpcTarget.Others, 0.4f);
            }
            else
            {
                PV.RPC("RPC_SetLayerWeight", RpcTarget.Others, 1f);
            }
            cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
        }
    }

    [PunRPC]
    void RPC_SetLayerWeight(float value)
    {
        animator.SetLayerWeight(animator.GetLayerIndex("Arm Layer"), value);
    }

    void SetJointSettings(float _jointSpring, float _jointDamper)
    {
        joint.yDrive = new JointDrive
        {
            positionSpring = _jointSpring,
            positionDamper = _jointDamper,
            maximumForce = jointMaxForce
        };
    }

    void OnDisable()
    {
        animator.SetInteger("Vertical", 0);
        animator.SetInteger("Horizontal", 0);
    }
}
