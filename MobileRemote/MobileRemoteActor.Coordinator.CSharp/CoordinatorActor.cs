using System;
using Akka.Actor;
using MobileRemoteActor.Shared.CSharp;

namespace MobileRemoteActor.Coordinator.CSharp
{
    public class SendHashRequest { }

    public class CoordinatorActor : ReceiveActor
    {
        private readonly IActorRef _hashers;
        private ICancelable _sendTask;

        public CoordinatorActor(IActorRef hashers)
        {
            _hashers = hashers;

            Receive<SendHashRequest>(msg =>
                _hashers.Tell(new CreateHash(Guid.NewGuid().ToString())));
        }

        protected override void PreStart() =>
            _sendTask = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(1),
                Context.Self,
                new SendHashRequest(),
                ActorRefs.NoSender);

        protected override void PostStop() => _sendTask.Cancel();
    }
}
