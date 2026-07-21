using UnityEngine;

namespace TravesiaACasa.Rooms
{
    /// <summary>
    /// Poner este componente en cada objeto recolectable dentro de un
    /// roomPrefab (ej: la flor con néctar, la tapa de botella, el caracol
    /// marcados en rojo en el mapa).
    ///
    /// Se auto-desactiva al iniciar si ya fue recogido antes. Al
    /// interactuar/picotear, avisa al CollectibleManager e InventoryManager
    /// y se destruye.
    /// </summary>
    public class CollectibleItem : MonoBehaviour
    {
        [Tooltip("Id único de esta instancia dentro de la room, ej: flor_nectar_01")]
        [SerializeField] private string itemId;

        [Tooltip("Tipo de item para el inventario/misión, ej: flor_nectar, tapa_botella, caracol")]
        [SerializeField] private string inventoryKey;

        private string roomId;

        private void Start()
        {
            roomId = RoomGraphManager.Instance != null && RoomGraphManager.Instance.CurrentNode != null
                ? RoomGraphManager.Instance.CurrentNode.roomId
                : "unknown_room";

            if (CollectibleManager.Instance != null && CollectibleManager.Instance.IsCollected(roomId, itemId))
            {
                gameObject.SetActive(false);
            }
        }

        /// <summary>Llamar desde el botón de Interactuar/Picotear cuando el jugador esté sobre este objeto.</summary>
        public void Collect()
        {
            CollectibleManager.Instance?.Collect(roomId, itemId, inventoryKey);
            gameObject.SetActive(false);
            // Si prefieres una animación de recogida antes de desactivar,
            // reemplaza la línea de arriba por un Destroy(gameObject, delay)
            // o dispara aquí un trigger de Animator.
        }
    }
}
