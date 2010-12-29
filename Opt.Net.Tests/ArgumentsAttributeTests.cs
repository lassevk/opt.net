using System;
using System.Collections.ObjectModel;
using System.Reflection;
using NUnit.Framework;

namespace Opt.Tests
{
    [TestFixture]
    public class ArgumentsAttributeTests
    {
        public class ContainerWithOneArgumentsAttributeProperty
        {
            [Arguments]
            public Collection<string> Arguments1
            {
                get;
                set;
            }
        }

        public class ContainerWithTwoArgumentsAttributeProperties : ContainerWithOneArgumentsAttributeProperty
        {
            [Arguments]
            public Collection<string> Arguments2
            {
                get;
                set;
            }
        }

        public class ContainerWithPropertyWithoutArgumentsAttribute
        {
            public Collection<string> Arguments1
            {
                get;
                set;
            }
        }

        public class ContainerWithNonCollectionArgumentsProperty
        {
            [Arguments]
            public int Arguments
            {
                get;
                set;
            }
        }

        public class ContainerWithInt32CollectionArgumentsProperty
        {
            [Arguments]
            public Collection<int> Arguments
            {
                get;
                set;
            }
        }

        public class ContainerWithStringCollectionArgumentsProperty
        {
            [Arguments]
            public Collection<string> Arguments
            {
                get;
                set;
            }
        }

        [Test]
        public void
            ValidateUsageWithContainerType_ContainerTypeWithNoPropertyWithArgumentsAttribute_ThrowsInvalidOperationException
            ()
        {
            Type containerType = typeof (ContainerWithNoProperties);
            var attr = new ArgumentsAttribute();

            Assert.Throws<InvalidOperationException>(() => attr.ValidateUsage(containerType));
        }

        [Test]
        public void ValidateUsageWithContainerType_ContainerTypeWithOnePropertyWithArgumentsAttribute_ValidatesOK()
        {
            Type containerType = typeof (ContainerWithOneArgumentsAttributeProperty);
            var attr = new ArgumentsAttribute();

            attr.ValidateUsage(containerType);
        }

        [Test]
        public void
            ValidateUsageWithContainerType_ContainerTypeWithTwoPropertiesWithArgumentsAttribute_ThrowsInvalidOperationException
            ()
        {
            Type containerType = typeof (ContainerWithTwoArgumentsAttributeProperties);
            var attr = new ArgumentsAttribute();

            Assert.Throws<InvalidOperationException>(() => attr.ValidateUsage(containerType));
        }

        [Test]
        public void ValidateUsageWithContainerType_NullType_ThrowsArgumentNullException()
        {
            Type containerType = null;
            var attr = new ArgumentsAttribute();

            Assert.Throws<ArgumentNullException>(() => attr.ValidateUsage(containerType));
        }

        [Test]
        public void ValidateUsageWithPropertyInfo_NullProperty_ThrowsArgumentNullException()
        {
            PropertyInfo property = null;
            var attr = new ArgumentsAttribute();

            Assert.Throws<ArgumentNullException>(() => attr.ValidateUsage(property));
        }

        [Test]
        public void
            ValidateUsageWithPropertyInfo_PropertyWithArgumentsAttributeButCollectionWithIncorrectElementType_ThrowsInvalidOperationException
            ()
        {
            PropertyInfo property = typeof (ContainerWithInt32CollectionArgumentsProperty).GetProperty("Arguments");
            var attr = new ArgumentsAttribute();

            Assert.Throws<InvalidOperationException>(() => attr.ValidateUsage(property));
        }

        [Test]
        public void
            ValidateUsageWithPropertyInfo_PropertyWithArgumentsAttributeButNotACollection_ThrowsInvalidOperationException
            ()
        {
            PropertyInfo property = typeof (ContainerWithNonCollectionArgumentsProperty).GetProperty("Arguments");
            var attr = new ArgumentsAttribute();

            Assert.Throws<InvalidOperationException>(() => attr.ValidateUsage(property));
        }

        [Test]
        public void ValidateUsageWithPropertyInfo_PropertyWithArgumentsAttributeOnStringCollectionProperty_ValidatesOK()
        {
            PropertyInfo property = typeof (ContainerWithStringCollectionArgumentsProperty).GetProperty("Arguments");
            var attr = new ArgumentsAttribute();

            attr.ValidateUsage(property);
        }

        [Test]
        public void ValidateUsageWithPropertyInfo_PropertyWithoutArgumentsAttribute_ThrowsInvalidOperationException()
        {
            PropertyInfo property = typeof (ContainerWithPropertyWithoutArgumentsAttribute).GetProperty("Arguments1");
            var attr = new ArgumentsAttribute();

            Assert.Throws<InvalidOperationException>(() => attr.ValidateUsage(property));
        }
    }
}