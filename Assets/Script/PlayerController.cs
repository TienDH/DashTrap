using UnityEngine;
using System.Collections;
using System.Text;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 10f;
    public float skin = 0.04f;
    public float maxCastDistance = 200f;
    public float cellStep = 1f;
    public bool enableFallbackMove = true;

    [Header("Layers")]
    public LayerMask wallLayer;
    public LayerMask winLayer;

    [Header("Refs")]
    public InputSwipe inputSwipe;
    public Animator animator;

    [Header("State")]
    public bool isActivePlayer = true;
    public bool isDead = false;

    [Header("Debug")]
    public bool debugLogs = false;     // Đã tắt log
    public bool logSteps = false;
    public bool drawGizmos = true;

    private Rigidbody2D rb;
    private Collider2D col;
    private Vector2 targetPosition;
    private bool isMoving;
    private float winDelay = 0.8f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        targetPosition = rb.position;
    }

    void Update()
    {
        if (!isActivePlayer) return;
        if (isDead) return;
        if (isMoving) return;
        if (inputSwipe == null) return;

        Vector2 raw = inputSwipe.SwipeDirection;
        if (raw == Vector2.zero) return;

        Vector2 dir = Mathf.Abs(raw.x) > Mathf.Abs(raw.y)
            ? (raw.x > 0 ? Vector2.right : Vector2.left)
            : (raw.y > 0 ? Vector2.up : Vector2.down);

        Vector2 size = col.bounds.size - new Vector3(skin * 2f, skin * 2f, 0f);
        int mask = wallLayer | winLayer;

        RaycastHit2D hit = Physics2D.BoxCast(rb.position, size, 0f, dir, maxCastDistance, mask);

        if (hit.collider != null)
        {
            float dist = Mathf.Max(0f, hit.distance - skin);
            targetPosition = rb.position + dir * dist;
            isMoving = true;

            if (((1 << hit.collider.gameObject.layer) & winLayer) != 0)
            {
                animator?.SetTrigger("Shrink");
                AudioManager.Instance?.PlaySFX(AudioManager.Instance.WinClip);
                StartCoroutine(WinAfterDelay());
            }
        }
        else
        {
            RaycastHit2D any = Physics2D.BoxCast(rb.position, size, 0f, dir, maxCastDistance, ~0);
            if (enableFallbackMove)
            {
                targetPosition = rb.position + dir * cellStep;
                isMoving = true;
            }
        }
    }

    void FixedUpdate()
    {
        if (isDead || !isMoving) return;

        Vector2 next = Vector2.MoveTowards(rb.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(next);

        if (Vector2.Distance(rb.position, targetPosition) < 0.001f)
        {
            rb.MovePosition(targetPosition);
            isMoving = false;
        }
    }

    public void StopImmediately()
    {
        isDead = true;
        isMoving = false;
        targetPosition = rb.position;
    }

    private IEnumerator WinAfterDelay()
    {
        yield return new WaitForSeconds(winDelay);
        GameManager.Instance.WinGame();
        gameObject.SetActive(false);
    }

    // ===== Helpers =====
    string Tag() => $"[PC:{name}] ";

    string MaskToNames(int mask)
    {
        if (mask == 0) return "(empty)";
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < 32; i++)
            if ((mask & (1 << i)) != 0) sb.Append(LayerToName(i)).Append("|");
        return sb.ToString();
    }

    string LayerToName(int layer)
    {
        string n = LayerMask.LayerToName(layer);
        return string.IsNullOrEmpty(n) ? $"Layer{layer}" : n;
    }

    void WarnCollisionMatrixAgainstMask(LayerMask m)
    {
        // Đã bỏ log, giữ hàm trống để tránh lỗi biên dịch nếu bị gọi
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (!drawGizmos) return;
        if (col == null) col = GetComponent<Collider2D>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        Vector2 size = col ? (Vector2)col.bounds.size - new Vector2(skin * 2f, skin * 2f) : new Vector2(0.9f, 0.9f);

        Gizmos.color = new Color(0, 1, 1, 0.25f);
        Gizmos.DrawCube(transform.position, new Vector3(size.x, size.y, 0.01f));
    }
#endif
}
