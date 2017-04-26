open System
open Akka.Actor
open Akka.FSharp
    
type Message = 
    | Greet of string

let handleMessage (mailbox: Actor<'a>) msg =
    match msg with
    | Greet who -> printfn "Hello, %s" who

[<EntryPoint>]
let main argv = 
    use system = ActorSystem.Create("my-system")
    let greeter = spawn system "greeter" (actorOf2 handleMessage)
    greeter <! Greet "Greg"

    Console.ReadKey() |> ignore
    0