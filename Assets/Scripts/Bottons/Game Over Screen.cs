using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameOverScreen : MonoBehaviour
{
    public TMP_Text resultText; // Texto que muestra "Round Finished" o "You have died"
    public TMP_Text scoreText; // Texto que muestra el puntaje final
    public string mainMenuScene = "MainMenu"; // Nombre de la escena del menú principal
    public string gameScene = "GameScene"; // Nombre de la escena del juego

    // Sonidos
    public AudioClip hoverSound; // Sonido cuando el mouse pasa por encima del botón
    public AudioClip clickSound; // Sonido cuando se hace clic en el botón
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Obtener el resultado del juego y el puntaje final desde el GameManager
        bool isTimeUp = GameManager.Instance.IsTimeUp();
        int finalScore = ScoreManager.Instance.score;

        // Configurar los textos
        resultText.text = isTimeUp ? "Round Finished" : "You have died";
        scoreText.text = finalScore.ToString();

        // Configurar los sonidos para los botones
        ConfigureButtons();
    }

    private void ConfigureButtons()
    {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => OnButtonClick(button));
            AddHoverEffect(button);
        }
    }

    private void OnButtonClick(Button button)
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }

        StartCoroutine(DelayedAction(button));
    }

    private System.Collections.IEnumerator DelayedAction(Button button)
    {
        yield return new WaitForSeconds(0.2f); // Esperar 0.2 segundos

        if (button.name == "RestartButton") RestartGame();
        else if (button.name == "MainMenuButton") ExitToMainMenu();
    }

    private void AddHoverEffect(Button button)
    {
        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

        var pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((data) => OnPointerEnterButton(button));
        trigger.triggers.Add(pointerEnter);
    }

    private void OnPointerEnterButton(Button button)
    {
        if (hoverSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }

    public void RestartGame() => SceneManager.LoadScene(gameScene);
    public void ExitToMainMenu() => SceneManager.LoadScene(mainMenuScene);
}