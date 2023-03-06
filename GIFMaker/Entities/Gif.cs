namespace GIFMaker.Entities
{
    public class Gif
    {
        public Gif(string name, byte[] fileBytes)
        {
            ArgumentNullException.ThrowIfNull(name);
            ArgumentNullException.ThrowIfNull(fileBytes);
            if (fileBytes.Length == 0)
            {
                throw new ArgumentException($"{nameof(fileBytes)} was empty.");
            }

            if (!name.All(char.IsLetterOrDigit))
            {
                throw new ArgumentException($"{nameof(name)} can only contain alphanumerical characters.", nameof(name));
            }

            Name = name;
            FileBytes = fileBytes;
        }

        public string Name { get; }

        public byte[] FileBytes { get; }
    }
}
