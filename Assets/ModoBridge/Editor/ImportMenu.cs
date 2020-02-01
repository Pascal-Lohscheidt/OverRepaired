using UnityEngine;
using UnityEditor;

namespace Foundry
{

/// <summary>
/// Extend the Editor's menus with import options.
/// </summary>
[InitializeOnLoad]
public class ImportMenu
{
    // STP client control and status functions
    [System.Runtime.InteropServices.DllImport("STPNativeUnity")]
    private static extern void stpUnityStartClient();

    [System.Runtime.InteropServices.DllImport("STPNativeUnity")]
    private static extern void stpUnityStopClient();

    [System.Runtime.InteropServices.DllImport("STPNativeUnity")]
    public static extern bool stpUnityIsClientRunning();

    [System.Runtime.InteropServices.DllImport("STPNativeUnity")]
    private static extern void stpUnityImport();

    [System.Runtime.InteropServices.DllImport("STPNativeUnity")]
    private static extern void stpUnityUserCancelledImport();

    // Mesh import/export functions
    [System.Runtime.InteropServices.DllImport("STPNativeUnity")]
    public static extern bool stpUnitySendMeshData(System.IntPtr meshData);

    [System.Runtime.InteropServices.DllImport("STPNativeUnity")]
    private static extern void stpUnitySetMeshAvailableDelegate(NodeHandler.AvailableDelegate meshAvailable);

    [System.Runtime.InteropServices.DllImport("STPNativeUnity")]
    public static extern void stpUnityDeleteMeshData(System.IntPtr meshData);

    // Mesh instance import/export functions
    [System.Runtime.InteropServices.DllImport("STPNativeUnity")]
    public static extern bool stpUnitySendMeshInstanceData(System.IntPtr meshInstanceData);

    [System.Runtime.InteropServices.DllImport("STPNativeUnity")]
    private static extern void stpUnitySetMeshInstanceAvailableDelegate(NodeHandler.AvailableDelegate meshInstanceAvailable);

    [System.Runtime.InteropServices.DllImport("STPNativeUnity")]
    public static extern void stpUnityDeleteMeshInstanceData(System.IntPtr meshInstanceData);

    // Other import functions
    [System.Runtime.InteropServices.DllImport("STPNativeUnity")]
    private static extern void stpUnitySetOtherAvailableDelegate(MarshalData.OtherAvailableDelegate otherAvailable);

    // Material import functions
    [System.Runtime.InteropServices.DllImport("STPNativeUnity")]
    private static extern void stpUnitySetMaterialAvailableDelegate(MarshalData.MaterialAvailableDelegate materialAvailable);

    // Light import functions
    [System.Runtime.InteropServices.DllImport("STPNativeUnity")]
    private static extern void stpUnitySetDirectionalLightAvailableDelegate(MarshalData.DirectionalLightAvailableDelegate lightAvailable);

    [System.Runtime.InteropServices.DllImport("STPNativeUnity")]
    private static extern void stpUnitySetSpotLightAvailableDelegate(MarshalData.SpotLightAvailableDelegate lightAvailable);

    [System.Runtime.InteropServices.DllImport("STPNativeUnity")]
    private static extern void stpUnitySetPointLightAvailableDelegate(MarshalData.PointLightAvailableDelegate lightAvailable);

    // Material import functions
    [System.Runtime.InteropServices.DllImport("STPNativeUnity")]
    private static extern void stpUnitySetMaterialParameterDelegates(
        MarshalData.MaterialSetFloatParameterDelegate setFloat,
        MarshalData.MaterialSetFloatArrayParameterDelegate setFloatArray,
        MarshalData.MaterialSetIntegerParameterDelegate setInteger);

    // Material definition import/export functions
    [System.Runtime.InteropServices.DllImport("STPNativeUnity")]
    public static extern void stpUnitySendMaterialDefinitionData(System.IntPtr materialDefinitionData);

    [System.Runtime.InteropServices.DllImport("STPNativeUnity")]
    private static extern void stpUnitySetMaterialDefinitionAvailableDelegate(NodeHandler.AvailableDelegate materialDefinitionAvailable);

    [System.Runtime.InteropServices.DllImport("STPNativeUnity")]
    public static extern void stpUnityDeleteMaterialDefinitionData(System.IntPtr materialDefinitionData);

    // Texture import functions
    [System.Runtime.InteropServices.DllImport("STPNativeUnity")]
    private static extern bool stpUnitySetTextureAvailableDelegate(NodeHandler.AvailableDelegate textureAvailable);

    [System.Runtime.InteropServices.DllImport("STPNativeUnity")]
    public static extern void stpUnityDeleteTextureData(System.IntPtr textureData);

    // Camera import functions
    [System.Runtime.InteropServices.DllImport("STPNativeUnity")]
    private static extern bool stpUnitySetCameraAvailableDelegate(NodeHandler.AvailableDelegate cameraAvailable);

    [System.Runtime.InteropServices.DllImport("STPNativeUnity")]
    public static extern void stpUnityDeleteCameraData(System.IntPtr cameraData);

    // Idle update function checking if we have packages waiting to be parsed
    [System.Runtime.InteropServices.DllImport("STPNativeUnity")]
    private static extern void stpUnityIdleUpdate();

    // Error handling delegate
    [System.Runtime.InteropServices.DllImport("STPNativeUnity")]
    private static extern void stpUnitySetReportErrorDelegate(MarshalData.ReportErrorDelegate reportError);

    static ImportMenu()
    {
        EditorApplication.update += Update;
    }

