using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameEventManager : Singleton<InGameEventManager>
{
    [SerializeField] private int maxNumberOfEvents = 3;

    [SerializeField] private float timeBetweenEvents = 11f;
    [SerializeField] private float minTimeAfterFix = 10f;
    float timeUntilNextEvent = 3f;

    List<BreakableObject> breakableObjects = new List<BreakableObject>();


    public void Register(BreakableObject breakableObject)
    {
        breakableObjects.Add(breakableObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeUntilNextEvent -= Time.deltaTime;

        if (timeUntilNextEvent < 0 && GetNumberOfEvents() < maxNumberOfEvents && GetNumberOfWorkingObjects() > 0)
            CreateRandomEvent();
    }

    int GetNumberOfEvents()
    {
        int numberOfEvents = 0;

        foreach(var breakableObject in breakableObjects)
        {
            if (breakableObject.IsBroken()) numberOfEvents++;
        }

        return numberOfEvents;
    }

    int GetNumberOfWorkingObjects()
    {
        int numberOfObjects = 0;

        foreach(var breakableObject in breakableObjects)
        {
            if (!breakableObject.IsBroken()) numberOfObjects++;
        }

        return numberOfObjects;
    }

    void CreateRandomEvent()
    {
        //Debug.Log("CreateRandomEvent()");
        if(GetNumberOfWorkingObjects() > 0)
        {
            int i = Random.Range(0, breakableObjects.Count);
            // get random item from the list
            int counter = 0;
            while(Time.unscaledTime - breakableObjects[i].lastTimeFixed < minTimeAfterFix && counter < 3)
            {
                i = Random.Range(0, breakableObjects.Count);
                counter++;

            }

            var breakableObject = breakableObjects[i];

            timeUntilNextEvent = timeBetweenEvents;

            if(!breakableObject.IsBroken())
            {
                breakableObject.BreakObject();
            }
        }
    }

    public List<BreakableObject> GetBreakableObjects()
    {
        return breakableObjects;
    }
}
