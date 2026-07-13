using UnityEngine;

namespace TravesiaACasa.Rooms
{
    /// <summary>
    /// Cámara 2D fija por room, estilo "pantalla fija": queda clavada
    /// en el centro del RoomNode actual y al cambiar de room corta en
    /// seco al centro del nodo nuevo. Sin seguimiento del jugador ni
    /// paneo: el encuadre nunca puede descuadrarse ni asomarse a la
    /// room vecina.
    /// </summary>
    public class CameraRoomFollower : MonoBehaviour
    {
        private void LateUpdate()
        {
            RoomNode current = RoomGraphManager.Instance != null ? RoomGraphManager.Instance.CurrentNode : null;
            if (current == null) return;

            Vector3 center = current.testWorldPosition;
            transform.position = new Vector3(center.x, center.y, transform.position.z);
        }
    }
}
