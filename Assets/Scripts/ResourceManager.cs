using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    Resource energyResource = new Resource(Resource.ResourceType.Energy);
    Resource integrityResource = new Resource(Resource.ResourceType.Integrity);
    Resource lifeSupportResource = new Resource(Resource.ResourceType.LifeSupport);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
