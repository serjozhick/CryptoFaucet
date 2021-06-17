using CryptoFaucet.Logging;
using System;

namespace CryptoFaucetTests.Mocks
{
    public class LogMock : ILog
    {
        public void Debug(string message)
        {
            Console.WriteLine($"Debug: {message}");
        }

        public void Error(string message)
        {
            Console.WriteLine($"Error: {message}");
        }

        public void Info(string message)
        {
            Console.WriteLine($"Info: {message}");
        }

        public void Warn(string message)
        {
            Console.WriteLine($"Warn: {message}");
        }
    }
}
