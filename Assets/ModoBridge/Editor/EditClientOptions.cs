using UnityEngine;
using UnityEditor;

namespace Foundry
{

/// <summary>
/// Dialog for editing the client options of the Scene Transmission Protocol.
/// The details are saved in a separate object (for serialization) called ClientOptions.
/// </summary>
public class EditClientOptions : EditorWindow
{
    [MenuItem("Tools/Modo Bridge/Set Client Options")]
    static void Init()
    {
        var window = GetWindow<EditClientOptions>();
        var title = new GUIContent ("Options");
        window.titleContent = title;
    }

    [MenuItem("Foundry/Set Client Options", true)]
    static bool CheckInit()
    {
        if (!ImportMenu.stpUnityIsClientRunning())
        {
            return true;
        }

        return false;
    }

    void Awake()
    {
        // ensure that this exists
        // it was causing a null reference exception when only called from OnGUI
        // when the resource asset didn't exist
        var instance = ClientOptions.Instance;
    }

    void OnGUI()
    {
        GUILayout.Label ("Details", EditorStyles.boldLabel);
        ClientOptions.Instance.OnGUI ();

        if (GUILayout.Button ("Save"))
        {
            ClientOptions.Instance.save ();
            GetWindow<EditClientOptions>().Close ();
        }
    }
}

} // namespace Foundry
