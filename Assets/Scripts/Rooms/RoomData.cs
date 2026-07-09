using UnityEngine;

namespace TravesiaACasa.Rooms
{
    /// <summary>
    /// Representa una celda del mapa (una "pantalla" fija, como las
    /// capturas de Travesía a casa). Se crea como asset desde el menú
    /// Assets > Create > Game > RoomData.
    ///
    /// Cada RoomData referencia un prefab que contiene el fondo
    /// ilustrado, los colliders del escenario y los colectables
    /// propios de esa celda.
    /// </summary>
    [CreateAssetMenu(menuName = "Game/RoomData", fileName = "Room_")]
    public class RoomData : ScriptableObject
    {
        [Header("Identidad")]
        [Tooltip("Identificador único de la room, ej: A1, B2, ZonaArida_03")]
        public string roomId;

        [Tooltip("Posición dentro de la cuadrícula del mapa. Solo referencia/debug.")]
        public Vector2Int gridPosition;

        [Header("Contenido")]
        [Tooltip("Prefab con el fondo, colliders y colectables de esta celda")]
        public GameObject roomPrefab;

        [Header("Conexiones")]
        public RoomData north;
        public RoomData south;
        public RoomData east;
        public RoomData west;

        /// <summary>
        /// Devuelve la room vecina en la dirección indicada, o null
        /// si no hay conexión (borde del mapa o zona bloqueada).
        /// </summary>
        public RoomData GetNeighbor(Direction dir)
        {
            switch (dir)
            {
                case Direction.North: return north;
                case Direction.South: return south;
                case Direction.East: return east;
                case Direction.West: return west;
                default: return null;
            }
        }
    }
}
