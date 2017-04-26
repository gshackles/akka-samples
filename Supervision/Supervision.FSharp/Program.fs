open System
open System.Threading
open Akka.Actor
open Akka.FSharp
    
let workerActor (msg: string) =
    match msg.ToLowerInvariant() with
    | "null" -> raise <| ArgumentNullException()
    | "cast" -> raise <| InvalidCastException()
    | "system" -> raise <| SystemException()
    | _ -> raise <| Exception()

let strategy = Strategy.OneForOne ((fun ex ->
    printfn "Determining supervision directive"

    match ex with 
    | :? ArgumentNullException -> 
        printfn "Stopping worker"
        Directive.Stop
    | :? InvalidCastException -> 
        printfn "Restarting worker"
        Directive.Restart
    | :? SystemException -> 
        printfn "Resuming worker"
        Directive.Resume
    | _ -> 
        printfn "Escalating the error"
        Directive.Escalate), 3, TimeSpan.FromSeconds(30.))

let supervisorActor (mailbox: Actor<_>) =
    let worker = spawn mailbox "worker" (actorOf workerActor)
    let rec loop() =
        actor {
            let! message = mailbox.Receive()
            worker <! message

            return! loop()
        }
    loop()

[<EntryPoint>]
let main argv = 
    use system = ActorSystem.Create("my-system")
    let supervisor = spawnOpt system "supervisor" supervisorActor [ SpawnOption.SupervisorStrategy strategy ]

    let rec loop() =
        Thread.Sleep 500

        printf "Enter command (null|cast|system): "
        let command = Console.ReadLine()

        if command <> "quit" then
            supervisor <! command
            loop()

    loop()

    0