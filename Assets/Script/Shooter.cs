using UnityEngine;

public class AutoShooterNoPhysics : MonoBehaviour
{
    public GameObject bulletPrefab;    // Gán prefab viên đạn tại Inspector
    public Transform firePoint;        // Điểm bắn
    public float fireRate = 1f;        // Tốc độ bắn
    public float bulletSpeed = 5f;     // Tốc độ bay của viên đạn
    public Vector2 shootDirection = Vector2.right; // Hướng bắn, điều chỉnh trong Inspector

    void Start()
    {
        // Bắt đầu coroutine để bắn tự động
        StartCoroutine(AutoShoot());
    }

    System.Collections.IEnumerator AutoShoot()
    {
        while (true)
        {
            Shoot();
            yield return new WaitForSeconds(1f / fireRate); // Đợi theo fireRate
        }
    }

    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            // Tính góc xoay dựa trên hướng bắn
            float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
            Quaternion rot = Quaternion.AngleAxis(angle, Vector3.forward);

            // Tạo viên đạn với góc xoay đúng hướng
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, rot);
            StartCoroutine(MoveBullet(bullet)); // Bắt đầu coroutine di chuyển viên đạn
        }
    }

    System.Collections.IEnumerator MoveBullet(GameObject bullet)
    {
        while (bullet != null) // Kiểm tra nếu viên đạn vẫn tồn tại
        {
            // Di chuyển viên đạn theo hướng đã định nghĩa
            bullet.transform.Translate(shootDirection.normalized * bulletSpeed * Time.deltaTime, Space.World);
            yield return null; // Chờ frame tiếp theo
        }
    }
}