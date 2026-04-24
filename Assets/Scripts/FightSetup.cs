using UnityEngine;

public class FightSetup : MonoBehaviour
{
    public GameObject[] characters;

    public Transform spawnPointP1;
    public Transform spawnPointP2;

    public UnityEngine.UI.Image healthFillP1;
    public UnityEngine.UI.Image healthFillP2;

    public UnityEngine.UI.Image blockFillP1;
    public UnityEngine.UI.Image blockFillP2;

    public GameManager gameManager;

    void Start()
    {
        int p1Index = PlayerPrefs.GetInt("P1Character");
        int p2Index = PlayerPrefs.GetInt("P2Character");

        int p1ID = PlayerPrefs.GetInt("P1CharacterID");
        int p2ID = PlayerPrefs.GetInt("P2CharacterID");

        GameObject p1 = Instantiate(characters[p1Index], spawnPointP1.position, Quaternion.identity);
        GameObject p2 = Instantiate(characters[p2Index], spawnPointP2.position, Quaternion.identity);

        int layerP1 = LayerMask.NameToLayer("Player1");
        int layerP2 = LayerMask.NameToLayer("Player2");

        foreach (Transform t in p1.GetComponentsInChildren<Transform>())
            t.gameObject.layer = layerP1;

        foreach (Transform t in p2.GetComponentsInChildren<Transform>())
            t.gameObject.layer = layerP2;

        FighterHealth fighterhealth1 = p1.GetComponent<FighterHealth>();
        FighterHealth fighterhealth2 = p2.GetComponent<FighterHealth>();

        fighterhealth1.characterID = p1ID;
        fighterhealth2.characterID = p2ID;

        gameManager.player1 = fighterhealth1;
        gameManager.player2 = fighterhealth2;

        fighterhealth1.healthFill = healthFillP1;
        fighterhealth2.healthFill = healthFillP2;

        fighterhealth1.blockFill = blockFillP1;
        fighterhealth2.blockFill = blockFillP2;

        FighterStateMachine fsm1 = p1.GetComponent<FighterStateMachine>();
        FighterStateMachine fsm2 = p2.GetComponent<FighterStateMachine>();

        fsm1.isPlayer1 = true;
        fsm2.isPlayer1 = false;

        fsm1.animationPrefix = characters[p1Index].name.ToLower();
        fsm2.animationPrefix = characters[p2Index].name.ToLower();

        fsm1.enemy = p2.transform;
        fsm2.enemy = p1.transform;

        Vector3 scale = p2.transform.localScale;
        scale.x *= -1;
        p2.transform.localScale = scale;
    }
}
