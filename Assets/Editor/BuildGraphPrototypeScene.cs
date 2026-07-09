using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using TravesiaACasa.Rooms;

namespace TravesiaACasa.Rooms.Editor
{
    /// <summary>
    /// Genera (o actualiza) los 9 RoomNode del boceto y arma la escena de
    /// prueba del grafo descrita en GRAFO_README.md: GraphManager, el cubo
    /// jugador y un RoomExitPoint por cada dirección de cada conexión.
    ///
    /// Re-ejecutable: si los RoomNode ya existen los reutiliza y solo
    /// actualiza posiciones/conexiones; la escena se sobreescribe entera
    /// cada vez que se corre.
    /// </summary>
    public static class BuildGraphPrototypeScene
    {
        private const string RoomsFolder = "Assets/Rooms";
        private const string ScenePath = "Assets/Scenes/GraphPrototype.unity";

        // Posiciones de prueba (no calzan con el boceto real, solo dan
        // espacio para probar sin overlap - ver nota en GRAFO_README.md).
        private static readonly Dictionary<string, Vector3> Positions = new Dictionary<string, Vector3>
        {
            { "1", new Vector3(-8, 8, 0) },
            { "2", new Vector3(0, 8, 0) },
            { "3", new Vector3(8, 8, 0) },
            { "5", new Vector3(0, 0, 0) },
            { "6", new Vector3(-8, 0, 0) },
            { "4", new Vector3(0, -8, 0) },
            { "7", new Vector3(0, -16, 0) },
            { "8", new Vector3(8, -16, 0) },
            { "9", new Vector3(16, -16, 0) },
        };

        private static readonly (string a, string b)[] Edges =
        {
            ("1", "2"), ("2", "3"), ("2", "5"), ("5", "6"),
            ("5", "4"), ("4", "7"), ("7", "8"), ("8", "9"),
        };

        [MenuItem("Game/Build Graph Prototype Scene (Cap 1)")]
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

            Debug.Log($"[BuildGraphPrototypeScene] Listo: {nodes.Count} RoomNode en {RoomsFolder}, escena guardada en {ScenePath}");
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
            // startingNode/targetNode en null en la escena generada.
            Dictionary<string, RoomNode> nodes = LoadNodes();

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "Yal_CubePrototype";
            Object.DestroyImmediate(cube.GetComponent<BoxCollider>());
            cube.AddComponent<Rigidbody2D>();
            BoxCollider2D playerCollider = cube.AddComponent<BoxCollider2D>();
            playerCollider.isTrigger = false;
            cube.AddComponent<CubePlayerController>();
            cube.tag = "Player";
            cube.transform.position = nodes["1"].testWorldPosition;

            var managerGO = new GameObject("GraphManager");
            RoomGraphManager graphManager = managerGO.AddComponent<RoomGraphManager>();
            SetPrivateField(graphManager, "startingNode", nodes["1"]);
            SetPrivateField(graphManager, "player", cube.transform);

            Transform exitsParent = new GameObject("Exits").transform;
            foreach ((string a, string b) in Edges)
            {
                CreateExit(exitsParent, nodes[a], nodes[b]);
                CreateExit(exitsParent, nodes[b], nodes[a]);
            }

            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), ScenePath);
        }

        private static void CreateExit(Transform parent, RoomNode from, RoomNode to)
        {
            var go = new GameObject($"Exit_{from.roomId}_to_{to.roomId}");
            go.transform.SetParent(parent);

            Vector3 dir = (to.testWorldPosition - from.testWorldPosition).normalized;
            go.transform.position = from.testWorldPosition + dir * 3f;

            BoxCollider2D col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(2f, 2f);

            RoomExitPoint exit = go.AddComponent<RoomExitPoint>();
            SetPrivateField(exit, "targetNode", to);
        }

        private static void SetPrivateField(Object target, string fieldName, Object value)
        {
            var so = new SerializedObject(target);
            so.FindProperty(fieldName).objectReferenceValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
