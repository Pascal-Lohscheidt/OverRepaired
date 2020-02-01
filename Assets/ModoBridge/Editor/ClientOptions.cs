using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Foundry
{

/// <summary>
/// Serialized resource storing client options, some of which are transmitted to the server.
/// </summary>
[InitializeOnLoad]
public class ClientOptions : ScriptableObject
{
    [System.Runtime.InteropServices.DllImport("STPNativeUnity")]
    private static extern void stpUnitySetServerOptions(string server, int port, int connection_timeout, int cancel_check_interval);

    [SerializeField]
    private string server = "localhost";
    [SerializeField]
    private int port = 12000;
    [SerializeField]
    private int connection_timeout = 10000; // milliseconds
    [SerializeField]
    private int cancel_check_interval = 1000; // milliseconds

    private static string resourcesDir = "Resources";
    private static readonly string resourceFile = "clientoptions";
    private static ClientOptions instance = null;

    /// <summary>
    /// Gets the singleton instance of this class.
    /// This will create the asset resource if it does not exist, with default values.
    /// Otherwise, it will load the asset resource from the project.
    /// </summary>
    /// <value>The instance.</value>
    public static ClientOptions Instance
    {
        get
        {
            // Try to find existing client options
            if (null == instance)
            {
                instance = Resources.FindObjectsOfTypeAll<ClientOptions>().FirstOrDefault();
            }

            // Try to find existing client options file
            if (null == instance)
            {
                var guids = UnityEditor.AssetDatabase.FindAssets (resourceFile);
                if (guids.Length > 0)
                {
                    var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                    // Note that the following functions do not actually fetch the resource,
                    // even though FindAssets is returning the GUIDs
                    //  UnityEditor.AssetDatabase.LoadAssetAtPath<ClientOptions>(p);
                    //  Resources.LoadAll ("");
                    //  AssetDatabase.LoadAllAssetRepresentationsAtPath (p);
                    //  Resources.FindObjectsOfTypeAll (typeof(ClientOptions));
                    var assets = UnityEditor.AssetDatabase.LoadAllAssetsAtPath (path);
                    if (assets.Length > 0)
                    {
                        instance = assets[0] as ClientOptions;
                    }
                }
            }

            // Create a new client option file in the relative folder to the ClientOption.cs folder
            if (null == instance)
            {
                // Create the object instance
                instance = CreateInstance<ClientOptions>();

                // Get the ClientOption.cs folder
                MonoScript scriptObj = MonoScript.FromScriptableObject(instance);
                string scriptPath = AssetDatabase.GetAssetPath(scriptObj);

                string clientOptionsPath = System.IO.Path.GetDirectoryName(scriptPath);
                clientOptionsPath = System.IO.Path.Combine(clientOptionsPath, resourcesDir);
                clientOptionsPath = System.IO.Path.Combine(clientOptionsPath, resourceFile) + ".asset";
                DefaultAssets.EnsureAssetFolderHierarchyExists(clientOptionsPath);

                AssetDatabase.CreateAsset (instance, clientOptionsPath);
                AssetDatabase.SaveAssets ();
                AssetDatabase.Refresh ();
                EditorUtility.FocusProjectWindow ();
                Selection.activeObject = instance;
            }
            return instance;
        }
    }

    /// <summary>
    /// Invoked when the GUI is shown.
    /// </summary>
    public void OnGUI()
    {
        this.server = EditorGUILayout.TextField ("Server IP/name", this.server);
        this.port = EditorGUILayout.IntField ("Port", this.port);
        this.connection_timeout = EditorGUILayout.IntField("Connection timeout (ms)", this.connection_timeout);
        this.cancel_check_interval = EditorGUILayout.IntField("Cancel check interval (ms)", this.cancel_check_interval);
    }

    /// <summary>
    /// Save the data to the asset database.
    /// </summary>
    public void save()
    {
        EditorUtility.SetDirty (this);
        AssetDatabase.SaveAssets ();
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// Broadcast the data to the native plugin.
    /// </summary>
    public void broadcast()
    {
        stpUnitySetServerOptions(this.server, this.port, this.connection_timeout, this.cancel_check_interval);
    }
}

} // namespace Foundry
