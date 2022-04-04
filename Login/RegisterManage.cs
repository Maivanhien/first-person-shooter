using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

[System.Serializable]
public class ErrorRegister
{
    public string[] username = new string[0];
    public string[] email = new string[0];
    public string[] password = new string[0];
    public string[] password_confirmation = new string[0];
}

public class RegisterManage : MonoBehaviour
{
    [SerializeField] InputField usernameField;
    [SerializeField] InputField emailField;
    [SerializeField] InputField passwordField;
    [SerializeField] InputField passwordConfirmationField;
    [SerializeField] Text errorUsername, errorEmail, errorPassword, errorPasswordConfirmation;
    [SerializeField] Button btnRegister;
    [SerializeField] GameObject loginPanel, successPanel;
    private string username;
    private string email;
    private string password;
    private string passwordConfirmation;

    void Start()
    {
        
    }

    void Update()
    {
        username = usernameField.text;
        email = emailField.text;
        password = passwordField.text;
        passwordConfirmation = passwordConfirmationField.text;
        if(username != "" && email != "" && password != "" && passwordConfirmation != "")
        {
            btnRegister.interactable = true;
        }
    }

    public void BtnLoginClick()
    {
        loginPanel.SetActive(true);
        gameObject.SetActive(false);
    }

    public void BtnRegisterClick()
    {
        StartCoroutine(CallRegister(username, email, password, passwordConfirmation));
    }

    public IEnumerator CallRegister(string username, string email, string password, string passwordConfirmation)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("email", email);
        form.AddField("password", password);
        form.AddField("password_confirmation", passwordConfirmation);
        UnityWebRequest www = UnityWebRequest.Post("https://multiplayergamefps.herokuapp.com/api/users/signup", form);
        yield return www.SendWebRequest();
        errorUsername.text = "";
        errorEmail.text = "";
        errorPassword.text = "";
        errorPasswordConfirmation.text = "";
        if (www.error != null)
        {
            ErrorRegister errorRegister = JsonUtility.FromJson<ErrorRegister>(www.downloadHandler.text);
            if(errorRegister.username.Length > 0)
            {
                errorUsername.text = errorRegister.username[0];
            }
            if (errorRegister.email.Length > 0)
            {
                errorEmail.text = errorRegister.email[0];
            }
            if (errorRegister.password.Length > 0)
            {
                errorPassword.text = errorRegister.password[0];
            }
            if (errorRegister.password_confirmation.Length > 0)
            {
                errorPasswordConfirmation.text = errorRegister.password_confirmation[0];
            }
        }
        else
        {
            loginPanel.SetActive(true);
            successPanel.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
