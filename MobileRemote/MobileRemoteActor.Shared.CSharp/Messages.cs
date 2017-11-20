namespace MobileRemoteActor.Shared.CSharp
{
    public class CreateHash
    {
        public string Input { get; }

        public CreateHash(string input) => Input = input;
    }

    public class HashResult
    {
        public string Hash { get; }
        public string Salt { get; }

        public HashResult(string hash, string salt) 
        {
            Hash = hash;
            Salt = salt;
        }
    }
}
