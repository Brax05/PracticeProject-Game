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
    ///
    /// El input final es la SUMA de teclado + D-pad en pantalla
    /// (HudMoveButton). Antes Update() pisaba `input` con el teclado
    /// cada frame, así que cualquier botón táctil quedaba anulado al
    /// frame siguiente; ahora el HUD acumula en su propio vector y se
    /// combinan recién al momento de mover.
    /// También voltea el sprite según la dirección horizontal.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class BirdPlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 4f;

        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;
        private Vector2 input;     // teclado + HUD ya combinados (se recalcula en Update)
        private Vector2 hudInput;  // suma de los HudMoveButton actualmente presionados

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            rb.gravityScale = 0f; // vista top-down, no queremos caída
        }

        private void Update()
        {
            input = ReadKeyboardInput() + hudInput;
            // Cada componente a [-1,1] por si teclado y D-pad apuntan igual
            input.x = Mathf.Clamp(input.x, -1f, 1f);
            input.y = Mathf.Clamp(input.y, -1f, 1f);

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

        /// <summary>Un botón del D-pad empezó a presionarse (HudMoveButton).</summary>
        public void AddHudDirection(Vector2 direction) => hudInput += direction;

        /// <summary>Un botón del D-pad se soltó (HudMoveButton).</summary>
        public void RemoveHudDirection(Vector2 direction) => hudInput -= direction;

        /// <summary>Fija de golpe el input del HUD (ej. un joystick virtual futuro).</summary>
        public void SetInput(Vector2 direction) => hudInput = direction;
    }
}
