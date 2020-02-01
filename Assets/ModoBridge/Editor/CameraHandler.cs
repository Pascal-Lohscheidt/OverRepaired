using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEditor;
using UnityEngine;

namespace Foundry
{
    /// <summary>
    /// Unmanaged camera data structure
    /// Used to marshal memory from unmanaged native plugin to managed memory
    /// </summary>
    /// <param name="entityToken">Entity token.</param>
    /// <param name="displayName">Entity display name.</param>
    /// <param name="xform">Xform.</param>
    /// <param name="type">
    /// STP camera projection enumeration
    ///    Perspective,  Perspective projection (with foreshortening)
    ///    Orthographic,  Orthographic projection (no foreshortening)
    ///    Unknown,  Unknown projection type
    /// </param>
    /// <param name="verticalFov">Vertical field of view in radians. Will return 0.0f if this
    /// is an orthographic or invalid camera.</param>
    /// <param name="aspectRatio">The aspect ratio of the field of view. Will return 0.0f if
    /// this is an orthographic or invalid camera.</param>
    /// <param name="horizontalMag">The horizontal magnification of the view. Will return 0.0f
    /// if this is a perspective or invalid camera.</param>
    /// <param name="nearClip">The distance to the near clipping plane. Will return -1.0f
    /// if this is an invalid camera.</param>
    /// <param name="farClip">The distance to the far clipping plane. Will return -1.0f
    /// if this is an invalid camera.</param>
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct CameraData
    {
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)] public string entityToken;
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)] public string displayName;
        public System.IntPtr xform;
        public int type;
        public float verticalFov;
        public float aspectRatio;
        public float horizontalMag;
        public float nearClip;
        public float farClip;
    }

    enum CameraProjectionType : int
    {
        Perspective,  //Perspective projection (with foreshortening)
        Orthographic,   //Orthographic projection (no foreshortening)
        Unknown   //Unknown format
    }

    class CameraHandler
    {
        public static GameObject CreateCamera(CameraData cameraData)
        {
            CameraProjectionType camType = (CameraProjectionType)cameraData.type;
            Debug.LogFormat("Camera - Token:{0}, Name:{1}, Projection Type:{2}, VerticalFov:{3}, AspectRatio:{4}, HorizontalMag:{5}, NearClip:{6}, FarClip:{7}", cameraData.entityToken, cameraData.displayName, camType, cameraData.verticalFov, cameraData.aspectRatio, cameraData.horizontalMag, cameraData.nearClip, cameraData.farClip);

            // Get transformation data
            var transform = MarshalData.GetXFormFromUnmanagedArray(cameraData.xform);

            // Create a new game object
            GameObject cameraInst = new GameObject(cameraData.displayName);

            // Set transformation
            SceneTransmissionProtocolUtilities.UpdateObjectHierarchy(cameraInst, null, transform);

            // Add camera component
            Camera camera = cameraInst.AddComponent<Camera>();

            SetCameraData(camera, cameraData);

            // Add token store
            PackageMapper.AddUniqueTokenStore(cameraInst, cameraData.entityToken);

            return cameraInst;
        }

        public static bool UpdateCamera(CameraData cameraData)
        {
            CameraProjectionType camType = (CameraProjectionType)cameraData.type;
            Debug.LogFormat("Camera Update - Token:{0}, Name:{1}, Projection Type:{2}, VerticalFov:{3}, AspectRatio:{4}, HorizontalMag:{5}, NearClip:{6}, FarClip:{7}", cameraData.entityToken, cameraData.displayName, camType, cameraData.verticalFov, cameraData.aspectRatio, cameraData.horizontalMag, cameraData.nearClip, cameraData.farClip);

            // Get transformation data
            var transform = MarshalData.GetXFormFromUnmanagedArray(cameraData.xform);

            // Get camera game object
            GameObject cameraInst = PackageMapper.GetObjectFromToken(cameraData.entityToken) as GameObject;

            if (cameraInst)
            {
                // Set transformation
                SceneTransmissionProtocolUtilities.UpdateObjectHierarchy(cameraInst, null, transform);

                // Get camera component
                Camera camera = cameraInst.GetComponent<Camera>();

                if (camera)
                {
                    SetCameraData(camera, cameraData);
                }
                else
                {
                    Debug.LogErrorFormat("Camera Update error! Camera component not found! Token:{0}");
                    return false;
                }
            }
            else
            {
                Debug.LogErrorFormat("Camera Update error! Camera game object not found! Token:{0}");
                return false;
            }

            return true;
        }

        private static void SetCameraData(Camera camera, CameraData cameraData)
        {
            // Set projection type
            if ((CameraProjectionType)cameraData.type == CameraProjectionType.Orthographic)
            {
                camera.orthographic = true;

                // Set specific orthographic data
                camera.orthographicSize = cameraData.horizontalMag;
            }
            else
            {
                camera.orthographic = false;

                // Set specific perspective data

                // TODO: Actually add vertical and horizontal FOV values to STP camera and fix server sending horizontal
                // FOV instead of vertical
                // TODO: Fix server sending degrees instead of radians for vertical fov
                
                // Convert from horizontal to vertical FOV and convert from degrees to radians
                // https://en.wikipedia.org/wiki/Field_of_view_in_video_games
                float horizontalFOV = 2.0f * Mathf.Atan(Mathf.Tan((cameraData.verticalFov * Mathf.PI / 360.0f)) / 2.0f) * (1.0f / cameraData.aspectRatio);
                // Convert from radians to degrees
                camera.fieldOfView = horizontalFOV / Mathf.PI * 360.0f;
                camera.aspect = cameraData.aspectRatio;
            }

            // Set clipping planes
            camera.nearClipPlane = cameraData.nearClip;
            camera.farClipPlane = cameraData.farClip;
        }
    }
}
