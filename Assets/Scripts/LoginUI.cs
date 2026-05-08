using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LoginUI : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public UserManager userManager;

    public void OnLoginButton()
    {
        string user = usernameInput.text;
        string pass = passwordInput.text;

        StartCoroutine(userManager.Login(user, pass));
    }

    public void OnRegisterButton()
    {
        SceneManager.LoadScene("Register");
    }
}
