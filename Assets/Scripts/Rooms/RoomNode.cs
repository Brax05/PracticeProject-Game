using System.Collections.Generic;
using UnityEngine;

namespace TravesiaACasa.Rooms
{
    /// <summary>
    /// Versión "grafo libre" de RoomData: en vez de 4 direcciones fijas
    /// (norte/sur/este/oeste), cada room tiene una lista de rooms
    /// conectadas — porque en el boceto las salidas no siempre son
    /// cardinales (ej: room 2 sale hacia 3 y hacia 5, con flechas en
    /// ángulos distintos).
    ///
    /// Para el prototipo del cubo, worldPosition es simplemente dónde
    /// se coloca el cubo en la escena de prueba al entrar a esa room
    /// (más adelante, cuando haya arte, esto se reemplaza por el
    /// roomPrefab + Entry points como en RoomData.cs).
    /// </summary>
    [CreateAssetMenu(menuName = "Game/RoomNode", fileName = "RoomNode_")]
    public class RoomNode : ScriptableObject
    {
        [Tooltip("Id de la room, ej: 1, 2, 3... según tu boceto")]
        public string roomId;

        [Tooltip("Posición en la escena de prueba donde aparece el cubo al entrar a esta room")]
        public Vector3 testWorldPosition;

        [Tooltip("Rooms conectadas directamente a esta (bidireccional, se puede volver por el mismo camino)")]
        public List<RoomNode> connections = new List<RoomNode>();

        public bool IsConnectedTo(RoomNode other) => connections.Contains(other);
    }
}
