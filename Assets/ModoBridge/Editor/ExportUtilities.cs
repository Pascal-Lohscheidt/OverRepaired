using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

using System.Collections.Generic;

namespace Foundry
{
    public class ExportUtilities
    {
        public static void PushAllItems()
        {
            Debug.Log("Pushing all available items...");
            GameObject[] objList = GameObject.FindObjectsOfType<GameObject>();

            ExecutePushCommand(objList);
        }

        public static void PushSelectedItems()
        {
            Debug.Log("Pushing selected items...");
            GameObject[] objList = Selection.gameObjects;

            ExecutePushCommand(objList);
        }

        private static void ExecutePushCommand(GameObject[] objList)
        {
            try
            {
                float currentProgress = 0.0f;
                float progressPerChunk;
                bool cancel = false;
                EditorUtility.DisplayCancelableProgressBar("Gathering unique meshes...", "", currentProgress);

                // Filter only unique meshes to be sent to remove data duplication
                HashSet<GameObject> uniqueMeshList = new HashSet<GameObject>();
                HashSet<Material> uniqueMaterialList = new HashSet<Material>();
                progressPerChunk = 0.10f / objList.Length;
                foreach (GameObject obj in objList)
                {
                    cancel = EditorUtility.DisplayCancelableProgressBar("Gathering unique meshes...", "", currentProgress);
                    if (cancel)
                    {
                        EditorUtility.ClearProgressBar();
                        return;
                    }

                    // If are dealing with an GameObject that has MeshFilter mesh item
                    var meshFilter = obj.GetComponent<MeshFilter>();
                    if (meshFilter && meshFilter.sharedMesh)
                    {
                        uniqueMeshList.Add(obj);

                        // Add mesh materials to the list if available
                        var meshRenderer = obj.GetComponent<MeshRenderer>();
                        if (meshRenderer && meshRenderer.sharedMaterials.Length > 0)
                        {
                            for (var i = 0; i < meshRenderer.sharedMaterials.Length; i++)
                            {
                                uniqueMaterialList.Add(meshRenderer.sharedMaterials[i]);
                            }
                        }
                    }

                    currentProgress += progressPerChunk;
                }

                // Send material definitions first
                progressPerChunk = 0.10f / uniqueMaterialList.Count;
                foreach (Material material in uniqueMaterialList)
                {
                    cancel = EditorUtility.DisplayCancelableProgressBar("Pushing Materials...", "Parsing data from Unity materials and pushing to the STP server.", currentProgress);
                    if (cancel)
                    {
                        EditorUtility.ClearProgressBar();
                        return;
                    }

                    PushMaterial(material);

                    currentProgress += progressPerChunk;
                }

                // Send mesh items first
                progressPerChunk = 0.60f / uniqueMeshList.Count;
                foreach (GameObject obj in uniqueMeshList)
                {
                    cancel = EditorUtility.DisplayCancelableProgressBar("Pushing Meshes...", "Parsing data from Unity meshes and pushing to the STP server.", currentProgress);
                    if (cancel)
                    {
                        EditorUtility.ClearProgressBar();
                        return;
                    }

                    PushMesh(obj);

                    currentProgress += progressPerChunk;
                }

                // Send mesh instance items second
                progressPerChunk = 0.20f / objList.Length;
                foreach (GameObject obj in objList)
                {
                    cancel = EditorUtility.DisplayCancelableProgressBar("Pushing Game Objects...", "Pushing game objects to the STP server.", currentProgress);
                    if (cancel)
                    {
                        EditorUtility.ClearProgressBar();
                        return;
                    }

                    // If are dealing with an GameObject that has MeshFilter mesh item
                    var meshFilter = obj.GetComponent<MeshFilter>();
                    if (meshFilter && meshFilter.sharedMesh)
                    {
                        PushMeshInstance(obj);
                    }

                    currentProgress += progressPerChunk;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogErrorFormat("Push command failed! Error message: {0}", e.Message);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private static bool PushMaterial(Material material)
        {
            var materialPrefab = PackageMapper.GetPrefabFromMaterial(material);

            // If material prefab doesn't exist it means this material is new to STP and we need
            // to add it to the database and generate a prefab for it
            if (!materialPrefab)
            {
                materialPrefab = MaterialDefinitionHandler.CreateNonSTPMaterialPrefab(material);
            }

            var tokenStore = materialPrefab.GetComponent<FoundryUniqueToken>();

            // If the token store is non existant bail out
            if (!tokenStore || System.String.IsNullOrEmpty(tokenStore.uniqueToken))
            {
                Debug.LogWarningFormat("Material '{0}' push failed! No available unique token!", material.name);
                return false;
            }

            MaterialDefinitionHandler.CreateAndSendMaterialDefinitionRequest(material, tokenStore.uniqueToken);
            return true;
        }

        private static void PushMesh(GameObject obj)
        {
            // Get the mesh from object
            Mesh mesh = obj.GetComponent<MeshFilter>().sharedMesh;

            // Get the prefab and extract material data and get or generate a mesh token
            var prefab = PackageMapper.GetPrefabFromMesh(mesh);

            string uniqueToken;
            if (!prefab)
            {
                // Create unique mesh token
                uniqueToken = GenerateMeshToken(mesh);
            }
            else
            {
                // Grab existing token or generate a new one
                uniqueToken = GetTokenFromStorage(prefab);
                if (System.String.IsNullOrEmpty(uniqueToken))
                {
                    uniqueToken = GenerateMeshToken(mesh);
                }
            }

            // Gather material unique tokens
            List<string> materialTokens = new List<string>();
            var meshRenderer = obj.GetComponent<MeshRenderer>();
            if (meshRenderer)
            {
                for (var i = 0; i < meshRenderer.sharedMaterials.Length; i++)
                {
                    var materialPrefab = PackageMapper.GetPrefabFromMaterial(meshRenderer.sharedMaterials[i]);

                    var tokenStore = materialPrefab.GetComponent<FoundryUniqueToken>();
                    if (tokenStore && !System.String.IsNullOrEmpty(tokenStore.uniqueToken))
                    {
                        materialTokens.Add(tokenStore.uniqueToken);
                    }
                    else
                    {
                        materialTokens.Add("");
                    }
                }
            }

            // Send the mesh package to the server
            MeshHandler.CreateAndSendMeshRequest(mesh, materialTokens, uniqueToken);
        }

        private static void PushMeshInstance(GameObject obj)
        {
            // Get the mesh from object
            Mesh mesh = obj.GetComponent<MeshFilter>().sharedMesh;

            // Get the prefab and extract material data and get or generate a mesh token
            var prefab = PackageMapper.GetPrefabFromMesh(mesh);

            // Get or generate unique mesh token
            string meshToken;
            if (!prefab)
            {
                // Create unique mesh token
                meshToken = GenerateMeshToken(mesh);
            }
            else
            {
                // Grab existing token or generate a new one
                meshToken = GetTokenFromStorage(prefab);
                if (System.String.IsNullOrEmpty(meshToken))
                {
                    meshToken = GenerateMeshToken(mesh);
                }
            }

            // Grab existing instance token or generate a new one
            string instanceToken = GetTokenFromStorage(obj);
            if (System.String.IsNullOrEmpty(instanceToken))
            {
                // Create unique instance token
                instanceToken = GenerateMeshInstanceToken(obj);
            }

            // Send the mesh instance package to the server
            MeshInstanceHandler.CreateAndSendMeshInstanceRequest(obj, instanceToken, meshToken, "");
        }

        private static string GetTokenFromStorage(GameObject obj)
        {
            // Check if the mesh object
            FoundryUniqueToken tokenStore = obj.GetComponent<FoundryUniqueToken>();

            // If the token store exists on the game object
            // use that one because it's probably an object that STP created
            if (tokenStore && !System.String.IsNullOrEmpty(tokenStore.uniqueToken))
            {
                return tokenStore.uniqueToken;
            }

            return null;
        }

        private static string GenerateMeshToken(Mesh mesh)
        {
            // If it doesn't exist generate a new one
            string uniqueMeshToken;
            var scene = SceneManager.GetActiveScene();

            uniqueMeshToken = "/";
            uniqueMeshToken += (System.String.IsNullOrEmpty(scene.name)) ? "Untitled" : scene.name;
            uniqueMeshToken += "/meshes/";
            uniqueMeshToken += mesh.name;

            if (PackageMapper.TokenExistsInCache(uniqueMeshToken))
            {
                appendUniqueSuffix(ref uniqueMeshToken);
            }

            return uniqueMeshToken;
        }

        private static string GenerateMeshInstanceToken(GameObject obj)
        {
            // If it doesn't exist generate a new one
            string uniqueMeshInstanceToken;
            var scene = SceneManager.GetActiveScene();

            uniqueMeshInstanceToken = "/";
            uniqueMeshInstanceToken += (System.String.IsNullOrEmpty(scene.name)) ? "Untitled" : scene.name;
            uniqueMeshInstanceToken += "/instances/";
            uniqueMeshInstanceToken += obj.name;
            uniqueMeshInstanceToken += "_inst";

            if (PackageMapper.TokenExistsInCache(uniqueMeshInstanceToken))
            {
                appendUniqueSuffix(ref uniqueMeshInstanceToken);
            }

            // Add unique token store to the GameObject and add the token to the mapper
            // so we can reuse and update the mesh instance with STP
            PackageMapper.AddUniqueTokenStore(obj, uniqueMeshInstanceToken);
            PackageMapper.MapTokenToObject(uniqueMeshInstanceToken, obj);

            return uniqueMeshInstanceToken;
        }

        public static string GenerateMaterialToken(Material material)
        {
            // If it doesn't exist generate a new one
            string uniqueMaterialToken;
            var scene = SceneManager.GetActiveScene();

            uniqueMaterialToken = "/";
            uniqueMaterialToken += (System.String.IsNullOrEmpty(scene.name)) ? "Untitled" : scene.name;
            uniqueMaterialToken += "/materials/";
            uniqueMaterialToken += material.name;

            if (PackageMapper.TokenExistsInCache(uniqueMaterialToken))
            {
                appendUniqueSuffix(ref uniqueMaterialToken);
            }

            return uniqueMaterialToken;
        }

        private static void appendUniqueSuffix(ref string token)
        {
            string tokenStore;
            tokenStore = token;
            int suffixNum = 1;
            bool exists = true;
            while (exists)
            {
                tokenStore = token;
                tokenStore += "_";
                tokenStore += suffixNum;
                suffixNum++;

                exists = PackageMapper.TokenExistsInCache(tokenStore);
            }

            token = tokenStore;
        }
    }
}
