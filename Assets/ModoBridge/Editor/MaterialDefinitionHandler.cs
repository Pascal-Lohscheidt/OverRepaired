using UnityEditor;
using UnityEngine;
using System.IO;

namespace Foundry
{

    /// <summary>
    /// Unmanaged material definition data structure
    /// Used to marshal memory from unmanaged native plugin to managed memory
    /// </summary>
    /// <param name="entityToken">Entity token.</param>
    /// <param name="displayName">Entity display name.</param>
    /// <param name="shaderModel">
    /// Shader model for this material enumeration flag.
    ///     StandardPBR,    - Physically-based, metallic roughness model
    ///     ModelUnknown    - Model is not recognised.
    /// </param>
    /// <param name="blendMode">
    /// The blend mode of the shader model enumeration flag.
    ///     Opaque,         - Opaque blend mode
    ///     Translucent,    - Translucent blend mode
    ///     ModeUnknown     - Blend mode is not recognised.
    /// </param>
    /// <param name="parameterCount">Parameter count for this material.</param>
    /// <param name="parameters">Parameter structures in bytes.</param>
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct MaterialDefinitionData
    {
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)] public string entityToken;
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)] public string displayName;
        public int shaderModel;
        public int blendMode;
        public int parameterCount;
        public System.IntPtr parameters;
    }

    enum ShaderModel : int
    {
        StandardPBR,    // Physically-based, metallic roughness model
        ModelUnknown    // Model is not recognised.
    }

    enum BlendMode : int
    {
        Opaque,         //Opaque blend mode
        Translucent,    //Translucent blend mode
        ModeUnknown     //Blend mode is not recognised.
    }

    /// <summary>
    /// Unmanaged material definition parameter data structure
    /// Used to marshal memory from unmanaged native plugin to managed memory
    /// </summary>
    /// <param name="name">Parameter name.</param>
    /// <param name="target">Paramter target. Targets are used to choose what material input should be targeted.</param>
    /// <param name="type">
    /// The type of a parameter to be mapped to a target enumeration flag.
    /// Float,          - Single floating point constant
    /// Float2,         - Double floating point constant
    /// Float3,         - Triple floating point constant
    /// Float4,         - Quadruple floating point constant
    /// RGBA8,          - 8 bit four element color.
    /// Texture,        - Entity token of a texture asset.
    /// TypeUnknown     - Type was not recognised.
    /// </param>
    /// <param name="textureToken">Texture unique token to be used for targeted input.</param>
    /// <param name="floatCount">Count of float data array.</param>
    /// <param name="floatData">Float data in raw bytes.</param>
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct MaterialDefinitionParameterData
    {
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)] public string name;
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)] public string target;
        public int type;

        // Only one of texture token or float data will not be null
        // depending on the parameter type. If we are dealing with a
        // texture parameter it will hold textureToken data 
        // else float count and float data will hold parameter data.
        // This is mostly for marshalling safety.
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)] public string textureToken;

        public int floatCount;
        public System.IntPtr floatData;
    }

    enum ParamType : int
    {
        Float,          // Single floating point constant
        Float2,         // Double floating point constant
        Float3,         // Triple floating point constant
        Float4,         // Quadruple floating point constant
        RGBA8,          // 8 bit four element color.
        Texture,        // Entity token of a texture asset.
        TypeUnknown     // Type was not recognised.
    }

    class MaterialDefinitionHandler
    {
        /// <summary>
        /// Define material asset STP default path
        /// </summary>
        public static string assetPath = "Foundry/Materials";

        /// <summary>
        /// Dependency check
        /// Checks if all of the dependencies of the mesh are available in Unity
        /// </summary>
        public static bool DependencyCheck(MaterialDefinitionData materialDefinitionData)
        {
            // Check if we have textures available
            var currentReadPtr = materialDefinitionData.parameters;
            var parameterDataSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(MaterialDefinitionParameterData));
            for (var i = 0; i < materialDefinitionData.parameterCount; ++i)
            {
                // Marshal parameter data
                MaterialDefinitionParameterData parameterData = (MaterialDefinitionParameterData)System.Runtime.InteropServices.Marshal.PtrToStructure(currentReadPtr, typeof(MaterialDefinitionParameterData));
                currentReadPtr = MarshalData.AddToIntPtr(currentReadPtr, parameterDataSize);

                // Get the parameter type
                ParamType type = (ParamType)parameterData.type;

                if (type == ParamType.Texture)
                {
                    // Check if we have a texture ready for this material definition
                    if (!PackageMapper.GetTexture2DFromToken(parameterData.textureToken))
                    {
                        // TODO: messages slow down the recieve mechanism and spam the log
                        //Debug.LogWarningFormat("Texture '{0}' for '{1}' material has not yet been created!",
                        //                         parameterData.textureToken,
                        //                         materialDefinitionData.entityToken);

                        return false;
                    }
                }
            }

            return true;
        }

        public static Object CreateMaterial(MaterialDefinitionData materialDefinitionData)
        {
            ShaderModel shaderModel = (ShaderModel)materialDefinitionData.shaderModel;
            BlendMode blendMode = (BlendMode)materialDefinitionData.blendMode;

            Debug.LogFormat("Material Definition - Token:{0} Display Name:{1}, {2} shader model {3} blend mode {4} parameter count", materialDefinitionData.entityToken, materialDefinitionData.displayName, shaderModel, blendMode, materialDefinitionData.parameterCount);

            // Create a new Unity material with "Standard" Unity shader
            // Shader string names are assumed to use the Unity Standard shader code
            Material newMaterial = new Material(Shader.Find("Standard"));

            // Set blend mode options
            switch(blendMode)
            {
                case BlendMode.ModeUnknown:
                case BlendMode.Opaque:
                    // Default blend mode, skip modification
                    break;

                case BlendMode.Translucent:
                    // Code taken from https://answers.unity.com/questions/1004666/change-material-rendering-mode-in-runtime.html
                    // Changes what options are available in Unity shader GUI
                    newMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    newMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    newMaterial.SetInt("_ZWrite", 0);
                    newMaterial.DisableKeyword("_ALPHATEST_ON");
                    newMaterial.DisableKeyword("_ALPHABLEND_ON");
                    newMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    newMaterial.renderQueue = 3000;
                    break;
            }

            var currentReadPtr = materialDefinitionData.parameters;
            var parameterDataSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(MaterialDefinitionParameterData));
            for (var i = 0; i < materialDefinitionData.parameterCount; ++i)
            {
                // Marshal parameter data
                MaterialDefinitionParameterData parameterData = (MaterialDefinitionParameterData)System.Runtime.InteropServices.Marshal.PtrToStructure(currentReadPtr, typeof(MaterialDefinitionParameterData));
                currentReadPtr = MarshalData.AddToIntPtr(currentReadPtr, parameterDataSize);

                // Get the parameter type
                ParamType type = (ParamType)parameterData.type;

                Debug.LogFormat("Material Parameter - Name:{0} Type:{1} Target:{2}, {3} texture token", parameterData.name, type, parameterData.target, parameterData.textureToken);

                // Get the right shader input and connect parameter data
                string shaderStringTarget;
                Vector4 vect;
                int offset = 0;
                switch (type)
                {
                    case ParamType.Float:
                        shaderStringTarget = FindScalarInput(parameterData.target);
                        if (System.String.IsNullOrEmpty(shaderStringTarget))
                        {
                            break;
                        }

                        newMaterial.SetFloat(shaderStringTarget, MarshalData.GetFloatFromUnmanagedArray(parameterData.floatData, offset));
                        break;

                    case ParamType.Float2:
                        shaderStringTarget = FindVectorInput(parameterData.target);
                        if (System.String.IsNullOrEmpty(shaderStringTarget))
                        {
                            break;
                        }

                        vect.x = MarshalData.GetFloatFromUnmanagedArray(parameterData.floatData, offset);
                        offset += MarshalData.sizeOfInt;
                        vect.y = MarshalData.GetFloatFromUnmanagedArray(parameterData.floatData, offset);
                        vect.z = 0.0f;
                        vect.w = 0.0f;
                        newMaterial.SetVector(shaderStringTarget, vect);
                        break;

                    case ParamType.Float3:
                        shaderStringTarget = FindVectorInput(parameterData.target);
                        if (System.String.IsNullOrEmpty(shaderStringTarget))
                        {
                            break;
                        }

                        vect.x = MarshalData.GetFloatFromUnmanagedArray(parameterData.floatData, offset);
                        offset += MarshalData.sizeOfInt;
                        vect.y = MarshalData.GetFloatFromUnmanagedArray(parameterData.floatData, offset);
                        offset += MarshalData.sizeOfInt;
                        vect.z = MarshalData.GetFloatFromUnmanagedArray(parameterData.floatData, offset);
                        vect.w = 0.0f;
                        newMaterial.SetVector(shaderStringTarget, vect);
                        break;

                    case ParamType.Float4:
                    case ParamType.RGBA8:
                        shaderStringTarget = FindVectorInput(parameterData.target);
                        if (System.String.IsNullOrEmpty(shaderStringTarget))
                        {
                            break;
                        }

                        vect.x = MarshalData.GetFloatFromUnmanagedArray(parameterData.floatData, offset);
                        offset += MarshalData.sizeOfInt;
                        vect.y = MarshalData.GetFloatFromUnmanagedArray(parameterData.floatData, offset);
                        offset += MarshalData.sizeOfInt;
                        vect.z = MarshalData.GetFloatFromUnmanagedArray(parameterData.floatData, offset);
                        offset += MarshalData.sizeOfInt;
                        vect.w = MarshalData.GetFloatFromUnmanagedArray(parameterData.floatData, offset);
                        newMaterial.SetVector(shaderStringTarget, vect);
                        break;

                    case ParamType.Texture:
                        shaderStringTarget = FindTextureInput(parameterData.target);
                        if (System.String.IsNullOrEmpty(shaderStringTarget))
                        {
                            break;
                        }

                        Texture texture = PackageMapper.GetTexture2DFromToken(parameterData.textureToken);
                        
                        newMaterial.SetTexture(shaderStringTarget, texture);

                        // TODO: Emission keyword doesn't work in Unity 2018.2.15f1 as it doesn't enable
                        // emission checkbox for some reason
                        // Special case for emission textures, enable the emission map on the shader
                        if (shaderStringTarget == ShaderEmissionTexture)
                        {
                            newMaterial.EnableKeyword("_EMISSION");
                        }
                        break;
                }
            }

            // Create material default STP folders if they don't exist
            PackageMapper.CreateAssetFolders(MaterialDefinitionHandler.assetPath);

            // Generate a unique material name
            string materialPath = Application.dataPath + "/" + MaterialDefinitionHandler.assetPath;
            string uniqueMaterialName;
            GenUniqueMaterialAssetName(materialPath, materialDefinitionData.displayName, out uniqueMaterialName);

            // Set the unique material name as the display name
            newMaterial.name = uniqueMaterialName;

            // Create a GameObject prefab object and use it as a database item of STP used materials
            GameObject materialPrefab = new GameObject(uniqueMaterialName);

            // Add Foundry unique token storage component, store unique material token
            var tokenStorage = materialPrefab.AddComponent<FoundryUniqueToken>();
            tokenStorage.uniqueToken = materialDefinitionData.entityToken;

            // Create the prefab asset
            var localPrefabPath = System.String.Format("{0}/{1}/{2}", PackageMapper.rootPath, MaterialDefinitionHandler.assetPath, uniqueMaterialName);
            string prefabPath;

            PackageMapper.GenUniquePrefabAssetPath(localPrefabPath, out prefabPath);
