using GIFMaker.Contracts;
using GIFMaker.Entities;
using ImageMagick;
using GIFMaker.Configuration;
using GIFMaker.Exceptions;

namespace GIFMaker.Services
{
    public class MagickGifGenerator : IGifGenerator
    {
        private readonly int animationDelay = 100;

        public MagickGifGenerator(GifGeneratorSettings settings)
        {
            animationDelay = settings.AnimationDelay;
        }
        public async Task<Gif> GenerateAsync(string name, IEnumerable<Stream> fileStreams)
        {
            ArgumentNullException.ThrowIfNull(name);
            ArgumentNullException.ThrowIfNull(fileStreams);

            if (!fileStreams.Any())
            {
                throw new InvalidOperationException("No files given.");
            }

            // Add images
            using var collection = new MagickImageCollection();
            foreach (var stream in fileStreams)
            {
                try
                {
                    collection.Add(new MagickImage(stream) { AnimationDelay = animationDelay });
                }
                catch (MagickException)
                {
                    throw new InvalidFileException();
                }
            }

            //Check for smallest image and resize the rest of images
            var smallestImage = collection.MinBy(x => x.BaseHeight * x.BaseWidth);
            var newImageSize = new MagickGeometry(smallestImage.Width, smallestImage.Height);
            newImageSize.IgnoreAspectRatio = true;

            foreach (var image in collection)
            {
                image.Resize(newImageSize);
            }

            // Create Gif
            using var gifStream = new MemoryStream();
            await collection.WriteAsync(gifStream, MagickFormat.Gif);
            var resultGif = new Gif(name, gifStream.ToArray());
            return resultGif;
        }
    }
}
