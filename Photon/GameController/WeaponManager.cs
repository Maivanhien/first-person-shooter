using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WeaponManager : MonoBehaviour
{
    public PlayerWeapon[] primaryWeapon;
    [SerializeField] GameObject fakeGrenadePrefab;
    public GameObject fakeGrenade;
    public GameObject grenadePrefab;
    private WeaponGraphics[] weaponGraphics;
    [SerializeField] Transform weaponHolder;
    public bool isReloading = false;
    public int selectedWeapon = 0;

    private Animator animator;
    private PlayerWeapon currentWeapon;
    private WeaponGraphics currentGraphics;
    private PlayerSetup playerSetup;
    private PlayerShoot playerShoot;
    private PhotonView PV;

    void Start()
    {
        PV = GetComponent<PhotonView>();
        animator = GetComponentInChildren<Animator>();
        playerSetup = GetComponent<PlayerSetup>();
        playerShoot = GetComponent<PlayerShoot>();
        weaponGraphics = new WeaponGraphics[primaryWeapon.Length];
        int count = 0;
        foreach( PlayerWeapon weapon in primaryWeapon)
        {
            GameObject weaponIns = Instantiate(weapon.graphics, weaponHolder);
            weaponGraphics[count] = weaponIns.GetComponent<WeaponGraphics>();
            weapon.SetBullets();
            if (PV.IsMine)
            {
                SetLayerRecursively(weaponIns, LayerMask.NameToLayer("Weapon"));
            }
            count++;
        }
        fakeGrenade = Instantiate(fakeGrenadePrefab, weaponHolder);
        if(PV.IsMine)
        {
            fakeGrenade.layer = LayerMask.NameToLayer("Weapon");
        }
        selectedWeapon = 0;
        if (PV.IsMine)
            PV.RPC("RPC_EquipWeapon", RpcTarget.All, selectedWeapon);
    }

    public PlayerWeapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public WeaponGraphics GetCurrentgraphics()
    {
        return currentGraphics;
    }

    void Update()
    {
        if(playerSetup.playerUI != null)
        {
            if(playerSetup.playerUI.pauseMenu.activeSelf == true)
            {
                return;
            }
        }
        if(PV.IsMine)
        {
            if(Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                if (selectedWeapon >= primaryWeapon.Length)
                    selectedWeapon = 0;
                else selectedWeapon++;
                isReloading = false;
                if(selectedWeapon == 2)
                {
                    animator.SetBool("IsGrenade", true);
                    if (playerShoot.isScoped == true)
                    {
                        playerShoot.isScoped = false;
                        if (currentWeapon.sniper)
                        {
                            playerShoot.weaponCamera.gameObject.SetActive(true);
                            playerShoot.cam.fieldOfView = 65f;
                            playerSetup.playerUI.crosshair.SetActive(true);
                            playerSetup.playerUI.scoped.SetActive(false);
                        }
                    }
                    playerShoot.weaponCamera.nearClipPlane = 0.3f;
                    playerShoot.weaponCamera.transform.localPosition = new Vector3(0.061f, 0.771f, 0.126f);
                    playerShoot.weaponCamera.transform.localEulerAngles = new Vector3(10.649f, -13.616f, 7.065001f);
                }
                else
                {
                    animator.SetBool("IsGrenade", false);
                    if (playerShoot.isScoped == true)
                    {
                        playerShoot.isScoped = false;
                        if (currentWeapon.sniper)
                        {
                            playerShoot.weaponCamera.gameObject.SetActive(true);
                            playerShoot.cam.fieldOfView = 65f;
                            playerSetup.playerUI.crosshair.SetActive(true);
                            playerSetup.playerUI.scoped.SetActive(false);
                        }
                    }
                    playerShoot.weaponCamera.nearClipPlane = 0.17f;
                    playerShoot.weaponCamera.transform.localPosition = new Vector3(0.002f, 0.73f, 0.079f);
                    playerShoot.weaponCamera.transform.localEulerAngles = new Vector3(-15.979f, -56.465f, 22.974f);
                    playerShoot.weaponCamera.GetComponent<CameraShake>().originalPos = new Vector3(0.002f, 0.73f, 0.079f);
                }
                PV.RPC("RPC_EquipWeapon", RpcTarget.All, selectedWeapon);
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                if (selectedWeapon <= 0)
                    selectedWeapon = primaryWeapon.Length;
                else selectedWeapon--;
                isReloading = false;
                if (selectedWeapon == 2)
                {
                    animator.SetBool("IsGrenade", true);
                    if (playerShoot.isScoped == true)
                    {
                        playerShoot.isScoped = false;
                        if (currentWeapon.sniper)
                        {
                            playerShoot.weaponCamera.gameObject.SetActive(true);
                            playerShoot.cam.fieldOfView = 65f;
                            playerSetup.playerUI.crosshair.SetActive(true);
                            playerSetup.playerUI.scoped.SetActive(false);
                        }
                    }
                    playerShoot.weaponCamera.nearClipPlane = 0.3f;
                    playerShoot.weaponCamera.transform.localPosition = new Vector3(0.061f, 0.771f, 0.126f);
                    playerShoot.weaponCamera.transform.localEulerAngles = new Vector3(10.649f, -13.616f, 7.065001f);
                }
                else
                {
                    animator.SetBool("IsGrenade", false);
                    if (playerShoot.isScoped == true)
                    {
                        playerShoot.isScoped = false;
                        if (currentWeapon.sniper)
                        {
                            playerShoot.weaponCamera.gameObject.SetActive(true);
                            playerShoot.cam.fieldOfView = 65f;
                            playerSetup.playerUI.crosshair.SetActive(true);
                            playerSetup.playerUI.scoped.SetActive(false);
                        }
                    }
                    playerShoot.weaponCamera.nearClipPlane = 0.17f;
                    playerShoot.weaponCamera.transform.localPosition = new Vector3(0.002f, 0.73f, 0.079f);
                    playerShoot.weaponCamera.transform.localEulerAngles = new Vector3(-15.979f, -56.465f, 22.974f);
                    playerShoot.weaponCamera.GetComponent<CameraShake>().originalPos = new Vector3(0.002f, 0.73f, 0.079f);
                }
                PV.RPC("RPC_EquipWeapon", RpcTarget.All, selectedWeapon);
            }
            if(Input.GetKeyDown(KeyCode.Alpha1) && selectedWeapon != 0)
            {
                selectedWeapon = 0;
                isReloading = false;
                animator.SetBool("IsGrenade", false);
                if (playerShoot.isScoped == true)
                {
                    playerShoot.isScoped = false;
                    if (currentWeapon.sniper)
                    {
                        playerShoot.weaponCamera.gameObject.SetActive(true);
                        playerShoot.cam.fieldOfView = 65f;
                        playerSetup.playerUI.crosshair.SetActive(true);
                        playerSetup.playerUI.scoped.SetActive(false);
                    }
                }
                playerShoot.weaponCamera.nearClipPlane = 0.17f;
                playerShoot.weaponCamera.transform.localPosition = new Vector3(0.002f, 0.73f, 0.079f);
                playerShoot.weaponCamera.transform.localEulerAngles = new Vector3(-15.979f, -56.465f, 22.974f);
                playerShoot.weaponCamera.GetComponent<CameraShake>().originalPos = new Vector3(0.002f, 0.73f, 0.079f);
                PV.RPC("RPC_EquipWeapon", RpcTarget.All, selectedWeapon);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2) && selectedWeapon != 1)
            {
                selectedWeapon = 1;
                isReloading = false;
                animator.SetBool("IsGrenade", false);
                if (playerShoot.isScoped == true)
                {
                    playerShoot.isScoped = false;
                    if (currentWeapon.sniper)
                    {
                        playerShoot.weaponCamera.gameObject.SetActive(true);
                        playerShoot.cam.fieldOfView = 65f;
                        playerSetup.playerUI.crosshair.SetActive(true);
                        playerSetup.playerUI.scoped.SetActive(false);
                    }
                }
                playerShoot.weaponCamera.nearClipPlane = 0.17f;
                playerShoot.weaponCamera.transform.localPosition = new Vector3(0.002f, 0.73f, 0.079f);
                playerShoot.weaponCamera.transform.localEulerAngles = new Vector3(-15.979f, -56.465f, 22.974f);
                playerShoot.weaponCamera.GetComponent<CameraShake>().originalPos = new Vector3(0.002f, 0.73f, 0.079f);
                PV.RPC("RPC_EquipWeapon", RpcTarget.All, selectedWeapon);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3) && selectedWeapon != 2)
            {
                selectedWeapon = 2;
                isReloading = false;
                animator.SetBool("IsGrenade", true);
                if (playerShoot.isScoped == true)
                {
                    playerShoot.isScoped = false;
                    if (currentWeapon.sniper)
                    {
                        playerShoot.weaponCamera.gameObject.SetActive(true);
                        playerShoot.cam.fieldOfView = 65f;
                        playerSetup.playerUI.crosshair.SetActive(true);
                        playerSetup.playerUI.scoped.SetActive(false);
                    }
                }
                playerShoot.weaponCamera.nearClipPlane = 0.3f;
                playerShoot.weaponCamera.transform.localPosition = new Vector3(0.061f, 0.771f, 0.126f);
                playerShoot.weaponCamera.transform.localEulerAngles = new Vector3(10.649f, -13.616f, 7.065001f);
                PV.RPC("RPC_EquipWeapon", RpcTarget.All, selectedWeapon);
            }
        }
    }

    [PunRPC]
    void RPC_EquipWeapon(int _selectedWeapon)
    {
        if(_selectedWeapon != 2)
        {
            currentWeapon = primaryWeapon[_selectedWeapon];
            currentGraphics = weaponGraphics[_selectedWeapon];
        }
        int count = 0;
        foreach(Transform child in weaponHolder)
        {
            if(count == _selectedWeapon)
            {
                child.gameObject.SetActive(true);
            }
            else
            {
                child.gameObject.SetActive(false);
            }
            count++;
        }
    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null)
            return;
        obj.layer = newLayer;
        foreach(Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    public void Reload()
    {
        if (isReloading)
            return;
        StartCoroutine(Reload_coroutine());
    }

    IEnumerator Reload_coroutine()
    {
        PlayerWeapon PreviousWeapon = currentWeapon;
        isReloading = true;
        yield return new WaitForSeconds(currentWeapon.reloadTime);
        if(PreviousWeapon == currentWeapon && isReloading == true)
        {
            currentWeapon.bullets = currentWeapon.maxBullets;
            isReloading = false;
        }
    }
}
