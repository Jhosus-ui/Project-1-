using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameOverScreen : MonoBehaviour
{
    public TMP_Text resultText; // Mensaje Final
    public TMP_Text scoreText; // Tu puntaje
    public string mainMenuScene = "MainMenu"; 
    public string gameScene = "GameScene"; 

    public AudioClip hoverSound; 
    public AudioClip clickSound; 
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        bool isTimeUp = GameManager.Instance.IsTimeUp();
        int finalScore = ScoreManager.Instance.score;

        resultText.text = isTimeUp ? "Round Finished" : "You have died";
        scoreText.text = finalScore.ToString();
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
        yield return new WaitForSeconds(1f); //Una espera talvez funcione pero lo dejo...
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