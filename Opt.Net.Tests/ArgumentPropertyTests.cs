using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

namespace Opt.Tests
{
    [TestFixture]
    public class ArgumentPropertyTests
    {
        public class OptionsWithArguments
        {
            private readonly Collection<string> _Args = new Collection<string>(new List<string>());
            private string _Arg1 = string.Empty;
            private string _Arg2 = string.Empty;

            [System.ComponentModel.Description("The input filename, the Tera-WURFL xml file")]
            [Argument(OrderIndex = 1, Name = "INPUT")]
            public string Arg1
            {
                get
                {
                    return _Arg1;
                }

                set
                {
                    _Arg1 = value;
                }
            }

            [System.ComponentModel.Description("The output filename")]
            [Argument(OrderIndex = 2, Name = "OUTPUT")]
            public string Arg2
            {
                get
                {
                    return _Arg2;
                }

                set
                {
                    _Arg2 = value;
                }
            }

            [Arguments]
            public Collection<string> Args
            {
                get
                {
                    return _Args;
                }
            }
        }

        [Test]
        public void FirstArgumentProperty_WhenOnlyOneArgumentIsPassed_IsFilledWithArgument()
        {
            var options = OptParser.Parse<OptionsWithArguments>(new[] { "arg1" });

            Assert.That(options.Arg1, Is.EqualTo("arg1"));
        }

        [Test]
        public void SecondArgumentProperty_WhenOnlyOneArgumentIsPassed_IsNotFilledWithArgument()
        {
            var options = OptParser.Parse<OptionsWithArguments>(new[] { "arg1" });

            Assert.That(options.Arg2, Is.EqualTo(string.Empty));
        }

        [Test]
        public void FirstArgumentProperty_WhenTwoArgumentsArePassed_IsFilledWithArgument()
        {
            var options = OptParser.Parse<OptionsWithArguments>(new[] { "arg1", "arg2" });

            Assert.That(options.Arg1, Is.EqualTo("arg1"));
        }

        [Test]
        public void SecondArgumentProperty_WhenTwoArgumentsArePassed_IsFilledWithArgument()
        {
            var options = OptParser.Parse<OptionsWithArguments>(new[] { "arg1", "arg2" });

            Assert.That(options.Arg2, Is.EqualTo("arg2"));
        }

        [Test]
        public void ArgumentsProperty_GetsLeftOverArguments()
        {
            var options = OptParser.Parse<OptionsWithArguments>(new[] { "arg1", "arg2", "arg3", "arg4" });

            CollectionAssert.AreEqual(options.Args, new[] { "arg3", "arg4" });
        }
    }
}