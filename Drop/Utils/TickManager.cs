using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
namespace Drop.Utils
{
    //a simple tick system for GUI update
    //coming from a game engine background it seemed obvius to have one
    //i might replace this with something in the future
    public static class TickManager
    {
        static Timer tick;
        //60/sec
        static float interval = 0.16f;

        public delegate void OnTickDelegate();
        public static event OnTickDelegate OnTickEvent;
        public static void InitMe()
        {
            tick = new Timer();
            tick.Interval = interval;
            tick.AutoReset = true;
            tick.Elapsed += OnTick;
            tick.Start();
        }

        private static void OnTick(object sender, ElapsedEventArgs e)
        {
            if (OnTickEvent!=null)
                OnTickEvent.Invoke();
        }
    }
}
