using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LoginUI : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;

    public void OnLoginButton() //agafa el text i el passa a UserManager per fer el login
    {
        string user = usernameInput.text;
        string pass = passwordInput.text;

        StartCoroutine(UserManager.Instance.Login(user, pass));
    }

    public void OnRegisterButton()
    {
        SceneManager.LoadScene("Register");
    }
}
