using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public LayerMask wallLayer;
    public InputSwipe inputSwipe;   // Gán script InputSwipe từ Inspector
    public Animator animator;
    public LayerMask winLayer;
    private Rigidbody2D rb;
    private Vector2 targetPosition;
    private bool isMoving;
    public bool isDead = false;
    private Queue<GameObject> hitEffectPool = new Queue<GameObject>();
    [SerializeField] private int poolSize = 20; // Tăng poolSize để an toàn hơn
    [SerializeField] private float winDelay = 0.8f;

    // Drag prefab vào Inspector
    public GameObject hitWallEffect;
    private Vector2 lastMoveDir;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        targetPosition = rb.position;

        // Kiểm tra các thành phần cần thiết
        if (inputSwipe == null || animator == null || hitWallEffect == null)
        {
            Debug.LogError("Một hoặc nhiều thành phần chưa được gán trong PlayerController!");
        }

        // Khởi tạo pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject effect = Instantiate(hitWallEffect);
            effect.SetActive(false);
            hitEffectPool.Enqueue(effect);
        }
    }

    private void SpawnHitEffect(Vector2 position, Quaternion rotation)
    {
        // Kiểm tra và làm sạch pool
        while (hitEffectPool.Count > 0 && hitEffectPool.Peek() == null)
        {
            hitEffectPool.Dequeue();
            Debug.LogWarning("Đã loại bỏ một đối tượng hiệu ứng bị hủy khỏi pool!");
        }

        if (hitEffectPool.Count > 0)
        {
            GameObject effect = hitEffectPool.Dequeue();
            if (effect != null)
            {
                effect.transform.position = position;
                effect.transform.rotation = rotation;
                effect.SetActive(true);
                StartCoroutine(RecycleEffect(effect, 1f));
            }
            else
            {
                Debug.LogWarning("Đối tượng hiệu ứng trong pool là null!");
                // Tạo mới đối tượng nếu cần
                GameObject newEffect = Instantiate(hitWallEffect);
                newEffect.transform.position = position;
                newEffect.transform.rotation = rotation;
                newEffect.SetActive(true);
                StartCoroutine(RecycleEffect(newEffect, 1f));
            }
        }
        else
        {
            // Tạo mới đối tượng nếu pool trống
            GameObject newEffect = Instantiate(hitWallEffect);
            newEffect.transform.position = position;
            newEffect.transform.rotation = rotation;
            newEffect.SetActive(true);
            StartCoroutine(RecycleEffect(newEffect, 1f));
        }
    }

    private IEnumerator RecycleEffect(GameObject effect, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (effect != null)
        {
            effect.SetActive(false);
            hitEffectPool.Enqueue(effect);
        }
    }

    private void Update()
    {
        if (isDead || isMoving) return;

        if (inputSwipe != null && inputSwipe.SwipeDirection != Vector2.zero)
        {
            Vector2 inputDir = inputSwipe.SwipeDirection;
            RaycastHit2D hit = Physics2D.Raycast(rb.position, inputDir, Mathf.Infinity, wallLayer | winLayer);

            if (hit.collider != null)
            {
                targetPosition = hit.point - inputDir * 0.5f;
                isMoving = true;
                lastMoveDir = inputDir;

                if (((1 << hit.collider.gameObject.layer) & winLayer) != 0)
                {
                    if (animator != null)
                    {
                        animator.SetTrigger("Shrink");
                    }
                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlaySFX(AudioManager.Instance.WinClip);
                    }
                    StartCoroutine(WinAfterDelay());
                }
            }
        }
    }

    public void StopImmediately()
    {
        isDead = true;
        isMoving = false;
    }

    private IEnumerator WinAfterDelay()
    {
        yield return new WaitForSeconds(winDelay);
        GameManager.Instance.WinGame();
        gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        if (isMoving)
        {
            rb.MovePosition(Vector2.MoveTowards(rb.position, targetPosition, moveSpeed * Time.fixedDeltaTime));

            if (Vector2.Distance(rb.position, targetPosition) < 0.01f)
            {
                rb.MovePosition(targetPosition);
                isMoving = false;

                if (hitWallEffect != null)
                {
                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlaySFX(AudioManager.Instance.clip_1);
                    }
                    Quaternion rot = Quaternion.LookRotation(Vector3.forward, -lastMoveDir);
                    SpawnHitEffect(targetPosition, rot);
                }
            }
        }
    }
}