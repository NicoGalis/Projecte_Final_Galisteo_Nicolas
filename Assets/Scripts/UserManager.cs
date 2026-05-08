using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;

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
    public IEnumerator Login(string username, string password)
    {
        string url = "https://elservidor.cat/~elcampalab/campalab/pau/files/nico/UserLogin.php";

        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();

        string result = www.downloadHandler.text.Trim();
        Debug.Log("LOGIN RESULT: '" + result + "'");

        if (result.StartsWith("OK|"))
        {
            string[] data = result.Split('|');
            int userId = int.Parse(data[1]);

            UserController.Instance.userId = userId;
            UserController.Instance.username = username;

            yield return StartCoroutine(LoadStats(userId));

            SceneManager.LoadScene("StartMenu");
        }
        else
        {
            Debug.Log("Login incorrecto");
        }
    }

    // ---------------- REGISTER ----------------
    public IEnumerator Register(string username, string password)
    {
        string url = "https://elservidor.cat/~elcampalab/campalab/pau/files/nico/UserRegister.php";

        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();

        string result = www.downloadHandler.text.Trim();
        Debug.Log("REGISTER RESULT: '" + result + "'");

        if (result.StartsWith("OK|"))
        {
            string[] data = result.Split('|');
            int userId = int.Parse(data[1]);

            UserController.Instance.userId = userId;
            UserController.Instance.username = username;

            yield return StartCoroutine(LoadStats(userId));

            SceneManager.LoadScene("Login");
        }
        else
        {
            Debug.Log("Registro incorrecto");
        }
    }

    // ---------------- ADD WIN ----------------
    public IEnumerator AddUserWin(int userId)
    {
        string url = "https://elservidor.cat/~elcampalab/campalab/pau/files/nico/AddUserWin.php";

        WWWForm form = new WWWForm();
        form.AddField("userId", userId);

        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();
    }

    // ---------------- ADD LOSS ----------------
    public IEnumerator AddUserLoss(int userId)
    {
        string url = "https://elservidor.cat/~elcampalab/campalab/pau/files/nico/AddUserLoss.php";

        WWWForm form = new WWWForm();
        form.AddField("userId", userId);

        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();
    }

    // ---------------- LOAD STATS ----------------
    public IEnumerator LoadStats(int userId)
    {
        string url = "https://elservidor.cat/~elcampalab/campalab/pau/files/nico/GetUserStats.php";

        WWWForm form = new WWWForm();
        form.AddField("userId", userId);

        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();

        Debug.Log("HTTP CODE: " + www.responseCode);
        Debug.Log("RAW RESULT: '" + www.downloadHandler.text + "'");

        string result = www.downloadHandler.text.Trim();

        if (result == "NO_USER_ID")
        {
            Debug.LogError("El PHP no recibió userId.");
            yield break;
        }

        if (result == "NO_DATA")
        {
            Debug.LogError("No existe fila en userstats para este userId.");
            yield break;
        }

        if (result.Contains("|"))
        {
            string[] parts = result.Split('|');

            UserController.Instance.wins = int.Parse(parts[0]);
            UserController.Instance.loses = int.Parse(parts[1]);

            Debug.Log("Stats cargados: wins=" + parts[0] + " loses=" + parts[1]);
        }
        else
        {
            Debug.LogError("Respuesta inesperada del servidor: " + result);
        }
    }
}
