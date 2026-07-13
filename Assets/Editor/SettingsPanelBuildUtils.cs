using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TravesiaACasa.Menu;
using static TravesiaACasa.Rooms.Editor.RoomSceneBuildUtils;

namespace TravesiaACasa.Menu.Editor
{
    /// <summary>
    /// Constructor compartido del panel de configuracion. Lo usan el menu
    /// principal y el HUD del juego para mantener el mismo layout.
    /// </summary>
    public static class SettingsPanelBuildUtils
    {
        private const string ArtRoot = "Assets/assets juego/assets juego aves";

        private const float LeftLabelX = 0.15f;
        private const float LeftControlX = 0.34f;
        private const float RightLabelX = 0.66f;
        private const float RightControlX = 0.84f;
        private const float RowTop = 0.66f;
        private const float RowMiddle = 0.47f;
        private const float RowBottom = 0.28f;
        private const float SliderWidth = 340f;
        private const float SliderHandleWidth = 70f;
        private const float ToggleWidth = 120f;
        private const string ConfigTitlePath = ArtRoot + "/configuraci\u00f3n/slider_separado/configfondo.png";
        private const string SliderBarPath = ArtRoot + "/configuraci\u00f3n/slider_separado/nuevabarra.png";
        private const string SliderRemainderBarPath = ArtRoot + "/configuraci\u00f3n/slider_separado/nuevabarra_cafe.png";
        private const string LeafIconPath = ArtRoot + "/configuraci\u00f3n/slider_separado/ChatGPT Image 10 jul 2026, 11_09_23 (6).png";
        private const string BirdIconPath = ArtRoot + "/configuraci\u00f3n/slider_separado/ChatGPT Image 10 jul 2026, 11_09_25 (8).png";
        private const string PinkFlowerIconPath = ArtRoot + "/configuraci\u00f3n/slider_separado/0abd847e-7b1e-46e9-b7d2-d14733a6f4a1.png";
        private const string YellowFlowerIconPath = ArtRoot + "/configuraci\u00f3n/slider_separado/ChatGPT Image 10 jul 2026, 11_09_23 (4).png";

        private static readonly Vector2 SonidoTitleAnchor = new Vector2(LeftLabelX, 0.84f);
        private static readonly Vector2 OpcionesTitleAnchor = new Vector2(RightLabelX, 0.84f);
        private static readonly Vector2 AmbienteLabelAnchor = new Vector2(LeftLabelX, RowTop);
        private static readonly Vector2 PersonajesLabelAnchor = new Vector2(LeftLabelX, RowMiddle);
        private static readonly Vector2 CinematicaLabelAnchor = new Vector2(LeftLabelX, RowBottom);
        private static readonly Vector2 AmbienteAnchor = new Vector2(LeftControlX, RowTop);
        private static readonly Vector2 PersonajesAnchor = new Vector2(LeftControlX, RowMiddle);
        private static readonly Vector2 CinematicaAnchor = new Vector2(LeftControlX, RowBottom);
        private static readonly Vector2 DaltonicoLabelAnchor = new Vector2(RightLabelX, RowTop);
        private static readonly Vector2 VibracionLabelAnchor = new Vector2(RightLabelX, RowMiddle);
        private static readonly Vector2 BrilloLabelAnchor = new Vector2(RightLabelX, RowBottom);
        private static readonly Vector2 BrilloAnchor = new Vector2(RightControlX, RowBottom);
        private static readonly Vector2 DaltonicoAnchor = new Vector2(RightControlX, RowTop);
        private static readonly Vector2 VibracionAnchor = new Vector2(RightControlX, RowMiddle);
        private static readonly Vector2 VolverAnchor = new Vector2(0.055f, 0.92f);
        private static readonly Vector2 PanelAnchor = new Vector2(0.5f, 0.46f);
        private static readonly Vector2 PanelSize = new Vector2(1540f, 650f);

        public static readonly Vector2 Center = new Vector2(0.5f, 0.5f);

