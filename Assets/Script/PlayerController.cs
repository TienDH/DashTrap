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
    public bool debugLogs = true;     // Bật/tắt log
    public bool logSteps = false;     // Log từng bước di chuyển trong FixedUpdate
    public bool drawGizmos = true;    // Vẽ gizmo boxcast

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

        if (debugLogs)
        {
            Debug.Log(Tag() + $"Awake | layer={LayerToName(gameObject.layer)} | wall={MaskToNames(wallLayer.value)} | win={MaskToNames(winLayer.value)}");
            Debug.Log(Tag() + $"Physics2D.queriesHitTriggers={Physics2D.queriesHitTriggers}");
            WarnCollisionMatrixAgainstMask(wallLayer);
            WarnCollisionMatrixAgainstMask(winLayer);
        }
    }

    void Update()
    {
        if (!isActivePlayer) { if (debugLogs) Debug.Log(Tag() + "BLOCK: not active"); return; }
        if (isDead)          { if (debugLogs) Debug.Log(Tag() + "BLOCK: dead"); return; }
        if (isMoving)        { if (debugLogs) Debug.Log(Tag() + "BLOCK: already moving"); return; }
        if (inputSwipe == null) { if (debugLogs) Debug.Log(Tag() + "BLOCK: no InputSwipe ref"); return; }

        Vector2 raw = inputSwipe.SwipeDirection;
        if (raw == Vector2.zero) { if (debugLogs) Debug.Log(Tag() + "BLOCK: no swipe"); return; }

        // Chuẩn hoá về 4 hướng
        Vector2 dir = Mathf.Abs(raw.x) > Mathf.Abs(raw.y)
            ? (raw.x > 0 ? Vector2.right : Vector2.left)
            : (raw.y > 0 ? Vector2.up : Vector2.down);

        // BoxCast 1 lần
        Vector2 size = col.bounds.size - new Vector3(skin * 2f, skin * 2f, 0f);
        int mask = wallLayer | winLayer;

        if (debugLogs)
            Debug.Log(Tag() + $"INPUT raw={raw} dir={dir} | pos={rb.position} | boxSize={size} | mask={MaskToNames(mask)}");

        RaycastHit2D hit = Physics2D.BoxCast(rb.position, size, 0f, dir, maxCastDistance, mask);

        if (hit.collider != null)
        {
            if (debugLogs)
                Debug.Log(Tag() + $"HIT {hit.collider.name} | layer={LayerToName(hit.collider.gameObject.layer)} | dist={hit.distance:F3} | trig={hit.collider.isTrigger}");

            float dist = Mathf.Max(0f, hit.distance - skin);
            targetPosition = rb.position + dir * dist;
            isMoving = true;

            // Nếu chạm ô Win
            if (((1 << hit.collider.gameObject.layer) & winLayer) != 0)
            {
                if (debugLogs) Debug.Log(Tag() + "WIN tile detected → play anim & schedule Win");
                animator?.SetTrigger("Shrink");
                AudioManager.Instance?.PlaySFX(AudioManager.Instance.WinClip);
                StartCoroutine(WinAfterDelay());
            }
        }
        else
        {
            if (debugLogs) Debug.Log(Tag() + "NO HIT in mask → checking 'any' cast to see what's phía trước");

            // Thử cast không filter để biết có collider nào thực sự phía trước không
            RaycastHit2D any = Physics2D.BoxCast(rb.position, size, 0f, dir, maxCastDistance, ~0);
            if (any.collider != null)
            {
                if (debugLogs)
                {
                    bool ignored = Physics2D.GetIgnoreLayerCollision(gameObject.layer, any.collider.gameObject.layer);
                    Debug.Log(Tag() + $"ANY HIT {any.collider.name} | layer={LayerToName(any.collider.gameObject.layer)} | dist={any.distance:F3} | trig={any.collider.isTrigger} | ignoredByMatrix={ignored} | NOT IN MASK={MaskToNames(mask)}");
                }
            }
            else
            {
                if (debugLogs) Debug.Log(Tag() + "ANY HIT = null → thật sự không có collider nào phía trước (thiếu viền/tường).");
            }

            if (enableFallbackMove)
            {
                targetPosition = rb.position + dir * cellStep;
                isMoving = true;
                if (debugLogs) Debug.LogWarning(Tag() + $"FALLBACK: move {cellStep} on dir {dir}");
            }
        }
    }

    void FixedUpdate()
    {
        if (isDead || !isMoving) return;

        Vector2 next = Vector2.MoveTowards(rb.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
        if (logSteps) Debug.Log(Tag() + $"Step: {rb.position} -> {next} | target={targetPosition}");
        rb.MovePosition(next);

        if (Vector2.Distance(rb.position, targetPosition) < 0.001f)
        {
            rb.MovePosition(targetPosition);
            isMoving = false;
            if (debugLogs) Debug.Log(Tag() + "ARRIVED target");
        }
    }

    public void StopImmediately()
    {
        isDead = true;
        isMoving = false;
        targetPosition = rb.position;
        if (debugLogs) Debug.Log(Tag() + "StopImmediately()");
    }

    private IEnumerator WinAfterDelay()
    {
        if (debugLogs) Debug.Log(Tag() + $"WinAfterDelay {winDelay}s");
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

    // Cảnh báo nếu Layer Collision Matrix đang ignore Player với bất kỳ layer trong mask
    void WarnCollisionMatrixAgainstMask(LayerMask m)
    {
        int playerLayer = gameObject.layer;
        for (int i = 0; i < 32; i++)
        {
            if ((m.value & (1 << i)) == 0) continue;
            bool ignored = Physics2D.GetIgnoreLayerCollision(playerLayer, i);
            if (ignored)
                Debug.LogWarning(Tag() + $"Layer Collision Matrix is IGNORING {LayerToName(playerLayer)} ↔ {LayerToName(i)}. Cast/collision có thể không thấy nhau.");
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (!drawGizmos) return;
        if (col == null) col = GetComponent<Collider2D>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        Vector2 size = col ? (Vector2)col.bounds.size - new Vector2(skin * 2f, skin * 2f) : new Vector2(0.9f, 0.9f);

        // Vẽ box tại vị trí hiện tại
        Gizmos.color = new Color(0, 1, 1, 0.25f);
        Gizmos.DrawCube(transform.position, new Vector3(size.x, size.y, 0.01f));
    }
#endif
}
