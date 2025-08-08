using UnityEngine;

public class LaserLoopWithCollider : MonoBehaviour
{
    public Transform firePoint;
    public float maxDistance = 50f;
    public LayerMask wallLayer;
    public BoxCollider2D boxCollider;

    [Header("Timing")]
    public float activeTime = 2f;
    public float blinkTime = 1f;
    public float offTime = 1f;
    public float blinkFrequency = 15f;

    private LineRenderer lineRenderer;
    private float timer;
    private enum LaserState { Active, Blinking, Off }
    private LaserState state = LaserState.Active;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        timer = activeTime;
    }

    void Update()
    {
        switch (state)
        {
            case LaserState.Active:
                lineRenderer.enabled = true;
                boxCollider.enabled = true;
                UpdateLaser();
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    state = LaserState.Blinking;
                    timer = blinkTime;
                }
                break;

            case LaserState.Blinking:
                UpdateLaser();
                timer -= Time.deltaTime;

                // Nhấp nháy
                bool visible = Mathf.Sin(Time.time * blinkFrequency) > 0;
                lineRenderer.enabled = visible;
                boxCollider.enabled = visible;

                if (timer <= 0)
                {
                    state = LaserState.Off;
                    lineRenderer.enabled = false;
                    boxCollider.enabled = false;
                    timer = offTime;
                }
                break;

            case LaserState.Off:
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    state = LaserState.Active;
                    timer = activeTime;
                }
                break;
        }
    }

    void UpdateLaser()
    {
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, firePoint.right, maxDistance, wallLayer);
        Vector3 endPos = hit.collider ? (Vector3)hit.point : firePoint.position + firePoint.right * maxDistance;

        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, endPos);

        // Cập nhật collider theo độ dài và hướng
        float length = Vector2.Distance(firePoint.position, endPos);
        boxCollider.size = new Vector2(length, 0.1f);
        boxCollider.offset = new Vector2(length / 2f, 0);
        float angle = Mathf.Atan2(firePoint.right.y, firePoint.right.x) * Mathf.Rad2Deg;
        boxCollider.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
