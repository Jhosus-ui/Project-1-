using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseText;       // Referencia al texto "Juego Pausado"
    public Button pauseButton;         // Referencia al botón de pausa
    public Button menuButton;          // Referencia al botón "Ir al Menú"
    public string menuSceneName = "MainMenu"; // Nombre de la escena del menú

    // Sonidos
    public AudioClip hoverSound; // Sonido cuando el mouse pasa por encima del botón
    public AudioClip clickSound; // Sonido cuando se hace clic en el botón
    private AudioSource audioSource;

    public static bool isPaused = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Configurar los sonidos para los botones
        AddHoverEffect(pauseButton);
        AddHoverEffect(menuButton);

        pauseButton.onClick.AddListener(() => OnButtonClick(pauseButton, TogglePause));
        menuButton.onClick.AddListener(() => OnButtonClick(menuButton, GoToMenu));

        pauseText?.SetActive(false);
        menuButton?.gameObject.SetActive(false);
    }

    private void OnButtonClick(Button button, System.Action action)
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }

        StartCoroutine(DelayedAction(action));
    }

    private System.Collections.IEnumerator DelayedAction(System.Action action)
    {
        yield return new WaitForSeconds(0.2f); // Esperar 0.2 segundos
        action.Invoke();
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

    private void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        pauseText?.SetActive(isPaused);
        menuButton?.gameObject.SetActive(isPaused);
    }

    public static bool IsPaused() => isPaused;

    private void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }
}