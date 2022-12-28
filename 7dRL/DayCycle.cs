
using Engine.Core;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.WpfInterop;
using System;
using System.Runtime.InteropServices;

namespace _7dRL;

public static class DayCycle
{
    //public static float Time { get; set; }
    //public static float HoursPerSeconds { get; set; }
    public static float MinutesPerRealSecond { get; set; }

    private static float timer = 0f;

    public static TimeOnly Time { get; set; }
    
    public static void Update(float dt)
    {
        timer += dt;
        if (timer > 1f)
        {
            Time = Time.AddMinutes(MinutesPerRealSecond);
            timer = 0f;
        }        
    }
    public static int GetSightRangeBasedOnTime(int min, int max)
    {


        //todo: rethink this        
        //if (Time <= 12) 
        //    return (int)(Time * 2f) + min;        
        //else 
        //    return (int)(24f - ((Time - 12f) * 2f)) + min;

        //midnight we just want min
        //noon we want max

        //distance from midnight

        //light at 3pm(15) should be same as 9am
        //light at 6pm(18) should be same as 6am
        //light at 9pm(21) should be same as 3am
        //light at 12am(24) should be same as 12am

        //get distance to or from midnight
        //9am = 9
        //3pm = 9
        //6pm = 6
        //9pm = 3

        //calulcate absolute distance from midnight
        //if (over 12) subtract from 24
        //if (under 12) subtract from 0
        //3pm ... 24 - 15 = 9

        
        var noon = new TimeOnly(12, 0);
        var midnight = new TimeOnly(0, 0);

        if (Time == midnight)
            return min;
        
        if (Time > noon)
        {
            var distance = Time - midnight;
            var distanceFromMidnight = (int)distance.TotalHours;
            var distanceFromNoon = 24 - distanceFromMidnight;
            var distanceFromNoonAsPercent = (float)distanceFromNoon / 12f;
            var sightRange = (int)(distanceFromNoonAsPercent * max);

            if (sightRange < min)
                sightRange = min;

            return sightRange;
        }
        else
        {
            var distance = midnight - Time;
            var distanceFromMidnight = (int)distance.TotalHours;
            var distanceFromNoon = 24 - distanceFromMidnight;
            var distanceFromNoonAsPercent = (float)distanceFromNoon / 12f;
            var sightRange = (int)(distanceFromNoonAsPercent * max);
            
            if (sightRange < min)
                sightRange = min;
            
            return sightRange;
        }
    }

}