        public static Button BuildSettingsPanel(Transform canvasT, out GameObject root)
        {
            root = new GameObject("SettingsPanel", typeof(RectTransform));
            root.transform.SetParent(canvasT, false);
            StretchFull(root.GetComponent<RectTransform>());

            Image dim = CreateImage(root.transform, "Dim", null);
            StretchFull(dim.rectTransform);
            dim.color = new Color(0f, 0f, 0f, 0.6f);
            dim.raycastTarget = true;

            Sprite titleSprite = RequireSprite(ConfigTitlePath);
            Image title = CreateImage(root.transform, "TituloConfiguracion", titleSprite);
            PlaceUI(title.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -42f), SizeFromSprite(titleSprite, 820f));

            Image panel = CreateImage(root.transform, "PanelCafe", null);
            panel.color = new Color(0.56f, 0.47f, 0.37f, 0.96f);
            panel.raycastTarget = true;
            PlaceUI(panel.rectTransform, PanelAnchor, Center, Vector2.zero, PanelSize);
            Outline panelOutline = panel.gameObject.AddComponent<Outline>();
            panelOutline.effectColor = new Color(0.25f, 0.19f, 0.14f);
            panelOutline.effectDistance = new Vector2(5f, -5f);
            Transform panelT = panel.rectTransform;

            CreateLabel(panelT, "TituloSonido", "Sonido", 64, TextAnchor.MiddleLeft, SonidoTitleAnchor, new Vector2(300f, 80f));
            CreateLabel(panelT, "TituloOpciones", "Opciones", 64, TextAnchor.MiddleLeft, OpcionesTitleAnchor, new Vector2(360f, 80f));
            CreateLabel(panelT, "LabelAmbiente", "Ambiente", 42, TextAnchor.MiddleLeft, AmbienteLabelAnchor, new Vector2(300f, 64f));
            CreateLabel(panelT, "LabelPersonajes", "Personajes", 42, TextAnchor.MiddleLeft, PersonajesLabelAnchor, new Vector2(300f, 64f));
            CreateLabel(panelT, "LabelCinematica", "Cinematica", 42, TextAnchor.MiddleLeft, CinematicaLabelAnchor, new Vector2(300f, 64f));
            CreateLabel(panelT, "LabelModoDaltonico", "Modo daltonico", 42, TextAnchor.MiddleLeft, DaltonicoLabelAnchor, new Vector2(380f, 64f));
            CreateLabel(panelT, "LabelVibracion", "Vibracion", 42, TextAnchor.MiddleLeft, VibracionLabelAnchor, new Vector2(380f, 64f));
            CreateLabel(panelT, "LabelBrillo", "Brillo", 42, TextAnchor.MiddleLeft, BrilloLabelAnchor, new Vector2(380f, 64f));

            Image separator = CreateImage(panelT, "Separador", null);
            separator.color = new Color(0.35f, 0.30f, 0.24f, 0.9f);
            PlaceUI(separator.rectTransform, Center, Center, Vector2.zero, new Vector2(10f, 500f));

            Slider ambiente = CreateSlider(panelT, "SliderAmbiente", LeafIconPath, AmbienteAnchor);
            Slider personajes = CreateSlider(panelT, "SliderPersonajes", BirdIconPath, PersonajesAnchor);
            Slider cinematica = CreateSlider(panelT, "SliderCinematica", PinkFlowerIconPath, CinematicaAnchor);
            Slider brillo = CreateSlider(panelT, "SliderBrillo", YellowFlowerIconPath, BrilloAnchor);

            Sprite offSprite = LoadSprite($"{ArtRoot}/configuraci\u00f3n/boton marron.png");
            Sprite onSprite = LoadSprite($"{ArtRoot}/configuraci\u00f3n/boton naranjo.png");
            Button daltonicoBtn = CreateToggleButton(panelT, "ToggleModoDaltonico", offSprite, DaltonicoAnchor, out Image daltonicoImg);
            Button vibracionBtn = CreateToggleButton(panelT, "ToggleVibracion", offSprite, VibracionAnchor, out Image vibracionImg);

            Sprite flechaSprite = LoadSprite($"{ArtRoot}/juego/flecha.png");
            Button volverBtn = CreateButton(root.transform, "BotonVolver", flechaSprite);
            RectTransform volverRt = volverBtn.GetComponent<RectTransform>();
            PlaceUI(volverRt, VolverAnchor, Center, Vector2.zero, SizeFromSprite(flechaSprite, 90f));
            volverRt.localEulerAngles = new Vector3(0f, 0f, 90f);

