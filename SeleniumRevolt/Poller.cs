using System;
using OpenQA.Selenium;

namespace SeleniumRevolt
{
    public class Poller
    {
        private int _interval;
        /// <summary>
        /// Time span in milliseconds to wait until the next attempt at an action.
        /// </summary>
        public int Interval
        {
            get { return _interval; }
            set
            {
                if (value < 0) value = 0;
                _interval = value;
            }
        }
        private int _attempts;
        /// <summary>
        /// Number of attempts to try to an action before throwing an error/exception.
        /// </summary>
        public int Attempts
        {
            get { return _attempts; }
            set
            {
                if (value < 0) value = 0;
                _attempts = value;
            }
        }
        private int _refreshAtt;
        /// <summary>
        /// Number of attempts at an action before the browser refreshes.
        /// </summary>
        public int RefreshAttempts
        {
            get { return _refreshAtt; }
            set
            {
                if (value < 0) value = 0;
                else if (value > Attempts) value = Attempts;
                _refreshAtt = value;
            }
        }
        /// <summary>
        /// Creates a Polling object to set polling options on anew web page or selection function.
        /// </summary>
        /// <param name="interval">Time span in milliseconds to wait until the next attempt at an action.  Default 1000 or 1 second</param>
        /// <param name="attempts">Number of attempts to try to an action before throwing an error/exception.  Default 3 attempts</param>
        /// <param name="refreshAttempts">Number of attempts at an action before the browser refreshes.</param>
        public Poller(int interval = 1000, int attempts = 5,int refreshAttempt =3)
        {
            Interval = interval;
            Attempts = attempts;
            RefreshAttempts = refreshAttempt;
        }
    }
}
