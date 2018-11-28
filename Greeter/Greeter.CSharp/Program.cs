using System;
using System.IO;
using Akka.Actor;
using Akka.Configuration;
using OpenTracing.Util;
using Phobos.Actor;

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
        private readonly IPhobosActorContext _instrumentation = Context.GetInstrumentation();

        public GreetingActor()
        {
            Receive<Greet>(msg =>
            {
                _instrumentation.Monitor.IncrementCounter("awesome-counter", 1);

                _instrumentation.ActiveSpan.SetTag("who", msg.Who);

                for (var i = 0; i < 5; i++)
                {
                    using (_instrumentation.Tracer.BuildSpan("nap-time").WithTag("iteration", i).StartActive())
                        System.Threading.Thread.Sleep(100);

                    System.Threading.Thread.Sleep(50);
                }

                Console.WriteLine($"Hello, {msg.Who}");
            });
        }
    }

    public static class Program
    {
        public static void Main(string[] args)
        {
            GlobalTracer.Register(Datadog.Trace.OpenTracing.OpenTracingTracerFactory.CreateTracer());

            var config = ConfigurationFactory.ParseString(@"
                akka.actor {
                    loglevel = DEBUG
                    provider = ""Phobos.Actor.PhobosActorRefProvider,Phobos.Actor""
                }

                phobos {
                    monitoring {
                        provider-type = statsd
                        monitor-mailbox-depth = on
                        statsd {
                            endpoint = 127.0.0.1
                            port = 8125
                        }
                    }

                    tracing {
		                provider-type = default
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
