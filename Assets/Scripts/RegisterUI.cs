using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class RegisterUI : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;

    public void OnRegisterButton() //agafa el text i el passa a UserManager per fer el registre
    {
        string user = usernameInput.text;
        string pass = passwordInput.text;

        StartCoroutine(UserManager.Instance.Register(user, pass));
    }

    public void OnBackToLoginButton()
    {
        SceneManager.LoadScene("Login");
    }
}
