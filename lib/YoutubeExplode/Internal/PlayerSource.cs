using System.Collections.Generic;
using YoutubeExplode.Internal.CipherOperations;

namespace YoutubeExplode.Internal
{
    internal class PlayerSource
    {
        public IList<ICipherOperation> CipherOperations { get; }

        public PlayerSource(IList<ICipherOperation> cipherOperations)
        {
            CipherOperations = cipherOperations;
        }

        public string Decipher(string input)
        {
            foreach (var operation in CipherOperations)
                input = operation.Decipher(input);
            return input;
        }
    }
}