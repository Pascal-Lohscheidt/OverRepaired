using UnityEngine;
using UnityEditor;

namespace Foundry
{

/// <summary>
/// Utility class for specific options for the Scene Transmission Protocol.
/// </summary>
public static class SceneTransmissionProtocolUtilities
{ 
    /// <summary>
    /// Create a brand new scene, with default game objects (camera + directional light).
    /// </summary>
    public static void
    CreateNewScene()
    {
        var sceneSetup = UnityEditor.SceneManagement.NewSceneSetup.DefaultGameObjects;
        /*var scene = */UnityEditor.SceneManagement.EditorSceneManager.NewScene (sceneSetup);

        /*
         * experiment if an empty scene is created
         * strangely, objects were not being lit by this light
        var lightObj = new GameObject ("Directional light");
        var lightComp = lightObj.AddComponent<Light> ();
        lightComp.color = new Color(0.9f, 0.9f, 0.9f);
        lightComp.type = LightType.Directional;
        var lightXform = lightObj.GetComponent<Transform> ();
        lightXform.position = new Vector3 (0, 3, 0);
        lightXform.rotation = Quaternion.Euler (50, -30, 0);

        var cameraObj = new GameObject ("Default camera");
        var something = cameraObj.AddComponent<Camera> ();
        var cameraXform = cameraObj.GetComponent<Transform> ();
        cameraXform.position = new Vector3 (cameraXform.position.x, cameraXform.position.y, -10); // move the camera back enough to see something
        */
    }

    public static void UpdateObjectHierarchy(
        GameObject gameObj,
        GameObject parent,
        Matrix4x4 transform)
    {
        //Debug.Log (System.String.Format ("* Processing {0}, with parent {1}", name, parent_name));

        // going to need to decompose
        // see https://answers.unity.com/questions/402280/how-to-decompose-a-trs-matrix.html
        var translation = transform.GetColumn(3);
        var rotationQ = Quaternion.LookRotation (
            transform.GetColumn (2),
            transform.GetColumn (1)
        );
        var scale = new Vector3 (
            transform.GetColumn (0).magnitude,
            transform.GetColumn (1).magnitude,
            transform.GetColumn (2).magnitude
        );

        // now parent it, so the hierarchy looks right
        var xformComp = gameObj.GetComponent<Transform>();
        xformComp.parent = null;

        // set the world transform as local
        // parenting will convert these values to true local transformations
        xformComp.localPosition = translation;
        xformComp.localRotation = rotationQ;
        xformComp.localScale = scale;

        if (null != parent)
        {
            xformComp.SetParent (parent.GetComponent<Transform>(), true);
        }
    }

    public static GameObject CreateObjectIntoHierarchy(
        string name,
        string parent_name,
        Matrix4x4 transform)
    {
        //Debug.Log (System.String.Format ("* Processing {0}, with parent {1}", name, parent_name));
        var new_game_object = new GameObject (name);
        GameObject parent_object = null;

        if (!System.String.IsNullOrEmpty(parent_name))
        {
            // TODO: this needs to be fixed, as the brute force approach is O(n), so will not be performant
            // for large scene graphs
            // this is not working, I find
            // lots of references on the web to NOT use GameObject.Find as it's not fast either
            parent_object = GameObject.Find(parent_name);
            if (null == parent_object)
            {
                // brute force search
                GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
                foreach (object go in allObjects)
                {
                    if ((go as GameObject).name == parent_name)
                    {
                        parent_object = go as GameObject;
                        break;
                    }
                }
            }
        }

        UpdateObjectHierarchy(new_game_object, parent_object, transform);

        return new_game_object;
    }

    /// <summary>
    /// Creates a new directional light GameObject.
    /// Contains a Light component.
    /// </summary>
    /// <param name="name">Name.</param>
    /// <param name="parent_name">Parent name.</param>
    /// <param name="transform">Transform.</param>
    /// <param name="colour">Colour.</param>
    /// <param name="intensity">Intensity.</param>
    public static void
    CreateDirectionalLight(
        string name,
        string parent_name,
        Matrix4x4 transform,
        Color colour,
        float intensity)
    {
        var new_light_object = CreateObjectIntoHierarchy(name, parent_name, transform);

        var light_comp = new_light_object.AddComponent<Light>();
        light_comp.type = LightType.Directional;
        light_comp.color = colour;
        light_comp.intensity = intensity;
    }

    /// <summary>
    /// Create a new spot light GameObject.
    /// Contains a Light component.
    /// </summary>
    /// <param name="name">Name.</param>
    /// <param name="parent_name">Parent name.</param>
    /// <param name="transform">Transform.</param>
    /// <param name="colour">Colour.</param>
    /// <param name="intensity">Intensity.</param>
    /// <param name="range">Range.</param>
    /// <param name="outer_cone_angle_radians">Outer cone angle radians.</param>
    public static void
    CreateSpotLight(
        string name,
        string parent_name,
        Matrix4x4 transform,
        Color colour,
        float intensity,
        float range,
        float outer_cone_angle_radians)
    {
        var new_light_object = CreateObjectIntoHierarchy(name, parent_name, transform);

        var light_comp = new_light_object.AddComponent<Light>();
        light_comp.type = LightType.Spot;
        light_comp.color = colour;
        light_comp.intensity = intensity;
        light_comp.range = range;
        light_comp.spotAngle = outer_cone_angle_radians * Mathf.Rad2Deg;
    }

