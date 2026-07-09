using System;
using UnityEngine;

namespace TravesiaACasa.Rooms
{
    /// <summary>
    /// Controla en qué RoomNode está el jugador y valida que solo se
    /// pueda mover a rooms realmente conectadas en el grafo.
    /// Para el prototipo del cubo, "moverse a una room" = teletransportar
    /// al cubo a testWorldPosition del nodo destino (luego, con arte,
    /// esto se puede animar o usar el sistema de roomPrefab real).
    /// </summary>
    public class RoomGraphManager : MonoBehaviour
    {
        public static RoomGraphManager Instance { get; private set; }

        [Header("Nodo inicial")]
        [SerializeField] private RoomNode startingNode;

        [Header("Referencias")]
        [SerializeField] private Transform player;

        public RoomNode CurrentNode { get; private set; }

        /// <summary>Se dispara cada vez que el jugador entra a una nueva room.</summary>
        public event Action<RoomNode> NodeChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            if (startingNode != null)
                EnterNode(startingNode);
        }

        /// <summary>
        /// Intenta moverse al nodo destino. Si no está conectado al nodo
        /// actual en el grafo, no hace nada (evita "atajos" ilegales
        /// aunque algún trigger esté mal configurado).
        /// </summary>
        public void TravelTo(RoomNode target)
        {
            if (target == null) return;

            if (CurrentNode != null && !CurrentNode.IsConnectedTo(target))
            {
                Debug.LogWarning($"[RoomGraphManager] '{target.roomId}' no está conectado a " +
                                  $"'{CurrentNode.roomId}'. Revisa las conexiones en el RoomNode.");
                return;
            }

            EnterNode(target);
        }

        private void EnterNode(RoomNode node)
        {
            CurrentNode = node;

            if (player != null)
                player.position = node.testWorldPosition;

            NodeChanged?.Invoke(node);
        }
    }
}
