using UnityEngine;

public class FightSetup : MonoBehaviour
{
    [Header("Character Prefabs")]
    public GameObject[] characters;

    [Header("Spawn Points")]
    public Transform spawnPointP1;
    public Transform spawnPointP2;

    [Header("Health Bars")]
    public UnityEngine.UI.Image healthFillP1;
    public UnityEngine.UI.Image healthFillP2;

    [Header("Block Bars")] // NUEVO
    public UnityEngine.UI.Image blockFillP1;
    public UnityEngine.UI.Image blockFillP2;

    [Header("Game Manager")]
    public GameManager gameManager;

    void Start()
    {
        int p1Index = PlayerPrefs.GetInt("P1Character");
        int p2Index = PlayerPrefs.GetInt("P2Character");

        int p1ID = PlayerPrefs.GetInt("P1CharacterID");
        int p2ID = PlayerPrefs.GetInt("P2CharacterID");

        GameObject p1 = Instantiate(characters[p1Index], spawnPointP1.position, Quaternion.identity);
        GameObject p2 = Instantiate(characters[p2Index], spawnPointP2.position, Quaternion.identity);

        FighterHealth fighterhealth1 = p1.GetComponent<FighterHealth>();
        FighterHealth fighterhealth2 = p2.GetComponent<FighterHealth>();

        // Asignar ID real del personaje
        fighterhealth1.characterID = p1ID;
        fighterhealth2.characterID = p2ID;

        // Asignar al GameManager
        gameManager.player1 = fighterhealth1;
        gameManager.player2 = fighterhealth2;

        // Asignar barras de vida
        fighterhealth1.healthFill = healthFillP1;
        fighterhealth2.healthFill = healthFillP2;

        // Asignar barras de bloqueo (NUEVO)
        fighterhealth1.blockFill = blockFillP1;
        fighterhealth2.blockFill = blockFillP2;

        // Configurar enemigos
        FighterStateMachine fsm1 = p1.GetComponent<FighterStateMachine>();
        FighterStateMachine fsm2 = p2.GetComponent<FighterStateMachine>();

        fsm1.enemy = p2.transform;
        fsm2.enemy = p1.transform;

        // Invertir escala del P2
        Vector3 scale = p2.transform.localScale;
        scale.x *= -1;
        p2.transform.localScale = scale;
    }
}
