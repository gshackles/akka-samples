open System
open Akka.Actor
open Akka.FSharp
open Akka.Configuration
    
type Message = 
    | Greet of string

let handleMessage (mailbox: Actor<'a>) msg =
    match msg with
    | Greet who -> printfn "Hello, %s" who

[<EntryPoint>]
let main argv = 
    let config = ConfigurationFactory.ParseString("""
                akka.actor {
                    loglevel = DEBUG
                    provider = "Phobos.Actor.PhobosActorRefProvider,Phobos.Actor"
                }

                phobos {
                    monitoring {
                        provider-type = statsd
                        statsd {
                            endpoint = 127.0.0.1
                            port = 8125
                        }
                    }
                }""");

    use system = ActorSystem.Create("my-system", config)
    let greeter = spawn system "greeter" (actorOf2 handleMessage)
    greeter <! Greet "Greg"

    Console.ReadKey() |> ignore
    0