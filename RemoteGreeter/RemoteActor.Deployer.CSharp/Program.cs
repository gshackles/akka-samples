using System;
using Akka.Actor;
using Akka.Configuration;
using RemoteActor.Shared.CSharp;

namespace RemoteActor.Deployer.CSharp
{
    public class SendGreet { }

    public class SendActor : ReceiveActor
    {
        private readonly IActorRef _remoteActor;
        private ICancelable _sendTask;

        public SendActor(IActorRef remoteActor)
        {
            _remoteActor = remoteActor;

            Receive<SendGreet>(msg => 
                _remoteActor.Tell(new Greet("Greg")));
        }

        protected override void PreStart()
        {
            _sendTask = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(
                TimeSpan.FromSeconds(1), 
                TimeSpan.FromSeconds(1), 
                Context.Self, 
                new SendGreet(), 
                ActorRefs.NoSender);
        }

        protected override void PostStop()
        {
            _sendTask.Cancel();
        }
    }

    class MainClass
    {
        public static void Main(string[] args)
        {
            using (var system = ActorSystem.Create("Deployer", ConfigurationFactory.ParseString(@"
	            akka {  
	                actor {
	                    provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
	                    deployment {
	                        /greeter {
	                            remote = ""akka.tcp://DeployTarget@localhost:8090""
	                        }
	                    }
	                }
	                remote.helios.tcp {
                        port = 0
                        hostname = localhost
	                }
	            }")))
            {
                var greeter = system.ActorOf(Props.Create(() => new GreetingActor()), "greeter");

                system.ActorOf(Props.Create(() => new SendActor(greeter)));

                Console.ReadKey();
            }
        }
    }
}
