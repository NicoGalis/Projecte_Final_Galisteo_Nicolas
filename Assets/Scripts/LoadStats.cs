using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadStats : MonoBehaviour
{
    [Header("Characters")]
    public GameObject[] characters;
    public Sprite[] characterNames;
    public int[] characterIDs;

    [Header("UI Display")]
    public Image characterImage;
    public Image characterNameImage;

    [Header("Stats Display")]
    public TextMeshProUGUI winsText;
    public TextMeshProUGUI lossesText;
    public TextMeshProUGUI heavyText;
    public TextMeshProUGUI lightText;

    [Header("PHP URL")]
    public string getStatsURL = "https://elservidor.cat/~elcampalab/campalab/pau/files/nico/GetStats.php";

    private int index = 0;

    void Start()
    {
        UpdateDisplay();
        StartCoroutine(LoadStatsFromServer());
    }

    public void Next()
    {
        index++;
        if (index >= characters.Length) index = 0;

        UpdateDisplay();
        StartCoroutine(LoadStatsFromServer());
    }

    public void Previous()
    {
        index--;
        if (index < 0) index = characters.Length - 1;

        UpdateDisplay();
        StartCoroutine(LoadStatsFromServer());
    }

    public void GoBack()
    {
        SceneManager.LoadScene("StartMenu");
    }

    void UpdateDisplay()
    {
        characterImage.sprite = characters[index].GetComponent<SpriteRenderer>().sprite;
        characterNameImage.sprite = characterNames[index];
    }

    IEnumerator LoadStatsFromServer()
    {
        WWWForm form = new WWWForm();
        form.AddField("characterId", characterIDs[index]);

        using (UnityWebRequest www = UnityWebRequest.Post(getStatsURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                winsText.text = "Error";
                lossesText.text = "";
                heavyText.text = "";
                lightText.text = "";
                yield break;
            }

            string data = www.downloadHandler.text;

            if (data == "NO_DATA")
            {
                winsText.text = "Wins: 0";
                lossesText.text = "Losses: 0";
                heavyText.text = "Heavy: 0";
                lightText.text = "Light: 0";
                yield break;
            }

            string[] parts = data.Split('|');

            winsText.text = "Wins: " + parts[1];
            lossesText.text = "Losses: " + parts[2];
            heavyText.text = "Heavy: " + parts[3];
            lightText.text = "Light: " + parts[4];
        }
    }
}