            GameObject spcGO = new GameObject("SettingsPanelController");
            spcGO.transform.SetParent(root.transform, false);
            SettingsPanelController spc = spcGO.AddComponent<SettingsPanelController>();
            SetPrivateField(spc, "ambienteSlider", ambiente);
            SetPrivateField(spc, "personajesSlider", personajes);
            SetPrivateField(spc, "cinematicaSlider", cinematica);
            SetPrivateField(spc, "brilloSlider", brillo);
            SetPrivateField(spc, "modoDaltonicoButton", daltonicoBtn);
            SetPrivateField(spc, "modoDaltonicoImage", daltonicoImg);
            SetPrivateField(spc, "vibracionButton", vibracionBtn);
            SetPrivateField(spc, "vibracionImage", vibracionImg);
            SetPrivateField(spc, "toggleOffSprite", offSprite);
            SetPrivateField(spc, "toggleOnSprite", onSprite);

            return volverBtn;
        }

        public static void AddBrightnessOverlay(Transform canvasT)
        {
            var go = new GameObject("BrightnessOverlay", typeof(RectTransform), typeof(Image), typeof(BrightnessOverlay));
            go.transform.SetParent(canvasT, false);
            go.transform.SetAsLastSibling();
            StretchFull(go.GetComponent<RectTransform>());
            Image img = go.GetComponent<Image>();
            img.color = new Color(0f, 0f, 0f, 0f);
            img.raycastTarget = false;
        }

