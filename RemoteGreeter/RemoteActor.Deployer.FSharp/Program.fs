open System
open Akka.FSharp
open Akka.Actor
open Akka.Configuration

type Message = 
    | SendGreet

let sendActor (remoteActor: IActorRef) (mailbox: Actor<_>) =
    let sendTask = mailbox.Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(
                    TimeSpan.FromSeconds 1.,
                    TimeSpan.FromSeconds 1.,
                    mailbox.Context.Self,
                    SendGreet,
                    ActorRefs.NoSender)

    mailbox.Defer sendTask.Cancel

    let rec loop() = actor {
        let! msg = mailbox.Receive()

        match msg with
        | SendGreet -> remoteActor <! "Greg"

        return! loop()
    }
    loop()


let config = ConfigurationFactory.ParseString @"
    akka {  
	    actor.provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
	    remote.helios.tcp {
	          port = 0
	          hostname = localhost
	    }
	}"

let deployRemotely address = Deploy(RemoteScope (Address.Parse address))  
let spawnRemote systemOrContext remoteSystemAddress actorName expr =  
    spawne systemOrContext actorName expr [SpawnOption.Deploy (deployRemotely remoteSystemAddress)]

[<EntryPoint>]
let main argv = 
    use system = System.create "my-system" config  

    let greeter = spawnRemote system "akka.tcp://DeployTarget@localhost:8090" "greeter"
                    <@ actorOf2 (
                        fun (mailbox: Actor<string>) msg ->
                            printfn "Hello, %s" msg) @>

    let sender = spawn system "sender" (sendActor greeter)

    Console.ReadKey() |> ignore
    0