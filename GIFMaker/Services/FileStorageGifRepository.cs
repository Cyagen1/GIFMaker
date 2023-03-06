using GIFMaker.Configuration;
using GIFMaker.Contracts;
using GIFMaker.Entities;
using GIFMaker.Exceptions;

namespace GIFMaker.Services
{
    public class FileStorageGifRepository : IGifRepository
    {
        private readonly string folderPath;

        public FileStorageGifRepository(FileStorageGifRepositorySettings settings)
        {
            folderPath = settings.FolderPath;
            Directory.CreateDirectory(folderPath);
        }
        public async Task<Gif?> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException($"Empty {nameof(name)} given.", nameof(name));
            }

            var path = Path.Combine(folderPath, name + ".gif");
            if(!File.Exists(path))
            {
                return null;
            }

            return new Gif(name, await File.ReadAllBytesAsync(path));
        }

        public async Task<IEnumerable<Gif>> GetWhereNameContainsAsync(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentNullException($"Empty {nameof(input)} given.", nameof(input));
            }

            var result = new List<Gif>();

            foreach (var filePath in Directory.GetFiles(folderPath, $"*{input}*"))
            {
                result.Add(new Gif(Path.GetFileNameWithoutExtension(filePath), await File.ReadAllBytesAsync(filePath)));
            }

            return result;
        }

        public async Task InsertGifAsync(Gif gif)
        {
            if (await GetByNameAsync(gif.Name) != null) 
            {
                throw new GifNameTakenException(gif.Name);
            }

            await File.WriteAllBytesAsync(Path.Combine(folderPath, GetFileName(gif.Name)), gif.FileBytes);
        }

        private string GetFileName(string gifName)
        {
            return $"{gifName}.gif";
        }
    }
}
