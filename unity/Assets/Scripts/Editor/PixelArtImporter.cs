// Tự đặt chế độ nhập ảnh pixel art cho mọi ảnh trong Resources/Frames và Resources/Bg:
// lọc Point (không nhòe), không nén, giữ đúng cỡ (không làm tròn lũy thừa 2).
using UnityEditor;
using UnityEngine;

namespace LongThan.EditorTools
{
    public class PixelArtImporter : AssetPostprocessor
    {
        void OnPreprocessTexture()
        {
            if (!assetPath.Contains("/Resources/Frames/") && !assetPath.Contains("/Resources/Bg/")) return;
            var ti = (TextureImporter)assetImporter;
            ti.textureType = TextureImporterType.Default;
            ti.filterMode = FilterMode.Point;
            ti.textureCompression = TextureImporterCompression.Uncompressed;
            ti.npotScale = TextureImporterNPOTScale.None;
            ti.mipmapEnabled = false;
            ti.alphaIsTransparency = true;
            ti.wrapMode = TextureWrapMode.Clamp;
            ti.maxTextureSize = 2048;
        }
    }
}
