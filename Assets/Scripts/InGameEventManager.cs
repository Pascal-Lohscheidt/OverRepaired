using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameEventManager : Singleton<InGameEventManager>
{
    [SerializeField] private int maxNumberOfEvents = 3;

    [SerializeField] private float timeBetweenEvents = 3f;
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
            // get random item from the list
            int i = Random.Range(0, breakableObjects.Count);
            var breakableObject = breakableObjects[i];

            timeUntilNextEvent = timeBetweenEvents;

            if(!breakableObject.IsBroken())
            {
                breakableObject.BreakObject();
            }
            //else
            //{
            //    //Debug.Log("Already broken!");

            //    // previous object was already broken, find another one
            //    CreateRandomEvent();
            //}
        }
    }

    public List<BreakableObject> GetBreakableObjects()
    {
        return breakableObjects;
    }
}