        private static Slider CreateSlider(Transform parent, string name, string iconPath, Vector2 anchor)
        {
            Sprite fillSprite = RequireSprite(SliderBarPath);
            Sprite remainderSprite = RequireSprite(SliderRemainderBarPath);
            Sprite handleSprite = RequireSprite(iconPath);
            Vector2 size = SizeFromSprite(fillSprite, SliderWidth);
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Slider));
            go.transform.SetParent(parent, false);
            PlaceUI(go.GetComponent<RectTransform>(), anchor, Center, Vector2.zero, size);

            return ConfigureSlider(go, fillSprite, remainderSprite, handleSprite, size);
        }

        private static Slider ConfigureSlider(GameObject go, Sprite fillSprite, Sprite remainderSprite, Sprite handleSprite, Vector2 size)
        {
            Image bgImage = go.GetComponent<Image>();
            bgImage.sprite = remainderSprite;
            bgImage.color = Color.white;
            bgImage.preserveAspect = true;
            bgImage.raycastTarget = true;

            Vector2 handleSize = SizeFromSprite(handleSprite, SliderHandleWidth);

            GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
            fillArea.transform.SetParent(go.transform, false);
            RectTransform fillAreaRt = fillArea.GetComponent<RectTransform>();
            fillAreaRt.anchorMin = Vector2.zero;
            fillAreaRt.anchorMax = Vector2.one;
            fillAreaRt.offsetMin = Vector2.zero;
            fillAreaRt.offsetMax = Vector2.zero;

            GameObject fillClip = new GameObject("Fill Clip", typeof(RectTransform), typeof(RectMask2D));
            fillClip.transform.SetParent(fillArea.transform, false);
            RectTransform fillClipRt = fillClip.GetComponent<RectTransform>();
            fillClipRt.anchorMin = Vector2.zero;
            fillClipRt.anchorMax = Vector2.one;
            fillClipRt.offsetMin = Vector2.zero;
            fillClipRt.offsetMax = Vector2.zero;

            GameObject fillGO = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            fillGO.transform.SetParent(fillClip.transform, false);
            RectTransform fillRt = fillGO.GetComponent<RectTransform>();
            fillRt.anchorMin = new Vector2(0f, 0.5f);
            fillRt.anchorMax = new Vector2(0f, 0.5f);
            fillRt.pivot = new Vector2(0f, 0.5f);
            fillRt.anchoredPosition = Vector2.zero;
            fillRt.sizeDelta = size;

            Image fillImage = fillGO.GetComponent<Image>();
            fillImage.sprite = fillSprite;
            fillImage.color = Color.white;
            fillImage.preserveAspect = true;
            fillImage.raycastTarget = false;

            GameObject slideArea = new GameObject("Handle Slide Area", typeof(RectTransform));
            slideArea.transform.SetParent(go.transform, false);
            RectTransform slideRt = slideArea.GetComponent<RectTransform>();
            slideRt.anchorMin = Vector2.zero;
            slideRt.anchorMax = Vector2.one;
            float handleInset = handleSize.x * 0.18f;
            slideRt.offsetMin = new Vector2(handleInset, 0f);
            slideRt.offsetMax = new Vector2(-handleInset, 0f);

            GameObject handleGO = new GameObject("Handle", typeof(RectTransform), typeof(Image));
            handleGO.transform.SetParent(slideArea.transform, false);
            RectTransform handleRt = handleGO.GetComponent<RectTransform>();
            handleRt.anchorMin = new Vector2(0f, 0.5f);
            handleRt.anchorMax = new Vector2(0f, 0.5f);
            handleRt.pivot = Center;
            handleRt.sizeDelta = handleSize;
            Image handleImg = handleGO.GetComponent<Image>();
            handleImg.sprite = handleSprite;
            handleImg.color = Color.white;
            handleImg.preserveAspect = true;
            handleImg.raycastTarget = true;

            Slider slider = go.GetComponent<Slider>();
            slider.transition = Selectable.Transition.None;
            slider.direction = Slider.Direction.LeftToRight;
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.wholeNumbers = false;
            slider.targetGraphic = handleImg;
            slider.fillRect = fillClipRt;
            slider.handleRect = handleRt;
            slider.value = 1f;

            return slider;
        }

        private static Sprite RequireSprite(string assetPath)
        {
            Sprite sprite = LoadSprite(assetPath);
            if (sprite == null)
            {
                throw new System.InvalidOperationException($"No se pudo cargar el sprite requerido: {assetPath}");
            }
            return sprite;
        }

        private static Button CreateToggleButton(Transform parent, string name, Sprite offSprite, Vector2 anchor, out Image image)
        {
            Button btn = CreateButton(parent, name, offSprite);
            RectTransform rt = btn.GetComponent<RectTransform>();
            PlaceUI(rt, anchor, Center, Vector2.zero, SizeFromSprite(offSprite, ToggleWidth));
            image = btn.GetComponent<Image>();
            return btn;
        }

        private static Text CreateLabel(Transform parent, string name, string text, int fontSize, TextAnchor alignment)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            Text label = go.GetComponent<Text>();
            label.text = text;
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontStyle = FontStyle.Bold;
            label.fontSize = fontSize;
            label.alignment = alignment;
            label.color = new Color(0.13f, 0.10f, 0.08f);
            label.raycastTarget = false;
            return label;
        }

        private static Text CreateLabel(Transform parent, string name, string text, int fontSize, TextAnchor alignment, Vector2 anchor, Vector2 size)
        {
            Text label = CreateLabel(parent, name, text, fontSize, alignment);
            PlaceUI(label.rectTransform, anchor, Center, Vector2.zero, size);
            return label;
        }

        public static Image CreateImage(Transform parent, string name, Sprite sprite)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            Image img = go.GetComponent<Image>();
            img.sprite = sprite;
            img.preserveAspect = sprite != null;
            img.raycastTarget = false;
            return img;
        }

        public static Button CreateButton(Transform parent, string name, Sprite sprite)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            Image img = go.GetComponent<Image>();
            img.sprite = sprite;
            img.preserveAspect = sprite != null;
            Button btn = go.GetComponent<Button>();
            btn.targetGraphic = img;
            btn.transition = Selectable.Transition.ColorTint;
            return btn;
        }

        public static void PlaceUI(RectTransform rt, Vector2 anchor, Vector2 pivot, Vector2 anchoredOffset, Vector2 size)
        {
            rt.anchorMin = anchor;
            rt.anchorMax = anchor;
            rt.pivot = pivot;
            rt.anchoredPosition = anchoredOffset;
            rt.sizeDelta = size;
        }
    }
}
