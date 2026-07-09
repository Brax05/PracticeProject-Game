using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TravesiaACasa.Rooms
{
    /// <summary>
    /// Feedback visual de que SÍ hubo una transición de room: un blink
    /// rápido a negro (para que el salto en seco de la cámara no se
    /// sienta como un glitch) y una etiqueta con el id de la room
    /// actual, útil mientras se prueba el grafo sin arte de
    /// transición/UI de mapa final.
    /// </summary>
    public class RoomTransitionUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup fadeGroup;
        [SerializeField] private Text roomLabel;
        [SerializeField] private float fadeDuration = 0.12f;

        private bool skippedFirst;
        private Coroutine fadeRoutine;

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

            if (!skippedFirst)
            {
                // La primera vez es la carga inicial, no una transición
                // real: no hace falta el blink, solo mostrar la etiqueta.
                skippedFirst = true;
                return;
            }

            if (fadeRoutine != null) StopCoroutine(fadeRoutine);
            fadeRoutine = StartCoroutine(Blink());
        }

        private IEnumerator Blink()
        {
            yield return Fade(0f, 1f);
            yield return Fade(1f, 0f);
        }

        private IEnumerator Fade(float from, float to)
        {
            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                fadeGroup.alpha = Mathf.Lerp(from, to, t / fadeDuration);
                yield return null;
            }
            fadeGroup.alpha = to;
        }
    }
}
