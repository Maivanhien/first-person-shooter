using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerShoot : MonoBehaviour
{
    public Camera cam;
    public Camera weaponCamera;
    [SerializeField] GameObject modelPlayer;
    [SerializeField] LayerMask mask;
    [HideInInspector] public bool isScoped = false;

    [HideInInspector] public PlayerWeapon currentWeapon;
    private bool throwable = true;
    private PhotonView PV;
    private Animator animator;
    private WeaponManager weaponManager;
    private PlayerSetup playerSetup;

    void Start()
    {
        PV = GetComponent<PhotonView>();
        animator = GetComponentInChildren<Animator>();
        weaponManager = GetComponent<WeaponManager>();
        playerSetup = GetComponent<PlayerSetup>();
        if(cam == null)
        {
            Debug.LogError("No camera referenced!");
            this.enabled = false;
        }
        if(PV.IsMine)
        {
            modelPlayer.layer = LayerMask.NameToLayer("Weapon");
        }
    }

    void Update()
    {

        if(playerSetup.playerUI != null)
        {
            if (playerSetup.playerUI.pauseMenu.activeSelf == true)
            {
                return;
            }
        }
        currentWeapon = weaponManager.GetCurrentWeapon();
        if(PV.IsMine)
        {
            if (currentWeapon == null)
                return;
            if(weaponManager.selectedWeapon == 2)
            {
                if(throwable == true)
                {
                    if (Input.GetButtonDown("Fire1"))
                    {
                        PV.RPC("RPC_ThrowGrenade", RpcTarget.All);
                    }
                }
                else
                {
                    if(weaponManager.fakeGrenade.activeSelf == true)
                    {
                        weaponManager.fakeGrenade.SetActive(false);
                    }
                }
            }
            else
            {
                if (currentWeapon.bullets <= 0 && weaponManager.isReloading == false)
                {
                    weaponManager.Reload();
                    return;
                }
                if (currentWeapon.bullets < currentWeapon.maxBullets)
                {
                    if (Input.GetKeyDown(KeyCode.R))
                    {
                        weaponManager.Reload();
                    }
                }
                if (Input.GetButtonDown("Fire2"))
                {
                    isScoped = !isScoped;
                    if (isScoped == true)
                    {
                        StartCoroutine(ScopedCoroutine());
                    }
                    else
                    {
                        StartCoroutine(DontScopedCoroutine());
                    }
                }
                if (currentWeapon.fireRate <= 0f)
                {
                    if (Input.GetButtonDown("Fire1"))
                    {
                        Shoot();
                    }
                }
                else
                {
                    if (Input.GetButtonDown("Fire1"))
                    {
                        InvokeRepeating("Shoot", 0f, 1f / currentWeapon.fireRate);
                    }
                    else if (Input.GetButtonUp("Fire1"))
                    {
                        CancelInvoke("Shoot");
                    }
                }
            }
        }
    }

    void Shoot()
    {
        if (weaponManager.isReloading)
            return;
        if(currentWeapon.bullets <= 0)
        {
            weaponManager.Reload();
            return;
        }
        currentWeapon.bullets--;
        Debug.Log("Shoot");
        PV.RPC("RPC_DoShootEffect", RpcTarget.All);
        if(currentWeapon.fireRate <= 0f)
        {
            StartCoroutine(cam.GetComponent<CameraShake>().Shake(1/10, 0.1f));
            StartCoroutine(weaponCamera.GetComponent<CameraShake>().Shake(1/10, 0.05f));
        }
        else
        {
            StartCoroutine(cam.GetComponent<CameraShake>().Shake(1 / currentWeapon.fireRate, 0.05f));
            StartCoroutine(weaponCamera.GetComponent<CameraShake>().Shake(1 / currentWeapon.fireRate, 0.01f));
        }
        RaycastHit hit;
        // we hit something
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, currentWeapon.range, mask))
        {
            if(hit.collider.gameObject != null)
            {
                PV.RPC("RPC_DoHitEffect", RpcTarget.All, hit.point, hit.normal);
                PV.RPC("RPC_BulletTrail", RpcTarget.All, hit.point);
            }
            if(hit.collider.tag == "Player")
            {
                PV.RPC("RPC_Shoot", RpcTarget.All, hit.collider.gameObject.GetPhotonView().ViewID);
            }
        }
        if (currentWeapon.bullets <= 0)
        {
            weaponManager.Reload();
        }
    }

    [PunRPC]
    void RPC_DoShootEffect()
    {
        weaponManager.GetCurrentgraphics().muzzleFlash.Play();
    }

    [PunRPC]
    void RPC_DoHitEffect(Vector3 pos, Vector3 normal)
    {
        GameObject hitEffect = Instantiate(weaponManager.GetCurrentgraphics().hitEffect, pos, Quaternion.LookRotation(normal));
        Destroy(hitEffect, 2f);
    }

    [PunRPC]
    void RPC_BulletTrail(Vector3 hitpoint)
    {
        GameObject bulletTrailIns = Instantiate(weaponManager.GetCurrentgraphics().bulletTrail);
        bulletTrailIns.GetComponent<LineFade>().SetPoint(weaponManager.GetCurrentgraphics().shootPoint.position, hitpoint);
        Destroy(bulletTrailIns, 1f);
    }

    [PunRPC]
    void RPC_Shoot(int hitViewId)
    {
        GameObject hit = PhotonView.Find(hitViewId).gameObject;
        hit.GetComponent<PlayerSetup>().SetCurrentPlayerHealth(currentWeapon.damage, playerSetup);
    }

    [PunRPC]
    void RPC_ThrowGrenade()
    {
        throwable = false;
        weaponManager.fakeGrenade.SetActive(false);
        animator.SetTrigger("ThrowGrenade");
        GameObject grenadeIns = Instantiate(weaponManager.grenadePrefab, (cam.transform.position + cam.transform.forward*4), cam.transform.rotation);
        grenadeIns.GetComponent<Grenade>().ThrowGrenade(cam.transform.forward);
        StartCoroutine(ThrowableCoroutine());
    }

    IEnumerator ThrowableCoroutine()
    {
        yield return new WaitForSeconds(4f);
        throwable = true;
        if(weaponManager.selectedWeapon == 2)
            weaponManager.fakeGrenade.SetActive(true);
    }

    IEnumerator ScopedCoroutine()
    {
        Vector3 targetPos = new Vector3(0f, 0.781f, 0.33f);
        Vector3 targetRot = new Vector3(-21.321f, -53.196f, 25.949f);
        Vector3 originPos = weaponCamera.transform.localPosition;
        float t = 0;
        while(weaponCamera.transform.localPosition != targetPos)
        {
            t += Time.deltaTime * 6;
            t = Mathf.Clamp01(t);
            weaponCamera.transform.localPosition = Vector3.Lerp(originPos, targetPos, t);
            yield return null;
        }
        weaponCamera.transform.localEulerAngles = targetRot;
        weaponCamera.GetComponent<CameraShake>().originalPos = targetPos;
        if(currentWeapon.sniper)
        {
            weaponCamera.gameObject.SetActive(false);
            cam.fieldOfView = 16f;
            playerSetup.playerUI.crosshair.SetActive(false);
            playerSetup.playerUI.scoped.SetActive(true);
        }
    }

    IEnumerator DontScopedCoroutine()
    {
        if (currentWeapon.sniper)
        {
            weaponCamera.gameObject.SetActive(true);
            cam.fieldOfView = 65f;
            playerSetup.playerUI.crosshair.SetActive(true);
            playerSetup.playerUI.scoped.SetActive(false);
        }
        Vector3 targetPos = new Vector3(0.002f, 0.73f, 0.079f);
        Vector3 targetRot = new Vector3(-15.979f, -56.465f, 22.974f);
        Vector3 originPos = weaponCamera.transform.localPosition;
        float t = 0;
        while (weaponCamera.transform.localPosition != targetPos)
        {
            t += Time.deltaTime * 6;
            t = Mathf.Clamp01(t);
            weaponCamera.transform.localPosition = Vector3.Lerp(originPos, targetPos, t);
            yield return null;
        }
        weaponCamera.transform.localEulerAngles = targetRot;
        weaponCamera.GetComponent<CameraShake>().originalPos = targetPos;
    }
}
