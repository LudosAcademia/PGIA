using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AuthenticateClient : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInfo;
    [SerializeField] private TMP_InputField passwordInfo;
    [SerializeField] private TextMeshProUGUI userDetailText;
    [SerializeField] private GameObject startButton;
    string userInfo;

    public void Login()
    {
        List<UserData> userData = GameManager.Instance.GetGameData().userData;
        UserData user = userData.Find(p => p.username == usernameInfo.text && p.password == passwordInfo.text);

        for (int i = 0; i < userData.Count; i++)
        {
            Debug.Log(userData[i].username + " " + userData[i].password);
        }

        if (user == null)
        {
            userInfo = "Password or Username incorrect";
        }
        else
        {
            userInfo = "Welcome " + user.username + " \n Position: " + user.type;
            GameManager.Instance.GetGameData().currentUser = user;

            startButton.SetActive(true);
            usernameInfo.gameObject.SetActive(false);
            passwordInfo.gameObject.SetActive(false);
        }


        userDetailText.text = userInfo;
        Debug.Log(userInfo);
        Debug.Log(usernameInfo.text + " " + passwordInfo.text);

    }

    public void StartGame()
    {
        GameManager.Instance.GoToLevel("MainScene");
    }


}
