using System;
using System.Collections.Generic;
using UnityEngine;

namespace TravesiaACasa.Rooms
{
    /// <summary>
    /// Inventario simple por conteo (no slots), pensado para el flujo
    /// de "recolecta N materiales" que se ve en las pantallas de Misión.
    /// Ej: InventoryManager.Instance.Add("tapa_botella") suma 1 al conteo
    /// de ese tipo de item.
    /// </summary>
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance { get; private set; }

        private readonly Dictionary<string, int> counts = new Dictionary<string, int>();

        /// <summary>Se dispara cada vez que cambia el conteo de un item.</summary>
        public event Action<string, int> ItemCountChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void Add(string itemKey, int amount = 1)
        {
            if (!counts.ContainsKey(itemKey)) counts[itemKey] = 0;
            counts[itemKey] += amount;
            ItemCountChanged?.Invoke(itemKey, counts[itemKey]);
        }

        public int GetCount(string itemKey) => counts.TryGetValue(itemKey, out int c) ? c : 0;

        public bool HasAtLeast(string itemKey, int amount) => GetCount(itemKey) >= amount;
    }
}
