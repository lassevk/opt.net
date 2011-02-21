using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace Opt.Tests
{
    public abstract class TestBase
    {
        private readonly List<string> _TempFiles = new List<string>();

        [TestFixtureTearDown]
        public virtual void TestFixtureTearDown()
        {
            foreach (string fileName in _TempFiles)
            {
                if (File.Exists(fileName))
                    File.Delete(fileName);
            }
        }

        protected string GetTempFileName()
        {
            string result = Path.GetTempFileName();
            _TempFiles.Add(result);
            return result;
        }
    }
}