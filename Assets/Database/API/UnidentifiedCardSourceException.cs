using System;

namespace Database.API
{
    /// <summary>
    /// Exception thrown by the card database API where card source is unsupported. 
    /// </summary>
    public class UnidentifiedCardSourceException : Exception
    {
        public UnidentifiedCardSourceException() : base("Card source is unsupported!")
        {
            
        }
        
        public UnidentifiedCardSourceException(string source) : base
        ($"Card source is unsupported, it was: {source}")
        {
            
        }
    }
}