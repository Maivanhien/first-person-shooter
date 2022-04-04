using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Login
{
    public string email;
    public string authentication_token;
}

[System.Serializable]
public class ErrorLogin
{
    public string message = "";
    public string error = "";
}

public class LoginManage : MonoBehaviour
{
    [SerializeField] InputField usernameField;
    [SerializeField] InputField passwordField;
    [SerializeField] Text error;
    [SerializeField] Button btnLogin;
    [SerializeField] GameObject registerPanel;
    private string username;
    private string password;

    void Start()
    {
        
    }

    void Update()
    {
        // set data string from input field
        username = usernameField.text;
        password = passwordField.text;
        if(username != "" && password != "")
        {
            btnLogin.interactable = true;
        }
    }

    public void BtnRegisterClick()
    {
        registerPanel.SetActive(true);
        gameObject.SetActive(false);
    }
    public void BtnLoginClick()
    {
        StartCoroutine(CallLogin(username, password));
    }

    public IEnumerator CallLogin(string username, string password)
    {
        // create a form, in which you send your request, similar to json
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);
        // call api through Unitywebrequest
        UnityWebRequest www = UnityWebRequest.Post("https://multiplayergamefps.herokuapp.com/api/users/login", form);
        // here we will get back out response
        yield return www.SendWebRequest();
        if(www.error != null)
        {
            ErrorLogin errorLogin = JsonUtility.FromJson<ErrorLogin>(www.downloadHandler.text);
            if(errorLogin.message != "")
            {
                error.text = errorLogin.message;
            }
            else
            {
                error.text = errorLogin.error;
            }
        }
        else
        {
            Login login = JsonUtility.FromJson<Login>(www.downloadHandler.text);
            PlayerPrefs.SetString("authentication_token", login.authentication_token);
            SceneManager.LoadScene(1);
        }
    }
}
