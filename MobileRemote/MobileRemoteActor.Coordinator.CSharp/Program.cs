using System;
using Akka.Actor;
using Akka.Configuration;
using Akka.Routing;
using MobileRemoteActor.Shared.CSharp;

namespace MobileRemoteActor.Coordinator.CSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var system = ActorSystem.Create("mobilecompute", ConfigurationFactory.ParseString(@"
                akka {  
                    actor {
                        provider = cluster
                        deployment {
                            /hashers {
                                router = round-robin-pool
                                nr-of-instances = 3
                                cluster {
                                    enabled = on
                                    allow-local-routees = on
                                    use-role = hasher
                                }
                            }
                        }
                    }

                    cluster {
                        seed-nodes = [""akka.tcp://mobilecompute@127.0.0.1:4053""]
                        roles = [coordinator]
                    }
                }")))
            {
                var hashers = system.ActorOf(Props.Create<HasherActor>().WithRouter(FromConfig.Instance), "hashers");
                var coordinator = system.ActorOf(Props.Create(() => new CoordinatorActor(hashers)), "coordinator");

                Console.ReadKey();
            }
        }
    }
}
