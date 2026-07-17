using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TravesiaACasa.Rooms.Editor
{
    /// <summary>
    /// Convierte piezas grandes de la GUI (HUD del juego, menú principal,
    /// panel de configuración) en Prefabs, para poder editarlas en Prefab
    /// Mode sin que el diff toque el resto de la escena — así dos agentes
    /// no chocan en el mismo Canvas gigante (motivo: el 17/07 Antigravity,
    /// Cristhian y Claude Code editaron el mismo HUD en Juego.unity el
    /// mismo día).
    ///
    /// OJO con "MenuContent" y "SettingsPanel" de MenuPrincipal.unity:
    /// `BuildMenuScene.Build()` (Game/Build Menu Scene) reconstruye esa
    /// escena ENTERA desde cero cada vez que corre — si la vuelves a
    /// ejecutar después de prefabificar, te va a pisar el Prefab con
    /// jerarquía nueva sin prefabricar. Si vas a seguir usando ese botón
    /// para regenerar el menú, no prefabifiques el menú (o actualiza
    /// BuildMenuScene para instanciar el prefab en vez de crear todo a mano).
    /// </summary>
    public static class PrefabizeGuiTool
    {
        private const string PrefabFolder = "Assets/Prefabs/UI";

        [MenuItem("Game/Prefabs/Convertir HUD del juego (Juego.unity)")]
        public static void PrefabizeGameHud()
        {
            Prefabize("Assets/Escenas/Juego.unity", "HUD", "GameHUD");
        }

        [MenuItem("Game/Prefabs/Convertir MenuContent (MenuPrincipal.unity)")]
        public static void PrefabizeMenuContent()
        {
            Debug.LogWarning("[PrefabizeGuiTool] Si vuelves a correr 'Game/Build Menu Scene' esto se pisa. Ver comentario de la clase.");
            Prefabize("Assets/Escenas/MenuPrincipal.unity", "Canvas/MenuContent", "MainMenuContent");
        }

        [MenuItem("Game/Prefabs/Convertir SettingsPanel (MenuPrincipal.unity)")]
        public static void PrefabizeSettingsPanel()
        {
            Debug.LogWarning("[PrefabizeGuiTool] Si vuelves a correr 'Game/Build Menu Scene' esto se pisa. Ver comentario de la clase.");
            Prefabize("Assets/Escenas/MenuPrincipal.unity", "Canvas/SettingsPanel", "SettingsPanel");
        }

        private static void Prefabize(string scenePath, string objectPath, string prefabName)
        {
            Scene scene = EditorSceneManager.GetActiveScene();
            bool wasAlreadyOpen = scene.path == scenePath;

            if (!wasAlreadyOpen)
            {
                if (scene.isDirty &&
                    !EditorUtility.DisplayDialog(
                        "Escena con cambios sin guardar",
                        $"'{scene.name}' tiene cambios sin guardar. ¿Guardarlos antes de abrir '{scenePath}'?",
                        "Guardar y continuar", "Cancelar"))
                {
                    Debug.LogWarning("[PrefabizeGuiTool] Cancelado por el usuario.");
                    return;
                }

                EditorSceneManager.SaveOpenScenes();
                scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            }

            GameObject target = GameObject.Find(objectPath);
            if (target == null)
            {
                Debug.LogError($"[PrefabizeGuiTool] No encontré '{objectPath}' en {scenePath}. ¿Ya se movió o se renombró?");
                return;
            }

            if (PrefabUtility.IsPartOfAnyPrefab(target))
            {
                Debug.LogWarning($"[PrefabizeGuiTool] '{objectPath}' ya es (parte de) un Prefab. No hago nada.");
                return;
            }

            Directory.CreateDirectory(PrefabFolder);
            string prefabPath = $"{PrefabFolder}/{prefabName}.prefab";

            if (File.Exists(prefabPath) &&
                !EditorUtility.DisplayDialog(
                    "El Prefab ya existe",
                    $"'{prefabPath}' ya existe. ¿Sobreescribirlo?",
                    "Sobreescribir", "Cancelar"))
            {
                Debug.LogWarning("[PrefabizeGuiTool] Cancelado por el usuario.");
                return;
            }

            GameObject result = PrefabUtility.SaveAsPrefabAssetAndConnect(target, prefabPath, InteractionMode.UserAction);
            if (result == null)
            {
                Debug.LogError($"[PrefabizeGuiTool] Falló al crear el prefab de '{objectPath}'.");
                return;
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[PrefabizeGuiTool] '{objectPath}' de {scenePath} ahora es {prefabPath}, conectado como instancia en la escena.");
        }
    }
}
