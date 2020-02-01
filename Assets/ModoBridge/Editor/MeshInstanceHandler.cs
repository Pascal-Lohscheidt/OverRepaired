using UnityEngine;
using UnityEditor;

namespace Foundry
{
    /// <summary>
    /// Unmanaged mesh instance data structure
    /// </summary>
    /// <param name="entityToken">Entity token.</param>
    /// <param name="displayName">Entity display name.</param>
    /// <param name="entityParentToken">Entity parent token.</param>
    /// <param name="xform">Xform.</param>
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct MeshInstanceData
    {
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)] public string entityToken;
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)] public string displayName;
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)] public string entityParentToken;
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)] public string originalMesh;
        public System.IntPtr xform;
    }

    public static class MeshInstanceHandler
    {
        public static bool DependencyCheck(MeshInstanceData meshInstanceData)
        {
            if (!PackageMapper.TokenExistsInCache(meshInstanceData.originalMesh))
            {
                // TODO: messages slow down the recieve mechanism and spam the log
                //Debug.LogWarningFormat("Mesh Instance '{0}' Dependancy failed! Missing mesh item {1}!", meshInstanceData.entityToken, meshInstanceData.originalMesh);
                return false;
            }

            if (!System.String.IsNullOrEmpty(meshInstanceData.entityParentToken))
            {
                if (!PackageMapper.TokenExistsInCache(meshInstanceData.entityParentToken))
                {
                    // TODO: messages slow down the recieve mechanism and spam the log
                    //Debug.LogWarningFormat("Mesh Instance '{0}' Dependancy failed! Missing parent item {1}!", meshInstanceData.entityToken, meshInstanceData.entityParentToken);
                    return false;
                }
            }

            return true;
        }

        public static bool UpdateMeshInstance(MeshInstanceData meshInstanceData)
        {
            Debug.LogFormat("Mesh Instance Update - Token:{0}, Parent:{1}, Mesh:{2}", meshInstanceData.entityToken, meshInstanceData.entityParentToken, meshInstanceData.originalMesh);

            Object meshInstObj = PackageMapper.GetObjectFromToken(meshInstanceData.entityToken);
            var meshInst = meshInstObj as GameObject;

            var transform = MarshalData.GetXFormFromUnmanagedArray(meshInstanceData.xform);

            // Update the transformation of the item, ignore hierarchy updates for now
            // TODO: Work on hierarchy control systems
            Transform parentTrs = meshInst.transform.parent;
            GameObject parent = null;
            if (parentTrs)
            {
                parent = parentTrs.gameObject;
            }

            SceneTransmissionProtocolUtilities.UpdateObjectHierarchy(meshInst, parent, transform);

            // Update mesh filter
            Mesh mesh = PackageMapper.GetMeshFromToken(meshInstanceData.originalMesh);

            var meshFilter = meshInst.GetComponent<MeshFilter>();

            if (meshFilter)
            {
                meshFilter.sharedMesh = mesh;
            }

            // Update mesh renderer
            GameObject meshPrefab = PackageMapper.GetObjectFromToken(meshInstanceData.originalMesh) as GameObject;
            var meshRenderer = meshPrefab.GetComponent<MeshRenderer>();
            var meshInstRenderer = meshInst.GetComponent<MeshRenderer>();

            if (meshRenderer && meshInstRenderer)
            {
                meshInstRenderer.sharedMaterials = meshRenderer.sharedMaterials;
            }

            return true;
        }

        public static GameObject CreateMeshInstance(MeshInstanceData meshInstanceData)
        {
            Debug.LogFormat("Mesh Instance - Token:{0}, Name:{1} Parent:{2}, Mesh:{3}", meshInstanceData.entityToken, meshInstanceData.displayName, meshInstanceData.entityParentToken, meshInstanceData.originalMesh);

            var transform = MarshalData.GetXFormFromUnmanagedArray(meshInstanceData.xform);

            // Find mesh prefab
            Object meshPrefabObj = PackageMapper.GetObjectFromToken(meshInstanceData.originalMesh);

            // Create a new game object
            GameObject meshInst = Object.Instantiate(meshPrefabObj as GameObject);

            // Remove the "(Clone)" string from name that Unity automatically adds when instantiating
            // and apply the actual display name of the instance
            meshInst.name = meshInstanceData.displayName;

            // Find instance parent
            Object parentObj = PackageMapper.GetObjectFromToken(meshInstanceData.entityParentToken);

            SceneTransmissionProtocolUtilities.UpdateObjectHierarchy(meshInst, parentObj as GameObject, transform);

            // Add token store
            PackageMapper.AddUniqueTokenStore(meshInst, meshInstanceData.entityToken);

            return meshInst;
        }

        public static void CreateAndSendMeshInstanceRequest(
            GameObject meshInstanceObj, 
            string instanceToken,
            string meshToken,
            string parentToken)
        {
            MeshInstanceData meshInstanceData = new MeshInstanceData();

            meshInstanceData.displayName = meshInstanceObj.name;
            meshInstanceData.entityToken = instanceToken;
            meshInstanceData.entityParentToken = parentToken;
            meshInstanceData.originalMesh = meshToken;

            // Add transformation
            meshInstanceData.xform = MarshalData.GetXFormToUnmanagedArray(meshInstanceObj.transform.localToWorldMatrix);

            System.IntPtr meshInstanceDataPtr = System.Runtime.InteropServices.Marshal.AllocHGlobal(System.Runtime.InteropServices.Marshal.SizeOf(meshInstanceData));
            System.Runtime.InteropServices.Marshal.StructureToPtr(meshInstanceData, meshInstanceDataPtr, false);

            Debug.LogFormat("Mesh Instance Push - Token:{0}, Parent:{1}, Mesh:{2}", meshInstanceData.entityToken, meshInstanceData.entityParentToken, meshInstanceData.originalMesh);
            ImportMenu.stpUnitySendMeshInstanceData(meshInstanceDataPtr);

            // Free allocated memory
            System.Runtime.InteropServices.Marshal.FreeHGlobal(meshInstanceData.xform);
            System.Runtime.InteropServices.Marshal.FreeHGlobal(meshInstanceDataPtr);
        }
    }
}
