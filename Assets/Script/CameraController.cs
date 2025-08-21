using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float yOffset = 0f; // Khoảng cách dọc giữa camera và Player
    [SerializeField] private float minY = -10f; // Giới hạn y dưới
    [SerializeField] private float maxY = 10f;  // Giới hạn y trên
    [SerializeField] private float smoothSpeed = 5f; // Tốc độ mượt

    private Transform player; // Player hiện tại mà camera theo dõi
    private float initialX; // Vị trí x ban đầu của camera
    private float targetY;

    private void Start()
    {
        initialX = transform.position.x; // Lưu vị trí x ban đầu
        if (player != null)
        {
            targetY = Mathf.Clamp(player.position.y + yOffset, minY, maxY);
            Vector3 startPos = transform.position;
            startPos.y = targetY;
            transform.position = startPos;
        }
    }

    private void LateUpdate()
    {
        if (player != null)
        {
            targetY = Mathf.Clamp(player.position.y + yOffset, minY, maxY);
            Vector3 newPosition = transform.position;
            newPosition.y = Mathf.Lerp(transform.position.y, targetY, Time.deltaTime * smoothSpeed);
            newPosition.x = initialX;
            transform.position = newPosition;
        }
    }

    // Chỉ nhận target từ PlayerManager
    public void SetTarget(Transform newTarget)
    {
        player = newTarget;
    }
}

