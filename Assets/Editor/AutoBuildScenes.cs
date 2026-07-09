using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using System.Linq;

namespace TravesiaACasa.Rooms.Editor
{
    /// <summary>
    /// Automáticamente detecta si las escenas del juego (MainMenu y Game) no han sido
    /// generadas o no están agregadas en los Build Settings, y las genera/agrega.
    /// Esto evita el error de "Scene 'Game' couldn't be loaded" cuando se abre el proyecto
    /// por primera vez o se limpian los assets generados.
    /// </summary>
    [InitializeOnLoad]
    public static class AutoBuildScenes
    {
        static AutoBuildScenes()
        {
            // Se ejecuta en el siguiente frame del editor una vez inicializado
            EditorApplication.delayCall += CheckAndBuildScenes;
        }

        private static void CheckAndBuildScenes()
        {
            bool gameSceneExists = File.Exists("Assets/Scenes/Game.unity");
            bool mainMenuSceneExists = File.Exists("Assets/Scenes/MainMenu.unity");
            
            bool buildSettingsContainMainMenu = EditorBuildSettings.scenes.Any(s => s.path == "Assets/Scenes/MainMenu.unity");
            bool buildSettingsContainGame = EditorBuildSettings.scenes.Any(s => s.path == "Assets/Scenes/Game.unity");

            if (!gameSceneExists || !mainMenuSceneExists || !buildSettingsContainMainMenu || !buildSettingsContainGame)
            {
                UnityEngine.Debug.Log("[AutoBuildScenes] Detectada ausencia de escenas de juego o desconfiguración de Build Settings. Generando escenas automáticamente...");
                
                // Guardar la escena activa actual para restaurarla después de la generación
                string activeScenePath = EditorSceneManager.GetActiveScene().path;

                // Ejecutar el build general
                BuildAllScenes.Build();

                // Restaurar la escena activa original si existía y sigue existiendo
                if (!string.IsNullOrEmpty(activeScenePath) && File.Exists(activeScenePath))
                {
                    EditorSceneManager.OpenScene(activeScenePath);
                }
            }
        }
    }
}
