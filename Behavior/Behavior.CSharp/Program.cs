using System;
using Akka.Actor;

namespace Behavior.CSharp
{
    public class LogIn
    {
        public string Name { get; }

        public LogIn(string name)
        {
            Name = name;
        }
    }

    public class LogOut { }

    public class Message
    {
        public string Contents { get; }

        public Message(string contents)
        {
            Contents = contents;
        }
    }

    public class UserActor : ReceiveActor
    {
        private string _currentName;

        public UserActor()
        {
            Unauthenticated();
        }

        private void Unauthenticated()
        {
            Receive<LogIn>(msg =>
            {
                _currentName = msg.Name;

                Become(Authenticated);
            });

            ReceiveAny(msg =>
               Console.WriteLine($"Ignoring message of type {msg.GetType().Name} due to being unauthenticated"));
        }

        private void Authenticated()
        {
            Receive<LogOut>(msg =>
            {
                _currentName = null;

                Become(Unauthenticated);
            });

            Receive<Message>(msg =>
                Console.WriteLine($"Message to {_currentName}: {msg.Contents}"));

            ReceiveAny(msg =>
               Console.WriteLine($"Ignoring message of type {msg.GetType().Name} due to being authenticated"));
        }
    }

    public static class Program
    {
        public static void Main(string[] args)
        {
            using (var system = ActorSystem.Create("my-system"))
            {
                var user = system.ActorOf<UserActor>("user");

                user.Tell(new Message("Yo"));
                user.Tell(new LogOut());

                user.Tell(new LogIn("Greg"));
                user.Tell(new Message("Yo"));
                user.Tell(new LogOut());

                user.Tell(new Message("Yo"));

                Console.ReadKey();
            }
        }
    }
}
