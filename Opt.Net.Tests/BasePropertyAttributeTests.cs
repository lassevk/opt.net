using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Opt.Tests
{
    [TestFixture]
    public class BasePropertyAttributeTests
    {
        internal class OptionAttribute : BaseOptionAttribute
        {
            public OptionAttribute(string option, string parameterName)
                : base(option, parameterName)
            {
            }

            public override bool RequiresArgument
            {
                get
                {
                    return false;
                }
            }

            public override void ValidateUsage(Type containerType)
            {
            }

            public override void ValidateUsage(PropertyInfo propertyInfo)
            {
            }

            public override void AssignValueToProperty(object container, PropertyInfo propertyInfo, string value)
            {
            }
        }

        public IEnumerable<Type> PropertyTypes
        {
            get
            {
                return from type in typeof(BasePropertyAttribute).Assembly.GetTypes()
                       where !type.IsAbstract && typeof(BasePropertyAttribute).IsAssignableFrom(type)
                       select type;
            }
        }

        [TestCaseSource("PropertyTypes")]
        public void ValidateUsageWithTypeOnAllPropertyAttributes_WithNullType_ThrowsArgumentNullException(Type attributeType)
        {
            BasePropertyAttribute attribute;
            if (typeof(BaseOptionAttribute).IsAssignableFrom(attributeType))
                attribute = Activator.CreateInstance(attributeType, "-t") as BasePropertyAttribute;
            else
                attribute = Activator.CreateInstance(attributeType) as BasePropertyAttribute;

            Type type = null;
            Assert.Throws<ArgumentNullException>(() => attribute.ValidateUsage(type));
        }

        [TestCaseSource("PropertyTypes")]
        public void ValidateUsageWithPropertyInfoOnAllPropertyAttributes_WithNullPropertyInfo_ThrowsArgumentNullException(Type attributeType)
        {
            BasePropertyAttribute attribute;
            if (typeof(BaseOptionAttribute).IsAssignableFrom(attributeType))
                attribute = Activator.CreateInstance(attributeType, "-t") as BasePropertyAttribute;
            else
                attribute = Activator.CreateInstance(attributeType) as BasePropertyAttribute;

            PropertyInfo propertyInfo = null;
            Assert.Throws<ArgumentNullException>(() => attribute.ValidateUsage(propertyInfo));
        }

        [TestCase((string)null)]
        [TestCase("")]
        [TestCase(" \t\n\r")]
        public void Constructor_NullOrEmptyOption_ThrowsArgumentNullException(string input)
        {
            Assert.Throws<ArgumentNullException>(() => new OptionAttribute(input, string.Empty));
        }

        [TestCase("--l")]
        [TestCase("-short")]
        [TestCase("long")]
        public void Constructor_IncorrectOptions_ThrowsArgumentException(string input)
        {
            Assert.Throws<ArgumentException>(() => new OptionAttribute(input, string.Empty));
        }

        [TestCase("-s")]
        [TestCase("--long")]
        public void OptionProperty_AfterConstructorWithProperOptionAndParameterNames_ContainsSpecifiedOption(string input)
        {
            var attr = new OptionAttribute(input, string.Empty);

            Assert.That(attr.Option, Is.EqualTo(input));
        }

        [TestCase("VALUE")]
        [TestCase("NAME")]
        [TestCase("")]
        public void ParameterNameProperty_AfterConstructorWithProperOptionAndParameterNames_ContainsSpecifiedOption(string input)
        {
            var attr = new OptionAttribute("-t", input);

            Assert.That(attr.ParameterName, Is.EqualTo(input));
        }

        [Test]
        public void Constructor_NullOrEmptyOption_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new OptionAttribute("-t", null));
        }
    }
}