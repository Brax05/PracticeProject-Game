using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TravesiaACasa.Rooms
{
    /// <summary>
    /// Cerebro del HUD del juego: configuracion, acciones del jugador y
    /// elementos visuales superiores izquierdos.
    /// </summary>
    public class GameHudController : MonoBehaviour
    {
        [Header("Panel de Configuracion (dentro del HUD)")]
        [SerializeField] private GameObject settingsPanelRoot;

        [Header("HUD superior izquierdo")]
        [SerializeField] private Sprite topLeftAvatarSprite;
        [SerializeField] private Sprite topLeftHealthBarSprite;
        [SerializeField] private Sprite topLeftHeartSprite;
        [SerializeField] private Sprite topLeftMissionSprite;

        [Header("Controles de movimiento (se ocultan durante dialogos)")]
        [SerializeField] private GameObject dpad;
        [SerializeField] private GameObject interactuarBtn;
        [SerializeField] private GameObject picotearBtn;

        [Header("Acciones (enganchar la logica de cada room aqui)")]
        public UnityEvent onInteract;
        public UnityEvent onPeck;

        private void Awake()
        {
#if UNITY_EDITOR
            AutoAssignTopLeftSprites();
            AutoAssignMuteButtonSprite();
#endif
            TopLeftGameplayHud.Ensure(topLeftAvatarSprite, topLeftHealthBarSprite, topLeftHeartSprite, topLeftMissionSprite);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            AutoAssignTopLeftSprites();
            AutoAssignMuteButtonSprite();
            if (!Application.isPlaying)
            {
                TopLeftGameplayHud.Ensure(topLeftAvatarSprite, topLeftHealthBarSprite, topLeftHeartSprite, topLeftMissionSprite);
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
            }
        }

        private void Reset()
        {
            AutoAssignTopLeftSprites();
        }

        private void AutoAssignTopLeftSprites()
        {
            if (topLeftAvatarSprite == null)
                topLeftAvatarSprite = LoadEditorSprite("Assets/Arte/juego/UIIzquierda/AveAvatar.png");
            if (topLeftHealthBarSprite == null)
                topLeftHealthBarSprite = LoadEditorSprite("Assets/Arte/juego/UIIzquierda/SliderVida.png");
            if (topLeftHeartSprite == null)
                topLeftHeartSprite = LoadEditorSprite("Assets/Arte/juego/UIIzquierda/Corazon.png");
            if (topLeftMissionSprite == null)
                topLeftMissionSprite = LoadEditorSprite("Assets/Arte/juego/UIIzquierda/MisionLetrero.png");

            // El GameObject de GameHudController quedó sin hijos propios al
            // convertir el HUD en Prefab (Unity lo saca del prefab porque
            // onInteract apunta a un objeto de la escena, MissionBird), así
            // que Dpad/InteractuarBtn/PicotearBtn hay que buscarlos dentro
            // del Canvas del HUD, no como hijos directos de este componente.
            if (dpad == null)
                dpad = FindHudDescendant("Dpad");
            if (interactuarBtn == null)
                interactuarBtn = FindHudDescendant("InteractuarBtn");
            if (picotearBtn == null)
                picotearBtn = FindHudDescendant("PicotearBtn");
        }

        private static GameObject FindHudDescendant(string childName)
        {
            Canvas canvas = TopLeftGameplayHud.FindGameplayCanvas();
            if (canvas == null) return null;

            Transform found = TopLeftGameplayHud.FindDescendant(canvas.transform, childName);
            return found != null ? found.gameObject : null;
        }

        private static Sprite LoadEditorSprite(string assetPath)
        {
            foreach (Object asset in UnityEditor.AssetDatabase.LoadAllAssetsAtPath(assetPath))
            {
                if (asset is Sprite sprite)
                    return sprite;
            }

            Texture2D texture = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
            if (texture == null)
                return null;

            return Sprite.Create(
                texture,
                new Rect(0f, 0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                100f);
        }

        private void AutoAssignMuteButtonSprite()
        {
            Sprite muteSprite = LoadEditorSprite("Assets/Arte/juego/Mute.png");
            if (muteSprite == null)
                return;

            Canvas[] canvases = Object.FindObjectsByType<Canvas>();
            foreach (Canvas canvas in canvases)
            {
                Transform muteTransform = canvas.transform.Find("MuteBtn");
                if (muteTransform == null || !muteTransform.TryGetComponent(out Image image))
                    continue;

                image.sprite = muteSprite;
                image.preserveAspect = true;
                UnityEditor.EditorUtility.SetDirty(image);
                break;
            }
        }
#endif

        private void Update()
        {
            Keyboard kb = Keyboard.current;
            if (kb != null && kb.escapeKey.wasPressedThisFrame)
                ToggleSettings();
        }

        public void OnOpenSettingsClicked() => SetSettingsOpen(true);

        public void OnCloseSettingsClicked() => SetSettingsOpen(false);

        public void ToggleSettings()
        {
            if (settingsPanelRoot == null) return;
            SetSettingsOpen(!settingsPanelRoot.activeSelf);
        }

        private void SetSettingsOpen(bool open)
        {
            if (settingsPanelRoot == null) return;
            settingsPanelRoot.SetActive(open);
            Time.timeScale = open ? 0f : 1f;
        }

        private void OnDestroy()
        {
            Time.timeScale = 1f;
        }

        public void OnInteractClicked()
        {
            Debug.Log("[GameHud] Interactuar");
            onInteract?.Invoke();
        }

        public void OnPeckClicked()
        {
            Debug.Log("[GameHud] Picotear");
            onPeck?.Invoke();
        }

        public void OnMuteClicked()
        {
            AudioListener.pause = !AudioListener.pause;
            Debug.Log($"[GameHud] Mute {(AudioListener.pause ? "activado" : "desactivado")}");
        }

        /// <summary>
        /// Oculta/muestra el D-pad y el boton de Picotear (irrelevantes
        /// mientras se conversa). Lo llaman los NPCs de mision (ver
        /// MissionBird) al abrir/cerrar su cuadro de dialogo grande, para
        /// dejarle todo el protagonismo en pantalla.
        ///
        /// Interactuar se deja siempre visible a proposito: es el mismo
        /// boton que abre Y cierra el dialogo (OnInteractPressed lo
        /// alterna), asi que ocultarlo dejaria sin forma de cerrar el
        /// cuadro en tactil (no hay tecla E en un celular).
        /// </summary>
        public void SetGameplayControlsVisible(bool visible)
        {
            if (dpad != null) dpad.SetActive(visible);
            if (picotearBtn != null) picotearBtn.SetActive(visible);
        }
    }

    public static class TopLeftGameplayHud
    {
        private const string RootName = "TopLeftGameplayHud";

        public static void Ensure(Sprite avatarSprite, Sprite healthBarSprite, Sprite heartSprite, Sprite missionSprite)
        {
            Canvas canvas = FindGameplayCanvas();
            if (canvas == null)
                return;

            Transform existingRoot = canvas.transform.Find(RootName);
            if (existingRoot != null)
            {
                UpdateImage(existingRoot, "AveAvatar", avatarSprite);
                UpdateImage(existingRoot, "SliderVida", healthBarSprite);
                UpdateImage(existingRoot, "Corazon", heartSprite);
                UpdateImage(existingRoot, "MisionLetrero", missionSprite);
                return;
            }

            RectTransform root = CreateRect(canvas.transform, RootName);
            root.anchorMin = new Vector2(0f, 1f);
            root.anchorMax = new Vector2(0f, 1f);
            root.pivot = new Vector2(0f, 1f);
            root.anchoredPosition = Vector2.zero;
            root.sizeDelta = new Vector2(850f, 310f);
            root.SetAsFirstSibling();

            CreateImage(root, "AveAvatar", avatarSprite, new Vector2(14f, -14f), new Vector2(250f, 250f));
            RectTransform sliderVida = CreateImage(root, "SliderVida", healthBarSprite, new Vector2(280f, -54f), new Vector2(460f, 92f));
            CreateImage(sliderVida, "Corazon", heartSprite, new Vector2(412f, 24f), new Vector2(94f, 94f));
            CreateImage(root, "MisionLetrero", missionSprite, new Vector2(306f, -156f), new Vector2(360f, 126f));
        }

        private static void UpdateImage(Transform root, string name, Sprite sprite)
        {
            Transform child = FindDescendant(root, name);
            if (child == null || !child.TryGetComponent(out Image image))
                return;

            image.sprite = sprite;
            image.preserveAspect = true;
        }

        public static Transform FindDescendant(Transform root, string name)
        {
            if (root.name == name)
                return root;

            for (int i = 0; i < root.childCount; i++)
            {
                Transform found = FindDescendant(root.GetChild(i), name);
                if (found != null)
                    return found;
            }

            return null;
        }

        public static Canvas FindGameplayCanvas()
        {
            Canvas[] canvases = Object.FindObjectsByType<Canvas>();
            foreach (Canvas canvas in canvases)
            {
                // "HUD" es el nombre original en escena; "GameHUD" es como
                // quedó el root al convertirlo en Prefab (Game/Prefabs/...).
                if (canvas.name == "HUD" || canvas.name == "GameHUD")
                    return canvas;
            }

            return canvases.Length > 0 ? canvases[0] : null;
        }

        private static RectTransform CreateRect(Transform parent, string name)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go.GetComponent<RectTransform>();
        }

        private static RectTransform CreateImage(RectTransform parent, string name, Sprite sprite, Vector2 topLeftPosition, Vector2 size)
        {
            RectTransform rt = CreateRect(parent, name);
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0f, 1f);
            rt.anchoredPosition = topLeftPosition;
            rt.sizeDelta = size;

            Image image = rt.gameObject.AddComponent<Image>();
            image.sprite = sprite;
            image.preserveAspect = true;
            image.raycastTarget = false;

            if (image.sprite == null)
                Debug.LogWarning($"[TopLeftGameplayHud] Falta asignar el sprite '{name}' en GameHudController.");

            return rt;
        }
    }
}
