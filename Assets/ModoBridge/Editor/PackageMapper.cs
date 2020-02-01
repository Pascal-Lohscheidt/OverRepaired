using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

namespace Foundry
{
    [InitializeOnLoad]
    class PackageMapper : AssetPostprocessor
    {
        public static readonly string rootPath = "Assets";
        public static readonly string prefabExt = ".prefab";

        public static System.Collections.Generic.Dictionary<string, Material> materialCache = new System.Collections.Generic.Dictionary<string, Material>();

        private static System.Collections.Generic.Dictionary<string, Object> tokenToObjectCache = new System.Collections.Generic.Dictionary<string, Object>();

        private static System.Collections.Generic.Dictionary<Mesh, Object> meshToPrefabCache = new System.Collections.Generic.Dictionary<Mesh, Object>();

        private static System.Collections.Generic.Dictionary<Material, Object> materialToPrefabCache = new System.Collections.Generic.Dictionary<Material, Object>();

        public static void RegenerateTokenToObjectMaps()
        {
            Debug.Log("Foundry Bridge: Generating object maps");

            // Go over all items in the scenegraph and extract STP mesh instances
            Object[] allObjects = GameObject.FindObjectsOfType(typeof(MonoBehaviour));

            foreach (var obj in allObjects)
            {
                if (obj is FoundryUniqueToken)
                {
                    FoundryUniqueToken tokenStorage = obj as FoundryUniqueToken;
                    if (tokenStorage && tokenStorage.uniqueToken != "")
                    {
                        MapTokenToObject(tokenStorage.uniqueToken, tokenStorage.gameObject);
                    }
                }
            }

            // Go over all items in the project tab and extract all STP mesh prefab items
            // Construct the system path of the asset folder 
            string dataPath = Application.dataPath;
            // get the system file paths of all the files in the asset folder
            string[] aFilePaths = Directory.GetFiles(dataPath, searchPattern: "*.prefab", searchOption: SearchOption.AllDirectories);

            foreach (string sFilePath in aFilePaths)
            {
                string sAssetPath = sFilePath.Substring(dataPath.Length - rootPath.Length);

                Object objAsset = AssetDatabase.LoadAssetAtPath(sAssetPath, typeof(Object));

                if (objAsset is GameObject)
                {
                    GameObject prefab = objAsset as GameObject;
                    FoundryUniqueToken tokenStore = prefab.GetComponent<FoundryUniqueToken>();

                    if (tokenStore && tokenStore.uniqueToken.Length > 0)
                    {
                        MapTokenToObject(tokenStore.uniqueToken, prefab);

                        // Add mesh prefabs to the list
                        var meshFilter = prefab.GetComponent<MeshFilter>();
                        if (meshFilter && meshFilter.sharedMesh)
                        {
                            MapMeshToPrefab(meshFilter.sharedMesh, prefab);
                        }

                        // Add non STP material prefabs to the list
                        var materialStore = prefab.GetComponent<FoundryMaterialStore>();
                        if (materialStore && materialStore.material)
                        {
                            MapMaterialToPrefab(materialStore.material, prefab);
                        }
                        else
                        {
                            // Add STP materials
                            Material STPMaterial = GetMaterialFromToken(tokenStore.uniqueToken);

                            if (STPMaterial)
                            {
                                MapMaterialToPrefab(STPMaterial, prefab);
                            }
                        }
                    }
                }
            }
        }

        public static void ClearTokenMaps()
        {
            Debug.Log("Foundry Bridge: Clearing object maps");

            tokenToObjectCache.Clear();
            materialCache.Clear();
            meshToPrefabCache.Clear();
            materialToPrefabCache.Clear();
        }

        public static bool MapTokenToObject(string token, Object obj)
        {
            if (System.String.IsNullOrEmpty(token))
            {
                return false;
            }

            if (!tokenToObjectCache.ContainsKey(token))
            {
                tokenToObjectCache.Add(token, obj);
                return true;
            }
            return false;
        }

        public static bool MapMeshToPrefab(Mesh mesh, Object obj)
        {
            if (!meshToPrefabCache.ContainsKey(mesh))
            {
                meshToPrefabCache.Add(mesh, obj);
                return true;
            }
            return false;
        }

        public static bool MapMaterialToPrefab(Material material, Object obj)
        {
            if (!materialToPrefabCache.ContainsKey(material))
            {
                materialToPrefabCache.Add(material, obj);
                return true;
            }
            return false;
        }

        public static bool TokenExistsInCache(string token)
        {
            if (System.String.IsNullOrEmpty(token))
            {
                return false;
            }

            Object obj;
            tokenToObjectCache.TryGetValue(token, out obj);

            // Clear cache if Unity deleted the object
            // TODO: Find if there is a better way of handling game object deletions, like Unity scene graph deletion events
            if (!obj)
            {
                tokenToObjectCache.Remove(token);
                return false;
            }
            
            return true;
        }

        public static Object GetObjectFromToken(string token)
        {
            if (System.String.IsNullOrEmpty(token))
            {
                return null;
            }

            Object obj;
            tokenToObjectCache.TryGetValue(token, out obj);

            // Clear cache if Unity deleted the object
            // TODO: Find if there is a better way of handling game object deletions, like Unity scene graph deletion events
            if (!obj)
            {
                tokenToObjectCache.Remove(token);
            }

            return obj;
        }

