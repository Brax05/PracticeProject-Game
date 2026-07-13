using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace TravesiaACasa.Rooms.Editor
{
    /// <summary>
    /// "Game > Asignar animaciones del Yal": agrega (si falta) el
    /// BirdSpriteAnimator al Player de la escena abierta y le asigna
    /// los frames de Assets/.../yal_animaciones — idle_lado como idle y
    /// avanzar como movimiento — sin tener que arrastrar los 16 sprites
    /// a mano en el inspector.
    /// </summary>
    public static class SetupYalAnimations
    {
        private const string BaseFolder =
            "Assets/assets juego/assets juego aves/juego/yal_animaciones";

        [MenuItem("Game/Asignar animaciones del Yal")]
        public static void Assign()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogError("[SetupYalAnimations] No hay ningún objeto con tag 'Player' en la escena abierta.");
                return;
            }

            Sprite[] idle = LoadAnimation("idle_lado");
            Sprite[] move = LoadAnimation("avanzar");
            if (idle.Length == 0 || move.Length == 0) return;

            var animator = player.GetComponent<BirdSpriteAnimator>();
            if (animator == null)
                animator = player.AddComponent<BirdSpriteAnimator>();

            SetSpriteArray(animator, "idleSprites", idle);
            SetSpriteArray(animator, "moveSprites", move);

            var renderer = player.GetComponent<SpriteRenderer>();
            if (renderer != null)
                RoomSceneBuildUtils.SetPrivateField(renderer, "m_Sprite", idle[0]);

            EditorSceneManager.MarkSceneDirty(player.scene);
            Debug.Log($"[SetupYalAnimations] Listo: {idle.Length} frames de idle y {move.Length} de avanzar " +
                      $"asignados a '{player.name}'. Guarda la escena para conservarlo.");
        }

        private static Sprite[] LoadAnimation(string animName)
        {
            string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { $"{BaseFolder}/{animName}" });
            var sprites = new List<Sprite>();
            foreach (string guid in guids)
                sprites.Add(RoomSceneBuildUtils.LoadSprite(AssetDatabase.GUIDToAssetPath(guid)));
            sprites.Sort((a, b) => string.CompareOrdinal(a.name, b.name));

            if (sprites.Count == 0)
                Debug.LogError($"[SetupYalAnimations] No hay sprites en '{BaseFolder}/{animName}'.");
            return sprites.ToArray();
        }

        private static void SetSpriteArray(Object target, string fieldName, Sprite[] sprites)
        {
            var so = new SerializedObject(target);
            SerializedProperty prop = so.FindProperty(fieldName);
            if (prop == null)
            {
                Debug.LogError($"[SetupYalAnimations] No existe el campo '{fieldName}' en {target.GetType().Name}.");
                return;
            }
            prop.arraySize = sprites.Length;
            for (int i = 0; i < sprites.Length; i++)
                prop.GetArrayElementAtIndex(i).objectReferenceValue = sprites[i];
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
