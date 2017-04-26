using System;
using Akka.Actor;
using Akka.Configuration;

namespace RemoteActor.DeployTarget.CSharp
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            using (var system = ActorSystem.Create("DeployTarget", ConfigurationFactory.ParseString(@"
	            akka {  
	                actor.provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
			        remote.helios.tcp {
			            hostname = localhost
			            port = 8090
			        }
	            }")))
            {
                Console.ReadKey();
            }
        }
    }
}
