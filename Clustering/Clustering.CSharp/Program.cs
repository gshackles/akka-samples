using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Cluster.Tools.Singleton;
using Akka.Configuration;
using Akka.Routing;
using Clustering.CSharp.Actors;
using Petabridge.Cmd.Cluster;
using Petabridge.Cmd.Host;

namespace Clustering.CSharp
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            if (!int.TryParse(args[0], out var nodeId))
            {
                Console.Error.WriteLine("Please supply a node ID");

                return;
            }

            var config = GetAkkaConfig(nodeId);

            using (var system = ActorSystem.Create("demo-cluster", config))
            {
                var cmd = PetabridgeCmd.Get(system);
                cmd.RegisterCommandPalette(ClusterCommands.Instance);
                cmd.Start();

                system.ActorOf(ClusterSingletonManager.Props(
                        singletonProps: Props.Create<SenderActor>(),
                        terminationMessage: PoisonPill.Instance,
                        settings: ClusterSingletonManagerSettings.Create(system)),
                        name: "sender");

                var props = Props.Create<ReceiverActor>().WithRouter(FromConfig.Instance);
                system.ActorOf(props, "receivers");

                Console.ReadLine();
            }
        }

        private static Config GetAkkaConfig(int nodeId) => new List<string>
        {
            $"akka.remote.dot-netty.tcp.port=700{nodeId}",
            $"petabridge.cmd.port=900{nodeId}",
            @"akka {
                actor {
                    provider = cluster

                    deployment {
                        /receivers {
                            router = round-robin-pool # round-robin-pool, broadcast-pool, random-pool
                            routees.paths = [""/user/receiver""]

                            cluster {
                                enabled = on
                                allow-local-routees = on
                            }
                        }
                    }
                }
          
                remote.hostname = 0.0.0.0
                cluster.seed-nodes = [""akka.tcp://demo-cluster@0.0.0.0:7001"",""akka.tcp://demo-cluster@0.0.0.0:7002"",""akka.tcp://demo-cluster@0.0.0.0:7003""];
            }

            petabridge.cmd.host = 0.0.0.0"
        }.Aggregate(Config.Empty,
            (current, hocon) => ConfigurationFactory.ParseString(hocon).WithFallback(current));
    }
}
