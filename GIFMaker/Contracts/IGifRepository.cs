using GIFMaker.Entities;
using System.Drawing;

namespace GIFMaker.Contracts
{
    public interface IGifRepository
    {
        Task<Gif?> GetByNameAsync(string name);

        Task<IEnumerable<Gif>> GetWhereNameContainsAsync(string input);

        Task InsertGifAsync(Gif gif);
    }
}
