using UnityEngine;
using UnityEngine.EventSystems;

namespace TravesiaACasa.Rooms
{
    /// <summary>
    /// Botón de dirección del D-pad en pantalla (las 4 flechas del HUD,
    /// ver capturas del prototipo). No usa Button.onClick porque un
    /// click recién dispara al SOLTAR: para caminar hay que empujar
    /// mientras el dedo está abajo, así que se implementa con
    /// IPointerDown/Up y se avisa al BirdPlayerController para que
    /// sume/reste esta dirección a su input.
    /// IPointerExit también suelta, para no quedar "caminando solo" si
    /// el dedo se arrastra fuera del botón antes de levantarse.
    /// </summary>
    public class HudMoveButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [SerializeField] private BirdPlayerController player;
        [SerializeField] private Vector2 direction = Vector2.up;

        private bool pressed;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (pressed || player == null) return;
            pressed = true;
            player.AddHudDirection(direction);
        }

        public void OnPointerUp(PointerEventData eventData) => Release();

        public void OnPointerExit(PointerEventData eventData) => Release();

        private void OnDisable() => Release();

        private void Release()
        {
            if (!pressed || player == null) return;
            pressed = false;
            player.RemoveHudDirection(direction);
        }
    }
}
