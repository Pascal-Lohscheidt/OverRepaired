using UnityEngine;
using UnityEditor;
using System.Runtime.InteropServices;

namespace Foundry
{
    static class NodeHandler
    {
        // Data storage lists for items that we are unable to create due to missing dependents
        // Meshes, missing materials
        public static System.Collections.Generic.List<System.IntPtr> meshDataStore = new System.Collections.Generic.List<System.IntPtr>();
        // Instances, missing meshes
        public static System.Collections.Generic.List<System.IntPtr> meshInstanceDataStore = new System.Collections.Generic.List<System.IntPtr>();
        // Materials, missing textures
        public static System.Collections.Generic.List<System.IntPtr> materialDefinitionDataStore = new System.Collections.Generic.List<System.IntPtr>();

        /// <summary>
        /// Delegate function type for Response objects.
        /// </summary>
        public delegate void AvailableDelegate(System.IntPtr dataPtr);

        /// <summary>
        /// 
        /// Meshes
        /// 
        /// Parse mesh data
        /// Function decides if we are updating an already existing mesh or creating a new one.
        /// If a new mesh has been created this function will go through stored mesh instances
        /// and check if it can create new mesh instances.
        /// </summary>
        public static void ParseMeshData(System.IntPtr meshDataPtr)
        {
            try
            {
                var meshData = (MeshData)System.Runtime.InteropServices.Marshal.PtrToStructure(meshDataPtr, typeof(MeshData));

                // Convert unmanaged to managed memory
                ManagedMeshData managedMeshData = MeshHandler.ManageMeshMemory(meshData);

                // If all dependants are here, create or update the mesh
                bool dependencySuccess = MeshHandler.DependencyCheck(managedMeshData, meshData.entityToken);
                if (dependencySuccess)
                {
                    ParseMeshData(meshData, managedMeshData);

                    // Delete the data
                    ImportMenu.stpUnityDeleteMeshData(meshDataPtr);
                }
                else
                {
                    // If we are not able to create or update store the mesh data and try later
                    meshDataStore.Add(meshDataPtr);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);

                // Delete the data
                ImportMenu.stpUnityDeleteMeshData(meshDataPtr);
            }
        }

        private static void ParseMeshData(MeshData meshData, ManagedMeshData managedMeshData)
        {
            // Check if the mesh already exists in the package mapper
            if (PackageMapper.TokenExistsInCache(meshData.entityToken))
            {
                // If mesh exists try to update it
                MeshHandler.UpdateMesh(meshData, managedMeshData);
            }
            else
            {
                // Create a new mesh object in Unity
                MeshHandler.CreateMesh(meshData, managedMeshData);

                // This loop starts over if we created a parent and a child is waiting in the store
                bool newItemCreated = true;
                while (newItemCreated)
                {
                    // Iterate through mesh instance store and try to create or update
                    // mesh instances that this mesh is a dependent of
                    IterateMeshInstanceStore(out newItemCreated);
                }
            }
        }