#if UNITY_2018_3_OR_NEWER
            Object prefabAsset = PrefabUtility.SaveAsPrefabAsset(materialPrefab, prefabPath);
#else
            Object prefabAsset = PrefabUtility.CreatePrefab(prefabPath, materialPrefab);
#endif

            // Map the prefab to the material
            PackageMapper.MapMaterialToPrefab(newMaterial, prefabAsset);

            AssetDatabase.AddObjectToAsset(newMaterial, prefabAsset);
            AssetDatabase.SaveAssets();

            // Remove game object that's used for creating a prefab from scene hierarchy
            Object.DestroyImmediate(materialPrefab);

            return prefabAsset;
        }

        // STP material target naming
        private static readonly string MaterialDefinitionTargetColor = "materialdef.color";
        private static readonly string MaterialDefinitionTargetMetallic = "materialdef.metallic";
        private static readonly string MaterialDefinitionTargetRoughness = "materialdef.roughness";
        private static readonly string MaterialDefinitionTargetNormal = "materialdef.normal";
        private static readonly string MaterialDefinitionTargetEmissiveColor = "materialdef.emissive";
        private static readonly string MaterialDefinitionTargetBump = "materialdef.bump";
        private static readonly string MaterialDefinitionTargetAmbientOcclusion = "materialdef.ambientocclusion";
        private static readonly string MaterialDefinitionTargetDetailMask = "materialdef.detailmask";
        private static readonly string MaterialDefinitionTargetColorSecondary = "materialdef.colorsecondary";
        private static readonly string MaterialDefinitionTargetNormalSecondary = "materialdef.normalsecondary";

        // Unity Standard shader scalar properties string names
        private static readonly string ShaderMetallic = "_Metallic";
        private static readonly string ShaderRoughness = "_Glossiness";

        private static string FindScalarInput(string target)
        {
            if (target == MaterialDefinitionTargetMetallic)
            {
                return ShaderMetallic;
            }
            else if (target == MaterialDefinitionTargetRoughness)
            {
                return ShaderRoughness;
            }

            return "";
        }

        // Unity Standard shader vector/color properties
        private static readonly string ShaderColor = "_Color";
        private static readonly string ShaderEmissionColor = "_EmissionColor";

        private static string FindVectorInput(string target)
        {
            if (target == MaterialDefinitionTargetColor)
            {
                return ShaderColor;
            }
            else if (target == MaterialDefinitionTargetEmissiveColor)
            {
                return ShaderEmissionColor;
            }

            return "";
        }

        // Unity Standard shader texture properties
        private static readonly string ShaderColorTexture = "_MainTex";
        private static readonly string ShaderMetalicGlossinessTexture = "_MetallicGlossMap";
        private static readonly string ShaderNormalORBumpTexture = "_BumpMap";
        private static readonly string ShaderOcclusionTexture = "_OcclusionMap";
        private static readonly string ShaderEmissionTexture = "_EmissionMap";

        private static readonly string ShaderDetailMaskTexture = "_DetailMap";
        private static readonly string ShaderDetailAlbedoTexture = "_DetailAlbedoMap";
        private static readonly string ShaderDetailNormalTexture = "_DetailNormalMap";

        private static string FindTextureInput(string target)
        {
            if (target == MaterialDefinitionTargetColor)
            {
                return ShaderColorTexture;
            }
            else if (target == MaterialDefinitionTargetMetallic)
            {
                return ShaderMetalicGlossinessTexture;
            }
            else if (target == MaterialDefinitionTargetNormal || target == MaterialDefinitionTargetBump)
            {
                return ShaderNormalORBumpTexture;
            }
            else if (target == MaterialDefinitionTargetEmissiveColor)
            {
                return ShaderEmissionTexture;
            }
            else if (target == MaterialDefinitionTargetAmbientOcclusion)
            {
                return ShaderOcclusionTexture;
            }
            else if (target == MaterialDefinitionTargetDetailMask)
            {
                return ShaderDetailMaskTexture;
            }
            else if (target == MaterialDefinitionTargetColorSecondary)
            {
                return ShaderDetailAlbedoTexture;
            }
            else if (target == MaterialDefinitionTargetNormalSecondary)
            {
                return ShaderDetailNormalTexture;
            }

            return "";
        }

        /// <summary>
        /// Generate unique material asset name
        /// Function checks if the material asset is already existing with the provided path 
        /// and tries to create a unique asset name.
        /// </summary>
        private static void GenUniqueMaterialAssetName(string localMaterialPath, string materialName, out string uniqueMaterialName)
        {
            uniqueMaterialName = materialName;

            // Check if the material prefab is already existing and change name until we don't find a material prefab file
            int i = 2;
            while (File.Exists(localMaterialPath + "/" + uniqueMaterialName + PackageMapper.prefabExt))
            {
                uniqueMaterialName = System.String.Format("{0}_{1}", materialName, i);
                i++;
            }
        }

        /// <summary>
        /// Create and send material definition request
        /// Parses data for building of material deifintion request 
        /// and invokes native plugin to send the request to the server
        /// </summary>
        public static void CreateAndSendMaterialDefinitionRequest(Material material, string token)
        {
            MaterialDefinitionData materialDefinitionData = new MaterialDefinitionData();

            materialDefinitionData.displayName = material.name;
            materialDefinitionData.entityToken = token;

            System.IntPtr materialDefinitionDataPtr = System.Runtime.InteropServices.Marshal.AllocHGlobal(System.Runtime.InteropServices.Marshal.SizeOf(materialDefinitionData));
            System.Runtime.InteropServices.Marshal.StructureToPtr(materialDefinitionData, materialDefinitionDataPtr, false);

            Debug.LogFormat("Material Definition Push - Token:{0}", materialDefinitionData.entityToken);
            ImportMenu.stpUnitySendMaterialDefinitionData(materialDefinitionDataPtr);

            System.Runtime.InteropServices.Marshal.FreeHGlobal(materialDefinitionDataPtr);
        }

        /// <summary>
        /// CreateNonSTPMaterialPrefab
        /// Create a prefab for materials not created by STP to store token information 
        /// and connect the token to the material
        /// </summary>
        public static GameObject CreateNonSTPMaterialPrefab(Material material)
        {
            // Create material default STP folders if they don't exist
            PackageMapper.CreateAssetFolders(MaterialDefinitionHandler.assetPath);

            // Generate a unique material name
            string materialPath = Application.dataPath + "/" + MaterialDefinitionHandler.assetPath;
            string uniqueMaterialName;
            GenUniqueMaterialAssetName(materialPath, material.name, out uniqueMaterialName);

            // Create a GameObject prefab object and use it as a database item of STP used materials
            GameObject materialPrefab = new GameObject(uniqueMaterialName);

            // Add Foundry unique token storage component, store unique material token
            var tokenStorage = materialPrefab.AddComponent<FoundryUniqueToken>();
            tokenStorage.uniqueToken = ExportUtilities.GenerateMaterialToken(material);

            // Add Foundry material storage component, store non STP material
            var materialStorage = materialPrefab.AddComponent<FoundryMaterialStore>();
            materialStorage.material = material;

            // Create the prefab asset
            var localPrefabPath = System.String.Format("{0}/{1}/{2}", PackageMapper.rootPath, MaterialDefinitionHandler.assetPath, uniqueMaterialName);
            string prefabPath;

            PackageMapper.GenUniquePrefabAssetPath(localPrefabPath, out prefabPath);
#if UNITY_2018_3_OR_NEWER
            Object prefabAsset = PrefabUtility.SaveAsPrefabAsset(materialPrefab, prefabPath);
#else
            Object prefabAsset = PrefabUtility.CreatePrefab(prefabPath, materialPrefab);
#endif
            // Add the prefab object to package mapper
            PackageMapper.AddUniqueTokenStore(prefabAsset as GameObject, tokenStorage.uniqueToken);
            PackageMapper.MapMaterialToPrefab(material, prefabAsset);

            AssetDatabase.SaveAssets();

            // Remove game object that's used for creating a prefab from scene hierarchy
            Object.DestroyImmediate(materialPrefab);

            return prefabAsset as GameObject;
        }
    }
}
