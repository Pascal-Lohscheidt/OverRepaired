using UnityEditor;
using UnityEngine;
using System.IO;

namespace Foundry
{
    /// <summary>
    /// Unmanaged texture data structure
    /// Used to marshal memory from unmanaged native plugin to managed memory
    /// </summary>
    /// <param name="entityToken">Entity token.</param>
    /// <param name="displayName">Entity display name.</param>
    /// <param name="height">Height of the texture pixels.</param>
    /// <param name="width">Width of the texture pixels.</param>
    /// <param name="imgFormat">
    /// STP image format enumeration flag
    ///    RawRGBA,  32 bit uncompressed RGBA.
    ///    RawRGB,   24 bit uncompressed RGB.
    ///    RawGray,  8 bit uncompressed gray-scale.
    ///    Unknown   Unknown format
    /// </param>
    /// <param name="imgData">Image bytes of pixel data.</param>
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct TextureData
    {
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)] public string entityToken;
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)] public string displayName;
        public int              height;
        public int              width;
        public int              imgFormat;
        public System.IntPtr    imgData;
    }

    enum ImgFormat : int
    {
        RawRGBA,  //32 bit uncompressed RGBA.
        RawRGB,   //24 bit uncompressed RGB.
        RawGray,  //8 bit uncompressed gray-scale.
        Unknown   //Unknown format
    }

    public static class TextureHandler
    {
        /// <summary>
        /// Define texture asset STP default path
        /// </summary>
        public static string assetPath = "Foundry/Textures";

        public static string textureShaderName = "_MainTex";

        public static Object CreateTexture(TextureData textureData)
        {
            ImgFormat imgFormat = (ImgFormat)textureData.imgFormat;
            TextureFormat texFormat = GetTextureFormat(imgFormat, textureData.entityToken);

            Debug.LogFormat("Texture - Token:{0}, {1} width {2} height {3} image type", textureData.entityToken, textureData.width, textureData.height, imgFormat);

            // Create a new Unity texture
            Texture2D newTexture = new Texture2D(textureData.width, textureData.height, texFormat, true);

            UpdateTextureData(newTexture, textureData, imgFormat);

            // Create texture folders if they don't exist
            PackageMapper.CreateAssetFolders(TextureHandler.assetPath);

            // Generate a unique texture name
            string texturePath = Application.dataPath + "/" + TextureHandler.assetPath;
            string uniqueTextureName;
            GenUniqueTextureAssetName(texturePath, textureData.displayName, out uniqueTextureName);

            // Set the unique texture name as the display name
            newTexture.name = uniqueTextureName;

            // Create a GameObject prefab object and use it as a database item of STP used textures
            GameObject texturePrefab = new GameObject(textureData.displayName);

            // Add Foundry unique token storage component, store unique texture token
            var tokenStorage = texturePrefab.AddComponent<FoundryUniqueToken>();
            tokenStorage.uniqueToken = textureData.entityToken;

            // Create the prefab asset
            var localPrefabPath = System.String.Format("{0}/{1}/{2}", PackageMapper.rootPath, TextureHandler.assetPath, uniqueTextureName);
            string prefabPath;

            PackageMapper.GenUniquePrefabAssetPath(localPrefabPath, out prefabPath);

#if UNITY_2018_3_OR_NEWER
            Object prefabAsset = PrefabUtility.SaveAsPrefabAsset(texturePrefab, prefabPath);
#else
            Object prefabAsset = PrefabUtility.CreatePrefab(prefabPath, texturePrefab);
#endif
            AssetDatabase.AddObjectToAsset(newTexture, prefabAsset);
            AssetDatabase.SaveAssets();

            // Remove game object that's used for creating a prefab from scene hierarchy
            Object.DestroyImmediate(texturePrefab);

            return prefabAsset;
        }

        public static void UpdateTexture(TextureData textureData)
        {
            ImgFormat imgFormat = (ImgFormat)textureData.imgFormat;

            Debug.LogFormat("Texture Update - Token:{0}, {1} width {2} height {3} image type", textureData.entityToken, textureData.width, textureData.height, imgFormat);

            Texture2D texture = PackageMapper.GetTexture2DFromToken(textureData.entityToken);
            UpdateTextureData(texture, textureData, imgFormat);
        }

        private static TextureFormat GetTextureFormat(ImgFormat imgFormat, string entityToken)
        {
            // Convert STP image format to Unity texture format
            TextureFormat texFormat;
            switch (imgFormat)
            {
                case ImgFormat.RawRGBA:
                    texFormat = TextureFormat.RGBA32;
                    break;

                case ImgFormat.RawRGB:
                    texFormat = TextureFormat.RGB24;
                    break;

                case ImgFormat.RawGray:
                    texFormat = TextureFormat.R8;
                    break;

                default:
                    Debug.LogWarningFormat("Unknown texture type for item {0}! Using RGBA32 type.", entityToken);
                    texFormat = TextureFormat.RGBA32;
                    break;
            }

            return texFormat;
        }

        private static void UpdateTextureData(Texture2D texture, TextureData textureData, ImgFormat imgFormat)
        {
            // Fill image colors per pixel
            Color[] colorList = new Color[textureData.width * textureData.height];
            int positionOffset = 0;

            // Flip image pixels due to Unity SetPixels texture method flipping the image
            // First color in the color list will be the last pixel in the image 
            // and the resulting image will be flipped and mirrored
            for (int height = textureData.height - 1; height >= 0; height--)
            {
                for (int width = 0; width < textureData.width; width++)
                {
                    int pixelIndex = height * textureData.width + width;
                    // Convert STP image format to Unity texture format
                    switch (imgFormat)
                    {
                        case ImgFormat.RawRGBA:
                            colorList[pixelIndex].r = MarshalData.GetByteFromUnmanagedArray(textureData.imgData, positionOffset) / 255.0f;
                            positionOffset += MarshalData.sizeOfByte;
                            colorList[pixelIndex].g = MarshalData.GetByteFromUnmanagedArray(textureData.imgData, positionOffset) / 255.0f;
                            positionOffset += MarshalData.sizeOfByte;
                            colorList[pixelIndex].b = MarshalData.GetByteFromUnmanagedArray(textureData.imgData, positionOffset) / 255.0f;
                            positionOffset += MarshalData.sizeOfByte;
                            colorList[pixelIndex].a = MarshalData.GetByteFromUnmanagedArray(textureData.imgData, positionOffset) / 255.0f;
                            positionOffset += MarshalData.sizeOfByte;
                            break;

                        case ImgFormat.RawRGB:
                            colorList[pixelIndex].r = MarshalData.GetByteFromUnmanagedArray(textureData.imgData, positionOffset) / 255.0f;
                            positionOffset += MarshalData.sizeOfByte;
                            colorList[pixelIndex].g = MarshalData.GetByteFromUnmanagedArray(textureData.imgData, positionOffset) / 255.0f;
                            positionOffset += MarshalData.sizeOfByte;
                            colorList[pixelIndex].b = MarshalData.GetByteFromUnmanagedArray(textureData.imgData, positionOffset) / 255.0f;
                            positionOffset += MarshalData.sizeOfByte;
                            break;

                        case ImgFormat.RawGray:
                            colorList[pixelIndex].r = MarshalData.GetByteFromUnmanagedArray(textureData.imgData, positionOffset) / 255.0f;
                            positionOffset += MarshalData.sizeOfByte;
                            break;
                    }
                }
            }
            texture.SetPixels(colorList);
            texture.Apply(true);
        }

        /// <summary>
        /// Generate unique texture asset name
        /// Function checks if the texture asset is already existing with the provided path 
        /// and tries to create a unique asset name.
        /// </summary>
        private static void GenUniqueTextureAssetName(string localTexturePath, string textureName, out string uniqueTextureName)
        {
            uniqueTextureName = textureName;

            // Check if the texture is already existing and change name until we don't find a texture file
            int i = 2;
            while (File.Exists(localTexturePath + "/" + uniqueTextureName + PackageMapper.prefabExt))
            {
                uniqueTextureName = System.String.Format("{0}_{1}", textureName, i);
                i++;
            }
        }
    }
}
