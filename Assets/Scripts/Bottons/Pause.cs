using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseText;
    public Button pauseButton;
    public Button menuButton;
    public string menuSceneName = "MainMenu";

    public AudioClip hoverSound;
    public AudioClip clickSound;
    private AudioSource audioSource;
    public static bool isPaused = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
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

        action.Invoke(); // Ejecutar la acción inmediatamente sin retraso
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

        // Asegurarse de que el botón de pausa esté siempre activo
        pauseButton.interactable = true;
    }

    public static bool IsPaused() => isPaused;

    private void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }
}