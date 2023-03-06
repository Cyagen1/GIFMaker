namespace GIFMaker.Exceptions
{
    public class GifNameTakenException : Exception
    {
        public GifNameTakenException(string name) : base($"Name {name} already exists. Name must be unique!")
        {
        }
    }
}
