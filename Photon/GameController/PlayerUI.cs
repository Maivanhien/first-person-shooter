using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] RectTransform thrusterFuelFill;
    [SerializeField] RectTransform healthBarFill;
    [SerializeField] Text ammoText1, ammoText2;
    public GameObject crosshair, scoped;
    public GameObject pauseMenu;
    [SerializeField] GameObject scoreBoard;

    [HideInInspector] public PlayerController playerController;
    private PlayerSetup playerSetup;
    private WeaponManager weaponManager;

    public void SetController(PlayerController controller)
    {
        playerController = controller;
        playerController.rotationSpeed = (int)PlayerPrefs.GetFloat("GeneralSensitivity", playerController.rotationSpeed);
        playerController.scopingSensitivity = (int)PlayerPrefs.GetFloat("ScopingSensitivity", playerController.scopingSensitivity);
    }
    public void SetPlayerSetup(PlayerSetup setup)
    {
        playerSetup = setup;
        weaponManager = playerSetup.GetComponent<WeaponManager>();
    }

    void Update()
    {
        SetFuelAmount(playerController.GetThrusterFuelAmount());
        SetHealthAmount(playerSetup.GetHealthPct());
        SetAmmoAmount1(weaponManager.primaryWeapon[0].bullets, weaponManager.primaryWeapon[0].maxBullets);
        SetAmmoAmount2(weaponManager.primaryWeapon[1].bullets, weaponManager.primaryWeapon[1].maxBullets);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            scoreBoard.SetActive(true);
        }
        else if(Input.GetKeyUp(KeyCode.Tab))
        {
            scoreBoard.SetActive(false);
        }
    }

    void TogglePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
    }

    void SetFuelAmount(float amount)
    {
        thrusterFuelFill.localScale = new Vector3(amount, 1f, 1f);
    }

    void SetHealthAmount(float amount)
    {
        healthBarFill.localScale = new Vector3(amount, 1f, 1f);
    }

    void SetAmmoAmount1(int amount, int maxAmount)
    {
        ammoText1.text = "<size=22><b>" + amount + "</b></size>/" + maxAmount;
    }
    void SetAmmoAmount2(int amount, int maxAmount)
    {
        ammoText2.text = "<size=22><b>" + amount + "</b></size>/" + maxAmount;
    }
}
