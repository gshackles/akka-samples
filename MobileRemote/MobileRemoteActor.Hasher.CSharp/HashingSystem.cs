using Akka.Actor;
using Akka.Configuration;

namespace MobileRemoteActor.Hasher.CSharp
{
    public static class HashingSystem
    {
        private static readonly ActorSystem _system;

        static HashingSystem() =>
            _system = ActorSystem.Create("mobilecompute", ConfigurationFactory.ParseString(@"
                akka {  
                    actor {
                        provider = cluster
                    }
                    cluster {
                        seed-nodes = [""akka.tcp://mobilecompute@127.0.0.1:4053""]
                        roles = [hasher]
                    }
                }"));

        public static void Initialize()
        {
        }
    }
}
