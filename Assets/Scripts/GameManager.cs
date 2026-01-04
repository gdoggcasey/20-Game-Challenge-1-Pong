using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }


    [Header("Score UI")]
    [SerializeField] private TextMeshProUGUI leftScoreText;
    [SerializeField] private TextMeshProUGUI rightScoreText;

    [Header("Game Settings")]
    [SerializeField] private int maxScore = 5;

    [Header("Winner UI")]
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private Button restartButton;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;    // Add this if you don’t already have one
    [SerializeField] private AudioClip gameOverSound;    // New clip for end-of-game

    private int leftScore = 0;
    private int rightScore = 0;

    private bool gameOver = false;

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

    private void Start()
    {
        winnerText.text = "";
        restartButton.gameObject.SetActive(false);

        restartButton.onClick.AddListener(RestartGame);
    }

    public void ScoreGoal(bool leftGoalScored)
    {
        if (gameOver) return; //Ignore scoring after game over

        if (leftGoalScored)
        {
            leftScore++;
            leftScoreText.text = leftScore.ToString();
        }
        else
        {
            rightScore++;
            rightScoreText.text = rightScore.ToString();
        }

        if (leftScore >= maxScore)
        {
            EndGame("Left Player Wins!");
        }
        else if (rightScore >= maxScore)
        {
            EndGame("Right Player Wins!");
        }
        else
        {
            StartCoroutine(BallMovement.Instance.ResetBall());
        }
    }

    public void EndGame(string message)
    {
        gameOver = true;

        BallMovement.Instance.StopBall();
        BallMovement.Instance.StopMusic();

        winnerText.text = message;
        restartButton.gameObject.SetActive(true);

        if (audioSource != null && gameOverSound != null)
            {
                audioSource.PlayOneShot(gameOverSound);
            }
    }

    private void RestartGame()
    {
        // Optional: you can show a "Winner!: UI here

        //Restart entire Scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
