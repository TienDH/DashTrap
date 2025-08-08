using UnityEngine;

public class BulletShoot : MonoBehaviour
{
    //public Vector2 direction = Vector2.up;
    //public float speed = 5f;
    //public float lifeTime = 5f;



    //void Update()
    //{
    //    transform.Translate(direction.normalized * speed * Time.deltaTime);
    //}

    // Khi va chạm với tường hoặc player
    void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra nếu va chạm với tường (gán tag "Wall" cho tường)
        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject); // Hủy viên đạn khi chạm tường
        }
    }
}