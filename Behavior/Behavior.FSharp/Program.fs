open System
open Akka.Actor
open Akka.FSharp

type MessageType =
    | LogIn of string
    | LogOut
    | Message of string

let user (mailbox: Actor<_>) =
    let rec unauthenticated() =
        actor {
            let! message = mailbox.Receive()
            return! match message with
                    | LogIn name -> authenticated name
                    | ignored ->
                        printfn "Ignoring mesage of type %A due to being unauthenticated" ignored
                        unauthenticated()
        }
    and authenticated currentName =
        actor {
            let! message = mailbox.Receive()
            return! match message with
                    | LogOut -> unauthenticated()
                    | Message content ->
                        printfn "Message to %s: %s" currentName content
                        authenticated currentName
                    | ignored ->
                        printfn "Ignoring mesage of type %A due to being authenticated" ignored
                        unauthenticated()
        }
    unauthenticated()

[<EntryPoint>]
let main argv = 
    use system = ActorSystem.Create("my-system")
    let user = spawn system "counter" user

    user <! Message "Yo"
    user <! LogOut

    user <! LogIn "Greg"
    user <! Message "Yo"
    user <! LogOut

    user <! Message "Yo"

    Console.ReadKey() |> ignore
    0