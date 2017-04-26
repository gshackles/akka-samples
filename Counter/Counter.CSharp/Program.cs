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

    class MainClass
    {
        private static async Task RunAsync()
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
        }

        public static void Main(string[] args)
        {
            Task.Run(RunAsync).Wait();
            Console.ReadKey();
        }
    }
}
