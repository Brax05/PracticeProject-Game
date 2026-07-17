using UnityEngine;
using UnityEngine.InputSystem;

namespace TravesiaACasa.Rooms
{
    /// <summary>
    /// Ave (ave2/Canastero) fija dentro de una room que hace de NPC de
    /// misión. Mientras tiene una misión pendiente muestra un globo de
    /// diálogo encima; si el jugador se acerca y pulsa Interactuar (el
    /// botón del HUD dispara <see cref="OnInteractPressed"/> vía
    /// GameHudController.onInteract, o la tecla E en teclado) se abre el
    /// cuadro de diálogo grande del Canastero ("Holaaaa!!!!! pio pio").
    /// Otra pulsación lo cierra.
    ///
    /// El globo y el panel son GameObjects aparte asignados por
    /// Inspector: el ave no se mueve, así que basta activarlos y
    /// desactivarlos.
    /// </summary>
    public class MissionBird : MonoBehaviour
    {
        [Header("Diálogo")]
        [Tooltip("Globo de texto que aparece encima del ave cuando hay una misión activa.")]
        [SerializeField] private GameObject dialogueBubble;

        [Tooltip("Panel de conversación grande (en el HUD) que se abre al interactuar.")]
        [SerializeField] private GameObject dialoguePanel;

        [Tooltip("Distancia máxima (en unidades de mundo) a la que el jugador puede interactuar.")]
        [SerializeField] private float interactRadius = 3f;

        [Tooltip("Si está marcado, el ave arranca ya con una misión que ofrecer (globo visible).")]
        [SerializeField] private bool hasMissionOnStart = true;

        /// <summary>True mientras el ave tiene una misión pendiente para el jugador.</summary>
        public bool HasMission { get; private set; }

        /// <summary>True mientras el panel de conversación está abierto.</summary>
        public bool DialogueOpen => dialoguePanel != null && dialoguePanel.activeSelf;

        private Transform player;
        private GameHudController hud;

        private void Start()
        {
            // El jugador vive en la raíz de la escena (Jugador_Yal); se
            // busca una sola vez en vez de referenciarlo por Inspector
            // para que el ave funcione igual si se convierte en prefab.
            BirdPlayerController found = FindFirstObjectByType<BirdPlayerController>();
            if (found != null) player = found.transform;
            hud = FindFirstObjectByType<GameHudController>();

            if (dialoguePanel != null) dialoguePanel.SetActive(false);
            SetMission(hasMissionOnStart);
        }

        private void Update()
        {
            // Atajo de teclado para escritorio; el botón táctil entra por OnInteractPressed().
            Keyboard kb = Keyboard.current;
            if (kb != null && kb.eKey.wasPressedThisFrame)
                OnInteractPressed();
        }

        /// <summary>
        /// Punto de entrada del botón Interactuar (enganchado en
        /// GameHudController.onInteract). Abre la conversación si el
        /// jugador está lo bastante cerca; si ya está abierta, la cierra.
        /// </summary>
        public void OnInteractPressed()
        {
            if (dialoguePanel == null) return;

            if (DialogueOpen)
            {
                CloseDialogue();
                return;
            }

            if (!HasMission || !PlayerInRange()) return;

            dialoguePanel.SetActive(true);
            if (dialogueBubble != null) dialogueBubble.SetActive(false);
            hud?.SetGameplayControlsVisible(false);
        }

        private void CloseDialogue()
        {
            dialoguePanel.SetActive(false);
            // La misión sigue pendiente hasta que el sistema de misiones
            // llame a CompleteMission(): el globo vuelve a mostrarse.
            if (dialogueBubble != null) dialogueBubble.SetActive(HasMission);
            hud?.SetGameplayControlsVisible(true);
        }

        private bool PlayerInRange()
        {
            if (player == null) return false;
            return Vector2.Distance(player.position, transform.position) <= interactRadius;
        }

        /// <summary>
        /// Activa o desactiva la misión y muestra/oculta el globo en
        /// consecuencia. Llamar desde el sistema de misiones cuando el
        /// ave deba ofrecer (true) o dejar de ofrecer (false) una misión.
        /// </summary>
        public void SetMission(bool active)
        {
            HasMission = active;
            if (dialogueBubble != null)
                dialogueBubble.SetActive(active && !DialogueOpen);
        }

        /// <summary>Marca la misión como entregada/completada: oculta globo y panel.</summary>
        public void CompleteMission()
        {
            if (DialogueOpen) dialoguePanel.SetActive(false);
            SetMission(false);
        }
    }
}
