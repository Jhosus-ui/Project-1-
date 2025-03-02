using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    public string NameScene;
    public AudioClip hoverSound; 
    public AudioClip clickSound; 
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
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
        yield return new WaitForSeconds(0.1f); //otra espera, con todo lo dejo tambien
        if (button.name == "StartButton") StartGame();
        else if (button.name == "ExitButton") ExitGame();
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

    public void StartGame() => SceneManager.LoadScene(NameScene);
    public void ExitGame() => Application.Quit();
}