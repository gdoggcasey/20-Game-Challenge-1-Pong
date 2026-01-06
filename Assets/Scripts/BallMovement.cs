using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    public static BallMovement Instance { get; private set; } // Singleton instance

    [SerializeField] private float ballSpeed = 8f;
    private float defaultBallSpeed;
    [SerializeField] Transform leftGoal;
    [SerializeField] Transform rightGoal;
    [SerializeField] float resetDelay = 1f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip paddleHitSound;
    [SerializeField] private AudioClip wallHitSound;
    [SerializeField] private AudioClip goalSound;
    [SerializeField] private float maxMusicPitch = 2f;

    private float defaultMusicPitch = 1f;

    private Rigidbody2D rb;
    private Vector2 startPosition;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple BallMovement instances detected. Destroying extra");
            Destroy(gameObject);
            return;
        }

        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        defaultBallSpeed = ballSpeed;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (musicSource != null)
        {
            musicSource.pitch = defaultMusicPitch;
            musicSource.Play();
        }

        LaunchBall();
    }

    private void LaunchBall()
    {
        // Randomize horizontal direction left or right
        float xDirection = Random.value < 0.5f ? -1f : 1f;

        // Randomize vertical direction between -1 and 1
        float yDirection = Random.Range(-1f, 1f);

        Vector2 direction = new Vector2(xDirection, yDirection).normalized;

        rb.velocity = direction * ballSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Paddle"))
        {
            Vector3 paddlePos = collision.transform.position;
            float paddleHeight = collision.collider.bounds.size.y;

            float y = (transform.position.y - paddlePos.y) / (paddleHeight / 2);

            float xDirection = collision.transform.position.x < transform.position.x ? 1f : -1f;

            Vector2 direction = new Vector2(xDirection, y).normalized;

            ballSpeed *= 1.05f;
            rb.velocity = direction * ballSpeed;
            // Update music speed
            if (musicSource != null)
            {
                float pitch = ballSpeed / defaultBallSpeed;
                musicSource.pitch = Mathf.Clamp(pitch, defaultMusicPitch, maxMusicPitch);
            }
            audioSource.PlayOneShot(paddleHitSound);
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            audioSource.PlayOneShot(wallHitSound);
            // Only reflect if moving toward the wall
            if ((collision.transform.position.y > transform.position.y && rb.velocity.y > 0) ||
                (collision.transform.position.y < transform.position.y && rb.velocity.y < 0))
            {
                rb.velocity = new Vector2(rb.velocity.x, -rb.velocity.y);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform == leftGoal)
        {
            // Right player scores
            GameManager.Instance.ScoreGoal(false);
        }
        else if (collision.transform == rightGoal)
        {
            // Left player scores
            GameManager.Instance.ScoreGoal(true);
        }
    }

    public IEnumerator ResetBall()
    {
        rb.velocity = Vector2.zero;

        //Reset ball speed
        ballSpeed = defaultBallSpeed;

        // Optional: hide ball, play sound, or do a little flash here
        audioSource.PlayOneShot(goalSound);

        // 🎵 Reset music
        if (musicSource != null)
        {
            musicSource.pitch = defaultMusicPitch;
            musicSource.Stop();
        }

        //Wait for reset delay
        yield return new WaitForSeconds(resetDelay);

        transform.position = startPosition;

        musicSource.Play();
        LaunchBall();
    }

    public void StopBall()
    {
        rb.velocity = Vector2.zero;
    }

    public void StopMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
            musicSource.pitch = defaultMusicPitch;
        }
    }

    public void PauseMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Pause();
        }
    }

    public void ResumeMusic()
    {
        if (musicSource != null)
        {
            musicSource.UnPause();
        }
    }
}
