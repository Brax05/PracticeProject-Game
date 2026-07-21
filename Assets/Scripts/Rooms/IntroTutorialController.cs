using UnityEngine;
using UnityEngine.InputSystem;

namespace TravesiaACasa.Rooms
{
    /// <summary>
    /// Secuencia de bienvenida de la room inicial (room 1). Muestra por
    /// pasos, avanzando con un toque/clic o Espacio/Enter:
    ///   Paso 0: diálogo de intro ("Despertaste tras una larga siesta...").
    ///   Paso 1: carteles que explican los controles en pantalla.
    ///   Paso 2: se oculta todo y el tutorial queda terminado.
    ///
    /// Vive en el HUD (que no se destruye al cambiar de room), así que
    /// solo se reproduce una vez: si el jugador vuelve a room 1 más
    /// tarde, el tutorial ya está en el paso final y no reaparece.
    /// </summary>
    public class IntroTutorialController : MonoBehaviour
    {
        [Header("Paneles")]
        [Tooltip("Cuadro de diálogo de intro (paso 0).")]
        [SerializeField] private GameObject introPanel;

        [Tooltip("Contenedor con los carteles de controles (paso 1).")]
        [SerializeField] private GameObject controlesHints;

        private int phase; // 0 = intro, 1 = controles, 2 = terminado

        private void Start()
        {
            phase = 0;
            if (introPanel != null) introPanel.SetActive(true);
            if (controlesHints != null) controlesHints.SetActive(false);
        }

        private void Update()
        {
            if (phase >= 2) return;
            if (AdvancePressed()) Advance();
        }

        private void Advance()
        {
            phase++;
            if (phase == 1)
            {
                if (introPanel != null) introPanel.SetActive(false);
                if (controlesHints != null) controlesHints.SetActive(true);
            }
            else // paso 2: tutorial terminado
            {
                if (controlesHints != null) controlesHints.SetActive(false);
            }
        }

        /// <summary>True el frame en que el jugador pide avanzar (toque, clic o Espacio/Enter).</summary>
        private static bool AdvancePressed()
        {
            Keyboard kb = Keyboard.current;
            if (kb != null && (kb.spaceKey.wasPressedThisFrame || kb.enterKey.wasPressedThisFrame))
                return true;

            Mouse mouse = Mouse.current;
            if (mouse != null && mouse.leftButton.wasPressedThisFrame)
                return true;

            Touchscreen touch = Touchscreen.current;
            if (touch != null && touch.primaryTouch.press.wasPressedThisFrame)
                return true;

            return false;
        }
    }
}
