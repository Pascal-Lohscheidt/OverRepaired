using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource
{
    public ResourceType resourceType;

    // status 0 depleted - 100 intact
    protected int statusValue = 100;

    public enum ResourceType
    {
        Integrity,
        LifeSupport,
        Energy
    }

    public Resource(ResourceType selectedType)
    {
        resourceType = selectedType;
    }

    public void CauseDamage(int damage)
    {
        if(statusValue > 0)
        {
            statusValue -= damage;
            Debug.Log("Resource " + resourceType.ToString() + " damaged! Status: " + statusValue);
        }
        else
        {
            Debug.Log("Resource is destroyed: " + resourceType.ToString());
        }
    }

    public void RecoverDamage(int amount)
    {
        if(statusValue + amount > 100)
        {
            statusValue = 100;
        }
        else
        {
            statusValue += amount;
        }

    }
}
