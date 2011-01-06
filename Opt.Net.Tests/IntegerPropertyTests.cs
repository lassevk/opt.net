using System;
using System.Globalization;
using System.Reflection;
using NUnit.Framework;

namespace Opt.Tests
{
    [TestFixture]
    public class IntegerPropertyTests
    {
        public class Container
        {
            private int _Int32Property;

            [IntegerOption("--int32", "VALUE")]
            [IntegerOption("-i", "VALUE")]
            public int Int32Property
            {
                get
                {
                    return _Int32Property;
                }

                set
                {
                    _Int32Property = value;
                    Int32PropertyWasSet = true;
                }
            }

            public bool Int32PropertyWasSet
            {
                get;
                set;
            }
        }

        [TestCase("10")]
        [TestCase("-10")]
        [TestCase("1")]
        [TestCase("2147483647")]
        [TestCase("-2147483648")]
        public void AssignValueToProperty_GoodValues_AssignsValueToProperty(string input)
        {
            var attr = new IntegerOptionAttribute("-t", "VALUE");
            var container = new Container();
            PropertyInfo propertyInfo = typeof(Container).GetProperty("Int32Property");

            Assert.That(container.Int32Property, Is.EqualTo(0));

            attr.AssignValueToProperty(container, propertyInfo, input);

            Assert.That(container.Int32Property, Is.EqualTo(Int32.Parse(input, CultureInfo.InvariantCulture)));
            Assert.That(container.Int32PropertyWasSet, Is.True);
        }

        [TestCase("2147483648")]
        [TestCase("-2147483649")]
        public void AssignValueToProperty_OutOfRangeValues_ThrowsOverflowException(string input)
        {
            var attr = new IntegerOptionAttribute("-t", "VALUE");
            var container = new Container();
            PropertyInfo propertyInfo = typeof(Container).GetProperty("Int32Property");

            Assert.Throws<OverflowException>(() => attr.AssignValueToProperty(container, propertyInfo, input));
        }

        [TestCase("XYZ")]
        [TestCase("")]
        public void AssignValueToProperty_BadValues_ThrowsFormatException(string input)
        {
            var attr = new IntegerOptionAttribute("-t", "VALUE");
            var container = new Container();
            PropertyInfo propertyInfo = typeof(Container).GetProperty("Int32Property");

            Assert.Throws<FormatException>(() => attr.AssignValueToProperty(container, propertyInfo, input));
        }

        [TestCase("-i10", 10)]
        [TestCase("-i-10", -10)]
        [TestCase("--int32=10", 10)]
        [TestCase("--int32=-10", -10)]
        public void OptParser_WithIntegerArguments_AssignsToProperties(string argument, int expected)
        {
            var container = new Container();
            var map = new PropertyMap(typeof(Container));

            string[] leftovers = map.Map(new[] { argument }, container);

            Assert.That(container.Int32Property, Is.EqualTo(expected));
            Assert.That(container.Int32PropertyWasSet, Is.True);
            CollectionAssert.AreEqual(leftovers, new string[0]);
        }

        [Test]
        public void AssignValueToProperty_NullContainer_ThrowsArgumentNullException()
        {
            var attr = new IntegerOptionAttribute("-t", "VALUE");
            object container = null;
            PropertyInfo propertyInfo = typeof(Container).GetProperty("Int32Property");
            string value = "10";

            Assert.Throws<ArgumentNullException>(() => attr.AssignValueToProperty(container, propertyInfo, value));
        }

        [Test]
        public void AssignValueToProperty_NullPropertyInfo_ThrowsArgumentNullException()
        {
            var attr = new IntegerOptionAttribute("-t", "VALUE");
            var container = new Container();
            PropertyInfo propertyInfo = null;
            string value = "10";

            Assert.Throws<ArgumentNullException>(() => attr.AssignValueToProperty(container, propertyInfo, value));
        }

        [Test]
        public void AssignValueToProperty_NullValue_ThrowsArgumentNullException()
        {
            var attr = new IntegerOptionAttribute("-t", "VALUE");
            var container = new Container();
            PropertyInfo propertyInfo = typeof(Container).GetProperty("Int32Property");
            string value = null;

            Assert.Throws<ArgumentNullException>(() => attr.AssignValueToProperty(container, propertyInfo, value));
        }
    }
}