using UnityEngine;

namespace TravesiaACasa.Rooms
{
    /// <summary>
    /// Colocar uno de estos por cada flecha morada del boceto: un
    /// trigger que, al pisarlo el jugador, pide a RoomGraphManager
    /// viajar al RoomNode destino. Como RoomGraphManager valida contra
    /// el grafo, si te equivocas de conexión simplemente no pasa nada
    /// (revisa la consola).
    ///
    /// Usa OnTriggerStay2D + chequeo de dirección (en vez de un simple
    /// OnTriggerEnter2D): la salida solo se dispara si el jugador se
    /// está moviendo HACIA la room destino. Desde que la transición es
    /// un desplazamiento continuo, el jugador aterriza junto a la
    /// puerta, encima o al lado del trigger de vuelta; sin este chequeo,
    /// seguir de largo lo regresaba a la room anterior (ping-pong).
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class RoomExitPoint : MonoBehaviour
    {
        [Tooltip("A qué RoomNode lleva esta salida")]
        [SerializeField] private RoomNode targetNode;

        [Tooltip("(Opcional) Dónde queda exactamente el jugador dentro de la room destino. " +
                 "Si se deja vacío, el jugador simplemente avanza un poco hacia la room nueva.")]
        [SerializeField] private Transform entryPoint;

        [SerializeField] private string playerTag = "Player";

        private void Reset()
        {
            GetComponent<Collider2D>().isTrigger = true;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!other.CompareTag(playerTag)) return;

            RoomGraphManager manager = RoomGraphManager.Instance;
            if (manager == null) return;
            if (targetNode == null || targetNode == manager.CurrentNode) return;

            if (!PlayerIsMovingTowardTarget(other, manager)) return;

            manager.TravelTo(
                targetNode,
                entryPoint != null ? entryPoint.position : (Vector3?)null);
        }

        private bool PlayerIsMovingTowardTarget(Collider2D playerCollider, RoomGraphManager manager)
        {
            Rigidbody2D body = playerCollider.attachedRigidbody;
            if (body == null || manager.CurrentNode == null)
                return true; // sin datos para comparar, comportarse como antes

            Vector2 toTarget = targetNode.testWorldPosition - manager.CurrentNode.testWorldPosition;
            if (toTarget.sqrMagnitude < 0.0001f)
                return true; // nodos encimados, no hay dirección útil

            Vector2 velocity = body.linearVelocity;
            if (velocity.sqrMagnitude < 0.01f)
                return false; // parado sobre el trigger: no viajar

            return Vector2.Dot(velocity.normalized, toTarget.normalized) > 0.1f;
        }
    }
}
