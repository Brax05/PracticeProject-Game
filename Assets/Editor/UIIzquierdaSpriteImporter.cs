using UnityEditor;

namespace TravesiaACasa.Editor
{
    public sealed class UIIzquierdaSpriteImporter : AssetPostprocessor
    {
        private void OnPreprocessTexture()
        {
            if (!assetPath.StartsWith("Assets/Arte/juego/UIIzquierda/") && assetPath != "Assets/Arte/juego/Mute.png")
                return;

            TextureImporter importer = (TextureImporter)assetImporter;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
        }
    }
}
