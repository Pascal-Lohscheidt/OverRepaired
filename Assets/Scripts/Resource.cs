using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Resource
{
    public ResourceType resourceType;
    public Slider resourceBar;

    // status 0 depleted - 100 intact
    protected int statusValue = 100;

    public enum ResourceType
    {
        Integrity,
        LifeSupport,
        Energy
    }

    public Resource(ResourceType selectedType, Slider selectedBar)
    {
        resourceType = selectedType;
        resourceBar = selectedBar;
    }

    public void CauseDamage(int damage)
    {
        if(statusValue > 0)
        {
            statusValue -= damage;
        }
        else
        {
            Debug.Log("Resource is destroyed: " + resourceType.ToString());
        }

        if(resourceBar != null)
        {
            resourceBar.value = statusValue;
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

        if (resourceBar != null)
        {
            resourceBar.value = statusValue;
        }
    }
}
