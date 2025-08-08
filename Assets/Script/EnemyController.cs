using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 2f;           // Tốc độ di chuyển
    public float wallHitDelay = 1f;        // Thời gian chờ trước khi đổi hướng (giây)

    public LayerMask wallLayer;            // Layer của tường
    public string playerTag = "Player";    // Tag của player
    private Vector2 moveDirection;
    private bool playerDetected = false;
    private bool isWaiting = false;        // Trạng thái chờ khi chạm tường
    private float waitTimer = 0f;          // Bộ đếm thời gian chờ
    [SerializeField] private float raycasts = 0.5f;    

    public bool PlayerDetected => playerDetected; // Trả giá trị bool ra ngoài

    public enum MoveDirection { Right, Up, Down, Left }
    [SerializeField] private MoveDirection initialDirection;

    private void Start()
    {
        switch (initialDirection)
        {
            case MoveDirection.Right:
                moveDirection = Vector2.right;
                break;
            case MoveDirection.Up:
                moveDirection = Vector2.up;
                break;
            case MoveDirection.Down:
                moveDirection = Vector2.down;
                break;
            case MoveDirection.Left:
                moveDirection = Vector2.left;
                break;
        }
    }

    private void Update()
    {
        if (!playerDetected && !isWaiting)
        {
            Move();
            CheckWall();
        }
        else if (isWaiting)
        {
            // Đếm thời gian chờ
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                isWaiting = false; // Kết thúc thời gian chờ
                moveDirection *= -1; // Đổi hướng
            }
        }
    }

    private void Move()
    {
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }

    private void CheckWall()
    {
        // Raycast về hướng di chuyển để kiểm tra tường
        RaycastHit2D hit = Physics2D.Raycast(transform.position, moveDirection, moveSpeed * Time.deltaTime + raycasts, wallLayer);
        if (hit.collider != null)
        {
            // Bắt đầu chờ trước khi đổi hướng
            isWaiting = true;
            waitTimer = wallHitDelay;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            playerDetected = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            playerDetected = false;
        }
    }
}