using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{


    public string NameScene;


    public void StartGame()
    {
        SceneManager.LoadScene(NameScene); 
    }
    public void ExitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}