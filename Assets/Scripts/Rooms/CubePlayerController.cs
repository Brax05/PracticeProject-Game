using UnityEngine;

namespace TravesiaACasa.Rooms
{
    /// <summary>
    /// Movimiento de prueba con un cubo placeholder (reemplazar luego
    /// por el controller real del Yal austral con sprite/animaciones).
    /// Usa Rigidbody2D para que los triggers de salida (RoomExitPoint)
    /// funcionen con OnTriggerEnter2D.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class CubePlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 4f;

        private Rigidbody2D rb;
        private Vector2 input;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0f; // vista top-down, no queremos caída
        }

        private void Update()
        {
            // Soporta tanto WASD/flechas como el D-pad de UI si más
            // adelante conectas botones (usa SetInput desde ahí).
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");
        }

        private void FixedUpdate()
        {
            // Nota: si usas Unity 6+, puedes cambiar esto por rb.linearVelocity
            rb.linearVelocity = input.normalized * moveSpeed;
        }

        /// <summary>Para conectar botones de D-pad en pantalla en vez de teclado.</summary>
        public void SetInput(Vector2 direction)
        {
            input = direction;
        }
    }
}
