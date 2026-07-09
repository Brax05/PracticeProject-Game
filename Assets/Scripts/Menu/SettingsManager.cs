using UnityEngine;

namespace TravesiaACasa.Menu
{
    /// <summary>
    /// Valores de la pantalla de Configuración (ver boceto en
    /// assets juego aves/configuración/configuración.png: Sonido
    /// [Ambiente/Personajes/Cinemática], Brillo, Modo daltónico,
    /// Vibración). Se guarda en PlayerPrefs igual que CollectibleManager,
    /// como placeholder simple hasta que exista un AudioManager real
    /// que consuma estos valores.
    /// </summary>
    public class SettingsManager : MonoBehaviour
    {
        public static SettingsManager Instance { get; private set; }

        private const string KeyAmbiente = "settings_vol_ambiente";
        private const string KeyPersonajes = "settings_vol_personajes";
        private const string KeyCinematica = "settings_vol_cinematica";
        private const string KeyBrillo = "settings_brillo";
        private const string KeyDaltonico = "settings_modo_daltonico";
        private const string KeyVibracion = "settings_vibracion";

        public float AmbienteVolume
        {
            get => PlayerPrefs.GetFloat(KeyAmbiente, 1f);
            set => PlayerPrefs.SetFloat(KeyAmbiente, value);
        }

        public float PersonajesVolume
        {
            get => PlayerPrefs.GetFloat(KeyPersonajes, 1f);
            set => PlayerPrefs.SetFloat(KeyPersonajes, value);
        }

        public float CinematicaVolume
        {
            get => PlayerPrefs.GetFloat(KeyCinematica, 1f);
            set => PlayerPrefs.SetFloat(KeyCinematica, value);
        }

        public float Brillo
        {
            get => PlayerPrefs.GetFloat(KeyBrillo, 1f);
            set => PlayerPrefs.SetFloat(KeyBrillo, value);
        }

        public bool ModoDaltonico
        {
            get => PlayerPrefs.GetInt(KeyDaltonico, 0) == 1;
            set => PlayerPrefs.SetInt(KeyDaltonico, value ? 1 : 0);
        }

        public bool Vibracion
        {
            get => PlayerPrefs.GetInt(KeyVibracion, 0) == 1;
            set => PlayerPrefs.SetInt(KeyVibracion, value ? 1 : 0);
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void Save() => PlayerPrefs.Save();
    }
}
