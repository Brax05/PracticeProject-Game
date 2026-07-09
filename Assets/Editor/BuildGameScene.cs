using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using TravesiaACasa.Menu.Editor;
using TravesiaACasa.Rooms;
using static TravesiaACasa.Rooms.Editor.RoomSceneBuildUtils;
using static TravesiaACasa.Menu.Editor.SettingsPanelBuildUtils;

namespace TravesiaACasa.Rooms.Editor
{
    /// <summary>
    /// Genera Assets/Scenes/Game.unity: el mismo grafo de 9 rooms del
    /// prototipo del cubo (ver GRAFO_README.md) pero con el arte final
    /// entregado por la diseñadora (assets juego aves/juego/) — fondo
    /// ilustrado + decorado por room, el Yal austral real en vez del
    /// cubo, y cámara fija que salta de room en room sin scroll.
    ///
    /// Usa su propio set de RoomNode en Assets/Rooms/Bird/ (independiente
    /// de Assets/Rooms/RoomNode_*.asset, que sigue siendo del prototipo
    /// del cubo) para no romper GraphPrototype.unity si alguien la
    /// vuelve a generar con "Game > Build Graph Prototype Scene (Cap 1)".
    ///
    /// Re-ejecutable: reutiliza los RoomNode si ya existen y sobreescribe
    /// la escena entera cada vez que corre.
    /// </summary>
    public static class BuildGameScene
    {
        private const string ArtRoot = "Assets/assets juego/assets juego aves";
        private const string RoomsFolder = "Assets/Rooms/Bird";
        private const string ScenePath = "Assets/Scenes/Game.unity";

        // Misma topología que BuildGraphPrototypeScene (ver GRAFO_README.md),
        // pero con grilla no-uniforme (paso 20 en X, 10 en Y) para que el
        // fondo panorámico (aspect ~2.16:1) quede bien encajado sin overlap.
        private static readonly Dictionary<string, Vector3> Positions = new Dictionary<string, Vector3>
        {
            { "1", new Vector3(-20, 10, 0) },
            { "2", new Vector3(0, 10, 0) },
            { "3", new Vector3(20, 10, 0) },
            { "5", new Vector3(0, 0, 0) },
            { "6", new Vector3(-20, 0, 0) },
            { "4", new Vector3(0, -10, 0) },
            { "7", new Vector3(0, -20, 0) },
            { "8", new Vector3(20, -20, 0) },
            { "9", new Vector3(40, -20, 0) },
        };

        private static readonly (string a, string b)[] Edges =
        {
            ("1", "2"), ("2", "3"), ("2", "5"), ("5", "6"),
            ("5", "4"), ("4", "7"), ("7", "8"), ("8", "9"),
        };

        private const float ExitOffset = 6f;
        private const float BirdWorldHeight = 0.9f;

        [MenuItem("Game/Build Game Scene (Cap 1)")]
        public static void Build()
        {
            Directory.CreateDirectory(RoomsFolder);
            Directory.CreateDirectory("Assets/Scenes");
            AssetDatabase.Refresh();

            Dictionary<string, RoomNode> nodes = CreateOrUpdateNodes();
            foreach ((string a, string b) in Edges)
                Connect(nodes[a], nodes[b]);

            foreach (RoomNode node in nodes.Values)
                EditorUtility.SetDirty(node);
            AssetDatabase.SaveAssets();

            BuildScene();

            Debug.Log($"[BuildGameScene] Listo: {nodes.Count} RoomNode en {RoomsFolder}, escena guardada en {ScenePath}");
        }

        private static Dictionary<string, RoomNode> CreateOrUpdateNodes()
        {
            var nodes = new Dictionary<string, RoomNode>();
            foreach (KeyValuePair<string, Vector3> kv in Positions)
            {
                string path = $"{RoomsFolder}/RoomNode_{kv.Key}.asset";
                RoomNode node = AssetDatabase.LoadAssetAtPath<RoomNode>(path);
                if (node == null)
                {
                    node = ScriptableObject.CreateInstance<RoomNode>();
                    AssetDatabase.CreateAsset(node, path);
                }

                node.roomId = kv.Key;
                node.testWorldPosition = kv.Value;
                nodes[kv.Key] = node;
            }
            return nodes;
        }

