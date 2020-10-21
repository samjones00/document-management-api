using System;
using System.IO;

namespace DocumentManager.Core.Tests.Helpers
{
    public static class FileHelper
    {
        public static Stream GetExampleFile(string filename)
        {
            var directory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var stream = File.OpenRead($"{directory}\\ExampleFiles\\{filename}");

            stream.Position = 0;

            return stream;
        }
        
        public static string ConvertToBase64(Stream stream)
        {
            var bytes = new Byte[(int)stream.Length];

            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(bytes, 0, (int)stream.Length);

            return Convert.ToBase64String(bytes);
        }
    }
}
