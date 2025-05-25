using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public int score;
    public int learningScore;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI learningText;

    void Awake()
    {
        Instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }
    
    public void AddScore()
    {
        score += 1;
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    public void AddLearningScore(int value)
    {
        learningScore += value;
        if (learningText != null)
        {
            learningText.text = "Learning Score: " + learningScore;
        }
    }
    
    public void ResetScore()
    {
        score = 0;
        learningScore = 0;
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
        if (learningText != null)
        {
            learningText.text = "Learning Score: " + learningScore;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