    /// <summary>
    /// Creates a new point light GameObject.
    /// Contains a Light component.
    /// </summary>
    /// <param name="name">Name.</param>
    /// <param name="parent_name">Parent name.</param>
    /// <param name="transform">Transform.</param>
    /// <param name="colour">Colour.</param>
    /// <param name="intensity">Intensity.</param>
    /// <param name="range">Range.</param>
    public static void
    CreatePointLight(
        string name,
        string parent_name,
        Matrix4x4 transform,
        Color colour,
        float intensity,
        float range)
    {
        var new_light_object = CreateObjectIntoHierarchy(name, parent_name, transform);

        var light_comp = new_light_object.AddComponent<Light>();
        light_comp.type = LightType.Point;
        light_comp.color = colour;
        light_comp.intensity = intensity;
        light_comp.range = range;
    }

    /// <summary>
    /// Creates an empty GameObject.
    /// It will only have a Transform component.
    /// </summary>
    /// <param name="name">Name.</param>
    /// <param name="parent_name">Parent name.</param>
    /// <param name="transform">Transform.</param>
    public static void
    CreateEmptyGameObject(
        string name,
        string parent_name,
        Matrix4x4 transform)
    {
        CreateObjectIntoHierarchy(name, parent_name, transform);
    }

    /// <summary>
    /// Creates a Material. This is not a GameObject.
    /// </summary>
    /// <param name="name">Name.</param>
    /// <param name="target">Target.</param>
    public static void
    CreateMaterial(
        string name,
        string target)
    {
        if (PackageMapper.materialCache.ContainsKey(name))
        {
            throw new System.Exception(System.String.Format("Material '{0}' already exists", name));
        }
        Material material = null;
        switch (target)
        {
        case "M_SceneProtocolDefault":
            material = Material.Instantiate(DefaultAssets.DiffuseMaterial);
            break;

        case "M_SceneProtocolMetal":
            material = Material.Instantiate(DefaultAssets.MetalMaterial);
            break;

        case "M_SceneProtocolPlastic":
            material = Material.Instantiate(DefaultAssets.PlasticMaterial);
            break;

        case "M_SceneProtocolGlass":
            material = Material.Instantiate(DefaultAssets.GlassMaterial);
            break;

        default:
            Debug.Log(System.String.Format("Unknown shader, '{0}', for material '{1}'. Using default", target, name));
            material = Material.Instantiate(DefaultAssets.DiffuseMaterial);
            break;
        }
        material.name = name;
        PackageMapper.materialCache.Add(name, material);
    }

    /// <summary>
    /// Set the glossiness Material parameter.
    /// </summary>
    /// <param name="entity_name">Entity name.</param>
    /// <param name="glossiness">Glossiness.</param>
    public static void MaterialSetGlossiness(
        string entity_name,
        float glossiness)
    {
        Material material;
        if (!PackageMapper.materialCache.TryGetValue(entity_name, out material))
        {
            throw new System.Exception(System.String.Format("Material {0} has not yet been created", entity_name));
        }
        material.SetFloat("_Glossiness", glossiness);
    }

    /// <summary>
    /// Set the metallic Material parameter.
    /// </summary>
    /// <param name="entity_name">Entity name.</param>
    /// <param name="metallic">Metallic.</param>
    public static void MaterialSetMetallic(
        string entity_name,
        float metallic)
    {
        Material material;
        if (!PackageMapper.materialCache.TryGetValue(entity_name, out material))
        {
            throw new System.Exception(System.String.Format("Material {0} has not yet been created", entity_name));
        }
        material.SetFloat("_Metallic", metallic);
    }

    /// <summary>
    /// Set the emissive colour on the Material.
    /// </summary>
    /// <param name="entity_name">Entity name.</param>
    /// <param name="color">Color.</param>
    public static void MaterialSetEmissive(
        string entity_name,
        Color color)
    {
        Material material;
        if (!PackageMapper.materialCache.TryGetValue(entity_name, out material))
        {
            throw new System.Exception(System.String.Format("Material {0} has not yet been created", entity_name));
        }
        material.SetColor("_EmissionColor", color);
    }

    /// <summary>
    /// Set the Material's albedo colour.
    /// </summary>
    /// <param name="entity_name">Entity name.</param>
    /// <param name="color">Color.</param>
    public static void MaterialSetAlbedo(
        string entity_name,
        Color color)
    {
        Material material;
        if (!PackageMapper.materialCache.TryGetValue(entity_name, out material))
        {
            throw new System.Exception(System.String.Format("Material {0} has not yet been created", entity_name));
        }
        material.color = color;
    }

    /// <summary>
    /// Set the Material's albedo opacity.
    /// Note that this must be called *after* MaterialSetAlbedo, or it's value will be replaced.
    /// </summary>
    /// <param name="entity_name">Entity name.</param>
    /// <param name="opacity">Opacity.</param>
    public static void MaterialSetAlbedoOpacity(
        string entity_name,
        float opacity)
    {
        Material material;
        if (!PackageMapper.materialCache.TryGetValue(entity_name, out material))
        {
            throw new System.Exception(System.String.Format("Material {0} has not yet been created", entity_name));
        }
        // note that order matters here
        // it is assumed that the albeda colour has been set FIRST, because the BaseColor parameter
        // comes first in the parameter list
        // otherwise, this value is overwritten in MaterialSetAlbedo
        material.color = new Color(material.color.r, material.color.g, material.color.b, opacity);
    }
}

} // namespace Foundry
