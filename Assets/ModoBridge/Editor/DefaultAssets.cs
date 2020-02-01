using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Foundry
{

/// <summary>
/// New projects require some standard default assets, e.g. materials understood by the Scene Transmission Protocol.
/// </summary>
[InitializeOnLoad]
public class DefaultAssets : ScriptableObject
{
    static readonly string defaultAssetsDir = "Materials/Default";
    static string defaultAssetsPath;
    private static DefaultAssets instance = null;

    public static DefaultAssets Instance
    {
            get
            { 
                // Try to find existing client options
                if (null == instance)
                {
                    instance = Resources.FindObjectsOfTypeAll<DefaultAssets>().FirstOrDefault();
                }

                if (null == instance)
                {
                    // Create the object instance
                    instance = CreateInstance<DefaultAssets>();
                }

                return instance;
            }
    }

    /// <summary>
    /// Utility function to ensure that the given directory path exists, so that assets can be written within it.
    /// Paths need to begin with "Assets".
    /// </summary>
    /// <param name="path">The path to ensure exists.</param>
    public static void EnsureAssetFolderHierarchyExists(string path)
    {
        var parent = Path.GetDirectoryName(path);
        if ("Assets" != parent)
        {
            EnsureAssetFolderHierarchyExists(parent);
        }
        if (!AssetDatabase.IsValidFolder(path))
        {
            var child = Path.GetFileName(path);
            AssetDatabase.CreateFolder(parent, child);
        }
    }

    private static string DiffuseMaterialPath
    {
        get
        {
            return Path.Combine(defaultAssetsPath, "M_SceneProtocolDefault") + ".mat";
        }
    }

    /// <summary>
    /// Returns the default Unity diffuse material.
    /// </summary>
    /// <value>The diffuse material.</value>
    public static Material DiffuseMaterial
    {
        get;
        private set;
    }

    private static string PlasticMaterialPath
    {
        get
        {
            return Path.Combine(defaultAssetsPath, "M_SceneProtocolPlastic") + ".mat";
        }
    }

    /// <summary>
    /// Returns the default Unity plastic material.
    /// </summary>
    /// <value>The plastic material.</value>
    public static Material PlasticMaterial
    {
        get;
        private set;
    }

    private static string MetalMaterialPath
    {
        get
        {
            return Path.Combine(defaultAssetsPath, "M_SceneProtocolMetal") + ".mat";
        }
    }

    /// <summary>
    /// Returns the default Unity metal material.
    /// </summary>
    /// <value>The metal material.</value>
    public static Material MetalMaterial
    {
        get;
        private set;
    }

    private static string GlassMaterialPath
    {
        get
        {
            return Path.Combine(defaultAssetsPath, "M_SceneProtocolGlass") + ".mat";
        }
    }

    /// <summary>
    /// Returns the default Unity glass material.
    /// </summary>
    /// <value>The glass material.</value>
    public static Material GlassMaterial
    {
        get;
        private set;
    }

    private delegate void SetMaterialParametersDelegate(Material mat);

    private static Material CreateMaterial(
            string name,
            Shader shader,
            string path,
            SetMaterialParametersDelegate setParams,
            ref bool requires_save)
    {
        if (!File.Exists(path))
        {
            var mat = new Material(shader);
            mat.name = name;
            setParams(mat);
            AssetDatabase.CreateAsset (mat, path);
            requires_save = true;
            return mat;
        }
        else
        {
            return AssetDatabase.LoadAssetAtPath<Material>(path);
        }
    }

    /// <summary>
    /// Create the default assets.
    /// </summary>
    public void Create()
    {
        var stdShader = Shader.Find("Standard");

        // Get the DefaultAssets.cs folder
        MonoScript scriptObj = MonoScript.FromScriptableObject(instance);
        string scriptPath = AssetDatabase.GetAssetPath(scriptObj);

        defaultAssetsPath = System.IO.Path.GetDirectoryName(scriptPath);
        defaultAssetsPath = System.IO.Path.Combine(defaultAssetsPath, defaultAssetsDir);

        EnsureAssetFolderHierarchyExists(defaultAssetsPath);

        var requires_save = false;

        DiffuseMaterial = CreateMaterial("M_SceneProtocolDefault", stdShader, DiffuseMaterialPath, mat =>
            {
                mat.SetFloat("_Metallic", 0.0f);
                mat.SetFloat("_Glossiness", 0.0f);
            },
            ref requires_save);

        PlasticMaterial = CreateMaterial("M_SceneProtocolPlastic", stdShader, PlasticMaterialPath, mat =>
            {
                mat.SetFloat("_Metallic", 0.0f);
                mat.SetFloat("_Glossiness", 0.5f);
            },
            ref requires_save);

        MetalMaterial = CreateMaterial("M_SceneProtocolMetal", stdShader, MetalMaterialPath, mat =>
            {
                mat.SetFloat("_Metallic", 1.0f);
                mat.SetFloat("_Glossiness", 1.0f);
            },
            ref requires_save);

        GlassMaterial = CreateMaterial("M_SceneProtocolGlass", stdShader, GlassMaterialPath, mat =>
            {
                mat.SetFloat("_Metallic", 0.0f);
                mat.SetFloat("_Glossiness", 0.5f);
                // from https://forum.unity.com/threads/access-rendering-mode-var-on-standard-shader-via-scripting.287002/#post-1961025
                mat.SetFloat("_Mode", 3); // transparent, and necessary for serialization
                mat.color = new Color(1, 1, 1, 0);
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.DisableKeyword("_ALPHABLEND_ON");
                mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;
            },
            ref requires_save);

        if (requires_save)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}

} // namespace Foundry
