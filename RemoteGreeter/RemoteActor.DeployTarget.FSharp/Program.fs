open System
open Akka.FSharp
open Akka.Configuration
    
let config = ConfigurationFactory.ParseString @"
    akka {
        actor.provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
        remote.helios.tcp {
            hostname = localhost
            port = 8090
        }
    }"

[<EntryPoint>]
let main args =  
    use system = System.create "DeployTarget" config

    Console.ReadKey() |> ignore
    0