        private static void Connect(RoomNode a, RoomNode b)
        {
            if (!a.connections.Contains(b)) a.connections.Add(b);
            if (!b.connections.Contains(a)) b.connections.Add(a);
        }

        private static Dictionary<string, RoomNode> LoadNodes()
        {
            var nodes = new Dictionary<string, RoomNode>();
            foreach (string key in Positions.Keys)
                nodes[key] = AssetDatabase.LoadAssetAtPath<RoomNode>($"{RoomsFolder}/RoomNode_{key}.asset");
            return nodes;
        }

        private static void BuildScene()
        {
            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            // Releer los RoomNode desde disco DESPUÉS de NewScene: crear una
            // escena nueva puede invalidar ("fake null") referencias a
            // assets que se cargaron antes, que es lo que dejaba
            // startingNode/targetNode en null en la escena generada (los
            // triggers de salida no hacían nada al tocarlos).
            Dictionary<string, RoomNode> nodes = LoadNodes();

            // Dos fondos distintos (la diseñadora solo entregó estos dos de
            // tamaño "room completa") para que no todas las celdas se vean
            // idénticas mientras no haya ilustración única por room.
            Sprite[] bgSprites =
            {
                LoadSprite($"{ArtRoot}/juego/fondo  juego.png"),
                LoadSprite($"{ArtRoot}/juego/fondo inicio.png"),
            };
            Sprite bgSprite = bgSprites[0];
            Sprite hojasSprite = LoadSprite($"{ArtRoot}/juego/hojas.png");
            Sprite arbustoSprite = LoadSprite($"{ArtRoot}/juego/arbusto 1.png");
            Sprite nidoSprite = LoadSprite($"{ArtRoot}/juego/nido.png");
            Sprite birdSprite = LoadSprite($"{ArtRoot}/juego/ave.png");
            Sprite wasdSprite = LoadSprite($"{ArtRoot}/juego/wasd.png");

            // Sprite.bounds ya resuelve el tamaño real en unidades de mundo
            // (a diferencia de sprite.rect.width/PPU a mano): Unity recorta a
            // maxTextureSize las texturas que lo superan (el fondo es 3376px
            // y se recorta a 2048; el arbusto es 1148px y no), así que dos
            // sprites con el mismo "factor de escala" a mano terminan en
            // tamaños de mundo distintos si uno se recortó y el otro no.
            // Por eso cada sprite calcula su propio factor contra su propio
            // ancho objetivo, en vez de reusar un solo bgScale para todos.
            const float TargetBgWidth = 19f;
            float bgScale = TargetBgWidth / bgSprite.bounds.size.x;
            float bgHeight = bgSprite.bounds.size.y * bgScale;

            const float TargetArbustoWidth = 3.5f;
            const float TargetNidoWidth = 1.3f;
            float arbustoScale = TargetArbustoWidth / arbustoSprite.bounds.size.x;
            float nidoScale = TargetNidoWidth / nidoSprite.bounds.size.x;

            Transform roomsParent = new GameObject("Rooms").transform;
            foreach (RoomNode node in nodes.Values)
            {
                Sprite roomBg = bgSprites[Mathf.Abs(node.roomId.GetHashCode()) % bgSprites.Length];
                BuildRoomVisuals(roomsParent, node, roomBg, hojasSprite, arbustoSprite, nidoSprite, bgScale, bgHeight, arbustoScale, nidoScale);
            }

            GameObject bird = BuildBird(birdSprite, nodes["1"].testWorldPosition);

            var managerGO = new GameObject("GraphManager");
            RoomGraphManager graphManager = managerGO.AddComponent<RoomGraphManager>();
            SetPrivateField(graphManager, "startingNode", nodes["1"]);
            SetPrivateField(graphManager, "player", bird.transform);

            Transform exitsParent = new GameObject("Exits").transform;
            foreach ((string a, string b) in Edges)
            {
                CreateExit(exitsParent, nodes[a], nodes[b]);
                CreateExit(exitsParent, nodes[b], nodes[a]);
            }

            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                mainCamera.orthographic = true;
                mainCamera.orthographicSize = bgHeight / 2f;
                mainCamera.transform.position = new Vector3(nodes["1"].testWorldPosition.x, nodes["1"].testWorldPosition.y, mainCamera.transform.position.z);
                mainCamera.gameObject.AddComponent<CameraRoomFollower>();
            }

