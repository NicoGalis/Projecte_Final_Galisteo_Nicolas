using UnityEngine;
using TMPro;

public class StatsDisplay : MonoBehaviour
{
    public TMP_Text usernameText;
    public TMP_Text winsText;
    public TMP_Text losesText;

    void Start()
    {
        usernameText.text = UserController.Instance.username;
        winsText.text = UserController.Instance.wins.ToString();
        losesText.text = UserController.Instance.loses.ToString();
    }
}
