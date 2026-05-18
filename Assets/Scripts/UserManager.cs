using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class UserManager : MonoBehaviour
{
    public static UserManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- LOGIN amb callback de feedback ---
    public IEnumerator Login(string username, string password, Action<bool, string> callback = null)
    {
        string url = "https://elservidor.cat/~elcampalab/campalab/pau/files/nico/UserLogin.php";
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            callback?.Invoke(false, "Error de connexió. Torna-ho a provar.");
            yield break;
        }

        string result = www.downloadHandler.text.Trim();
        Debug.Log("LOGIN RESULT: '" + result + "'");

        if (result.StartsWith("OK|"))
        {
            string[] data = result.Split('|');
            int userId = int.Parse(data[1]);
            UserController.Instance.userId = userId;
            UserController.Instance.username = username;
            yield return StartCoroutine(LoadStats(userId));
            callback?.Invoke(true, "");
            SceneManager.LoadScene("StartMenu");
        }
        else if (result == "WRONG_PASSWORD")
        {
            callback?.Invoke(false, "Contrasenya incorrecta.");
        }
        else if (result == "USER_NOT_FOUND")
        {
            callback?.Invoke(false, "L'usuari no existeix.");
        }
        else
        {
            callback?.Invoke(false, "Login incorrecte. Comprova les dades.");
        }
    }

    // --- REGISTER ---
    public IEnumerator Register(string username, string password, Action<bool, string> callback = null)
    {
        string url = "https://elservidor.cat/~elcampalab/campalab/pau/files/nico/UserRegister.php";
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            callback?.Invoke(false, "Error de connexió. Torna-ho a provar.");
            yield break;
        }

        string result = www.downloadHandler.text.Trim();
        Debug.Log("REGISTER RESULT: '" + result + "'");

        if (result.StartsWith("OK|"))
        {
            string[] data = result.Split('|');
            int userId = int.Parse(data[1]);
            UserController.Instance.userId = userId;
            UserController.Instance.username = username;
            yield return StartCoroutine(LoadStats(userId));
            callback?.Invoke(true, "");
            SceneManager.LoadScene("Login");
        }
        else if (result == "USER_EXISTS")
        {
            callback?.Invoke(false, "Aquest nom d'usuari ja existeix.");
        }
        else
        {
            callback?.Invoke(false, "Error en el registre. Torna-ho a provar.");
        }
    }

    // --- STATS i BBDD (sense canvis) ---
    public IEnumerator AddUserWin(int userId)
    {
        string url = "https://elservidor.cat/~elcampalab/campalab/pau/files/nico/AddUserWin.php";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();
    }

    public IEnumerator AddUserLoss(int userId)
    {
        string url = "https://elservidor.cat/~elcampalab/campalab/pau/files/nico/AddUserLoss.php";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();
    }

    public IEnumerator LoadStats(int userId)
    {
        string url = "https://elservidor.cat/~elcampalab/campalab/pau/files/nico/GetUserStats.php";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();

        string result = www.downloadHandler.text.Trim();
        if (result.Contains("|"))
        {
            string[] parts = result.Split('|');
            UserController.Instance.wins = int.Parse(parts[0]);
            UserController.Instance.loses = int.Parse(parts[1]);
        }
    }
}
