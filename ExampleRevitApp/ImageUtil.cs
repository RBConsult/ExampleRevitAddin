using System;
using System.Linq;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace ExampleRevitAddin
{
    class ImageUtil
    {
        private static readonly BitmapSource DefaultLargeImage;

        static ImageUtil()
        {
            DefaultLargeImage = GetEmbeddedImage(Assembly.GetExecutingAssembly(), "");
        }

        /// <summary>
        /// Gets the embedded image.
        /// </summary>
        /// <param name="imageFullName">Full name of the image.</param>
        /// <returns></returns>
        public static BitmapSource GetEmbeddedImage(string imageFullName)
        {
            return GetEmbeddedImage(Assembly.GetExecutingAssembly(), imageFullName);
        }

        /// <summary>
        /// Gets the embedded image.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="imageFullName">Full name of the image.</param>
        /// <returns></returns>
        public static BitmapSource GetEmbeddedImage(Assembly assembly, string imageFullName)
        {
            try
            {
                if (!String.IsNullOrEmpty(imageFullName))
                {
                    var s = assembly.GetManifestResourceStream(imageFullName);
                    if (s != null) return BitmapFrame.Create(s);
                }
                return DefaultLargeImage;
            }
            catch
            {
                return null;
            }
        }
    }
}
