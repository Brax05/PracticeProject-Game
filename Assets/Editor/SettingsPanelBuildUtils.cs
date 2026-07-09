using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TravesiaACasa.Menu;
using static TravesiaACasa.Rooms.Editor.RoomSceneBuildUtils;

namespace TravesiaACasa.Menu.Editor
{
    /// <summary>
    /// Constructor compartido del panel de Configuración (boceto
    /// configuración/configuración.png: 3 sliders de Sonido + Brillo y
    /// 2 interruptores). Antes vivía dentro de BuildMenuScene; se movió
    /// aquí para que BuildGameScene arme EXACTAMENTE el mismo panel en
    /// el HUD del juego (ruedita) sin duplicar layout ni anclas.
    /// Devuelve el root (para activar/ocultar) y el botón Volver (para
    /// que cada escena le conecte su propio "cerrar").
    /// </summary>
    public static class SettingsPanelBuildUtils
    {
        private const string ArtRoot = "Assets/assets juego/assets juego aves";

        // Posiciones normalizadas (u, vDesdeAbajo) de cada control dentro
        // del panel, medidas sobre configuración.png (3376x1560) para
        // calzar con las etiquetas dibujadas en esa imagen.
        private static readonly Vector2 AmbienteAnchor = new Vector2(0.563f, 0.4423f);
        private static readonly Vector2 PersonajesAnchor = new Vector2(0.563f, 0.3064f);
        private static readonly Vector2 CinematicaAnchor = new Vector2(0.563f, 0.1686f);
        private static readonly Vector2 BrilloAnchor = new Vector2(0.7997f, 0.1878f);
        private static readonly Vector2 DaltonicoAnchor = new Vector2(0.844f, 0.4744f);
        private static readonly Vector2 VibracionAnchor = new Vector2(0.844f, 0.311f);
        private static readonly Vector2 VolverAnchor = new Vector2(0.0533f, 0.8846f);

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

            Sprite panelSprite = LoadSprite($"{ArtRoot}/configuración/configuración.png");
            Image panel = CreateImage(root.transform, "Panel", panelSprite);
            // FitInParent SIEMPRE fuerza anchors estirados (0,0)-(1,1) y usa
            // sizeDelta como margen respecto al padre — si se dejan anchors
            // de punto el aspectRatio calculado queda inconsistente.
            panel.rectTransform.anchorMin = Vector2.zero;
            panel.rectTransform.anchorMax = Vector2.one;
            panel.rectTransform.pivot = Center;
            panel.rectTransform.anchoredPosition = Vector2.zero;
            panel.rectTransform.sizeDelta = Vector2.zero;
            AspectRatioFitter panelFitter = panel.gameObject.AddComponent<AspectRatioFitter>();
            panelFitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
            panelFitter.aspectRatio = panelSprite.rect.width / panelSprite.rect.height;
            Transform panelT = panel.rectTransform;

            Slider ambiente = CreateSlider(panelT, "SliderAmbiente", $"{ArtRoot}/configuración/slider 1.png", AmbienteAnchor);
            Slider personajes = CreateSlider(panelT, "SliderPersonajes", $"{ArtRoot}/configuración/slider 2.png", PersonajesAnchor);
            Slider cinematica = CreateSlider(panelT, "SliderCinematica", $"{ArtRoot}/configuración/slider 3.png", CinematicaAnchor);
            Slider brillo = CreateSlider(panelT, "SliderBrillo", $"{ArtRoot}/configuración/slider 4.png", BrilloAnchor);

            Sprite offSprite = LoadSprite($"{ArtRoot}/configuración/boton marron.png");
            Sprite onSprite = LoadSprite($"{ArtRoot}/configuración/boton naranjo.png");
            Button daltonicoBtn = CreateToggleButton(panelT, "ToggleModoDaltonico", offSprite, DaltonicoAnchor, out Image daltonicoImg);
            Button vibracionBtn = CreateToggleButton(panelT, "ToggleVibracion", offSprite, VibracionAnchor, out Image vibracionImg);