    private static void AssociateManagedDelegatesWithNativePlugin()
    {
        // set delegates once
        // cannot do this in the static constructor, as the native plugin doesn't appear to have been loaded

        // Material instance delefate
        stpUnitySetMaterialAvailableDelegate (MarshalData.MaterialAvailable);
        stpUnitySetMaterialParameterDelegates(
            MarshalData.MaterialSetFloat,
            MarshalData.MaterialSetFloatArray,
            MarshalData.MaterialSetInt);

        // Mesh delegate
        stpUnitySetMeshAvailableDelegate (NodeHandler.ParseMeshData);

        // Mesh instance delegate
        stpUnitySetMeshInstanceAvailableDelegate (NodeHandler.ParseMeshInstanceData);

        // Light delegates
        stpUnitySetDirectionalLightAvailableDelegate (MarshalData.DirectionalLightAvailable);
        stpUnitySetSpotLightAvailableDelegate (MarshalData.SpotLightAvailable);
        stpUnitySetPointLightAvailableDelegate (MarshalData.PointLightAvailable);

        // Material Definition delegate
        stpUnitySetMaterialDefinitionAvailableDelegate(NodeHandler.ParseMaterialDefinitionData);

        // Texture delegate
        stpUnitySetTextureAvailableDelegate (NodeHandler.ParseTextureData);

        // Camera delegate
        stpUnitySetCameraAvailableDelegate (NodeHandler.ParseCameraData);

        // Other objects delegate
        stpUnitySetOtherAvailableDelegate (MarshalData.OtherAvailable);

        // Error delegate
        stpUnitySetReportErrorDelegate(
            MarshalData.ShowEditorErrorDialog);
    }

    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptReload()
    {
        // this may be useful, but it is ONLY called when the scripts change
        // it isn't called if the editor is closed, and then reopened, with no script changes
        // native plugins are also not available at this point
    }

    private static bool startup = true;
    static void Update()
    {
        if (startup)
        {
            AssociateManagedDelegatesWithNativePlugin();
            DefaultAssets.Instance.Create();
            startup = false;

            // Added the regenerate object maps on startup due to
            // Unity recompilation ( reimport and code change ) of scrips losing map data
            PackageMapper.RegenerateTokenToObjectMaps();
        }
        stpUnityIdleUpdate ();
    }

    // Start client command
    [MenuItem("Tools/Modo Bridge/Start Client")]
    private static void StartClient()
    {
        if (stpUnityIsClientRunning())
        {
            Debug.LogWarning("Client already running!");
            return;
        }

        ClientOptions.Instance.broadcast();

        PackageMapper.RegenerateTokenToObjectMaps();

        Debug.Log("Foundry Bridge: Starting client thread");
        stpUnityStartClient();
    }

    [MenuItem("Tools/Modo Bridge/Start Client", true)]
    private static bool CheckStartClient()
    {
        if (stpUnityIsClientRunning())
        {
            return false;
        }

        return true;
    }

    // Stop client command
    [MenuItem("Tools/Modo Bridge/Stop Client")]
    private static void StopClient()
    {
        Debug.Log("Foundry Bridge: Stopping client thread");
        stpUnityStopClient();

        // Stopping client clears package maps
        PackageMapper.ClearTokenMaps();
    }

    [MenuItem("Tools/Modo Bridge/Stop Client", true)]
    private static bool CheckStopClient()
    {
        if (!stpUnityIsClientRunning())
        {
            return false;
        }

        return true;
    }

    // Push all command
    [MenuItem("Tools/Modo Bridge/Push All")]
    private static void PushAll()
    {
        if (!stpUnityIsClientRunning())
        {
            Debug.LogError("Client not running! Canceling the 'Push All' command!");
            return;
        }

        ExportUtilities.PushAllItems();
    }

    [MenuItem("Tools/Modo Bridge/Push All", true)]
    private static bool CheckPushAll()
    {
        if (!stpUnityIsClientRunning())
        {
            return false;
        }

        return true;
    }

    // Push selected command
    [MenuItem("Tools/Modo Bridge/Push Selected")]
    private static void PushSelected()
    {
        if (!stpUnityIsClientRunning())
        {
            Debug.LogError("Client not running! Canceling the 'Push Selected' command!");
            return;
        }

        ExportUtilities.PushSelectedItems();
    }

    [MenuItem("Tools/Modo Bridge/Push Selected", true)]
    private static bool CheckPushSelected()
    {
        if (!stpUnityIsClientRunning() || Selection.gameObjects.Length == 0)
        {
            return false;
        }

        return true;
    }

    // Import from server
    [MenuItem("Tools/Modo Bridge/Import From Server")]
    private static void Import()
    {
        ClientOptions.Instance.broadcast();
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        stpUnityImport();
    }

    [MenuItem("Tools/Modo Bridge/Import From Server", true)]
    private static bool CheckImport()
    {
        if (!stpUnityIsClientRunning())
        {
            return false;
        }

        return true;
    }

    // Import from server into new scene
    [MenuItem("Tools/Modo Bridge/Import From Server (New Scene)")]
    private static void ImportIntoNewScene()
    {
        SceneTransmissionProtocolUtilities.CreateNewScene();
        Import();
    }

    [MenuItem("Tools/Modo Bridge/Import From Server (New Scene)", true)]
    private static bool CheckImportIntoNewScene()
    {
        if (!stpUnityIsClientRunning())
        {
            return false;
        }

        return true;
    }

    // Cancel import
    [MenuItem("Tools/Modo Bridge/Cancel Import")]
    private static void CancelImport()
    {
        stpUnityUserCancelledImport();
    }

    [MenuItem("Tools/Modo Bridge/Cancel Import", true)]
    private static bool CheckCancelImport()
    {
        if (!stpUnityIsClientRunning())
        {
            return false;
        }

        return true;
    }
}

} // namespace Foundry
