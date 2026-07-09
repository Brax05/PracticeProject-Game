using UnityEngine;

namespace TravesiaACasa.Rooms
{
    /// <summary>
    /// Cámara 2D fija por room (sin scroll continuo, ver nota en
    /// GRAFO_README.md): en vez de seguir al jugador en tiempo real,
    /// se posiciona en seco sobre el nodo actual del grafo cada vez
    /// que cambia, dando el efecto de "pantalla fija" de las capturas
    /// del prototipo.
    /// </summary>
    public class CameraRoomFollower : MonoBehaviour
    {
        private void LateUpdate()
        {
            RoomNode current = RoomGraphManager.Instance != null ? RoomGraphManager.Instance.CurrentNode : null;
            if (current == null) return;

            Vector3 target = current.testWorldPosition;
            transform.position = new Vector3(target.x, target.y, transform.position.z);
        }
    }
}
