using System;
using System.Security.Cryptography;
using Akka.Actor;

namespace MobileRemoteActor.Shared.CSharp
{
    public class HasherActor : ReceiveActor
    {
        public HasherActor() => Receive<CreateHash>(msg => Hash(msg));

        private void Hash(CreateHash request)
        {
            var derivedBytes = new Rfc2898DeriveBytes(request.Input, saltSize: 32)
            {
                IterationCount = 1000
            };
            var hash = Convert.ToBase64String(derivedBytes.GetBytes(20));
            var salt = Convert.ToBase64String(derivedBytes.Salt);
            var result = new HashResult(hash, salt);

            Sender.Tell(result);
        }
    }
}
