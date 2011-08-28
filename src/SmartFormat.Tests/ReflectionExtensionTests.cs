﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace SmartFormat.Tests
{
    [TestFixture]
    public class ReflectionExtensionTests
    {
        public object[] GetArgs()
        {
            return new object[] {
                "Zero",
                111,
                new DateTime(2222,2,2,2,2,2),
                new { Value = 3, Anon = true },
                TestFactory.GetPerson(),
            };
        }

        [Test]
        public void Test_Properties()
        {
            var formats = new string[] {
                "{0} {0.Length} {Length}",
                "{2.Year} {2.Month:00}-{2.Day:00}",
                "{3.Value} {3.Anon}",
                "Chained: {4.FirstName} {4.FirstName.Length} {4.Address.City} {4.Address.State}  ",
                "Nested: {4:{FirstName:{} {Length} }{Address:{City} {State} } }", // Due to double-brace escaping, the spacing in this nested format is irregular
            };
            var expected = new string[] {
                "Zero 4 4",
                "2222 02-02",
                "3 True",
                "Chained: Michael 7 Scranton Pennsylvania  ",
                "Nested: Michael 7 Scranton Pennsylvania  ",
            };
            var args = GetArgs();
            Smart.Default.Test(formats, args, expected);
        }

        [Test]
        public void Test_Methods()
        {
            var formats = new string[] {
                "{0} {0.ToLower} {ToLower} {ToUpper}",
            };
            var expected = new string[] {
                "Zero zero zero ZERO",
            };
            var args = GetArgs();
            Smart.Default.Test(formats, args, expected);
        }

        [Test]
        public void Test_Fields()
        {
            var formats = new string[] {
                "{Field}",
            };
            var expected = new string[] {
                "Field",
            };
            var args = new object[] {
                new MiscObject(),
            };
            Smart.Default.Test(formats, args, expected);
        }

        public class MiscObject
        {
            public MiscObject()
            {
                Field = "Field";
                ReadonlyProperty = "ReadonlyProperty";
                Property = "Property";
            }
            public string Field;
            public string ReadonlyProperty { get; private set; }
            public string Property { get; set; }
            public string Method()
            {
                return "Method";
            }
        }
    }
}