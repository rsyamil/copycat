using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Diagnostics;

namespace KinectGame
{
    class Timer
    {
        //private double elapsedTime;
        private Boolean eventInvoke;
        private System.Timers.Timer aTimer;

        public Timer(double interval)
        {
            eventInvoke = false;
            aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = interval;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            eventInvoke = true;
        }

        public void Start()
        {
            aTimer.Start();
        }

        public void Stop()
        {
            aTimer.Stop();
        }

        public Boolean CheckEventInvoke()
        {
            
            if (eventInvoke == true)
            {
                eventInvoke = false;
                return true;
            }
            else
                return false;
        }
    }
}
