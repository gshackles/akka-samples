using System;
using Akka.Actor;
using Akka.Configuration;

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

            var config = ConfigurationFactory.ParseString(@"
                ﻿akka.actor {
                    loglevel = DEBUG
	                provider = ""Phobos.Actor.PhobosActorRefProvider, Phobos.Actor""
                }

                phobos {
                    monitoring {
                        provider-type = statsd
                        
                        statsd {
                            endpoint = 127.0.0.1
                            port = 8129
                        }
                    }
                }");

            using (var system = ActorSystem.Create("my-system", config))
            {
                var greeter = system.ActorOf<GreetingActor>("greeter");

                greeter.Tell(new Greet("Greg"));

                Console.ReadKey();
            }
        }
    }
}
