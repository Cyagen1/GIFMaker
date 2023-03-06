using GIFMaker.Entities;

namespace GIFMaker.Contracts
{
    public interface IGifGenerator
    {
        Task<Gif> GenerateAsync(string name, IEnumerable<Stream> fileStreams);
    }
}
