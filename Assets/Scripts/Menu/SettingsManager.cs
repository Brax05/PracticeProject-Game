using System;
using UnityEngine;

namespace TravesiaACasa.Menu
{
    /// <summary>
    /// Valores de la pantalla de Configuración (ver boceto en
    /// assets juego aves/configuración/configuración.png: Sonido
    /// [Ambiente/Personajes/Cinemática], Brillo, Modo daltónico,
    /// Vibración). Se guarda en PlayerPrefs igual que CollectibleManager.
    ///
    /// Es persistente entre escenas (DontDestroyOnLoad) y se
    /// auto-instancia antes de cargar cualquier escena, así el panel de
    /// Configuración funciona igual abierto desde el menú que desde el
    /// HUD del juego, sin depender de que cada escena recuerde crear el
    /// objeto. Dispara Changed cada vez que un valor cambia para que
    /// consumidores (BrightnessOverlay, un futuro AudioManager) se
    /// actualicen al instante.
    /// </summary>
    public class SettingsManager : MonoBehaviour
    {
        public static SettingsManager Instance { get; private set; }

        /// <summary>Se dispara cada vez que cualquier ajuste cambia de valor.</summary>
        public event Action Changed;

        private const string KeyAmbiente = "settings_vol_ambiente";
        private const string KeyPersonajes = "settings_vol_personajes";
        private const string KeyCinematica = "settings_vol_cinematica";
        private const string KeyBrillo = "settings_brillo";
        private const string KeyDaltonico = "settings_modo_daltonico";
        private const string KeyVibracion = "settings_vibracion";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap()
        {
            if (Instance != null) return;
            var go = new GameObject("SettingsManager (Auto)");
            go.AddComponent<SettingsManager>();
        }

        public float AmbienteVolume
        {
            get => PlayerPrefs.GetFloat(KeyAmbiente, 1f);
            set { PlayerPrefs.SetFloat(KeyAmbiente, value); Changed?.Invoke(); }
        }

        public float PersonajesVolume
        {
            get => PlayerPrefs.GetFloat(KeyPersonajes, 1f);
            set { PlayerPrefs.SetFloat(KeyPersonajes, value); Changed?.Invoke(); }
        }

        public float CinematicaVolume
        {
            get => PlayerPrefs.GetFloat(KeyCinematica, 1f);
            set { PlayerPrefs.SetFloat(KeyCinematica, value); Changed?.Invoke(); }
        }

        public float Brillo
        {
            get => PlayerPrefs.GetFloat(KeyBrillo, 1f);
            set { PlayerPrefs.SetFloat(KeyBrillo, value); Changed?.Invoke(); }
        }

        public bool ModoDaltonico
        {
            get => PlayerPrefs.GetInt(KeyDaltonico, 0) == 1;
            set { PlayerPrefs.SetInt(KeyDaltonico, value ? 1 : 0); Changed?.Invoke(); }
        }

        public bool Vibracion
        {
            get => PlayerPrefs.GetInt(KeyVibracion, 0) == 1;
            set { PlayerPrefs.SetInt(KeyVibracion, value ? 1 : 0); Changed?.Invoke(); }
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Save() => PlayerPrefs.Save();
    }
}
