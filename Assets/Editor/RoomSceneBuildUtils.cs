using UnityEditor;
using UnityEngine;

namespace TravesiaACasa.Rooms.Editor
{
    /// <summary>
    /// Helpers compartidos por los scripts de "Game > Build ... Scene"
    /// (BuildMenuScene, BuildGameScene). BuildGraphPrototypeScene.cs
    /// mantiene su propia copia de SetPrivateField porque ya existía
    /// antes de este archivo; los scripts nuevos usan este.
    /// </summary>
    public static class RoomSceneBuildUtils
    {
        /// <summary>Asigna un campo privado [SerializeField] de tipo Object (Sprite, Slider, Button, GameObject, etc.).</summary>
        public static void SetPrivateField(Object target, string fieldName, Object value)
        {
            var so = new SerializedObject(target);
            SerializedProperty prop = so.FindProperty(fieldName);
            if (prop == null)
            {
                Debug.LogError($"[RoomSceneBuildUtils] No existe el campo '{fieldName}' en {target.GetType().Name}.");
                return;
            }
            prop.objectReferenceValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        /// <summary>
        /// Carga el Sprite de una textura, sin importar si el Texture Importer
        /// está en modo Single o Multiple (todo el arte de la diseñadora
        /// viene en modo Multiple con un único sprite dentro).
        /// </summary>
        public static Sprite LoadSprite(string assetPath)
        {
            foreach (Object obj in AssetDatabase.LoadAllAssetsAtPath(assetPath))
            {
                if (obj is Sprite sprite) return sprite;
            }
            Debug.LogError($"[RoomSceneBuildUtils] No se encontró un Sprite en '{assetPath}'.");
            return null;
        }

        /// <summary>Ancho x alto que conserva el aspect ratio original del sprite para un ancho objetivo dado.</summary>
        public static Vector2 SizeFromSprite(Sprite sprite, float targetWidth)
        {
            float aspect = sprite.rect.width / sprite.rect.height;
            return new Vector2(targetWidth, targetWidth / aspect);
        }

        public static void StretchFull(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
    }
}
