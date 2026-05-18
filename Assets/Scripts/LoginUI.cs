using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LoginUI : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_Text feedbackText; // Afegir aquest Text al Inspector

    void Start()
    {
        feedbackText.text = "";
    }

    public void OnLoginButton()
    {
        string user = usernameInput.text.Trim();
        string pass = passwordInput.text.Trim();

        if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
        {
            ShowError("Omple tots els camps.");
            return;
        }

        feedbackText.text = "Iniciant sessió...";
        feedbackText.color = Color.white;
        StartCoroutine(UserManager.Instance.Login(user, pass, OnLoginResult));
    }

    public void OnRegisterButton()
    {
        SceneManager.LoadScene("Register");
    }

    void OnLoginResult(bool success, string message)
    {
        if (success)
        {
            feedbackText.color = Color.green;
            feedbackText.text = "Login correcte!";
        }
        else
        {
            ShowError(message);
        }
    }

    void ShowError(string msg)
    {
        feedbackText.color = Color.red;
        feedbackText.text = msg;
    }
}
