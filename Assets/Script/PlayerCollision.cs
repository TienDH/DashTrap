using UnityEngine;
using System.Collections;

public class PlayerCollision : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private PlayerController playerController;
    [SerializeField] private float blinkDuration = 1f;
    [SerializeField] private float blinkInterval = 0.1f;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerController = GetComponent<PlayerController>();
        if (spriteRenderer == null || playerController == null)
        {
            Debug.LogError("SpriteRenderer hoặc PlayerController chưa được gán trong PlayerCollision!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Va chạm với: {collision.gameObject.name}, Tag: {collision.tag}");
        if (collision.CompareTag("Gem"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(1);
            }
            else
            {
                Debug.LogError("GameManager.Instance là null!");
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.clip_3, true);
            }
            else
            {
                Debug.LogError("AudioManager.Instance là null!");
            }

            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("Enemy"))
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.LossClip2);
            }
            else
            {
                Debug.LogError("AudioManager.Instance là null!");
            }
            StartCoroutine(BlinkAndDie());
        }
    }

    private IEnumerator BlinkAndDie()
    {
        if (playerController != null)
        {
            playerController.StopImmediately();
        }

        float elapsed = 0f;
        while (elapsed < blinkDuration)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
            }
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerDie();
        }
        else
        {
            Debug.LogError("GameManager.Instance là null khi gọi PlayerDie!");
        }
    }
}