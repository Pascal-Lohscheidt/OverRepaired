using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    private int damagePerBrokenObject = 2;
    private int recoveryDamageAmount = 1;
    private float damageCheckInterval = 2f;
    float nextCheckTime = 2f;

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
        if (Time.time >= nextCheckTime)
        {
            CheckResourcesDamage();
            nextCheckTime += damageCheckInterval;
        }
    }

    private void CheckResourcesDamage() {
        int totalEnergyBrokenObjects = 0;
        int totalIntegrityBrokenObjects = 0;
        int totalLifeSupportBrokenObjects = 0;

        var breakableObjects = InGameEventManager.Instance.GetBreakableObjects();

        foreach (var breakableObject in breakableObjects)
        {
            if (breakableObject.IsBroken())
            {
                switch (breakableObject.resourceType)
                {
                    case Resource.ResourceType.Energy:
                        totalEnergyBrokenObjects++;
                        break;
                    case Resource.ResourceType.Integrity:
                        totalIntegrityBrokenObjects++;
                        break;
                    case Resource.ResourceType.LifeSupport:
                        totalLifeSupportBrokenObjects++;
                        break;
                }
            }
        }

        if(totalEnergyBrokenObjects > 0)
        {
            energyResource.CauseDamage(damagePerBrokenObject * totalEnergyBrokenObjects);
        }
        else
        {
            energyResource.RecoverDamage(recoveryDamageAmount);
        }

        if (totalIntegrityBrokenObjects > 0)
        {
            integrityResource.CauseDamage(damagePerBrokenObject * totalIntegrityBrokenObjects);
        }
        else
        {
            integrityResource.RecoverDamage(recoveryDamageAmount);
        }

        if (totalLifeSupportBrokenObjects > 0)
        {
            lifeSupportResource.CauseDamage(damagePerBrokenObject * totalLifeSupportBrokenObjects);
        }
        else
        {
            lifeSupportResource.RecoverDamage(recoveryDamageAmount);
        }

    }
}
