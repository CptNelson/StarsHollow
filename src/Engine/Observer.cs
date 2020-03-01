using System;

namespace StarsHollow.Engine
{
    public abstract class Observer
    {
        // some helper fields
        protected object sender;
        protected EventArgs args;

        Subject _subject;
        // Every time the event is raised
        // (from eventHandler(this,EventArgs.Empty);), DoSomething(...) is called


        public void Subscribe(Subject subject)
        {
            subject.eventHandler += DoSomething;
        }
        // Now, when the event is raised,
        // DoSomething(...) is no longer called
        public void UnSubscribe(Subject subject)
        {
            subject.eventHandler -= DoSomething;
        }

        public virtual void DoSomething(object sender, EventArgs e)
        {
            Console.WriteLine("Observer " + sender + " e: " + e);
        }

    }
}