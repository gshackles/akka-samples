using Akka.Actor;

namespace Clustering.CSharp.Actors
{
    public class ReceiverActor : ReceiveActor
    {
        public ReceiverActor() =>
            Receive<string>(msg => Context.System.Log.Info($"Received: {msg}"));
    }
}