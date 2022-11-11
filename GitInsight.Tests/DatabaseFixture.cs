using GitInsight.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitInsight.Tests
{
    public class DatabaseFixture : IDisposable
    {
        public IRepository repo;
        private string extractPath;

        public DatabaseFixture()
        {
            extractPath = Directory.GetCurrentDirectory() + "../../../../LocalRepo";
            var zipPath = extractPath + "/.git.zip";

            ZipFile.ExtractToDirectory(zipPath, extractPath);
            repo = new Repository(extractPath);
        }

        public void Dispose()
        {
            repo.Dispose();
            Directory.Delete(extractPath + "/.git", true);
        }
    }
}
