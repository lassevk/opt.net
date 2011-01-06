using System;
using System.Globalization;
using System.Reflection;
using NUnit.Framework;

namespace Opt.Tests
{
    [TestFixture]
    public class FloatingPointPropertyTests
    {
        public class Container
        {
            private double _DoubleProperty;

            [FloatingPointOption("--double", "VALUE")]
            [FloatingPointOption("-d", "VALUE")]
            public double DoubleProperty
            {
                get
                {
                    return _DoubleProperty;
                }

                set
                {
                    _DoubleProperty = value;
                    DoublePropertyWasSet = true;
                }
            }

            public bool DoublePropertyWasSet
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
        [TestCase("1000.13")]
        [TestCase("-1000.13")]
        public void AssignValueToProperty_GoodValues_AssignsValueToProperty(string input)
        {
            var attr = new IntegerOptionAttribute("-t", "VALUE");
            var container = new Container();
            PropertyInfo propertyInfo = typeof(Container).GetProperty("DoubleProperty");

            Assert.That(container.DoubleProperty, Is.EqualTo(0));

            attr.AssignValueToProperty(container, propertyInfo, input);

            Assert.That(container.DoubleProperty, Is.EqualTo(Double.Parse(input, CultureInfo.InvariantCulture)));
            Assert.That(container.DoublePropertyWasSet, Is.True);
        }

        [TestCase("1.7976931348623157E+309")]
        [TestCase("-1.7976931348623157E+309")]
        public void AssignValueToProperty_OutOfRangeValues_ThrowsOverflowException(string input)
        {
            var attr = new IntegerOptionAttribute("-t", "VALUE");
            var container = new Container();
            PropertyInfo propertyInfo = typeof(Container).GetProperty("DoubleProperty");

            Assert.Throws<OverflowException>(() => attr.AssignValueToProperty(container, propertyInfo, input));
        }

        [TestCase("XYZ")]
        [TestCase("")]
        public void AssignValueToProperty_BadValues_ThrowsFormatException(string input)
        {
            var attr = new IntegerOptionAttribute("-t", "VALUE");
            var container = new Container();
            PropertyInfo propertyInfo = typeof(Container).GetProperty("DoubleProperty");

            Assert.Throws<FormatException>(() => attr.AssignValueToProperty(container, propertyInfo, input));
        }

        [TestCase("-d10.123", 10.123)]
        [TestCase("-d-10.345", -10.345)]
        [TestCase("--double=10.123", 10.123)]
        [TestCase("--double=-10.345", -10.345)]
        public void OptParser_WithIntegerArguments_AssignsToProperties(string argument, double expected)
        {
            var container = new Container();
            var map = new PropertyMap(typeof(Container));

            string[] leftovers = map.Map(new[]
                {
                    argument
                }, container);

            Assert.That(container.DoubleProperty, Is.EqualTo(expected));
            Assert.That(container.DoublePropertyWasSet, Is.True);
            CollectionAssert.AreEqual(leftovers, new string[0]);
        }

        [TestCase("-d", "10.123", 10.123)]
        [TestCase("-d", "-10.345", -10.345)]
        [TestCase("--double", "10.123", 10.123)]
        [TestCase("--double", "-10.345", -10.345)]
        public void OptParser_WithSeparateIntegerArguments_AssignsToProperties(string option, string argument, double expected)
        {
            var container = new Container();
            var map = new PropertyMap(typeof(Container));

            string[] leftovers = map.Map(new[]
                {
                    option, argument
                }, container);

            Assert.That(container.DoubleProperty, Is.EqualTo(expected));
            Assert.That(container.DoublePropertyWasSet, Is.True);
            CollectionAssert.AreEqual(leftovers, new string[0]);
        }

        [Test]
        public void AssignValueToProperty_NullContainer_ThrowsArgumentNullException()
        {
            var attr = new IntegerOptionAttribute("-t", "VALUE");
            object container = null;
            PropertyInfo propertyInfo = typeof(Container).GetProperty("DoubleProperty");
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
            PropertyInfo propertyInfo = typeof(Container).GetProperty("DoubleProperty");
            string value = null;

            Assert.Throws<ArgumentNullException>(() => attr.AssignValueToProperty(container, propertyInfo, value));
        }
    }
}