        // Iterate through mesh store that were missing dependants and check if we can create new mesh objects 
        // or update existing mesh objects
        public static void IterateMeshStore()
        {
            for (int i = meshDataStore.Count - 1; i >= 0; i--)
            {
                MeshData meshData = (MeshData)System.Runtime.InteropServices.Marshal.PtrToStructure(meshDataStore[i], typeof(MeshData));

                // Convert unmanaged to managed memory
                ManagedMeshData managedMeshData = MeshHandler.ManageMeshMemory(meshData);

                // If all dependants are here, create or update the mesh
                if (MeshHandler.DependencyCheck(managedMeshData, meshData.entityToken))
                {
                    ParseMeshData(meshData, managedMeshData);

                    // Delete the data
                    ImportMenu.stpUnityDeleteMeshData(meshDataStore[i]);

                    // Remove from the store
                    meshDataStore.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 
        /// Mesh Instances
        /// 
        /// Parse mesh instance data
        /// Function decides if we are updating an already existing mesh instance ( GameObject ) or creating a new one.
        /// </summary>
        public static void ParseMeshInstanceData(System.IntPtr meshInstanceDataPtr)
        {
            try
            {
                var meshInstanceData = (MeshInstanceData)System.Runtime.InteropServices.Marshal.PtrToStructure(meshInstanceDataPtr, typeof(MeshInstanceData));

                // If all dependants are here, create or update the mesh instance
                if (MeshInstanceHandler.DependencyCheck(meshInstanceData))
                {
                    bool newItemCreated;
                    ParseMeshInstanceData(meshInstanceData, out newItemCreated);

                    // Delete the data
                    ImportMenu.stpUnityDeleteMeshInstanceData(meshInstanceDataPtr);

                    // This loop starts over if we created a parent and a child is waiting in the store
                    while (newItemCreated)
                    {
                        // Iterate through mesh instance store and try to create or update
                        // mesh instances that this mesh instance is a dependent of
                        IterateMeshInstanceStore(out newItemCreated);
                    }
                }
                else
                {
                    // If we are not able to create or update store the mesh data and try later
                    meshInstanceDataStore.Add(meshInstanceDataPtr);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);

                // Delete the data
                ImportMenu.stpUnityDeleteMeshInstanceData(meshInstanceDataPtr);
            }
            
        }

        private static void ParseMeshInstanceData(MeshInstanceData meshInstData, out bool newItemCreated)
        {
            newItemCreated = false;
            // Check if the object already exists in the package mapper
            if (PackageMapper.TokenExistsInCache(meshInstData.entityToken))
            {
                // If object exists try to update it
                MeshInstanceHandler.UpdateMeshInstance(meshInstData);
            }
            else
            {
                // Create a new GameObject in the hierarchy
                GameObject meshInst = MeshInstanceHandler.CreateMeshInstance(meshInstData);

                if (meshInst)
                {
                    PackageMapper.MapTokenToObject(meshInstData.entityToken, meshInst);
                    newItemCreated = true;
                }
            }
        }

        // Iterate through mesh instance store that were missing dependants 
        // and check if we can create new mesh instance objects or update existing mesh objects
        public static void IterateMeshInstanceStore(out bool newItemCreated)
        {
            newItemCreated = false;
            for (int i = meshInstanceDataStore.Count - 1; i >= 0; i--)
            {
                MeshInstanceData meshInstanceData = (MeshInstanceData)System.Runtime.InteropServices.Marshal.PtrToStructure(meshInstanceDataStore[i], typeof(MeshInstanceData));
                // If all dependants are here, create or update the mesh
                bool currentItemCreated = false;
                bool dependencySuccess = MeshInstanceHandler.DependencyCheck(meshInstanceData);
                if (dependencySuccess)
                {
                    ParseMeshInstanceData(meshInstanceData, out currentItemCreated);

                    // Delete the data
                    ImportMenu.stpUnityDeleteMeshInstanceData(meshInstanceDataStore[i]);

                    // Remove from the store
                    meshInstanceDataStore.RemoveAt(i);
                }

                // True if at least one item was created
                newItemCreated = newItemCreated || currentItemCreated;
            }
        }

        /// <summary>
        /// 
        /// Textures
        /// 
        /// Parse texture data
        /// Function decides if we are updating an already existing texture or creating a new one.
        /// If a new texture has been created this function will go through stored materials
        /// and check if it can create a new material with this texture available.
        /// </summary>
        public static void ParseTextureData(System.IntPtr textureDataPtr)
        {
            try
            {
                var textureData = (TextureData)System.Runtime.InteropServices.Marshal.PtrToStructure(textureDataPtr, typeof(TextureData));

                // If all dependants are here, create or update the mesh
                ParseTextureData(textureData);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }

            ImportMenu.stpUnityDeleteTextureData(textureDataPtr);
        }

        public static void ParseTextureData(TextureData textureData)
        {
            // Check if the object already exists in the package mapper
            if (PackageMapper.TokenExistsInCache(textureData.entityToken))
            {
                // If object exists try to update it
                TextureHandler.UpdateTexture(textureData);
            }
            else
            {
                // Create a new GameObject in the hierarchy
                Object texture = TextureHandler.CreateTexture(textureData);

                if (texture)
                {
                    PackageMapper.MapTokenToObject(textureData.entityToken, texture);

                    // Iterate through material store and try to create or update
                    // materials that this texture is a dependent of
                    IterateMaterialDefinitionStore();
                }
            }
        }

        /// <summary>
        /// 
        /// Material definitions
        /// 
        /// Parse material definitions data
        /// Starts material definition node parsing.
        /// If a new material has been created this function will go through stored meshes 
        /// and check if it can create new mesh with this material available.
        /// </summary>
        public static void ParseMaterialDefinitionData(System.IntPtr materialDefinitionDataPtr)
        {
            try
            {
                var materialDefinitionData = (MaterialDefinitionData)System.Runtime.InteropServices.Marshal.PtrToStructure(materialDefinitionDataPtr, typeof(MaterialDefinitionData));

                // If all dependants are here, parse object data
                bool dependencySuccess = MaterialDefinitionHandler.DependencyCheck(materialDefinitionData);
                if (dependencySuccess)
                {
                    ParseMaterialDefinitionData(materialDefinitionData);

                    // Delete object data
                    ImportMenu.stpUnityDeleteMaterialDefinitionData(materialDefinitionDataPtr);
                }
                // If we are missing dependants store the object data
                else
                {
                    materialDefinitionDataStore.Add(materialDefinitionDataPtr);
                }   
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);

                // Delete object data
                ImportMenu.stpUnityDeleteMaterialDefinitionData(materialDefinitionDataPtr);
            }
        }

        public static void ParseMaterialDefinitionData(MaterialDefinitionData materialDefinitionData)
        {
            // Check if the object already exists in the package mapper
            if (PackageMapper.TokenExistsInCache(materialDefinitionData.entityToken))
            {
                // If object exists skip update
                // TODO: At some point decide how we handle material updates.
                // Important note to consider is that we will be adding already existing
                // materials in Unity that STP didn't create to the material map and how to handle those updates.
                Debug.LogWarning("Material Definition Update not supported yet! Discarding material data!!");
                return;
            }
            else
            {
                // Create a new GameObject in the hierarchy
                Object newMaterial = MaterialDefinitionHandler.CreateMaterial(materialDefinitionData);

                if (newMaterial)
                {
                    PackageMapper.MapTokenToObject(materialDefinitionData.entityToken, newMaterial);

                    // If a new material has been created go through the stored list of meshes
                    // to see if we can create a new one with this material available
                    IterateMeshStore();
                }
            }
        }

        // Iterate through material store that were missing dependants and check if we can create new material objects 
        // or update existing material objects
        public static void IterateMaterialDefinitionStore()
        {
            for (int i = materialDefinitionDataStore.Count - 1; i >= 0; i--)
            {
                MaterialDefinitionData materialDefinitionData = (MaterialDefinitionData)System.Runtime.InteropServices.Marshal.PtrToStructure(materialDefinitionDataStore[i], typeof(MaterialDefinitionData));

                // If all dependants are here, create the material
                bool dependencySuccess = MaterialDefinitionHandler.DependencyCheck(materialDefinitionData);
                if (dependencySuccess)
                {
                    ParseMaterialDefinitionData(materialDefinitionData);

                    // Delete object data
                    ImportMenu.stpUnityDeleteMaterialDefinitionData(materialDefinitionDataStore[i]);

                    // Remove from the store
                    materialDefinitionDataStore.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 
        /// Cameras
        /// 
        /// Parse camera data
        /// Function decides if we are updating an already existing camera or creating a new one.
        /// </summary>
        public static void ParseCameraData(System.IntPtr cameraDataPtr)
        {
            try
            {
                var cameraData = (CameraData)System.Runtime.InteropServices.Marshal.PtrToStructure(cameraDataPtr, typeof(CameraData));

                // Create or update the camera
                ParseCameraData(cameraData);

                // Delete the data
                ImportMenu.stpUnityDeleteCameraData(cameraDataPtr);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);

                // Delete the data
                ImportMenu.stpUnityDeleteCameraData(cameraDataPtr);
            }
        }

        private static void ParseCameraData(CameraData cameraData)
        {
            // Check if the camera already exists in the package mapper
            if (PackageMapper.TokenExistsInCache(cameraData.entityToken))
            {
                // If mesh exists try to update it
                CameraHandler.UpdateCamera(cameraData);
            }
            else
            {
                // Create a new camera object in Unity
                var cameraInst = CameraHandler.CreateCamera(cameraData);

                if (cameraInst)
                {
                    PackageMapper.MapTokenToObject(cameraData.entityToken, cameraInst);
                }
            }
        }
    }
}
