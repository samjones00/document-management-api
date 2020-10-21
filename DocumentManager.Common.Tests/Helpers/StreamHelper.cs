using System;
using System.IO;
using Xunit;

namespace DocumentManager.Core.Tests.Helpers
{
    public static class StreamHelper
    {
        public static Stream GetExampleFile(string filename)
        {
            var directory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var stream = File.OpenRead($"{directory}\\ExampleFiles\\{filename}");

            Assert.NotNull(stream);

            stream.Position = 0;

            return stream;
        }
        
        public static string ConvertStreamToBase64(Stream stream)
        {
            var bytes = new Byte[(int)stream.Length];

            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(bytes, 0, (int)stream.Length);

            return Convert.ToBase64String(bytes);
        }

        public static MemoryStream CreateExampleStream(int bytes)
        {
            byte[] byteArray = new byte[bytes];
            MemoryStream stream = new MemoryStream();
            stream.Write(byteArray, 0, byteArray.Length);

            return stream;
        }
    }
}
