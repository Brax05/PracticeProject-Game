using System;
using UnityEngine;

namespace TravesiaACasa.Rooms
{
    /// <summary>
    /// Controla qué room está activa en escena. Es el único punto de
    /// entrada para hacer transiciones entre celdas del grid.
    ///
    /// Cada roomPrefab debe tener, como hijos directos, GameObjects
    /// vacíos llamados exactamente:
    ///   Entry_North, Entry_South, Entry_East, Entry_West
    /// marcando dónde debe aparecer el jugador según por qué borde entró.
    /// Solo es obligatorio tener los Entry_ que correspondan a bordes
    /// realmente conectados.
    /// </summary>
    public class RoomManager : MonoBehaviour
    {
        public static RoomManager Instance { get; private set; }

        [Header("Room inicial")]
        [SerializeField] private RoomData startingRoom;

        [Header("Referencias")]
        [SerializeField] private Transform player;

        public RoomData CurrentRoom { get; private set; }

        /// <summary>Se dispara cada vez que termina una transición de room.</summary>
        public event Action<RoomData> RoomChanged;

        private GameObject currentRoomInstance;

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
            if (startingRoom != null)
                LoadRoom(startingRoom, entryDirection: null);
        }

        /// <summary>
        /// Transición completa: destruye la room actual, instancia la
        /// nueva y posiciona al jugador en el Entry_ correspondiente.
        /// </summary>
        /// <param name="newRoom">Room destino.</param>
        /// <param name="fromDirection">
        /// Dirección por la que el jugador cruzó el borde en la room
        /// ANTERIOR (ej: si cruzó el borde Norte, pásale Direction.North).
        /// El manager calcula solo la dirección opuesta para ubicar
        /// el punto de entrada correcto en la room nueva.
        /// </param>
        public void TransitionTo(RoomData newRoom, Direction fromDirection)
        {
            if (newRoom == null)
            {
                Debug.LogWarning("[RoomManager] Intento de transición a una RoomData nula.");
                return;
            }

            Direction entrySide = DirectionUtils.Opposite(fromDirection);
            LoadRoom(newRoom, entrySide);
        }

        private void LoadRoom(RoomData room, Direction? entryDirection)
        {
            if (currentRoomInstance != null)
                Destroy(currentRoomInstance);

            if (room.roomPrefab == null)
            {
                Debug.LogError($"[RoomManager] RoomData '{room.roomId}' no tiene roomPrefab asignado.");
                return;
            }

            currentRoomInstance = Instantiate(room.roomPrefab);
            CurrentRoom = room;

            if (entryDirection.HasValue)
                PositionPlayerAtEntry(entryDirection.Value);

            RoomChanged?.Invoke(room);
        }

        private void PositionPlayerAtEntry(Direction entrySide)
        {
            if (player == null) return;

            string entryName = $"Entry_{entrySide}";
            Transform entryPoint = currentRoomInstance.transform.Find(entryName);

            if (entryPoint == null)
            {
                Debug.LogWarning($"[RoomManager] No se encontró '{entryName}' en '{CurrentRoom.roomId}'. " +
                                  "El jugador quedará en su posición actual.");
                return;
            }

            player.position = entryPoint.position;
        }
    }
}
