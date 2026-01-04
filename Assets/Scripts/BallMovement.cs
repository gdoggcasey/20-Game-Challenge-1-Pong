using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    public static BallMovement Instance { get; private set; } // Singleton instance

    [SerializeField] private float ballSpeed = 8f;
    [SerializeField] Transform leftGoal;
    [SerializeField] Transform rightGoal;
    [SerializeField] float resetDelay = 1f;

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
    }

    // Start is called before the first frame update
    void Start()
    {
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

        // Optional: hide ball, pplay sound, or do a little flash here

        //Wait for reset delay
        yield return new WaitForSeconds(resetDelay);

        transform.position = startPosition;

        LaunchBall();
    }
    
    public void StopBall()
    {
        rb.velocity = Vector2.zero;
    }
}
