using UnityEngine;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    public List<PlayerController> players = new List<PlayerController>();
    public CameraController cameraController;
    private int currentPlayerIndex = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Tìm tất cả PlayerController trong scene nếu chưa gán
        if (players.Count == 0)
        {
            players.AddRange(FindObjectsOfType<PlayerController>());
        }

        // Chỉ cho player đầu tiên được điều khiển
        for (int i = 0; i < players.Count; i++)
        {
            players[i].isActivePlayer = (i == currentPlayerIndex); // Sửa chỗ này
        }

        // Gán camera theo player đầu tiên
        if (cameraController != null && players.Count > 0)
        {
            cameraController.SetTarget(players[currentPlayerIndex].transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && players.Count > 1)
        {
            // Tắt player hiện tại
            players[currentPlayerIndex].isActivePlayer = false; // Sửa chỗ này

            // Chuyển sang player tiếp theo
            currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;

            // Bật player mới
            players[currentPlayerIndex].isActivePlayer = true; // Sửa chỗ này

            // Đổi camera theo player mới
            if (cameraController != null)
            {
                cameraController.SetTarget(players[currentPlayerIndex].transform);
            }
        }
    }
}
