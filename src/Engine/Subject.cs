using System;

namespace StarsHollow.Engine
{
    public class Subject
    {
        public event EventHandler eventHandler; //We can also consider
        //using an auto-implemented property instead of a public field

        public void NotifyObservers()
        {
            if (eventHandler != null)   //Ensures that if there are no handlers,
                                        //the event won't be raised
            {
                eventHandler(this, EventArgs.Empty);    //We can also replace
                //EventArgs.Empty with our own message
            }
        }
    }
}