        // We depend on prefabs for meshes that are created from STP
        // take the first child mesh item from the prefab and return it
        public static Mesh GetMeshFromToken(string token)
        {
            Object obj = GetObjectFromToken(token);

            if (obj && obj is GameObject)
            {
                return AssetDatabase.LoadAssetAtPath<Mesh>(AssetDatabase.GetAssetPath(obj));
            }

            return null;
        }

        // We depend on prefabs for textures that are created from STP
        // take the first child texture item from the prefab and return it
        public static Texture2D GetTexture2DFromToken(string token)
        {
            Object obj = GetObjectFromToken(token);

            if (obj && obj is GameObject)
            {
                return AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GetAssetPath(obj));
            }

            return null;
        }

        // We depend on prefabs for materials that are created from STP
        // take the first child material item from the prefab and return it
        public static Material GetMaterialFromToken(string token)
        {
            if (System.String.IsNullOrEmpty(token))
            {
                return null;
            }

            // Try to get the material from material cache first
            Material material;
            PackageMapper.materialCache.TryGetValue(token, out material);

            if (material)
            {
                return material;
            }

            // If material instance does not exist try the STP material definition prefabs
            Object obj = GetObjectFromToken(token);

            if (obj && obj is GameObject)
            {
                material = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GetAssetPath(obj));
            }

            // If STP material definition doesn't exist try non STP materials
            if (obj && obj is GameObject && material == null)
            {
                var materialStore = ((GameObject)obj).GetComponent<FoundryMaterialStore>();

                if (materialStore && materialStore.material)
                {
                    return materialStore.material;
                }
            }

            return material;
        }

        public static GameObject GetPrefabFromMesh(Mesh mesh)
        {
            if (!mesh)
            {
                return null;
            }

            Object obj;
            meshToPrefabCache.TryGetValue(mesh, out obj);

            return obj as GameObject;
        }

        public static GameObject GetPrefabFromMaterial(Material material)
        {
            if (!material)
            {
                return null;
            }

            Object obj;
            materialToPrefabCache.TryGetValue(material, out obj);

            return obj as GameObject;
        }

        // Check if the user is deleting assets and remove them from the maps
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string str in deletedAssets)
            {
                Debug.Log("Deleted Asset: " + str);
            }

            // If anything is deleted go through our map and remove all null objects
            if (deletedAssets.Length > 0)
            {
                RemoveDeletedObjects();
            }
        }

        // Removes deleted objects from the mapping
        static void RemoveDeletedObjects()
        {
            var objToRemove = tokenToObjectCache.Where(f => f.Value == null).ToArray();
            foreach (var item in objToRemove)
                tokenToObjectCache.Remove(item.Key);

            var meshToRemove = meshToPrefabCache.Where(f => f.Key == null || f.Value == null).ToArray();
            foreach (var item in meshToRemove)
                meshToPrefabCache.Remove(item.Key);

            var materialToRemove = materialToPrefabCache.Where(f => f.Key == null || f.Value == null).ToArray();
            foreach (var item in materialToRemove)
                materialToPrefabCache.Remove(item.Key);
        }

        /// <summary>
        /// Generate unique prefab asset paths
        /// Function checks if the prefab asset is already existing with the provided path and tries to create a unique path.
        /// </summary>
        public static void GenUniquePrefabAssetPath(string localPrefabPath, out string prefabPath)
        {
            prefabPath = localPrefabPath + PackageMapper.prefabExt;

            // Check if the mesh is already existing and change name until we don't find a mesh
            Object existingPrefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(Object));
            if (existingPrefab)
            {
                int i = 2;
                while (existingPrefab)
                {
                    prefabPath = System.String.Format("{0}_{1}{2}", localPrefabPath, i, PackageMapper.prefabExt);
                    existingPrefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(Object));
                    i++;
                }
            }
        }

        /// <summary>
        /// Create STP folders
        /// Function that creates asset folders if they are not existent.
        /// e.g. "Assets/Foundry/Meshes"
        /// </summary>
        public static void CreateAssetFolders(string assetPath)
        {
            // Create folder path for meshes if it doesn't exist
            string dataPath = Application.dataPath;
            if (!Directory.Exists(dataPath + "/" + assetPath))
            {
                var pathList = assetPath.Split('/');
                var parent = PackageMapper.rootPath;
                foreach (var path in pathList)
                {
                    dataPath = dataPath + "/" + path;
                    if (!Directory.Exists(dataPath))
                    {
                        AssetDatabase.CreateFolder(parent, path);
                    }
                    parent = parent + "/" + path;
                }
            }
        }

        /// <summary>
        /// Add STP token store
        /// Function that adds Foundry token store to the game object.
        /// e.g. "Assets/Foundry/Meshes"
        /// </summary>
        public static void AddUniqueTokenStore(GameObject gameObj, string entityToken)
        {
            // Get token store
            var tokenStore = gameObj.GetComponent<FoundryUniqueToken>();

            // If token store is non existant create a new one
            if (!tokenStore)
            {
                tokenStore = gameObj.AddComponent<FoundryUniqueToken>();
            }

            tokenStore.uniqueToken = entityToken;
        }
    }
}
