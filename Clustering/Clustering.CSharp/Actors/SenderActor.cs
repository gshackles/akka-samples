using System;
using Akka.Actor;

namespace Clustering.CSharp.Actors
{
    public class SenderActor : ReceiveActor
    {
        private ICancelable _messagePump;

        public SenderActor()
        {
            var receiver = Context.ActorSelection("/user/receivers");

            Receive<SendMessage>(_ =>
                receiver.Tell(DateTime.UtcNow.ToLongTimeString()));
        }

        protected override void PreStart() =>
            _messagePump = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(
                initialDelay: TimeSpan.FromMilliseconds(1000),
                interval: TimeSpan.FromMilliseconds(1000),
                receiver: Self,
                message: new SendMessage(), 
                sender: Self);

        protected override void PostStop() => _messagePump?.Cancel();

        private class SendMessage { }
    }
}
