using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WorldTicker : MonoBehaviour
{
    public EventHandler timedEvents;
    void Start()
    {

    }


    void Update()
    {
        timedEvents(this, EventArgs.Empty);
    }

    public void registerTimedEvent(EventHandler timedEvents)
    {
        this.timedEvents += timedEvents;
    }

    public void unregisterTimedEvent(EventHandler timedEvents)
    {
        this.timedEvents -= timedEvents;
    }

}

