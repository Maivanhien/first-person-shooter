using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PlayerController))]
public class PlayerSetup : MonoBehaviour, IPunInstantiateMagicCallback
{
    [SerializeField] Behaviour[] componentsToDisable;
    [SerializeField] Behaviour[] disableOnDeath;
    [SerializeField] GameObject[] disableGameobjectOnDeath;
    private bool[] wasEnabled;
    [SerializeField] GameObject playerUIPrefab;
    public PlayerUI playerUI;
    [SerializeField] Transform sceneCameraHolder;
    private GameObject playerUIInstance;

    [Header("Player info")]
    [SerializeField] int maxHealth = 100;
    private bool isCompareTeam;
    private int myTeam;
    public int kills;
    public int deaths;
    public int assists;
    private int currentHealth;
    private bool isDead = false;
    //private int currentHeart = 3;

    [SerializeField] GameObject playerModel;
    [SerializeField] GameObject spawnEffect;
    [SerializeField] GameObject nameplateCanvas;
    private PhotonView PV;
    private Animator animator;

    void Start()
    {
        PV = GetComponent<PhotonView>();
        animator = GetComponentInChildren<Animator>();
        isCompareTeam = true;
        if (PV.IsMine)
        {
            GameSetup.gameSetup.localPlayerTeam = myTeam;
            PV.RPC("RPC_setPlayerName", RpcTarget.All, ProfileController.profileController.profile.nickname);
            GameManager.gameManager.sceneCamera.SetActive(false);
            // Create PlayerUI
            playerUIInstance = Instantiate(playerUIPrefab);
            playerUIInstance.name = playerUIPrefab.name;
            // configure PlayerUI
            playerUI = playerUIInstance.GetComponent<PlayerUI>();
            playerUI.SetController(GetComponent<PlayerController>());
            playerUI.SetPlayerSetup(this);
        }
        else
        {
            for(int i=0; i<componentsToDisable.Length; i++)
            {
                componentsToDisable[i].enabled = false;
            }
        }
        wasEnabled = new bool[disableOnDeath.Length];
        for (int i = 0; i < wasEnabled.Length; i++)
        {
            wasEnabled[i] = disableOnDeath[i].enabled;
        }
        SetDefaults();
    }

    void SetDefaults()
    {
        isDead = false;
        currentHealth = maxHealth;
        //Enable the components when respawn
        for(int i=0;i<disableOnDeath.Length;i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }
        //Gameobjects active when respawn
        for (int i = 0; i < disableGameobjectOnDeath.Length; i++)
        {
            disableGameobjectOnDeath[i].SetActive(true);
        }
        Collider collider = GetComponent<Collider>();
        if(collider != null)
        {
            collider.enabled = true;
        }
        //Create spawn effect
        if(PV.IsMine)
        {
            PV.RPC("RPC_SpawnEffect", RpcTarget.All, transform.position, transform.rotation);
        }
    }

    public float GetHealthPct()
    {
        return (float)currentHealth / maxHealth;
    }

    void AssignRemotePlayer()
    {
        gameObject.layer = LayerMask.NameToLayer("RemotePlayer");
    }

    [PunRPC]
    void RPC_setPlayerName(string name)
    {
        this.gameObject.name = name;
        GameManager.gameManager.SetupPlayer(name, this);
    }

    [PunRPC]
    void RPC_SpawnEffect(Vector3 position, Quaternion rotation)
    {
        GameObject spawnEffectIns = Instantiate(spawnEffect, new Vector3(position.x, position.y + 0.8f, position.z), rotation);
        Destroy(spawnEffectIns, 3f);
    }

    void Update()
    {
        if(!PV.IsMine && isCompareTeam == true)
        {
            if (GameSetup.gameSetup.localPlayerTeam == 0)
                return;
            if(myTeam != GameSetup.gameSetup.localPlayerTeam)
            {
                AssignRemotePlayer();
                nameplateCanvas.SetActive(false);
            }
            isCompareTeam = false;
        }
        if(PV.IsMine)
        {
            if(Input.GetKeyDown(KeyCode.K))
            {
                SetCurrentPlayerHealth(100, this);
            }
            if(isDead == true)
            {
                GameManager.gameManager.sceneCamera.transform.position = sceneCameraHolder.position;
                GameManager.gameManager.sceneCamera.transform.rotation = sceneCameraHolder.rotation;
            }
        }
    }

    public void SetCurrentPlayerHealth(int damage, PlayerSetup gameobjectShoot)
    {
        if (isDead)
            return;
        currentHealth -= damage;
        Debug.Log(gameObject.name + " now has " + currentHealth + " health.");
        if(currentHealth <= 0)
        {
            Die(gameobjectShoot);
        }
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(6f);
        animator.SetLayerWeight(animator.GetLayerIndex("Body Layer"), 1f);
        animator.SetLayerWeight(animator.GetLayerIndex("Arm Layer"), 1f);
        if (PV.IsMine)
        {
            GameManager.gameManager.sceneCamera.SetActive(false);
            playerUIInstance.SetActive(true);
            animator.applyRootMotion = false;
            playerModel.transform.localPosition = new Vector3(0f, -1.003f, 0f);
            playerModel.transform.localRotation = Quaternion.Euler(0f, 48.8f, 0f);
            animator.SetBool("IsDead", false);
            int spawnPicker = Random.Range(0, GameSetup.gameSetup.spawnPointsTeamOne.Length);
            transform.position = GameSetup.gameSetup.spawnPointsTeamOne[spawnPicker].position;
            transform.rotation = GameSetup.gameSetup.spawnPointsTeamOne[spawnPicker].rotation;
        }
        SetDefaults();
        Debug.Log("Player respawned");
    }

    void Die(PlayerSetup gameobjectShoot)
    {
        isDead = true;
        deaths++;
        gameobjectShoot.kills++;
        GameManager.gameManager.onPlayerKilledCallback.Invoke(this.gameObject.name, gameobjectShoot.gameObject.name);
        Debug.Log(transform.name + "is dead!");
        //Disable components when dead
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }
        //Disable gameobjects
        for(int i=0;i<disableGameobjectOnDeath.Length;i++)
        {
            disableGameobjectOnDeath[i].SetActive(false);
        }
        //Disable collider
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }
        // perform animation death for gameobject
        if (PV.IsMine)
        {
            GameManager.gameManager.sceneCamera.SetActive(true);
            GameManager.gameManager.sceneCamera.transform.position = sceneCameraHolder.position;
            GameManager.gameManager.sceneCamera.transform.rotation = sceneCameraHolder.rotation;
            playerUIInstance.SetActive(false);
            animator.applyRootMotion = true;
            PV.RPC("RPC_SetLayerWeightZero", RpcTarget.All);
            animator.SetBool("IsDead", true);
        }
        //Call respawn method
        StartCoroutine(Respawn());
    }

    [PunRPC]
    void RPC_SetLayerWeightZero()
    {
        animator.SetLayerWeight(animator.GetLayerIndex("Body Layer"), 0f);
        animator.SetLayerWeight(animator.GetLayerIndex("Arm Layer"), 0f);
    }

    private void OnDisable()
    {
        if(PV.IsMine)
        {
            Destroy(playerUIInstance);
            //GameManager.gameManager.sceneCamera.SetActive(true);
        }
        GameManager.gameManager.RemovePlayer(this.gameObject.name);
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] instantiationData = info.photonView.InstantiationData;
        myTeam = (int)instantiationData[0];
    }
}
