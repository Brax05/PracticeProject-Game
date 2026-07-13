using UnityEngine;

namespace TravesiaACasa.Rooms
{
    /// <summary>
    /// Animación por frames del yal sin Animator de Unity: alterna los
    /// sprites de la lista activa a un FPS fijo. Usa los frames de
    /// "yal_animaciones": idle cuando el ave está quieta y avanzar
    /// cuando se mueve (según la velocidad del Rigidbody2D). El volteo
    /// izquierda/derecha lo sigue haciendo BirdPlayerController con
    /// flipX, así que aquí solo se intercambian sprites.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class BirdSpriteAnimator : MonoBehaviour
    {
        [Tooltip("Frames cuando el ave está quieta (yal_animaciones/idle_lado)")]
        [SerializeField] private Sprite[] idleSprites;

        [Tooltip("Frames cuando el ave se mueve (yal_animaciones/avanzar)")]
        [SerializeField] private Sprite[] moveSprites;

        [Tooltip("Frames de animación por segundo")]
        [SerializeField] private float framesPerSecond = 8f;

        private SpriteRenderer spriteRenderer;
        private Rigidbody2D body;
        private Sprite[] current;
        private float timer;
        private int frame;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            body = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            bool moving = body != null && body.linearVelocity.sqrMagnitude > 0.01f;
            Sprite[] wanted = moving && moveSprites != null && moveSprites.Length > 0
                ? moveSprites
                : idleSprites;
            if (wanted == null || wanted.Length == 0) return;

            if (wanted != current)
            {
                // Cambio de estado (quieto <-> moviéndose): reiniciar el ciclo
                current = wanted;
                frame = 0;
                timer = 0f;
                spriteRenderer.sprite = current[0];
                return;
            }

            timer += Time.deltaTime;
            float frameTime = 1f / Mathf.Max(1f, framesPerSecond);
            if (timer < frameTime) return;

            timer -= frameTime;
            frame = (frame + 1) % current.Length;
            spriteRenderer.sprite = current[frame];
        }
    }
}
