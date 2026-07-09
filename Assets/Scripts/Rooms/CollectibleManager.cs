using System.Collections.Generic;
using UnityEngine;

namespace TravesiaACasa.Rooms
{
    /// <summary>
    /// Lleva el registro de qué colectables ya fueron recogidos, para
    /// que no reaparezcan si el jugador vuelve a una room ya visitada.
    ///
    /// Guarda en PlayerPrefs como respaldo simple; si más adelante
    /// necesitas un sistema de guardado más completo (JSON, slots, etc.)
    /// esta es la clase donde reemplazar la persistencia.
    /// </summary>
    public class CollectibleManager : MonoBehaviour
    {
        public static CollectibleManager Instance { get; private set; }

        private const string SaveKeyPrefix = "collected_";
        private readonly HashSet<string> collectedIds = new HashSet<string>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        /// <summary>Id único = roomId + itemId, ej: "A1_flor_nectar_01"</summary>
        private string BuildId(string roomId, string itemId) => $"{roomId}_{itemId}";

        public bool IsCollected(string roomId, string itemId)
        {
            string id = BuildId(roomId, itemId);
            if (collectedIds.Contains(id)) return true;

            // fallback por si el juego se reinició pero hay guardado previo
            if (PlayerPrefs.GetInt(SaveKeyPrefix + id, 0) == 1)
            {
                collectedIds.Add(id);
                return true;
            }
            return false;
        }

        public void Collect(string roomId, string itemId, string inventoryKey)
        {
            string id = BuildId(roomId, itemId);
            if (collectedIds.Contains(id)) return;

            collectedIds.Add(id);
            PlayerPrefs.SetInt(SaveKeyPrefix + id, 1);
            PlayerPrefs.Save();

            // inventoryKey es el tipo de objeto en sí (ej: "tapa_botella",
            // "caracol", "flor_nectar") para sumarlo al inventario/misión.
            InventoryManager.Instance?.Add(inventoryKey);
        }
    }
}
