using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int score = 0;
    public TMP_Text scoreText; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; 
        }
        else
        {
            Destroy(gameObject); 
        }
    }
    public void AddScore(int points)
    {
        score += points; 
        UpdateScoreUI(); 
    }
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }
}