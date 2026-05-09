using UnityEngine;

public class UserController : MonoBehaviour
{
    public static UserController Instance;

    public int userId;
    public string username;
    public int wins;
    public int loses;

    void Awake() //Singleton pattern per mantenir les dades de l'usuari a través de les escenes
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
}
