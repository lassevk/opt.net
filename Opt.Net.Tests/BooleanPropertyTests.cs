using System;
using System.Reflection;
using NUnit.Framework;

namespace Opt.Tests
{
    [TestFixture]
    public class BooleanPropertyTests
    {
        public class Container
        {
            private bool _DefaultFalseProperty;
            private bool _DefaultTrueProperty = true;

            [BooleanOption("--prop1", true)]
            [BooleanOption("-t", true)]
            public bool DefaultFalseProperty
            {
                get
                {
                    return _DefaultFalseProperty;
                }
                set
                {
                    DefaultFalsePropertyWasSet = true;
                    _DefaultFalseProperty = value;
                }
            }

            [BooleanOption("--prop2", false)]
            [BooleanOption("-f", false)]
            public bool DefaultTrueProperty
            {
                get
                {
                    return _DefaultTrueProperty;
                }
                set
                {
                    DefaultTruePropertyWasSet = true;
                    _DefaultTrueProperty = value;
                }
            }

            public bool DefaultFalsePropertyWasSet
            {
                get;
                set;
            }

            public bool DefaultTruePropertyWasSet
            {
                get;
                set;
            }
        }

        public class IncorrectContainer
        {
            [BooleanOption("-t")]
            public string DummyProperty
            {
                get;
                set;
            }
        }

        [TestCase(false)]
        [TestCase(true)]
        public void PropertyValue_AfterConstructorWithPropertyValue_ContainsSameValueGivenToConstructor(bool input)
        {
            var attr = new BooleanOptionAttribute("-t", input);
            Assert.That(attr.PropertyValue, Is.EqualTo(input));
        }

        [TestCase("YES")]
        [TestCase("Y")]
        [TestCase("TRUE")]
        [TestCase("1")]
        [TestCase("ON")]
        [TestCase("+")]
        public void AssignValueToProperty_TrueValues_AssignsTrueToProperty(string input)
        {
            var attr = new BooleanOptionAttribute("-t");
            var container = new Container();
            PropertyInfo propertyInfo = typeof (Container).GetProperty("DefaultFalseProperty");

            Assert.That(container.DefaultFalseProperty, Is.False);

            attr.AssignValueToProperty(container, propertyInfo, input);

            Assert.That(container.DefaultFalseProperty, Is.True);
            Assert.That(container.DefaultFalsePropertyWasSet, Is.True);
        }

        [TestCase("O")]
        [TestCase("++")]
        [TestCase("--")]
        [TestCase("NN")]
        [TestCase("F")]
        [TestCase("L")]
        [TestCase("X")]
        public void AssignValueToProperty_UnknownValues_ThrowsInvalidOperationException(string input)
        {
            var attr = new BooleanOptionAttribute("-t");
            var container = new Container();
            PropertyInfo propertyInfo = typeof (Container).GetProperty("DefaultFalseProperty");

            Assert.Throws<InvalidOperationException>(() => attr.AssignValueToProperty(container, propertyInfo, input));
        }

        [TestCase("NO")]
        [TestCase("N")]
        [TestCase("FALSE")]
        [TestCase("0")]
        [TestCase("OFF")]
        [TestCase("-")]
        public void AssignValueToProperty_FalseValues_AssignsFalseToProperty(string input)
        {
            var attr = new BooleanOptionAttribute("-f");
            var container = new Container();
            PropertyInfo propertyInfo = typeof (Container).GetProperty("DefaultTrueProperty");

            Assert.That(container.DefaultTrueProperty, Is.True);

            attr.AssignValueToProperty(container, propertyInfo, input);

            Assert.That(container.DefaultTrueProperty, Is.False);
            Assert.That(container.DefaultTruePropertyWasSet, Is.True);
        }

        [Test]
        public void AssignValueToProperty_NullContainer_ThrowsArgumentNullException()
        {
            var attr = new BooleanOptionAttribute("-t");
            object container = null;
            PropertyInfo propertyInfo = typeof (Container).GetProperty("DefaultFalseProperty");
            string value = "true";

            Assert.Throws<ArgumentNullException>(() => attr.AssignValueToProperty(container, propertyInfo, value));
        }

        [Test]
        public void AssignValueToProperty_NullPropertyInfo_ThrowsArgumentNullException()
        {
            var attr = new BooleanOptionAttribute("-t");
            var container = new Container();
            PropertyInfo propertyInfo = null;
            string value = "true";

            Assert.Throws<ArgumentNullException>(() => attr.AssignValueToProperty(container, propertyInfo, value));
        }

        [Test]
        public void AssignValueToProperty_NullValue_ThrowsArgumentNullException()
        {
            var attr = new BooleanOptionAttribute("-t");
            var container = new Container();
            PropertyInfo propertyInfo = typeof (Container).GetProperty("DefaultFalseProperty");
            string value = null;

            Assert.Throws<ArgumentNullException>(() => attr.AssignValueToProperty(container, propertyInfo, value));
        }

        [Test]
        public void Map_WithLongOption_SetsProperty()
        {
            var map = new PropertyMap(typeof (Container));
            var container = new Container();
            var arguments = new[] { "--prop1" };

            string[] leftovers = map.Map(arguments, container);

            Assert.That(container.DefaultFalseProperty, Is.True);
            Assert.That(container.DefaultFalsePropertyWasSet, Is.True);
            CollectionAssert.AreEqual(leftovers, new string[0]);
        }

        [Test]
        public void Map_WithShortOptionWithMinus_SetsPropertyToFalse()
        {
            var map = new PropertyMap(typeof (Container));
            var container = new Container();
            var arguments = new[] { "-t-" };

            string[] leftovers = map.Map(arguments, container);

            Assert.That(container.DefaultFalseProperty, Is.False);
            Assert.That(container.DefaultFalsePropertyWasSet, Is.True);
            CollectionAssert.AreEqual(leftovers, new string[0]);
        }

        [Test]
        public void Map_WithShortOptionWithPlus_SetsPropertyToTrue()
        {
            var map = new PropertyMap(typeof (Container));
            var container = new Container();
            var arguments = new[] { "-t+" };

            string[] leftovers = map.Map(arguments, container);

            Assert.That(container.DefaultFalseProperty, Is.True);
            Assert.That(container.DefaultFalsePropertyWasSet, Is.True);
            CollectionAssert.AreEqual(leftovers, new string[0]);
        }

        [Test]
        public void Map_WithShortOption_SetsProperty()
        {
            var map = new PropertyMap(typeof (Container));
            var container = new Container();
            var arguments = new[] { "-t" };

            string[] leftovers = map.Map(arguments, container);

            Assert.That(container.DefaultFalseProperty, Is.True);
            Assert.That(container.DefaultFalsePropertyWasSet, Is.True);
            CollectionAssert.AreEqual(leftovers, new string[0]);
        }

        [Test]
        public void Map_WithoutOption_LeaveOptionUnchanged()
        {
            var map = new PropertyMap(typeof (Container));
            var container = new Container();
            var arguments = new string[0];

            string[] leftovers = map.Map(arguments, container);

            Assert.That(container.DefaultFalseProperty, Is.False);
            Assert.That(container.DefaultFalsePropertyWasSet, Is.False);
            CollectionAssert.AreEqual(leftovers, new string[0]);
        }

        [Test]
        public void ValidateUsage_OnNonBooleanProperty_ThrowsInvalidOperationException()
        {
            PropertyInfo propertyInfo = typeof (IncorrectContainer).GetProperty("DummyProperty");
            var attr = new BooleanOptionAttribute("-t");

            Assert.Throws<InvalidOperationException>(() => attr.ValidateUsage(propertyInfo));
        }
    }
}