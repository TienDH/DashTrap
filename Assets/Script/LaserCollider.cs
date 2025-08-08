using UnityEngine;

public class LaserCollider : MonoBehaviour
{
    public Transform firePoint;
    public float maxDistance = 50f;
    public LayerMask wallLayer;
    public BoxCollider2D boxCollider;  // gắn BoxCollider2D vào đây

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
    }

    void Update()
    {
        UpdateLaser();
    }

    void UpdateLaser()
    {
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, firePoint.right, maxDistance, wallLayer);
        Vector3 endPos = firePoint.position + firePoint.right * maxDistance;
        if (hit.collider != null)
            endPos = hit.point;

        // Vẽ laser
        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, endPos);

        // Cập nhật collider
        float length = Vector2.Distance(firePoint.position, endPos);
        boxCollider.size = new Vector2(length, 0.1f); // 0.1f là độ dày laser
        boxCollider.offset = new Vector2(length / 2f, 0); // dịch collider ra giữa

        // Xoay collider theo hướng laser
        float angle = Mathf.Atan2(firePoint.right.y, firePoint.right.x) * Mathf.Rad2Deg;
        boxCollider.transform.rotation = Quaternion.Euler(0, 0, angle);
    }   
}
