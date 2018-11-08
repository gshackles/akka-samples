using System;
using Akka.Actor;

namespace Greeter.CSharp
{
    public class Greet
    {
        public string Who { get; }

        public Greet(string who)
        {
            Who = who;
        }
    }

    public class GreetingActor : ReceiveActor
    {
        public GreetingActor()
        {
            Receive<Greet>(msg =>
                Console.WriteLine($"Hello, {msg.Who}"));
        }
    }

    public static class Program
    {
        public static void Main(string[] args)
        {
            using (var system = ActorSystem.Create("my-system"))
            {
                var greeter = system.ActorOf<GreetingActor>("greeter");

                greeter.Tell(new Greet("Greg"));

                Console.ReadKey();
            }
        }
    }
}
