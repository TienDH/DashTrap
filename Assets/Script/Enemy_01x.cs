using UnityEngine;

public class Enemy_01x : MonoBehaviour
{
    [Header("Path endpoints (world coords if set in Inspector)")]
    public Vector2 startPoint;       // Điểm đầu
    public Vector2 endPoint;         // Điểm cuối
    public float moveSpeed = 2f;     // Tốc độ di chuyển
    public float waitTime = 1f;      // Thời gian dừng tại mỗi đầu
    public bool startToLeft = false; // true: đi về điểm có X nhỏ hơn trước

    private Vector2 targetPointLocal; // target trong tọa độ local của parent
    private Vector2 startLocal;
    private Vector2 endLocal;
    private bool waiting = false;
    private float waitTimer = 0f;

    // Dùng để chọn nhanh trái/phải theo trục X local
    private Vector2 leftPointLocal;
    private Vector2 rightPointLocal;

    void Start()
    {
        // Quy đổi 2 mốc về local (nếu có parent). Nếu không thì coi như local == world.
        if (transform.parent != null)
        {
            startLocal = transform.parent.InverseTransformPoint(startPoint);
            endLocal   = transform.parent.InverseTransformPoint(endPoint);
        }
        else
        {
            startLocal = startPoint;
            endLocal   = endPoint;
        }

        // Xác định mốc "trái" (x nhỏ hơn) và "phải" (x lớn hơn) theo không gian local
        if (startLocal.x <= endLocal.x)
        {
            leftPointLocal = startLocal;
            rightPointLocal = endLocal;
        }
        else
        {
            leftPointLocal = endLocal;
            rightPointLocal = startLocal;
        }

        // KHÔNG set lại vị trí enemy. Bắt đầu từ vị trí hiện tại (local)
        Vector2 current = transform.localPosition;

        // Target ban đầu: trái hoặc phải, theo startToLeft
        targetPointLocal = startToLeft ? leftPointLocal : rightPointLocal;

        // Nếu đang đứng sát target ban đầu, đảo sang đầu còn lại để khỏi "đứng chờ" vô lý
        if (Vector2.Distance(current, targetPointLocal) < 0.01f)
        {
            targetPointLocal = (targetPointLocal == leftPointLocal) ? rightPointLocal : leftPointLocal;
        }

        FlipSprite();
    }

    void Update()
    {
        if (waiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                waiting = false;
                // Đổi đầu
                targetPointLocal = (targetPointLocal == leftPointLocal) ? rightPointLocal : leftPointLocal;
                FlipSprite();
            }
            return;
        }

        // Di chuyển trong không gian local về target
        Vector2 current = transform.localPosition;
        Vector2 next = Vector2.MoveTowards(current, targetPointLocal, moveSpeed * Time.deltaTime);
        transform.localPosition = next;

        // Đến nơi -> chờ
        if (Vector2.Distance(next, targetPointLocal) < 0.01f)
        {
            waiting = true;
            waitTimer = waitTime;
        }
    }

    void FlipSprite()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr == null) return;

        Vector2 current = (Vector2)transform.localPosition;
        Vector2 dir = (targetPointLocal - current);

        // Lật theo hướng X đang di chuyển (nếu đứng dọc, flipX giữ nguyên)
        if (Mathf.Abs(dir.x) > 0.0001f)
            sr.flipX = dir.x < 0f;
    }

#if UNITY_EDITOR
    // Vẽ gizmos để dễ canh mốc trong Editor
    void OnDrawGizmosSelected()
    {
        Transform parent = transform.parent;
        Vector3 a = (parent != null) ? parent.TransformPoint((Vector3)startPoint) : (Vector3)startPoint;
        Vector3 b = (parent != null) ? parent.TransformPoint((Vector3)endPoint)   : (Vector3)endPoint;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(a, b);
        Gizmos.DrawSphere(a, 0.08f);
        Gizmos.DrawSphere(b, 0.08f);
    }
#endif
}
