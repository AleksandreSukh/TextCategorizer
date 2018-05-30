using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Stemmer
{
    public class EmbeddedResourceReader
    {
        static readonly Assembly ExecutingAssembly = Assembly.GetExecutingAssembly();
        public static byte[] ReadResource(string res)
        {
            using (var stream = GetResourceStream(res))
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }

        private static Stream GetResourceStream(string res)
        {
            var names = ExecutingAssembly.GetManifestResourceNames();
            var resourceName = names.Single(n => n.EndsWith(res, StringComparison.OrdinalIgnoreCase));

            Stream stream = ExecutingAssembly.GetManifestResourceStream(resourceName);
            return stream;
        }

        public static async Task<string> ReadResourceAsStringAsync(string res)
        {
            using (var stream = GetResourceStream(res))
            using (var st = new StreamReader(stream))
                return await st.ReadToEndAsync().ConfigureAwait(false);
        }
    }

    public static class StreamExtensions
    {
        public static Stream ToStream(this byte[] data)
        {
            var stream = new MemoryStream();
            stream.Write(data, 0, data.Length);
            return stream;
        }
    }
}