            Sprite flechaSprite = LoadSprite($"{ArtRoot}/juego/flecha.png");
            Button volverBtn = CreateButton(panelT, "BotonVolver", flechaSprite);
            RectTransform volverRt = volverBtn.GetComponent<RectTransform>();
            PlaceUI(volverRt, VolverAnchor, Center, Vector2.zero, SizeFromSprite(flechaSprite, 90f));
            volverRt.localEulerAngles = new Vector3(0f, 0f, 90f); // apunta hacia la izquierda ("volver")

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

        /// <summary>
        /// Image negra full-screen que aplica el ajuste de Brillo
        /// (BrightnessOverlay). Debe agregarse como ÚLTIMO hijo del Canvas
        /// para quedar encima de todo, incluido el panel de Configuración,
        /// y así ver el efecto en vivo mientras se mueve el slider.
        /// </summary>
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

        private static Slider CreateSlider(Transform parent, string name, string spritePath, Vector2 anchor)
        {
            Sprite sprite = LoadSprite(spritePath);
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Slider));
            go.transform.SetParent(parent, false);

            Vector2 size = SizeFromSprite(sprite, 420f);
            RectTransform rt = go.GetComponent<RectTransform>();
            PlaceUI(rt, anchor, Center, Vector2.zero, size);

            Image bgImage = go.GetComponent<Image>();
            bgImage.sprite = sprite;
            bgImage.raycastTarget = true;

            GameObject slideArea = new GameObject("Handle Slide Area", typeof(RectTransform));
            slideArea.transform.SetParent(go.transform, false);
            RectTransform slideRt = slideArea.GetComponent<RectTransform>();
            slideRt.anchorMin = Vector2.zero;
            slideRt.anchorMax = Vector2.one;
            float leftPad = size.y * 0.6f;
            float rightPad = size.y * 1.6f; // deja espacio a la hoja/ave/flor dibujada al final de la barra
            slideRt.offsetMin = new Vector2(leftPad, 0f);
            slideRt.offsetMax = new Vector2(-rightPad, 0f);

            GameObject handleGO = new GameObject("Handle", typeof(RectTransform), typeof(Image));
            handleGO.transform.SetParent(slideArea.transform, false);
            RectTransform handleRt = handleGO.GetComponent<RectTransform>();
            handleRt.anchorMin = new Vector2(0f, 0f);
            handleRt.anchorMax = new Vector2(0f, 1f);
            handleRt.pivot = Center;
            handleRt.sizeDelta = new Vector2(size.y * 0.8f, 0f);
            Image handleImg = handleGO.GetComponent<Image>();
            handleImg.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
            handleImg.color = new Color(0.24f, 0.16f, 0.12f);

            Slider slider = go.GetComponent<Slider>();
            slider.transition = Selectable.Transition.None;
            slider.direction = Slider.Direction.LeftToRight;
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.wholeNumbers = false;
            slider.targetGraphic = handleImg;
            slider.handleRect = handleRt;
            slider.value = 1f;

            return slider;
        }

        private static Button CreateToggleButton(Transform parent, string name, Sprite offSprite, Vector2 anchor, out Image image)
        {
            Button btn = CreateButton(parent, name, offSprite);
            RectTransform rt = btn.GetComponent<RectTransform>();
            PlaceUI(rt, anchor, Center, Vector2.zero, SizeFromSprite(offSprite, 130f));
            image = btn.GetComponent<Image>();
            return btn;
        }

        public static Image CreateImage(Transform parent, string name, Sprite sprite)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            Image img = go.GetComponent<Image>();
            img.sprite = sprite;
            img.raycastTarget = false;
            return img;
        }

        public static Button CreateButton(Transform parent, string name, Sprite sprite)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            Image img = go.GetComponent<Image>();
            img.sprite = sprite;
            Button btn = go.GetComponent<Button>();
            btn.targetGraphic = img;
            btn.transition = Selectable.Transition.ColorTint;
            return btn;
        }

        /// <summary>Ancla+pivotea un RectTransform en un punto normalizado del padre, con offset en píxeles y tamaño dados.</summary>
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
