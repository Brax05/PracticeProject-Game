using System.IO;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using TravesiaACasa.Menu;
using TravesiaACasa.Rooms.Editor;
using static TravesiaACasa.Rooms.Editor.RoomSceneBuildUtils;

namespace TravesiaACasa.Menu.Editor
{
    /// <summary>
    /// Genera Assets/Scenes/MainMenu.unity a partir del arte entregado en
    /// assets juego aves/menu/ y assets juego aves/configuración/:
    /// título + botón Jugar + botón Configuración, y el panel de
    /// Configuración (mismo layout que el boceto configuración.png:
    /// 3 sliders de Sonido + Brillo, y 2 interruptores).
    ///
    /// Re-ejecutable: sobreescribe la escena entera cada vez que corre.
    /// </summary>
    public static class BuildMenuScene
    {
        private const string ArtRoot = "Assets/assets juego/assets juego aves";
        private const string ScenePath = "Assets/Scenes/MainMenu.unity";

        // Posiciones normalizadas (u, vDesdeAbajo) de cada control dentro
        // del panel de Configuración, medidas sobre el boceto
        // configuración/configuración.png (3376x1560) para calzar con las
        // etiquetas "Ambiente/Personajes/Cinemática/Brillo/Modo daltónico/
        // Vibración" ya dibujadas en esa imagen.
        private static readonly Vector2 AmbienteAnchor = new Vector2(0.563f, 0.4423f);
        private static readonly Vector2 PersonajesAnchor = new Vector2(0.563f, 0.3064f);
        private static readonly Vector2 CinematicaAnchor = new Vector2(0.563f, 0.1686f);
        private static readonly Vector2 BrilloAnchor = new Vector2(0.7997f, 0.1878f);
        private static readonly Vector2 DaltonicoAnchor = new Vector2(0.844f, 0.4744f);
        private static readonly Vector2 VibracionAnchor = new Vector2(0.844f, 0.311f);
        private static readonly Vector2 VolverAnchor = new Vector2(0.0533f, 0.8846f);

        [MenuItem("Game/Build Menu Scene")]
        public static void Build()
        {
            Directory.CreateDirectory("Assets/Scenes");
            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));

            GameObject canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            Transform canvasT = canvasGO.transform;

            // Fondo (fondo inicio + ave, ver menu/Fondo menú.png)
            Sprite bgSprite = LoadSprite($"{ArtRoot}/menu/Fondo menú.png");
            Image bg = CreateImage(canvasT, "Fondo", bgSprite);
            bg.rectTransform.anchorMin = bg.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            bg.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            bg.rectTransform.anchoredPosition = Vector2.zero;
            AspectRatioFitter fitter = bg.gameObject.AddComponent<AspectRatioFitter>();
            fitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
            fitter.aspectRatio = bgSprite.rect.width / bgSprite.rect.height;

            // Título (ancla y pivote arriba-centro, para que el offset baje
            // el título desde el borde superior sin que se salga de cuadro)
            Sprite tituloSprite = LoadSprite($"{ArtRoot}/menu/titulo .png");
            Image titulo = CreateImage(canvasT, "Titulo", tituloSprite);
            PlaceUI(titulo.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -80), SizeFromSprite(tituloSprite, 780f));

            // Botón Jugar
            Sprite jugarSprite = LoadSprite($"{ArtRoot}/menu/jugar.png");
            Button jugarBtn = CreateButton(canvasT, "BotonJugar", jugarSprite);
            PlaceUI(jugarBtn.GetComponent<RectTransform>(), new Vector2(0.74f, 0.5f), Center, new Vector2(0, 60), SizeFromSprite(jugarSprite, 340f));

            // Botón Configuración
            Sprite configBtnSprite = LoadSprite($"{ArtRoot}/menu/configuración.png");
            Button configBtn = CreateButton(canvasT, "BotonConfiguracion", configBtnSprite);
            PlaceUI(configBtn.GetComponent<RectTransform>(), new Vector2(0.74f, 0.5f), Center, new Vector2(0, -60), SizeFromSprite(configBtnSprite, 340f));

            // Panel de Configuración (arranca oculto)
            Button volverBtn = BuildSettingsPanel(canvasT, out GameObject panelRoot);
            panelRoot.SetActive(false);

            // Controller del menú
            GameObject controllerGO = new GameObject("MenuController");
            MainMenuController controller = controllerGO.AddComponent<MainMenuController>();
            SetPrivateField(controller, "settingsPanelRoot", panelRoot);

            UnityEventTools.AddPersistentListener(jugarBtn.onClick, controller.OnPlayClicked);
            UnityEventTools.AddPersistentListener(configBtn.onClick, controller.OnOpenSettingsClicked);
            UnityEventTools.AddPersistentListener(volverBtn.onClick, controller.OnCloseSettingsClicked);

            // SettingsManager, para que el panel tenga de dónde leer/guardar valores
            new GameObject("SettingsManager").AddComponent<SettingsManager>();

            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), ScenePath);
            Debug.Log($"[BuildMenuScene] Escena guardada en {ScenePath}");
        }

        private static Button BuildSettingsPanel(Transform canvasT, out GameObject root)
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
            // de punto (como en un primer intento) el aspectRatio calculado
            // queda inconsistente. Los dejamos estirados de entrada.
            panel.rectTransform.anchorMin = Vector2.zero;
            panel.rectTransform.anchorMax = Vector2.one;
            panel.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            panel.rectTransform.anchoredPosition = Vector2.zero;
            panel.rectTransform.sizeDelta = Vector2.zero;
            // FitInParent: si la ventana es muy angosta/corta (aspect ratio
            // raro), el panel se achica para seguir cabiendo completo en
            // vez de desbordarse verticalmente y superponerse con todo.
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
            handleRt.pivot = new Vector2(0.5f, 0.5f);
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

        private static Image CreateImage(Transform parent, string name, Sprite sprite)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            Image img = go.GetComponent<Image>();
            img.sprite = sprite;
            img.raycastTarget = false;
            return img;
        }

        private static Button CreateButton(Transform parent, string name, Sprite sprite)
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

        private static readonly Vector2 Center = new Vector2(0.5f, 0.5f);

        /// <summary>Ancla+pivotea un RectTransform en un punto normalizado del padre, con offset en píxeles y tamaño dados.</summary>
        private static void PlaceUI(RectTransform rt, Vector2 anchor, Vector2 pivot, Vector2 anchoredOffset, Vector2 size)
        {
            rt.anchorMin = anchor;
            rt.anchorMax = anchor;
            rt.pivot = pivot;
            rt.anchoredPosition = anchoredOffset;
            rt.sizeDelta = size;
        }
    }
}
