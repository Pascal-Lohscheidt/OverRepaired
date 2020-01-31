using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameEventManager : Singleton<InGameEventManager>
{
    static int maxNumberOfEvents = 3;

    static float timeBetweenEvents = 30f;
    float timeUntilNextEvent = 0f;

    List<BreakableObject> breakableObjects = new List<BreakableObject>();


    public void Register(BreakableObject breakableObject){
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

        if(timeUntilNextEvent < 0 && GetNumberOfEvents() < maxNumberOfEvents)
        {
            CreateRandomEvent();
        }
    }

    int GetNumberOfEvents()
    {
        int numberOfEvents = 0;
        foreach(var breakableObject in breakableObjects)
        {
            if(breakableObject.IsBroken())
            {
                numberOfEvents++;
            }
        }
        return numberOfEvents;
    }

    int GetNumberOfWorkingObjects()
    {
        int numberOfObjects = 0;
        foreach(var breakableObject in breakableObjects)
        {
            if(!breakableObject.IsBroken())
            {
                numberOfObjects++;
            }
        }
        return numberOfObjects;
    }

    void CreateRandomEvent()
    {
        if(GetNumberOfWorkingObjects() > 0){

            // get random item from the list
            int i = Random.Range(0, breakableObjects.Count);
            var breakableObject = breakableObjects[i];

            if(breakableObject.IsBroken()){
                breakableObject.Break();
            }
            else
            {
                // previous object was already broken, find another one
                CreateRandomEvent();
            }

        }
    }
}
