using System;
using System.Threading.Tasks;
using Akka.Actor;

namespace Counter.CSharp
{
    public enum Message
    {
        Increment,
        Decrement,
        GetState
    }

    public class CounterActor : ReceiveActor
    {
        private int _counter = 0;

        public CounterActor()
        {
            Receive<Message>(msg =>
            {
                switch (msg)
                {
                    case Message.Increment:
                        _counter++;
                        break;
                    case Message.Decrement:
                        _counter--;
                        break;
                    case Message.GetState:
                        Sender.Tell(_counter);
                        break;
                }
            });
        }
    }

    public static class Program
    {
        public static async Task Main(string[] args)
        {
            using (var system = ActorSystem.Create("my-system"))
            {
                var counter = system.ActorOf<CounterActor>("counter");

                counter.Tell(Message.Increment);
                counter.Tell(Message.Increment);
                counter.Tell(Message.Increment);
                counter.Tell(Message.Increment);

                counter.Tell(Message.Decrement);
                counter.Tell(Message.Decrement);

                var currentCount = await counter.Ask(Message.GetState);

                Console.WriteLine($"The current count is {currentCount}");
            }

            Console.ReadKey();
        }
    }
}
