using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int p1Wins = 0;
    public int p2Wins = 0;
    public TextMeshProUGUI WinCounter;
    public TextMeshProUGUI roundMessage; 

    public FighterHealth player1;
    public FighterHealth player2;

    public GameObject pauseMenu;
    private bool isPaused = false;

    public void PlayerDied(FighterHealth deadPlayer)
    {
        FighterHealth winner;
        FighterHealth loser;
        string winnerName;

        if (deadPlayer == player1)
        {
            p2Wins++;
            winner = player2;
            loser = player1;
            winnerName = "PLAYER 2";
        }
        else
        {
            p1Wins++;
            winner = player1;
            loser = player2;
            winnerName = "PLAYER 1";
        }

        StartCoroutine(AddWin(winner.characterID));
        StartCoroutine(AddLoss(loser.characterID));

        WinCounter.text = p1Wins + " - " + p2Wins;

        StartCoroutine(ShowWinAndReset(winnerName));
    }

    IEnumerator ShowWinAndReset(string winnerName)
    {
        roundMessage.text = winnerName + " WINS!";
        roundMessage.gameObject.SetActive(true);

        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(3f);

        roundMessage.gameObject.SetActive(false);

        if (p1Wins >= 3 || p2Wins >= 3)
        {
            p1Wins = 0;
            p2Wins = 0;
            WinCounter.text = "0 - 0";
        }

        // Reprendre joc i reiniciar ronda
        Time.timeScale = 1f;
        ResetRound();
    }

    void ResetRound()
    {
        player1.ResetHealth();
        player2.ResetHealth();

        player1.transform.position = new Vector3(-3, 0, 0);
        player2.transform.position = new Vector3(3, 0, 0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartMenu");
    }

    IEnumerator AddWin(int characterId)
    {
        string url = "https://elservidor.cat/~elcampalab/campalab/pau/files/nico/AddWin.php";
        string body = "characterId=" + characterId;

        UnityWebRequest www = new UnityWebRequest(url, "POST");
        byte[] raw = System.Text.Encoding.UTF8.GetBytes(body);

        www.uploadHandler = new UploadHandlerRaw(raw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

        yield return www.SendWebRequest();
    }

    IEnumerator AddLoss(int characterId)
    {
        string url = "https://elservidor.cat/~elcampalab/campalab/pau/files/nico/AddLoss.php";
        string body = "characterId=" + characterId;

        UnityWebRequest www = new UnityWebRequest(url, "POST");
        byte[] raw = System.Text.Encoding.UTF8.GetBytes(body);

        www.uploadHandler = new UploadHandlerRaw(raw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

        yield return www.SendWebRequest();
    }

    public IEnumerator AddHeavy(int characterId)
    {
        string url = "https://elservidor.cat/~elcampalab/campalab/pau/files/nico/AddHeavy.php";
        string body = "characterId=" + characterId;

        UnityWebRequest www = new UnityWebRequest(url, "POST");
        byte[] raw = System.Text.Encoding.UTF8.GetBytes(body);

        www.uploadHandler = new UploadHandlerRaw(raw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

        yield return www.SendWebRequest();
    }

    public IEnumerator AddLight(int characterId)
    {
        string url = "https://elservidor.cat/~elcampalab/campalab/pau/files/nico/AddLight.php";
        string body = "characterId=" + characterId;

        UnityWebRequest www = new UnityWebRequest(url, "POST");
        byte[] raw = System.Text.Encoding.UTF8.GetBytes(body);

        www.uploadHandler = new UploadHandlerRaw(raw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

        yield return www.SendWebRequest();
    }

    public IEnumerator GetStats(int characterId, System.Action<CharacterStats> callback)
    {
        string url = "https://elservidor.cat/~elcampalab/campalab/pau/files/nico/GetStats.php";
        string body = "characterId=" + characterId;

        UnityWebRequest www = new UnityWebRequest(url, "POST");
        byte[] raw = System.Text.Encoding.UTF8.GetBytes(body);

        www.uploadHandler = new UploadHandlerRaw(raw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

        yield return www.SendWebRequest();

        string result = www.downloadHandler.text;

        if (result == "NO_CHARACTER_ID" ||
            result == "ERROR_DB" ||
            result == "NO_DATA" ||
            result == "ERROR_QUERY")
        {
            Debug.Log("Error: " + result);
            callback(null);
            yield break;
        }

        string[] parts = result.Split('|');

        CharacterStats stats = new CharacterStats();
        stats.wins = int.Parse(parts[0]);
        stats.losses = int.Parse(parts[1]);
        stats.heavy = int.Parse(parts[2]);
        stats.light = int.Parse(parts[3]);

        callback(stats);
    }
}
