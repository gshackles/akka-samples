namespace MobileRemoteActor.Shared.CSharp
{
    public class CreateHash
    {
        public string Input { get; }

        public CreateHash(string input) => Input = input;
    }

    public class HashResult
    {
        public string Result { get; }

        public HashResult(string result) => Result = result;
    }
}
