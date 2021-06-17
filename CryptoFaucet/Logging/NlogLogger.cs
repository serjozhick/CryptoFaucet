using System.IO;
using Microsoft.Extensions.Configuration;
using NLog;

namespace CryptoFaucet.Logging
{
    public class NlogLogger : ILog
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();  
        
        public NlogLogger(IConfiguration configuration)  
        {  
            LogManager.LoadConfiguration(configuration["Log:Config"]);              
        }  
   
        public void Info(string message)  
        {  
            logger.Info(message);  
        }  
   
        public void Warn(string message)  
        {  
            logger.Warn(message);  
        }  
   
        public void Debug(string message)  
        {  
            logger.Debug(message);  
        }  
   
        public void Error(string message)  
        {  
            logger.Error(message);  
        }  
    }
}