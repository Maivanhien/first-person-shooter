using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

[System.Serializable]
public class Profile
{
    public int user_id;
    public string nickname;
    public string avatar;
    public int level;
    public int experience;
    public int rank_point;
    public int number_of_ranks;
    public int gold;
    public int ruby;
    public string status;
    public int number_of_wins;
}

[System.Serializable]
public class ErrorProfile
{
    public string[] nickname = new string[0];
}

public class ProfileController : MonoBehaviour
{
    public static ProfileController profileController;
    public Profile profile;
    [SerializeField] Text coinText;
    [SerializeField] Text rubyText;
    [SerializeField] Text nickname;
    [SerializeField] Text level;
    [SerializeField] Text status;
    [SerializeField] Slider experience;
    [SerializeField] GameObject navBar, friendPanel, bar, setNicknamePanel;
    [SerializeField] InputField nicknameField;
    [SerializeField] Text error;

    private void Awake()
    {
        if(ProfileController.profileController == null)
        {
            ProfileController.profileController = this;
        }
        else
        {
            if(ProfileController.profileController != this)
            {
                Destroy(ProfileController.profileController.gameObject);
                ProfileController.profileController = this;
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        StartCoroutine(GetProfile());
    }

    IEnumerator GetProfile()
    {
        UnityWebRequest www = UnityWebRequest.Get("https://multiplayergamefps.herokuapp.com/api/profile");
        www.SetRequestHeader("Authorization", PlayerPrefs.GetString("authentication_token"));
        yield return www.SendWebRequest();
        if(www.error != null)
        {
            Debug.Log("Error: " + www.error);
        }
        else
        {
            profile = JsonUtility.FromJson<Profile>(www.downloadHandler.text);
            if(profile.nickname == "defaultname")
            {
                setNicknamePanel.SetActive(true);
            }
            else
            {
                navBar.SetActive(true);
                friendPanel.SetActive(true);
                bar.SetActive(true);
                coinText.text = profile.gold.ToString();
                rubyText.text = profile.ruby.ToString();
                nickname.text = profile.nickname;
                level.text = profile.level.ToString();
                status.text = profile.status;
                experience.value = 0.6f;
            }
        }
    }

    public void BtnSubmitClick()
    {
        StartCoroutine(ChangeNickname(nicknameField.text));
    }

    IEnumerator ChangeNickname(string nicknamefield)
    {
        string json = "{\"nickname\": \"" + nicknamefield + "\"}";
        byte[] data = System.Text.Encoding.UTF8.GetBytes(json);
        UnityWebRequest www = UnityWebRequest.Put("https://multiplayergamefps.herokuapp.com/api/profile", data);
        www.SetRequestHeader("Authorization", PlayerPrefs.GetString("authentication_token"));
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();
        if (www.error != null)
        {
            ErrorProfile errorProfile = JsonUtility.FromJson<ErrorProfile>(www.downloadHandler.text);
            if(errorProfile.nickname.Length > 0)
            {
                error.text = errorProfile.nickname[0];
            }
        }
        else
        {
            profile = JsonUtility.FromJson<Profile>(www.downloadHandler.text);
            setNicknamePanel.SetActive(false);
            navBar.SetActive(true);
            friendPanel.SetActive(true);
            bar.SetActive(true);
            coinText.text = profile.gold.ToString();
            rubyText.text = profile.ruby.ToString();
            nickname.text = profile.nickname;
            level.text = profile.level.ToString();
            status.text = profile.status;
            experience.value = 0.6f;
        }
    }
}
