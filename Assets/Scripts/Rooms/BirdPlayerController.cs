using UnityEngine;
using UnityEngine.InputSystem;

namespace TravesiaACasa.Rooms
{
    /// <summary>
    /// Movimiento real del Yal austral (ave.png) para el escenario con
    /// arte final, reemplazando al CubePlayerController de prueba.
    /// Misma base de Rigidbody2D para que los RoomExitPoint
    /// (OnTriggerEnter2D) sigan funcionando igual, pero lee WASD/flechas
    /// directo de Keyboard.current (Input System nuevo) en vez de la
    /// clase Input vieja: el proyecto tiene activeInputHandler = 1
    /// (Input System Package puro) en ProjectSettings, así que
    /// Input.GetAxis lanzaría una excepción en tiempo real.
    /// También voltea el sprite según la dirección horizontal.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class BirdPlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 4f;

        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;
        private Vector2 input;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            rb.gravityScale = 0f; // vista top-down, no queremos caída
        }

        private void Update()
        {
            input = ReadKeyboardInput();

            if (Mathf.Abs(input.x) > 0.01f)
                spriteRenderer.flipX = input.x < 0f;
        }

        private static Vector2 ReadKeyboardInput()
        {
            Keyboard kb = Keyboard.current;
            if (kb == null) return Vector2.zero;

            float x = (kb.dKey.isPressed || kb.rightArrowKey.isPressed ? 1f : 0f)
                    - (kb.aKey.isPressed || kb.leftArrowKey.isPressed ? 1f : 0f);
            float y = (kb.wKey.isPressed || kb.upArrowKey.isPressed ? 1f : 0f)
                    - (kb.sKey.isPressed || kb.downArrowKey.isPressed ? 1f : 0f);
            return new Vector2(x, y);
        }

        private void FixedUpdate()
        {
            rb.linearVelocity = input.normalized * moveSpeed;
        }

        /// <summary>Para conectar botones de D-pad en pantalla en vez de teclado.</summary>
        public void SetInput(Vector2 direction)
        {
            input = direction;
        }
    }
}
