using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject[] bar;
    [SerializeField] Text[] settingsText;
    [SerializeField] GameObject keyBindingsScroll;
    [SerializeField] GameObject controlsScroll;
    public Slider generalSensitivity;
    public Text generalSensitivityText;
    public Slider scopingSensitivity;
    public Text scopingSensitivityText;
    [SerializeField] PlayerUI playerUI;
    private int selectedButton;

    void OnEnable()
    {
        selectedButton = 0;
        bar[0].SetActive(true);
        bar[1].SetActive(false);
        bar[2].SetActive(false);
        settingsText[0].color = new Color32(231,231,231,255);
        settingsText[1].color = new Color32(183, 183, 183, 255);
        settingsText[2].color = new Color32(183, 183, 183, 255);
        keyBindingsScroll.SetActive(true);
        controlsScroll.SetActive(false);
        generalSensitivity.value = (int)PlayerPrefs.GetFloat("GeneralSensitivity", playerUI.playerController.rotationSpeed);
        generalSensitivityText.text = ((int)generalSensitivity.value).ToString();
        scopingSensitivity.value = (int)PlayerPrefs.GetFloat("ScopingSensitivity", playerUI.playerController.scopingSensitivity);
        scopingSensitivityText.text = ((int)scopingSensitivity.value).ToString();
    }

    public void OnKeyBindingsButtonClick()
    {
        if(selectedButton != 0)
        {
            selectedButton = 0;
            bar[0].SetActive(true);
            bar[1].SetActive(false);
            bar[2].SetActive(false);
            settingsText[0].color = new Color32(231, 231, 231, 255);
            settingsText[1].color = new Color32(183, 183, 183, 255);
            settingsText[2].color = new Color32(183, 183, 183, 255);
            keyBindingsScroll.SetActive(true);
            controlsScroll.SetActive(false);
        }
    }

    public void OnControlsButtonClick()
    {
        if (selectedButton != 1)
        {
            selectedButton = 1;
            bar[0].SetActive(false);
            bar[1].SetActive(true);
            bar[2].SetActive(false);
            settingsText[0].color = new Color32(183, 183, 183, 255);
            settingsText[1].color = new Color32(231, 231, 231, 255);
            settingsText[2].color = new Color32(183, 183, 183, 255);
            keyBindingsScroll.SetActive(false);
            controlsScroll.SetActive(true);
        }
    }

    public void OnValueChangeGeneralSensitivity()
    {
        generalSensitivityText.text = ((int)generalSensitivity.value).ToString();
        playerUI.playerController.rotationSpeed = (int)generalSensitivity.value;
        PlayerPrefs.SetFloat("GeneralSensitivity", (int)generalSensitivity.value);
    }

    public void OnValueChangeScopingSensitivity()
    {
        scopingSensitivityText.text = ((int)scopingSensitivity.value).ToString();
        playerUI.playerController.scopingSensitivity = (int)scopingSensitivity.value;
        PlayerPrefs.SetFloat("ScopingSensitivity", (int)scopingSensitivity.value);
    }

    public void OnLeaveRoomButtonClick()
    {

    }
}
