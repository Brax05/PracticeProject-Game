using UnityEngine;
using UnityEngine.UI;

namespace TravesiaACasa.Menu
{
    /// <summary>
    /// Conecta los 4 sliders y los 2 botones-interruptor del panel de
    /// Configuración (assets juego aves/configuración/) con SettingsManager.
    /// Los interruptores no usan el componente Toggle de Unity: son un
    /// Button que alterna manualmente entre el sprite "boton marron"
    /// (apagado) y "boton naranjo" (encendido), que es el par de assets
    /// que entregó la diseñadora para este control.
    /// </summary>
    public class SettingsPanelController : MonoBehaviour
    {
        [Header("Sliders (Sonido + Brillo)")]
        [SerializeField] private Slider ambienteSlider;
        [SerializeField] private Slider personajesSlider;
        [SerializeField] private Slider cinematicaSlider;
        [SerializeField] private Slider brilloSlider;

        [Header("Interruptores")]
        [SerializeField] private Button modoDaltonicoButton;
        [SerializeField] private Image modoDaltonicoImage;
        [SerializeField] private Button vibracionButton;
        [SerializeField] private Image vibracionImage;

        [Header("Sprites de interruptor")]
        [SerializeField] private Sprite toggleOffSprite; // boton marron
        [SerializeField] private Sprite toggleOnSprite;  // boton naranjo

        private SettingsManager settings;

        private void OnEnable()
        {
            settings = SettingsManager.Instance;
            if (settings == null) return;

            ambienteSlider.SetValueWithoutNotify(settings.AmbienteVolume);
            personajesSlider.SetValueWithoutNotify(settings.PersonajesVolume);
            cinematicaSlider.SetValueWithoutNotify(settings.CinematicaVolume);
            brilloSlider.SetValueWithoutNotify(settings.Brillo);

            RefreshToggle(modoDaltonicoImage, settings.ModoDaltonico);
            RefreshToggle(vibracionImage, settings.Vibracion);

            ambienteSlider.onValueChanged.AddListener(OnAmbienteChanged);
            personajesSlider.onValueChanged.AddListener(OnPersonajesChanged);
            cinematicaSlider.onValueChanged.AddListener(OnCinematicaChanged);
            brilloSlider.onValueChanged.AddListener(OnBrilloChanged);
            modoDaltonicoButton.onClick.AddListener(OnToggleModoDaltonico);
            vibracionButton.onClick.AddListener(OnToggleVibracion);
        }

        private void OnDisable()
        {
            if (settings == null) return;
            settings.Save();

            ambienteSlider.onValueChanged.RemoveListener(OnAmbienteChanged);
            personajesSlider.onValueChanged.RemoveListener(OnPersonajesChanged);
            cinematicaSlider.onValueChanged.RemoveListener(OnCinematicaChanged);
            brilloSlider.onValueChanged.RemoveListener(OnBrilloChanged);
            modoDaltonicoButton.onClick.RemoveListener(OnToggleModoDaltonico);
            vibracionButton.onClick.RemoveListener(OnToggleVibracion);
        }

        private void OnAmbienteChanged(float v) => settings.AmbienteVolume = v;
        private void OnPersonajesChanged(float v) => settings.PersonajesVolume = v;
        private void OnCinematicaChanged(float v) => settings.CinematicaVolume = v;
        private void OnBrilloChanged(float v) => settings.Brillo = v;

        private void OnToggleModoDaltonico()
        {
            settings.ModoDaltonico = !settings.ModoDaltonico;
            RefreshToggle(modoDaltonicoImage, settings.ModoDaltonico);
        }

        private void OnToggleVibracion()
        {
            settings.Vibracion = !settings.Vibracion;
            RefreshToggle(vibracionImage, settings.Vibracion);
        }

        private void RefreshToggle(Image image, bool on)
        {
            image.sprite = on ? toggleOnSprite : toggleOffSprite;
        }
    }
}
