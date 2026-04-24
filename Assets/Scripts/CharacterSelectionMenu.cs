using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CharacterSelectionMenu : MonoBehaviour
{
    [Header("Character Prefabs")]
    public GameObject[] characters;

    [Header("Character Names")]
    public Sprite[] characterNames;

    [Header("Character IDs (DB)")]
    public int[] characterIDs;

    [Header("Displays")]
    public Image imageP1;
    public Image imageP2;

    [Header("Name Displays")]
    public Image fighterNameP1;
    public Image fighterNameP2;

    [Header("Buttons")]
    public Button doneButtonP1;
    public Button doneButtonP2;
    public Button fightButton;

    private int indexP1 = 0;
    private int indexP2 = 0;

    private bool p1Locked = false;
    private bool p2Locked = false;

    void Start()
    {
        fightButton.gameObject.SetActive(false);
        UpdateDisplays();
    }

    void UpdateDisplays()
    {
        imageP1.sprite = characters[indexP1].GetComponent<SpriteRenderer>().sprite;
        imageP2.sprite = characters[indexP2].GetComponent<SpriteRenderer>().sprite;

        fighterNameP1.sprite = characterNames[indexP1];
        fighterNameP2.sprite = characterNames[indexP2];
    }

    public void P1_Next()
    {
        if (p1Locked) return;
        indexP1 = (indexP1 + 1) % characters.Length;
        UpdateDisplays();
    }

    public void P1_Previous()
    {
        if (p1Locked) return;
        indexP1--;
        if (indexP1 < 0) indexP1 = characters.Length - 1;
        UpdateDisplays();
    }

    public void P1_Done()
    {
        p1Locked = true;
        doneButtonP1.interactable = false;
        CheckFightReady();
    }

    public void P2_Next()
    {
        if (p2Locked) return;
        indexP2 = (indexP2 + 1) % characters.Length;
        UpdateDisplays();
    }

    public void P2_Previous()
    {
        if (p2Locked) return;
        indexP2--;
        if (indexP2 < 0) indexP2 = characters.Length - 1;
        UpdateDisplays();
    }

    public void P2_Done()
    {
        p2Locked = true;
        doneButtonP2.interactable = false;
        CheckFightReady();
    }

    void CheckFightReady()
    {
        if (p1Locked && p2Locked)
            fightButton.gameObject.SetActive(true);
    }

    public void StartFight()
    {
        // Guardar índices (para instanciar)
        PlayerPrefs.SetInt("P1Character", indexP1);
        PlayerPrefs.SetInt("P2Character", indexP2);

        // Guardar IDs reales de la base de datos
        PlayerPrefs.SetInt("P1CharacterID", characterIDs[indexP1]);
        PlayerPrefs.SetInt("P2CharacterID", characterIDs[indexP2]);

        SceneManager.LoadScene("FightScene");
    }
}