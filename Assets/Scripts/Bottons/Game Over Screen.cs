using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverScreen : MonoBehaviour
{
    public TMP_Text resultText; // Texto que muestra "Round Finished" o "You have died"
    public TMP_Text scoreText; // Texto que muestra el puntaje final
    public string mainMenuScene = "MainMenu"; // Nombre de la escena del menú principal
    public string gameScene = "GameScene"; // Nombre de la escena del juego

    private void Start()
    {
        // Obtener el resultado del juego y el puntaje final desde el GameManager
        bool isTimeUp = GameManager.Instance.IsTimeUp();
        int finalScore = ScoreManager.Instance.score;

        // Configurar los textos
        Setup(isTimeUp, finalScore);
    }

    // Método para configurar la pantalla de fin de juego
    public void Setup(bool isTimeUp, int finalScore)
    {
        // Mostrar el mensaje correspondiente
        if (isTimeUp)
        {
            resultText.text = "Round Finished";
        }
        else
        {
            resultText.text = "You have died";
        }

        // Mostrar el puntaje final
        scoreText.text = "" + finalScore;
    }

    // Método para reiniciar el juego
    public void RestartGame()
    {
        SceneManager.LoadScene(gameScene); // Cargar la escena del juego
    }

    // Método para salir al menú principal
    public void ExitToMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene); // Cargar la escena del menú principal
    }
}