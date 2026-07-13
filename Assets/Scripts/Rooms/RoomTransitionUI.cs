using UnityEngine;
using UnityEngine.UI;

namespace TravesiaACasa.Rooms
{
    /// <summary>
    /// Etiqueta con el id de la room actual, útil mientras se prueba
    /// el grafo sin arte de UI de mapa final.
    /// </summary>
    public class RoomTransitionUI : MonoBehaviour
    {
        [SerializeField] private Text roomLabel;

        private void OnEnable()
        {
            if (RoomGraphManager.Instance != null)
                RoomGraphManager.Instance.NodeChanged += OnNodeChanged;
        }

        private void OnDisable()
        {
            if (RoomGraphManager.Instance != null)
                RoomGraphManager.Instance.NodeChanged -= OnNodeChanged;
        }

        private void OnNodeChanged(RoomNode node)
        {
            if (roomLabel != null)
                roomLabel.text = $"Room {node.roomId}";
        }
    }
}
