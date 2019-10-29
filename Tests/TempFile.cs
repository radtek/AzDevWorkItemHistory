using System;
using System.IO;

namespace Tests
{
    public class TempFile : IDisposable
    {
        public string Path { get; }

        /// <summary>
        /// Creates a temp file (deletes when disposed).
        /// </summary>
        public static TempFile Create(string path)
        {
            using var _ = File.Create(path);
            return new TempFile(path);
        }

        /// <summary>
        /// Returns a temp file (deletes when disposed), but doesn't create it.
        /// </summary>
        public static TempFile Init(string path)
        {
            return new TempFile(path);
        }

        private TempFile(string path)
        {
            Path = path;
        }

        void IDisposable.Dispose()
        {
            File.Delete(Path);
        }
    }
}