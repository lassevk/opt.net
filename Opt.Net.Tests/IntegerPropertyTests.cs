using NUnit.Framework;

namespace Opt.Tests
{
    [TestFixture]
    public class IntegerPropertyTests
    {
        public class Container
        {
            [IntegerOption("--int32", "VALUE")]
            [IntegerOption("-i", "VALUE")]
            public int Int32Property
            {
                get;
                set;
            }
        }
    }
}