using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class RegisterUI : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public UserManager userManager;

    public void OnRegisterButton()
    {
        string user = usernameInput.text;
        string pass = passwordInput.text;

        StartCoroutine(userManager.Register(user, pass));
    }

    public void OnBackToLoginButton()
    {
        SceneManager.LoadScene("Login");
    }
}
