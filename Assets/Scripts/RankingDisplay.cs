using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;

public class RankingDisplay : MonoBehaviour
{
    public TMP_Text rankingText;
    private string url = "https://elservidor.cat/~elcampalab/campalab/pau/files/nico/GetRanking.php";

    void Start()
    {
        rankingText.text = "Carregant ranquing...";
        StartCoroutine(LoadRanking());
    }

    IEnumerator LoadRanking()
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            rankingText.text = "Error carregant el ranquing.";
            yield break;
        }

        string data = www.downloadHandler.text.Trim();

        if (data == "NO_DATA" || data == "ERROR_DB")
        {
            rankingText.text = "No hi ha dades de ranquing.";
            yield break;
        }

        string[] users = data.Split(';');
        string output = "RANQUING\n\n";

        for (int i = 0; i < users.Length; i++)
        {
            string[] parts = users[i].Split('|');
            if (parts.Length < 3) continue;

            string name  = parts[0];
            string wins  = parts[1];
            string loses = parts[2];

            output += (i + 1) + ". " + name + "  -  " + wins + "W / " + loses + "L\n";
        }

        rankingText.text = output;
    }
}
