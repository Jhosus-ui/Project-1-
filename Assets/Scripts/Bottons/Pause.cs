using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseText;       // Referencia al texto "Juego Pausado"
    public Button pauseButton;         // Referencia al botón de pausa
    public Button menuButton;          // Referencia al botón "Ir al Menú"
    public string menuSceneName = "MainMenu"; // Nombre de la escena del menú

    private bool isPaused = false;

    private void Start()
    {
        // Configurar el botón de pausa para que llame al método TogglePause cuando se haga clic
        pauseButton.onClick.AddListener(TogglePause);

        // Configurar el botón "Ir al Menú" para que llame al método GoToMenu cuando se haga clic
        menuButton.onClick.AddListener(GoToMenu);

        // Asegurarse de que el texto de pausa y el botón "Ir al Menú" estén desactivados al inicio
        if (pauseText != null)
        {
            pauseText.SetActive(false);
        }
        if (menuButton != null)
        {
            menuButton.gameObject.SetActive(false);
        }
    }

    private void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    private void PauseGame()
    {
        // Pausar el tiempo del juego
        Time.timeScale = 0f;

        // Mostrar el texto de pausa y el botón "Ir al Menú"
        if (pauseText != null)
        {
            pauseText.SetActive(true);
        }
        if (menuButton != null)
        {
            menuButton.gameObject.SetActive(true);
        }
    }

    private void ResumeGame()
    {
        // Reanudar el tiempo del juego
        Time.timeScale = 1f;

        // Ocultar el texto de pausa y el botón "Ir al Menú"
        if (pauseText != null)
        {
            pauseText.SetActive(false);
        }
        if (menuButton != null)
        {
            menuButton.gameObject.SetActive(false);
        }
    }

    private void GoToMenu()
    {
        // Reanudar el tiempo del juego antes de cambiar de escena
        Time.timeScale = 1f;

        // Cargar la escena del menú
        SceneManager.LoadScene(menuSceneName);
    }
}