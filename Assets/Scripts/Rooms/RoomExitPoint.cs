using UnityEngine;

namespace TravesiaACasa.Rooms
{
    /// <summary>
    /// Colocar uno de estos por cada flecha morada del boceto: un
    /// trigger que, al tocarlo el cubo, pide a RoomGraphManager viajar
    /// al RoomNode destino. Como RoomGraphManager valida contra el
    /// grafo, si te equivocas de conexión simplemente no pasa nada
    /// (revisa la consola).
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class RoomExitPoint : MonoBehaviour
    {
        [Tooltip("A qué RoomNode lleva esta salida")]
        [SerializeField] private RoomNode targetNode;

        [SerializeField] private string playerTag = "Player";

        private void Reset()
        {
            GetComponent<Collider2D>().isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(playerTag)) return;
            RoomGraphManager.Instance?.TravelTo(targetNode);
        }
    }
}