            BuildHud(wasdSprite, bird.GetComponent<BirdPlayerController>());

            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), ScenePath);
        }

        private static void BuildRoomVisuals(Transform parent, RoomNode node, Sprite bgSprite, Sprite hojasSprite,
            Sprite arbustoSprite, Sprite nidoSprite, float bgScale, float bgHeight, float arbustoScale, float nidoScale)
        {
            Vector3 pos = node.testWorldPosition;
            Transform roomT = new GameObject($"Room_{node.roomId}").transform;
            roomT.SetParent(parent);
            roomT.position = pos;

            CreateSpriteChild(roomT, "Fondo", bgSprite, Vector3.zero, bgScale, -10);
            CreateSpriteChild(roomT, "Hojas", hojasSprite, Vector3.zero, bgScale, 10);

            // Decoración pseudo-aleatoria (determinística por roomId) para dar
            // variedad room a room mientras se usa el mismo fondo compartido.
            System.Random rng = new System.Random(node.roomId.GetHashCode());
            // La cámara fija solo muestra ~bgHeight*aspect de ancho, así que el
            // rango de dispersión se mantiene dentro de lo que realmente se ve.
            float halfW = bgHeight * 0.8f;
            float halfH = bgHeight * 0.32f;

            int arbustoCount = 1 + rng.Next(0, 2);
            for (int i = 0; i < arbustoCount; i++)
            {
                Vector3 offset = new Vector3(RandRange(rng, -halfW, halfW), RandRange(rng, -halfH, -halfH * 0.3f), 0f);
                CreateSpriteChild(roomT, $"Arbusto_{i}", arbustoSprite, offset, arbustoScale, -5);
            }

            if (rng.NextDouble() < 0.6)
            {
                Vector3 offset = new Vector3(RandRange(rng, -halfW, halfW), RandRange(rng, 0f, halfH), 0f);
                CreateSpriteChild(roomT, "Nido", nidoSprite, offset, nidoScale, -5);
            }
        }

        private static float RandRange(System.Random rng, float min, float max) =>
            min + (float)rng.NextDouble() * (max - min);

        private static void CreateSpriteChild(Transform parent, string name, Sprite sprite, Vector3 localOffset, float scale, int sortingOrder)
        {
            var go = new GameObject(name, typeof(SpriteRenderer));
            go.transform.SetParent(parent);
            go.transform.localPosition = localOffset;
            go.transform.localScale = Vector3.one * scale;
            SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingOrder = sortingOrder;
        }

        private static GameObject BuildBird(Sprite birdSprite, Vector3 startPos)
        {
            var bird = new GameObject("Yal_Bird", typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(BirdPlayerController));
            bird.tag = "Player";
            bird.transform.position = startPos;

            SpriteRenderer sr = bird.GetComponent<SpriteRenderer>();
            sr.sprite = birdSprite;

            float birdScale = BirdWorldHeight / birdSprite.bounds.size.y;
            bird.transform.localScale = Vector3.one * birdScale;

            BoxCollider2D collider = bird.GetComponent<BoxCollider2D>();
            collider.size = (Vector2)birdSprite.bounds.size * 0.8f;
            collider.isTrigger = false;

            Rigidbody2D rb = bird.GetComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            return bird;
        }

        private static void CreateExit(Transform parent, RoomNode from, RoomNode to)
        {
            var go = new GameObject($"Exit_{from.roomId}_to_{to.roomId}");
            go.transform.SetParent(parent);

            Vector3 dir = (to.testWorldPosition - from.testWorldPosition).normalized;
            go.transform.position = from.testWorldPosition + dir * ExitOffset;

            BoxCollider2D col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(2f, 2f);

            RoomExitPoint exit = go.AddComponent<RoomExitPoint>();
            SetPrivateField(exit, "targetNode", to);
        }

        /// <summary>
        /// HUD de juego según las capturas del prototipo: D-pad de 4
        /// flechas abajo-izquierda (flecha.png rotada), Interactuar y
        /// Picotear abajo-derecha (botón.png + etiqueta), ruedita de
        /// Configuración arriba-derecha que abre el mismo panel del menú
        /// (SettingsPanelBuildUtils) pausando el juego, overlay de Brillo,
        /// y el blink + etiqueta "Room X" de transición entre rooms.
        /// Incluye el EventSystem, que antes faltaba en esta escena (sin
        /// él ningún botón responde).
        /// </summary>
        private static void BuildHud(Sprite wasdSprite, BirdPlayerController birdController)
        {
            new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));

            GameObject canvasGO = new GameObject("HUD", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            Transform canvasT = canvasGO.transform;

            Sprite flechaSprite = LoadSprite($"{ArtRoot}/juego/flecha.png");
            Sprite botonSprite = LoadSprite($"{ArtRoot}/juego/botón.png");
            Sprite rueditaSprite = LoadSprite($"{ArtRoot}/juego/ruedita.png");

            // ----- D-pad abajo-izquierda (flecha.png apunta hacia ARRIBA) -----
            var dpadRoot = new GameObject("Dpad", typeof(RectTransform));
            dpadRoot.transform.SetParent(canvasT, false);
            RectTransform dpadRt = dpadRoot.GetComponent<RectTransform>();
            PlaceUI(dpadRt, new Vector2(0f, 0f), Center, new Vector2(200f, 200f), new Vector2(320f, 320f));

            CreateDpadButton(dpadRt, "ArribaBtn", flechaSprite, birdController, Vector2.up, new Vector2(0f, 105f), 0f);
            CreateDpadButton(dpadRt, "AbajoBtn", flechaSprite, birdController, Vector2.down, new Vector2(0f, -105f), 180f);
            CreateDpadButton(dpadRt, "IzquierdaBtn", flechaSprite, birdController, Vector2.left, new Vector2(-105f, 0f), 90f);
            CreateDpadButton(dpadRt, "DerechaBtn", flechaSprite, birdController, Vector2.right, new Vector2(105f, 0f), -90f);

            // Hint de teclado, chico y translúcido junto al D-pad (en PC
            // también se puede jugar con WASD/flechas)
            Image hint = CreateImage(canvasT, "WasdHint", wasdSprite);
            hint.color = new Color(1f, 1f, 1f, 0.55f);
            PlaceUI(hint.rectTransform, new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(370f, 40f), SizeFromSprite(wasdSprite, 110f));

            // ----- Acciones abajo-derecha (botón.png + etiqueta) -----
            Button picotearBtn = CreateActionButton(canvasT, "PicotearBtn", botonSprite, "Picotear",
                new Vector2(1f, 0f), new Vector2(-170f, 160f), 220f);
            Button interactuarBtn = CreateActionButton(canvasT, "InteractuarBtn", botonSprite, "Interactuar",
                new Vector2(1f, 0f), new Vector2(-360f, 260f), 190f);

            // ----- Ruedita de Configuración arriba-derecha -----
            Button rueditaBtn = CreateButton(canvasT, "ConfiguracionBtn", rueditaSprite);
            PlaceUI(rueditaBtn.GetComponent<RectTransform>(), new Vector2(1f, 1f), Center, new Vector2(-70f, -70f), SizeFromSprite(rueditaSprite, 80f));

            // ----- Panel de Configuración (el mismo del menú, arranca oculto) -----
            Button volverBtn = BuildSettingsPanel(canvasT, out GameObject settingsRoot);
            settingsRoot.SetActive(false);

            // ----- Overlay de Brillo -----
            AddBrightnessOverlay(canvasT);

            // ----- Etiqueta "Room X" + blink de transición (encima de todo) -----
            Font legacyFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var labelGO = new GameObject("RoomLabel", typeof(RectTransform), typeof(Text));
            labelGO.transform.SetParent(canvasT, false);
            Text label = labelGO.GetComponent<Text>();
            label.font = legacyFont;
            label.fontSize = 36;
            label.alignment = TextAnchor.UpperCenter;
            label.color = Color.white;
            label.text = "Room ?";
            label.raycastTarget = false;
            RectTransform labelRt = labelGO.GetComponent<RectTransform>();
            labelRt.anchorMin = labelRt.anchorMax = new Vector2(0.5f, 1f);
            labelRt.pivot = new Vector2(0.5f, 1f);
            labelRt.anchoredPosition = new Vector2(0f, -20f);
            labelRt.sizeDelta = new Vector2(400f, 60f);
            Outline labelOutline = labelGO.AddComponent<Outline>();
            labelOutline.effectColor = Color.black;
            labelOutline.effectDistance = new Vector2(2f, -2f);

            var fadeGO = new GameObject("Fade", typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
            fadeGO.transform.SetParent(canvasT, false);
            Image fadeImg = fadeGO.GetComponent<Image>();
            fadeImg.color = Color.black;
            fadeImg.raycastTarget = false;
            StretchFull(fadeGO.GetComponent<RectTransform>());
            CanvasGroup fadeGroup = fadeGO.GetComponent<CanvasGroup>();
            fadeGroup.alpha = 0f;
            fadeGroup.blocksRaycasts = false;
            fadeGroup.interactable = false;

            RoomTransitionUI transitionUI = canvasGO.AddComponent<RoomTransitionUI>();
            SetPrivateField(transitionUI, "fadeGroup", fadeGroup);
            SetPrivateField(transitionUI, "roomLabel", label);

            // ----- Controller que amarra los botones de acción/configuración -----
            var hudControllerGO = new GameObject("GameHudController");
            GameHudController hud = hudControllerGO.AddComponent<GameHudController>();
            SetPrivateField(hud, "settingsPanelRoot", settingsRoot);

            UnityEventTools.AddPersistentListener(rueditaBtn.onClick, hud.OnOpenSettingsClicked);
            UnityEventTools.AddPersistentListener(volverBtn.onClick, hud.OnCloseSettingsClicked);
            UnityEventTools.AddPersistentListener(interactuarBtn.onClick, hud.OnInteractClicked);
            UnityEventTools.AddPersistentListener(picotearBtn.onClick, hud.OnPeckClicked);
        }

        private static void CreateDpadButton(Transform parent, string name, Sprite arrowSprite,
            BirdPlayerController birdController, Vector2 direction, Vector2 offset, float zRotation)
        {
            // No es un Button: HudMoveButton escucha PointerDown/Up para
            // mover MIENTRAS está presionado (Button recién dispara al soltar)
            var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(HudMoveButton));
            go.transform.SetParent(parent, false);

            Image img = go.GetComponent<Image>();
            img.sprite = arrowSprite;
            img.raycastTarget = true;

            RectTransform rt = go.GetComponent<RectTransform>();
            // flecha.png es más alta que ancha (325x671): ancho 60 mantiene proporción
            PlaceUI(rt, Center, Center, offset, SizeFromSprite(arrowSprite, 60f));
            rt.localEulerAngles = new Vector3(0f, 0f, zRotation);

            HudMoveButton move = go.GetComponent<HudMoveButton>();
            SetPrivateField(move, "player", birdController);
            var so = new SerializedObject(move);
            so.FindProperty("direction").vector2Value = direction;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static Button CreateActionButton(Transform parent, string name, Sprite sprite, string label,
            Vector2 anchor, Vector2 offset, float width)
        {
            Button btn = CreateButton(parent, name, sprite);
            PlaceUI(btn.GetComponent<RectTransform>(), anchor, Center, offset, SizeFromSprite(sprite, width));

            var labelGO = new GameObject("Label", typeof(RectTransform), typeof(Text));
            labelGO.transform.SetParent(btn.transform, false);
            RectTransform labelRt = labelGO.GetComponent<RectTransform>();
            StretchFull(labelRt);

            Text text = labelGO.GetComponent<Text>();
            text.text = label;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontStyle = FontStyle.Bold;
            text.alignment = TextAnchor.MiddleCenter;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 10;
            text.resizeTextMaxSize = 44;
            text.color = new Color(0.24f, 0.13f, 0.05f); // café oscuro sobre el botón naranjo
            text.raycastTarget = false;

            return btn;
        }
    }
}
