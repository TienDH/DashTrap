using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player; // Gán Player vào Inspector
    [SerializeField] private float yOffset = 0f; // Khoảng cách dọc giữa camera và Player
    private float initialX; // Vị trí x ban đầu của camera

    private void Start()
    {
        if (player == null)
        {
            player = FindFirstObjectByType<PlayerController>().transform; // Tự động tìm Player
            if (player == null)
            {
                Debug.LogError("Không tìm thấy Player trong scene!");
            }
        }
        initialX = transform.position.x; // Lưu vị trí x ban đầu
    }

    [SerializeField] private float minY = -10f; // Giới hạn y dưới
    [SerializeField] private float maxY = 10f;  // Giới hạn y trên

    private void LateUpdate()
    {
        if (player != null)
        {
            Vector3 newPosition = transform.position;
            newPosition.y = Mathf.Clamp(player.position.y + yOffset, minY, maxY);
            newPosition.x = initialX;
            transform.position = newPosition;
        }
    }
}
    
