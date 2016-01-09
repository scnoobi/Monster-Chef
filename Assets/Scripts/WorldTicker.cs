using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WorldTicker : MonoBehaviour
{
    public EventHandler<TimedEventArgs> timedEvents;
    public float tickSpeed = 1f;

    private float lastTick = 0f;
    private float time = 0f;

    void Start()
    {

    }

    void Update()
    {
        time += Time.deltaTime;
        if (time - lastTick > tickSpeed)
        {
            if (timedEvents != null)
                timedEvents(this, new TimedEventArgs(time));
            lastTick = time;
        }
    }

    public void registerTimedEvent(EventHandler<TimedEventArgs> timedEvents)
    {
        this.timedEvents += timedEvents;
    }

    public void unregisterTimedEvent(EventHandler<TimedEventArgs> timedEvents)
    {
        this.timedEvents -= timedEvents;
    }

}

