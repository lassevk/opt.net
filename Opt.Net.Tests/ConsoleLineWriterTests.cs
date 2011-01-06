using System;
using System.IO;
using NUnit.Framework;

namespace Opt.Tests
{
    [TestFixture]
    public class ConsoleLineWriterTests
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _ConsoleOut = Console.Out;
            _ConsoleError = Console.Error;

            _TempOut = new StringWriter();
            _TempError = new StringWriter();

            Console.SetOut(_TempOut);
            Console.SetError(_TempError);
        }

        [TearDown]
        public void TearDown()
        {
            Console.SetOut(_ConsoleOut);
            Console.SetError(_ConsoleError);

            _TempOut.Dispose();
            _TempError.Dispose();
        }

        #endregion

        private TextWriter _ConsoleOut;
        private TextWriter _ConsoleError;

        private StringWriter _TempOut;
        private StringWriter _TempError;

        [Test]
        public void WriteErrorLine_NonNullLine_DoesNotWriteToConsoleOut()
        {
            var writer = new ConsoleLineWriter();

            writer.WriteErrorLine("LINE1");
            writer.WriteErrorLine("LINE2");

            Assert.That(_TempOut.ToString(), Is.EqualTo(string.Empty));
        }

        [Test]
        public void WriteErrorLine_NonNullLine_WritesLineToErrorConsole()
        {
            var writer = new ConsoleLineWriter();

            writer.WriteErrorLine("LINE1");
            writer.WriteErrorLine("LINE2");

            CollectionAssert.AreEqual(_TempError.ToString().Split(new[]
            {
                Environment.NewLine
            }, StringSplitOptions.None), new[]
            {
                "LINE1", "LINE2", string.Empty
            });
        }

        [Test]
        public void WriteErrorLine_NullLine_ThrowsArgumentNullException()
        {
            var writer = new ConsoleLineWriter();

            Assert.Throws<ArgumentNullException>(() => writer.WriteErrorLine(null));
        }

        [Test]
        public void WriteLine_NonNullLine_DoesNotWriteToConsoleError()
        {
            var writer = new ConsoleLineWriter();

            writer.WriteLine("LINE1");
            writer.WriteLine("LINE2");

            Assert.That(_TempError.ToString(), Is.EqualTo(string.Empty));
        }

        [Test]
        public void WriteLine_NonNullLine_WritesLineToConsole()
        {
            var writer = new ConsoleLineWriter();

            writer.WriteLine("LINE1");
            writer.WriteLine("LINE2");

            CollectionAssert.AreEqual(_TempOut.ToString().Split(new[]
            {
                Environment.NewLine
            }, StringSplitOptions.None), new[]
            {
                "LINE1", "LINE2", string.Empty
            });
        }

        [Test]
        public void WriteLine_NullLine_ThrowsArgumentNullException()
        {
            var writer = new ConsoleLineWriter();

            Assert.Throws<ArgumentNullException>(() => writer.WriteLine(null));
        }
    }
}