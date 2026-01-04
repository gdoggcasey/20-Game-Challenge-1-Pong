using UnityEngine;

public class PaddleMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private KeyCode upKey;
    [SerializeField] private KeyCode downKey;

    [Header("Wall References")]
    [SerializeField] private BoxCollider2D topWall;
    [SerializeField] private BoxCollider2D bottomWall;

    private Rigidbody2D rb;
    private BoxCollider2D paddleCollider;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        paddleCollider = GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        float moveDirection = 0f;

        if (Input.GetKey(upKey))
            moveDirection = 1f;
        else if (Input.GetKey(downKey))
            moveDirection = -1f;

        Vector2 targetPosition = rb.position;
        targetPosition.y += moveDirection * moveSpeed * Time.fixedDeltaTime;

        // Calculate bounds dynamically
        float paddleHalfHeight = paddleCollider.bounds.extents.y;
        float minY = bottomWall.bounds.max.y + paddleHalfHeight;
        float maxY = topWall.bounds.min.y - paddleHalfHeight;

        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        rb.MovePosition(targetPosition);
    }
}
