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
using static TravesiaACasa.Menu.Editor.SettingsPanelBuildUtils;

namespace TravesiaACasa.Menu.Editor
{
    /// <summary>
    /// Genera Assets/Scenes/MainMenu.unity a partir del arte entregado en
    /// assets juego aves/menu/ y assets juego aves/configuración/:
    /// título + botón Jugar + botón Configuración, y el panel de
    /// Configuración (construido por SettingsPanelBuildUtils, compartido
    /// con la escena de juego) + overlay de Brillo aplicado en vivo.
    ///
    /// Re-ejecutable: sobreescribe la escena entera cada vez que corre.
    /// </summary>
    public static class BuildMenuScene
    {
        private const string ArtRoot = "Assets/assets juego/assets juego aves";
        private const string ScenePath = "Assets/Scenes/MainMenu.unity";

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
            bg.rectTransform.anchorMin = bg.rectTransform.anchorMax = Center;
            bg.rectTransform.pivot = Center;
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

            // Overlay de Brillo (último hijo: encima de todo)
            AddBrightnessOverlay(canvasT);

            // Controller del menú
            GameObject controllerGO = new GameObject("MenuController");
            MainMenuController controller = controllerGO.AddComponent<MainMenuController>();
            SetPrivateField(controller, "settingsPanelRoot", panelRoot);

            UnityEventTools.AddPersistentListener(jugarBtn.onClick, controller.OnPlayClicked);
            UnityEventTools.AddPersistentListener(configBtn.onClick, controller.OnOpenSettingsClicked);
            UnityEventTools.AddPersistentListener(volverBtn.onClick, controller.OnCloseSettingsClicked);

            // SettingsManager, para que el panel tenga de dónde leer/guardar
            // valores (también se auto-crea en runtime vía Bootstrap, esto
            // solo lo deja visible en la jerarquía de la escena)
            new GameObject("SettingsManager").AddComponent<SettingsManager>();

            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), ScenePath);
            Debug.Log($"[BuildMenuScene] Escena guardada en {ScenePath}");
        }
    }
}
