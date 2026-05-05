using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public GameObject CanvasOptions;
    public GameObject CanvasMainMenu;
    public void StartGame()
    {
        SceneManager.LoadScene("CharacterSelectionMenu");
    }
    public void StatsPage()
    {
        SceneManager.LoadScene("StatsMenu");
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void OptionsIn()
    {
        CanvasOptions.SetActive(true);
        CanvasMainMenu.SetActive(false);
    }

    public void OptionsOut()
    {
        CanvasOptions.SetActive(false);
        CanvasMainMenu.SetActive(true);
    }


}
