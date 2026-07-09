using UnityEditor;
using UnityEditor.Build.Profile;
using UnityEngine;

namespace TravesiaACasa.Rooms.Editor
{
    /// <summary>
    /// Corre ambos generadores de escena (Menú + Juego) y deja Build
    /// Settings apuntando a ellas: MainMenu primero (índice 0, se abre
    /// al iniciar el build), Game segundo. SampleScene se conserva al
    /// final por si se sigue usando para pruebas sueltas.
    /// </summary>
    public static class BuildAllScenes
    {
        [MenuItem("Game/Build All Scenes (Menu + Game)")]
        public static void Build()
        {
            TravesiaACasa.Menu.Editor.BuildMenuScene.Build();
            BuildGameScene.Build();

            var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>
            {
                new EditorBuildSettingsScene("Assets/Scenes/MainMenu.unity", true),
                new EditorBuildSettingsScene("Assets/Scenes/Game.unity", true),
            };
            foreach (EditorBuildSettingsScene existing in EditorBuildSettings.scenes)
            {
                if (existing.path != "Assets/Scenes/MainMenu.unity" && existing.path != "Assets/Scenes/Game.unity")
                    scenes.Add(existing);
            }
            EditorBuildSettings.scenes = scenes.ToArray();

            // Unity 6: si hay un Build Profile activo (File > Build Profiles),
            // SceneManager.LoadScene mira SU lista de escenas, no la de arriba.
            // Lo dejamos heredando la lista compartida para que no haya que
            // mantener las dos sincronizadas a mano.
            BuildProfile activeProfile = BuildProfile.GetActiveBuildProfile();
            if (activeProfile != null)
            {
                activeProfile.overrideGlobalScenes = false;
                activeProfile.scenes = scenes.ToArray();
                EditorUtility.SetDirty(activeProfile);
                AssetDatabase.SaveAssets();
            }

            Debug.Log("[BuildAllScenes] Build Settings actualizado: " + string.Join(", ", scenes.ConvertAll(s => s.path))
                + (activeProfile != null ? $" (+ Build Profile activo: {activeProfile.name})" : " (sin Build Profile activo)"));
        }
    }
}
