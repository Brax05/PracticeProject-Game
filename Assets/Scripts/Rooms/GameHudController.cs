using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace TravesiaACasa.Rooms
{
    /// <summary>
    /// Cerebro del HUD del juego (capturas del prototipo: D-pad a la
    /// izquierda, Interactuar/Picotear a la derecha, ruedita arriba a la
    /// derecha). El D-pad va directo a BirdPlayerController vía
    /// HudMoveButton; aquí vive lo demás:
    ///
    /// - Ruedita/Escape: abre y cierra el panel de Configuración pausando
    ///   el juego con Time.timeScale = 0 (FixedUpdate se detiene, así que
    ///   el ave no camina mientras el panel está abierto).
    /// - Interactuar y Picotear: por ahora solo disparan UnityEvents,
    ///   para que al armar cada room (diálogo del carpinterito, recoger
    ///   comida, etc.) se enganche la lógica desde el Inspector sin
    ///   tocar este script.
    /// </summary>
    public class GameHudController : MonoBehaviour
    {
        [Header("Panel de Configuración (dentro del HUD)")]
        [SerializeField] private GameObject settingsPanelRoot;

        [Header("Acciones (enganchar la lógica de cada room aquí)")]
        public UnityEvent onInteract;
        public UnityEvent onPeck;

        private void Update()
        {
            Keyboard kb = Keyboard.current;
            if (kb != null && kb.escapeKey.wasPressedThisFrame)
                ToggleSettings();
        }

        public void OnOpenSettingsClicked() => SetSettingsOpen(true);

        public void OnCloseSettingsClicked() => SetSettingsOpen(false);

        public void ToggleSettings()
        {
            if (settingsPanelRoot == null) return;
            SetSettingsOpen(!settingsPanelRoot.activeSelf);
        }

        private void SetSettingsOpen(bool open)
        {
            if (settingsPanelRoot == null) return;
            settingsPanelRoot.SetActive(open);
            Time.timeScale = open ? 0f : 1f;
        }

        private void OnDestroy()
        {
            // Por si se cambia de escena con el panel abierto
            Time.timeScale = 1f;
        }

        public void OnInteractClicked()
        {
            Debug.Log("[GameHud] Interactuar");
            onInteract?.Invoke();
        }

        public void OnPeckClicked()
        {
            Debug.Log("[GameHud] Picotear");
            onPeck?.Invoke();
        }
    }
}
