using UnityEngine;

public class BrekablePlatform : MonoBehaviour
{
    [Header("Configuración de la plataforma")]
    public float fallDelay = 2f;               // Tiempo antes de romperse
    public float respawnDelay = 5f;             // Tiempo para reaparecer
    public string playerTag = "Player";         // Tag para identificar al jugador

    [Header("Audio")]
    public AudioClip onPlayerTouchSound;
    public AudioClip onBreakSound;

    [Header("Sprites")]
    public Sprite touchedSprite;               // Sprite cuando el jugador pisa
    private Sprite originalSprite;

    private Collider2D platformCollider;
    private SpriteRenderer platformRenderer;
    private Animator animator;
    private AudioSource audioSource;

    private bool isBreaking = false;

    private void Awake()
    {
        platformCollider = GetComponent<Collider2D>();
        platformRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (platformRenderer != null)
            originalSprite = platformRenderer.sprite;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isBreaking) return; // Si ya se está rompiendo, no hacer nada más

        if (collision.collider.CompareTag(playerTag))
        {
            isBreaking = true;

            // Cambiar sprite si tenemos uno
            if (touchedSprite != null)
            {
                platformRenderer.sprite = touchedSprite;
            }

            // Reproducir sonido de contacto
            if (audioSource != null && onPlayerTouchSound != null)
            {
                audioSource.PlayOneShot(onPlayerTouchSound);
            }

            // Comenzar cuenta atrás para romperse
            Invoke(nameof(BreakPlatform), fallDelay);
        }
    }

    private void BreakPlatform()
    {
        // Reproducir sonido de ruptura
        if (audioSource != null && onBreakSound != null)
        {
            audioSource.PlayOneShot(onBreakSound);
        }

        // Reproducir animación si hay animador
        if (animator != null)
        {
            animator.SetTrigger("Break");
        }

        // Desactivar después de un pequeño retardo para que la animación se vea
        float disableDelay = animator != null ? 0.5f : 0f;
        Invoke(nameof(DisablePlatform), disableDelay);
    }

    private void DisablePlatform()
    {
        platformCollider.enabled = false;
        platformRenderer.enabled = false;

        // Programar la reaparición
        Invoke(nameof(RespawnPlatform), respawnDelay);
    }

    private void RespawnPlatform()
    {
        // Restaurar el estado original
        if (platformRenderer != null)
        {
            platformRenderer.enabled = true;
            platformRenderer.sprite = originalSprite;
        }

        if (platformCollider != null)
        {
            platformCollider.enabled = true;
        }

        isBreaking = false;
    }
}
