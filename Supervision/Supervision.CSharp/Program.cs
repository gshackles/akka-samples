using System;
using System.Threading;
using Akka.Actor;

namespace Supervision.CSharp
{
    public class WorkerActor : ReceiveActor
    {
        public WorkerActor()
        {
            Receive<string>(command =>
            {
                switch (command.ToLowerInvariant())
                {
                    case "null":
                        throw new ArgumentNullException();
                    case "cast":
                        throw new InvalidCastException();
                    case "system":
                        throw new SystemException();
                    default:
                        throw new Exception();
                }
            });
        }
    }

    public class SupervisingActor : ReceiveActor
    {
        private readonly IActorRef _worker = Context.ActorOf(Props.Create(typeof(WorkerActor)), "worker");

        public SupervisingActor()
        {
            Receive<string>(command => _worker.Tell(command));
        }

        protected override SupervisorStrategy SupervisorStrategy() => new OneForOneStrategy(
                maxNrOfRetries: 3,
                withinTimeRange: TimeSpan.FromSeconds(30),
                localOnlyDecider: ex =>
                {
                    Console.WriteLine("Determining supervision directive");

                    switch (ex)
                    {
                        case ArgumentNullException _:
                            Console.WriteLine("Stopping worker");
                            return Directive.Stop;

                        case InvalidCastException _:
                            Console.WriteLine("Restarting worker");
                            return Directive.Restart;

                        case SystemException _:
                            Console.WriteLine("Resuming worker");
                            return Directive.Resume;

                        default:
                            Console.WriteLine("Escalating the error");
                            return Directive.Escalate;
                    }
                });
    }

    class MainClass
    {
        public static void Main(string[] args)
        {
            using (var system = ActorSystem.Create("my-system"))
            {
                var supervisor = system.ActorOf<SupervisingActor>("supervisor");

                while (true)
                {
                    Thread.Sleep(500);

                    Console.Write("Enter command (null|cast|system): ");
                    var command = Console.ReadLine();

                    if (command == "quit")
                        break;

                    supervisor.Tell(command);
                }
            }
        }
    }
}
