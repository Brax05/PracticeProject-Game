using UnityEngine;
using UnityEngine.UI;

namespace TravesiaACasa.Menu
{
    /// <summary>
    /// Aplica el slider "Brillo" de Configuración de verdad: una Image
    /// negra que cubre toda la pantalla (último hijo del Canvas, encima
    /// de todo, raycastTarget desactivado para no bloquear botones) cuyo
    /// alpha sube a medida que el brillo baja. Es la forma estándar de
    /// simular brillo en un juego 2D móvil sin tocar la config del
    /// dispositivo. maxDarkness limita cuánto puede oscurecerse para que
    /// brillo 0 nunca deje la pantalla 100% negra e injugable.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class BrightnessOverlay : MonoBehaviour
    {
        [SerializeField, Range(0f, 1f)] private float maxDarkness = 0.85f;

        private Image image;

        private void Awake()
        {
            image = GetComponent<Image>();
            image.raycastTarget = false;
        }

        private void OnEnable()
        {
            if (SettingsManager.Instance != null)
                SettingsManager.Instance.Changed += Apply;
            Apply();
        }

        private void OnDisable()
        {
            if (SettingsManager.Instance != null)
                SettingsManager.Instance.Changed -= Apply;
        }

        private void Apply()
        {
            float brillo = SettingsManager.Instance != null ? SettingsManager.Instance.Brillo : 1f;
            image.color = new Color(0f, 0f, 0f, (1f - Mathf.Clamp01(brillo)) * maxDarkness);
        }
    }
}
