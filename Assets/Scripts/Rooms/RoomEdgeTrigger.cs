using UnityEngine;

namespace TravesiaACasa.Rooms
{
    /// <summary>
    /// Se coloca en cada uno de los 4 bordes del roomPrefab (como
    /// BoxCollider2D en modo Trigger). Cuando el jugador lo toca,
    /// pide al RoomManager la transición hacia el vecino correspondiente.
    ///
    /// Si la RoomData actual no tiene conexión en esa dirección
    /// (GetNeighbor devuelve null), el trigger simplemente no hace nada,
    /// así que sirve también como "muro invisible" en los bordes del mapa.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class RoomEdgeTrigger : MonoBehaviour
    {
        [Tooltip("Por qué borde de ESTA room se sale al tocar el trigger")]
        [SerializeField] private Direction direction;

        [SerializeField] private string playerTag = "Player";

        private void Reset()
        {
            GetComponent<Collider2D>().isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(playerTag)) return;
            if (RoomManager.Instance == null || RoomManager.Instance.CurrentRoom == null) return;

            RoomData target = RoomManager.Instance.CurrentRoom.GetNeighbor(direction);
            if (target == null) return; // borde del mapa, no hay a dónde ir

            RoomManager.Instance.TransitionTo(target, direction);
        }
    }
}
