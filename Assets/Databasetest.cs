using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Databasetest : MonoBehaviour
{
    void Start()
    {
        // Cambia el 1 por el ID del personaje que quieras probar
        StartCoroutine(AddWin(1));
    }

    IEnumerator AddWin(int characterId)
    {
        WWWForm form = new WWWForm();
        form.AddField("characterId", characterId);

        UnityWebRequest www = UnityWebRequest.Post(
            "http://elservidor.cat/~elcampalab/campalab/pau/files/nico/AddWin.php",
            form
        );

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Respuesta del servidor: " + www.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error: " + www.error);
        }
    }
}
