using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace Foundry
{
    [ExecuteInEditMode]
    public class FoundryUniqueToken : MonoBehaviour
    {
        public string uniqueToken;

        public void Awake()
        {
            Scene scene = SceneManager.GetActiveScene();
            if (scene.isLoaded)
            {
#if UNITY_2018_3_OR_NEWER
                // Seems that the prefab paradigm has changed from this point on. No need to disconnect the prefab instance.
#else
                // TODO: Clicking on apply still overwrites the prefab even though we disconnect the prefab instance
                // try to find a solution that doesn't break the prefab values.
                PrefabUtility.DisconnectPrefabInstance(this);
#endif
                uniqueToken = "";
            }
        }
    }

}
