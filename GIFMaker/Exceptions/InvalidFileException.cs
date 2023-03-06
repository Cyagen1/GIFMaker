namespace GIFMaker.Exceptions
{
    public class InvalidFileException : Exception
    {
        public InvalidFileException() : base($"Given file is not an image.")
        {            
        }
    }
}
