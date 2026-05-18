using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class RegisterUI : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_Text feedbackText; // Afegir aquest Text al Inspector

    void Start()
    {
        feedbackText.text = "";
    }

    public void OnRegisterButton()
    {
        string user = usernameInput.text.Trim();
        string pass = passwordInput.text.Trim();

        if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
        {
            ShowError("Omple tots els camps.");
            return;
        }

        if (user.Length < 3)
        {
            ShowError("El nom d'usuari ha de tenir mínim 3 caràcters.");
            return;
        }

        if (pass.Length < 4)
        {
            ShowError("La contrasenya ha de tenir mínim 4 caràcters.");
            return;
        }

        feedbackText.color = Color.white;
        feedbackText.text = "Registrant usuari...";
        StartCoroutine(UserManager.Instance.Register(user, pass, OnRegisterResult));
    }

    public void OnBackToLoginButton()
    {
        SceneManager.LoadScene("Login");
    }

    void OnRegisterResult(bool success, string message)
    {
        if (success)
        {
            feedbackText.color = Color.green;
            feedbackText.text = "Registre completat!";
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
