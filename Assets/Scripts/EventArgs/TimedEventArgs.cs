using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class TimedEventArgs : EventArgs
{
    private readonly float time;

    public TimedEventArgs(float time)
    {
        this.time = time;
    }

    public float Time
    {
        get { return time; }
    }
}

