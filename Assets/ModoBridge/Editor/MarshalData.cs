using UnityEngine;
using UnityEditor;

namespace Foundry
{

/// <summary>
/// Utility class for marshalling data from the unmanaged plugin to this managed code.
/// </summary>
public static class MarshalData
{ 
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    private struct LightColour
    {
        public float r;
        public float g;
        public float b;
    }

    /// <summary>
    /// Delegate type for unrecognized Response objects.
    /// </summary>
    public delegate void OtherAvailableDelegate(
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)]string entity_name,
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)]string entity_parent_name,
        System.IntPtr xform
    );

    /// <summary>
    /// Delegate type for material Response objects.
    /// </summary>
    public delegate void MaterialAvailableDelegate(
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)]string entity_name,
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)]string target
    );

    /// <summary>
    /// Delegate type for directional light Response objects.
    /// </summary>
    public delegate void DirectionalLightAvailableDelegate(
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)]string entity_name,
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)]string entity_parent_name,
        System.IntPtr xform,
        System.IntPtr light_colour_ptr,
        float intensity
    );

    /// <summary>
    /// Delegate type for spotlight Response objects.
    /// </summary>
    public delegate void SpotLightAvailableDelegate(
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)]string entity_name,
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)]string entity_parent_name,
        System.IntPtr xform,
        System.IntPtr light_colour_ptr,
        float intensity,
        float range,
        float outer_cone_angle_radians
    );

    /// <summary>
    /// Delegate type for point light Response objects.
    /// </summary>
    public delegate void PointLightAvailableDelegate(
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)]string entity_name,
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)]string entity_parent_name,
        System.IntPtr xform,
        System.IntPtr light_colour_ptr,
        float intensity,
        float range
    );

    /// <summary>
    /// Delegate type for setting a float material parameter.
    /// </summary>
    public delegate void MaterialSetFloatParameterDelegate(
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)]string entity_name,
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)]string param_name,
        float value);

    /// <summary>
    /// Delegate type for setting a float array material parameter.
    /// </summary>
    public delegate void MaterialSetFloatArrayParameterDelegate(
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)]string entity_name,
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)]string param_name,
        int num_elements,
        System.IntPtr array);

    /// <summary>
    /// Delegate type for setting an integer material parameter.
    /// </summary>
    public delegate void MaterialSetIntegerParameterDelegate(
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)]string entity_name,
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)]string param_name,
        int value);

    /// <summary>
    /// Delegate type for reporting an error to the Editor.
    /// </summary>
    public delegate void ReportErrorDelegate(
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)]string message);

    public static Matrix4x4 convertStpCoordinatesToUnity;
    public static Matrix4x4 convertUnityCoordinatesToStp;
    private static Matrix4x4 convertStpCoordinatesToUnityInverse;
    private static Matrix4x4 convertUnityCoordinatesToStpInverse;
    public static int sizeOfInt;
    public static int sizeOfByte;

    public static Vector3 scaleFromSTPtoUnity;
    public static Vector3 scaleFromUnitytoSTP;

    static MarshalData()
    {
        // STP units are in centimetres, but Unity units are in metres
        scaleFromSTPtoUnity = new Vector3(0.01f, 0.01f, 0.01f);
        scaleFromUnitytoSTP = new Vector3(100f, 100f, 100f);

        // make a rotation matrix to handle converting from the STP coordinate system
        // SGP  : left-handed, +x -> right, +y -> out, +z -> up
        // Unity: left-handed, +x -> right, +y -> up,  +z -> in
        // conversion: rotate -90 degrees about X
        var rotate_about_x = Quaternion.Euler(-90, 0, 0);
        // STP units are in centimetres, but Unity units are in metres
        var scale = Matrix4x4.Scale(scaleFromSTPtoUnity);
        convertStpCoordinatesToUnity = Matrix4x4.Rotate(rotate_about_x) * scale;
        convertStpCoordinatesToUnityInverse = convertStpCoordinatesToUnity.inverse;

        // make a rotation matrix to handle converting to the STP coordinate system
        // SGP  : left-handed, +x -> right, +y -> out, +z -> up
        // Unity: left-handed, +x -> right, +y -> up,  +z -> in
        // conversion: rotate 90 degrees about X
        rotate_about_x = Quaternion.Euler(90, 0, 0);
        // SGP units are in centimetres, but Unity units are in metres
        scale = Matrix4x4.Scale(scaleFromUnitytoSTP);
        convertUnityCoordinatesToStp = Matrix4x4.Rotate(rotate_about_x) * scale;
        convertUnityCoordinatesToStpInverse = convertUnityCoordinatesToStp.inverse;

        sizeOfInt = System.Runtime.InteropServices.Marshal.SizeOf (typeof(System.Int32));
        sizeOfByte = System.Runtime.InteropServices.Marshal.SizeOf (typeof(System.Byte));
    }

    public static float GetFloatFromUnmanagedArray(
        System.IntPtr array_base,
        int offset)
    {
        var value_as_int = System.Runtime.InteropServices.Marshal.ReadInt32 (array_base, offset);
        var int_as_bytes = System.BitConverter.GetBytes (value_as_int);
        var value_as_float = System.BitConverter.ToSingle (int_as_bytes, 0);
        return value_as_float;
    }

    public static int GetIntFromUnmanagedArray(
        System.IntPtr array_base,
        int offset)
    {
        var value_as_int = System.Runtime.InteropServices.Marshal.ReadInt32 (array_base, offset);
        return value_as_int;
    }

    public static byte GetByteFromUnmanagedArray(
        System.IntPtr array_base,
        int offset)
    {
        return System.Runtime.InteropServices.Marshal.ReadByte(array_base, offset);
    }

    public static Matrix4x4 GetXFormFromUnmanagedArray(
        System.IntPtr array_base)
    {
        var matrix_as_longs = new long[16];
        System.Runtime.InteropServices.Marshal.Copy (
            array_base,
            matrix_as_longs,
            0,
            16);
        Matrix4x4 result;
        result.m00 = (float)System.BitConverter.Int64BitsToDouble (matrix_as_longs [0]);
        result.m01 = (float)System.BitConverter.Int64BitsToDouble (matrix_as_longs [1]);
        result.m02 = (float)System.BitConverter.Int64BitsToDouble (matrix_as_longs [2]);
        result.m03 = (float)System.BitConverter.Int64BitsToDouble (matrix_as_longs [3]);
        result.m10 = (float)System.BitConverter.Int64BitsToDouble (matrix_as_longs [4]);
        result.m11 = (float)System.BitConverter.Int64BitsToDouble (matrix_as_longs [5]);
        result.m12 = (float)System.BitConverter.Int64BitsToDouble (matrix_as_longs [6]);
        result.m13 = (float)System.BitConverter.Int64BitsToDouble (matrix_as_longs [7]);
        result.m20 = (float)System.BitConverter.Int64BitsToDouble (matrix_as_longs [8]);
        result.m21 = (float)System.BitConverter.Int64BitsToDouble (matrix_as_longs [9]);
        result.m22 = (float)System.BitConverter.Int64BitsToDouble (matrix_as_longs [10]);
        result.m23 = (float)System.BitConverter.Int64BitsToDouble (matrix_as_longs [11]);
        result.m30 = (float)System.BitConverter.Int64BitsToDouble (matrix_as_longs [12]);
        result.m31 = (float)System.BitConverter.Int64BitsToDouble (matrix_as_longs [13]);
        result.m32 = (float)System.BitConverter.Int64BitsToDouble (matrix_as_longs [14]);
        result.m33 = (float)System.BitConverter.Int64BitsToDouble (matrix_as_longs [15]);
        result = result.transpose; // looks like Matrix4x4 are column-major
        if (!result.ValidTRS ())
        {
            throw new UnityException ("Transform is not a valid TRS");
        }
        // convert SGP transforms to Unity coordinate space
        result = convertStpCoordinatesToUnity * result * convertStpCoordinatesToUnityInverse;
        return result;
    }

    public static System.IntPtr GetXFormToUnmanagedArray(
        Matrix4x4 transform)
    {
        // convert SGP transforms to Unity coordinate space
        Matrix4x4 transformConvert = convertUnityCoordinatesToStp * transform * convertUnityCoordinatesToStpInverse;

        if (!transformConvert.ValidTRS ())
        {
            throw new UnityException ("Transform is not a valid TRS");
        }

        transformConvert = transformConvert.transpose;

        long[] matrixAsLongs = new long[16];
        matrixAsLongs[0] = System.BitConverter.DoubleToInt64Bits(transformConvert.m00);
        matrixAsLongs[1] = System.BitConverter.DoubleToInt64Bits(transformConvert.m01);
        matrixAsLongs[2] = System.BitConverter.DoubleToInt64Bits(transformConvert.m02);
        matrixAsLongs[3] = System.BitConverter.DoubleToInt64Bits(transformConvert.m03);
        matrixAsLongs[4] = System.BitConverter.DoubleToInt64Bits(transformConvert.m10);
        matrixAsLongs[5] = System.BitConverter.DoubleToInt64Bits(transformConvert.m11);
        matrixAsLongs[6] = System.BitConverter.DoubleToInt64Bits(transformConvert.m12);
        matrixAsLongs[7] = System.BitConverter.DoubleToInt64Bits(transformConvert.m13);
        matrixAsLongs[8] = System.BitConverter.DoubleToInt64Bits(transformConvert.m20);
        matrixAsLongs[9] = System.BitConverter.DoubleToInt64Bits(transformConvert.m21);
        matrixAsLongs[10] = System.BitConverter.DoubleToInt64Bits(transformConvert.m22);
        matrixAsLongs[11] = System.BitConverter.DoubleToInt64Bits(transformConvert.m23);
        matrixAsLongs[12] = System.BitConverter.DoubleToInt64Bits(transformConvert.m30);
        matrixAsLongs[13] = System.BitConverter.DoubleToInt64Bits(transformConvert.m31);
        matrixAsLongs[14] = System.BitConverter.DoubleToInt64Bits(transformConvert.m32);
        matrixAsLongs[15] = System.BitConverter.DoubleToInt64Bits(transformConvert.m33);

        var size = System.Runtime.InteropServices.Marshal.SizeOf(typeof(long));
        var result = System.Runtime.InteropServices.Marshal.AllocHGlobal(size * 16);

        System.Runtime.InteropServices.Marshal.Copy (
            matrixAsLongs,
            0,
            result,
            16);
        
        return result;
    }

    /// <summary>
    /// Delegate invoked when an unrecognised object is being handled.
    /// A game object is created, but only with a transform component.
    /// </summary>
    /// <param name="entity_name">Entity name.</param>
    /// <param name="entity_parent_name">Entity parent name.</param>
    /// <param name="xform">Xform.</param>
    public static void OtherAvailable(
        string entity_name,
        string entity_parent_name,
        System.IntPtr xform)
    {
        try
        {
            //Debug.Log (System.String.Format ("Other: {0} {1}", entity_name, entity_parent_name));

            var transform = GetXFormFromUnmanagedArray (xform);

            SceneTransmissionProtocolUtilities.CreateEmptyGameObject(
                entity_name,
                entity_parent_name,
                transform
            );
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }
    }

    private static Color GetLightColour(System.IntPtr light_colour_ptr)
    {
        var colour = (LightColour)System.Runtime.InteropServices.Marshal.PtrToStructure(light_colour_ptr, typeof(LightColour));
        return new Color(colour.r, colour.g, colour.b);
    }

    /// <summary>
    /// Creates a game object for a new directional light.
    /// </summary>
    /// <param name="entity_name">Entity name.</param>
    /// <param name="entity_parent_name">Entity parent name.</param>
    /// <param name="xform">Xform.</param>
    /// <param name="light_colour_ptr">Light colour ptr.</param>
    /// <param name="intensity">Intensity.</param>
    public static void DirectionalLightAvailable(
        string entity_name,
        string entity_parent_name,
        System.IntPtr xform,
        System.IntPtr light_colour_ptr,
        float intensity)
    {
        try
        {
            //Debug.Log (System.String.Format ("DirLight: {0} {1}", entity_name, entity_parent_name));

            var transform = GetXFormFromUnmanagedArray (xform);

            var colour = GetLightColour(light_colour_ptr);

            SceneTransmissionProtocolUtilities.CreateDirectionalLight(
                entity_name,
                entity_parent_name,
                transform,
                colour,
                intensity
            );
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }
    }

    /// <summary>
    /// Creates a game object for a new spot light.
    /// </summary>
    /// <param name="entity_name">Entity name.</param>
    /// <param name="entity_parent_name">Entity parent name.</param>
    /// <param name="xform">Xform.</param>
    /// <param name="light_colour_ptr">Light colour ptr.</param>
    /// <param name="intensity">Intensity.</param>
    /// <param name="range">Range.</param>
    /// <param name="outer_cone_angle_radians">Outer cone angle radians.</param>
    public static void SpotLightAvailable(
        string entity_name,
        string entity_parent_name,
        System.IntPtr xform,
        System.IntPtr light_colour_ptr,
        float intensity,
        float range,
        float outer_cone_angle_radians)
    {
        try
        {
            //Debug.Log (System.String.Format ("SpotLight: {0} {1}", entity_name, entity_parent_name));

            var transform = GetXFormFromUnmanagedArray (xform);

            var colour = GetLightColour(light_colour_ptr);

            SceneTransmissionProtocolUtilities.CreateSpotLight(
                entity_name,
                entity_parent_name,
                transform,
                colour,
                intensity,
                range,
                outer_cone_angle_radians
            );
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }
    }

    /// <summary>
    /// Creates a game object for a new point light.
    /// </summary>
    /// <param name="entity_name">Entity name.</param>
    /// <param name="entity_parent_name">Entity parent name.</param>
    /// <param name="xform">Xform.</param>
    /// <param name="light_colour_ptr">Light colour ptr.</param>
    /// <param name="intensity">Intensity.</param>
    /// <param name="range">Range.</param>
    public static void PointLightAvailable(
        string entity_name,
        string entity_parent_name,
        System.IntPtr xform,
        System.IntPtr light_colour_ptr,
        float intensity,
        float range)
    {
        try
        {
            //Debug.Log (System.String.Format ("PointLight: {0} {1}", entity_name, entity_parent_name));

            var transform = GetXFormFromUnmanagedArray (xform);

            var colour = GetLightColour(light_colour_ptr);

            SceneTransmissionProtocolUtilities.CreatePointLight(
                entity_name,
                entity_parent_name,
                transform,
                colour,
                intensity,
                range
            );
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }
    }

    /// <summary>
    /// Creates a new Material (not a game object).
    /// </summary>
    /// <param name="entity_name">Entity name.</param>
    /// <param name="target">Target.</param>
    public static void MaterialAvailable(
        string entity_name,
        string target)
    {
        try
        {
            //Debug.Log (System.String.Format ("Material: {0} {1}", entity_name, target));

            SceneTransmissionProtocolUtilities.CreateMaterial(
                entity_name,
                target
            );
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }
    }

    /// <summary>
    /// Assigns a float value to an existing material's named parameter.
    /// </summary>
    /// <param name="entity_name">Entity name.</param>
    /// <param name="param_name">Parameter name.</param>
    /// <param name="value">Value.</param>
    public static void MaterialSetFloat(
        string entity_name,
        string param_name,
        float value)
    {
        try
        {
            switch (param_name)
            {
            case "Opacity":
                SceneTransmissionProtocolUtilities.MaterialSetAlbedoOpacity(entity_name, value);
                break;

            case "Roughness":
                SceneTransmissionProtocolUtilities.MaterialSetGlossiness(entity_name, 1 - value);
                break;

            case "Metalic":
                SceneTransmissionProtocolUtilities.MaterialSetMetallic(entity_name, value);
                break;

            case "IndexOfRefraction":
            case "Specular":
            case "SpecularRoughness":
            case "ClearCoat":
            case "ClearCoatRoughness":
                Debug.LogFormat("No obvious setting for float Material parameter '{0}'", param_name);
                break;

            default:
                Debug.LogFormat("Unknown float Material parameter '{0}'", param_name);
                break;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }
    }

    private static Color GetColorFromArray(
        int num_elements,
        System.IntPtr array)
    {
        if (3 == num_elements || 4 == num_elements)
        {
            var rgba_offset = 0;
            var r = GetFloatFromUnmanagedArray (array, rgba_offset);
            rgba_offset += sizeOfInt;
            var g = GetFloatFromUnmanagedArray (array, rgba_offset);
            rgba_offset += sizeOfInt;
            var b = GetFloatFromUnmanagedArray (array, rgba_offset);
            rgba_offset += sizeOfInt;
            float a = 1;
            if (4 == num_elements)
            {
                a = GetFloatFromUnmanagedArray (array, rgba_offset);
                rgba_offset += sizeOfInt;
            }
            return new Color(r, g, b, a);
        }
        else
        {
            throw new System.Exception(System.String.Format("Don't how know to interpret a colour from {0} floats", num_elements));
        }
    }

    /// <summary>
    /// Assigns a float array to an existing material's named parameter.
    /// </summary>
    /// <param name="entity_name">Entity name.</param>
    /// <param name="param_name">Parameter name.</param>
    /// <param name="num_elements">Number elements.</param>
    /// <param name="array">Array.</param>
    public static void MaterialSetFloatArray(
        string entity_name,
        string param_name,
        int num_elements,
        System.IntPtr array)
    {
        try
        {
            switch (param_name)
            {
            case "BaseColor":
                {
                    var color = GetColorFromArray(num_elements, array);
                    SceneTransmissionProtocolUtilities.MaterialSetAlbedo(entity_name, color);
                }
                break;

            case "EmissiveColor":
                {
                    var color = GetColorFromArray(num_elements, array);
                    SceneTransmissionProtocolUtilities.MaterialSetEmissive(entity_name, color);
                }
                break;

            default:
                Debug.LogFormat("Unknown float array Material parameter '{0}'", param_name);
                break;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }
    }

    /// <summary>
    /// Assigns an integer to an existing Material's named parameter.
    /// </summary>
    /// <param name="entity_name">Entity name.</param>
    /// <param name="param_name">Parameter name.</param>
    /// <param name="value">Value.</param>
    public static void MaterialSetInt(
        string entity_name,
        string param_name,
        int value)
    {
        Debug.LogFormat("Unknown integer Material parameter '{0}'", param_name);
    }

    public static System.IntPtr AddToIntPtr(System.IntPtr ptr, int offset)
    {
        // cannot use System.IntPtr.Add as that was introduced in .NET Framework 4
        // Unity only comes in 64-bit flavours, so this should be fine
        return new System.IntPtr(ptr.ToInt64() + offset);
    }

    /// <summary>
    /// Display an Editor dialog for the error message.
    /// </summary>
    /// <param name="message">Message.</param>
    public static void ShowEditorErrorDialog(
        string message)
    {
        EditorUtility.DisplayDialog("Foundry remote communication error", message, "OK");
    }
}

} // namespace Foundry

