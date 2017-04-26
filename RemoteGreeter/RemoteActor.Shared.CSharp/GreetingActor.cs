using System;
using Akka.Actor;

namespace RemoteActor.Shared.CSharp
{
    public class GreetingActor : ReceiveActor
    {
        public GreetingActor()
        {
            Receive<Greet>(msg =>
               Console.WriteLine($"Hello, {msg.Who}"));
        }
    }
}
