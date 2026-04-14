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

    public FighterHealth player1;
    public FighterHealth player2;

    public GameObject pauseMenu;
    private bool isPaused = false;

    public void PlayerDied(FighterHealth deadPlayer)
    {
        FighterHealth winner;
        FighterHealth loser;

        if (deadPlayer == player1)
        {
            p2Wins++;
            winner = player2;
            loser = player1;
        }
        else
        {
            p1Wins++;
            winner = player1;
            loser = player2;
        }

        StartCoroutine(AddWin(winner.characterID));
        StartCoroutine(AddLoss(loser.characterID));

        WinCounter.text = p1Wins + " - " + p2Wins;

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
}
