using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using NUnit.Framework;

using Matveev.Common;
using System.Runtime.Serialization;

namespace Matveev.Mtk.Tests
{
    [TestFixture]
    [Serializable]
    public class YamlSerializerTest
    {
        private static IFormatter formatter = new YamlFormatter();

        [Test]
        public void TestSerialize()
        {
            string actual = "YamlTest.yaml";
            YamlTestType obj = YamlTestType.GetInstance();

            TestSerialize(actual, obj);
        }

        [Test]
        public void ArrayOfInts()
        {
            TestSerialize("ArrayOfInts.yaml", new int[] { 3, 2, 1, 6, 7 });
        }

        public static void TestSerialize(string actual, object obj)
        {
            string expected = Path.Combine("../../YamlExpected", actual);
            actual = Path.Combine("YamlActual", actual);
            using (Stream stream = new FileStream(actual, FileMode.Create))
                formatter.Serialize(stream, obj);
            if (File.Exists(expected))
                FileAssert.AreEqual(expected, actual);
            else
                File.Copy(actual, expected);
        }

        [Test]
        public void TestDeserialize()
        {
            YamlTestType expected = YamlTestType.GetInstance();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, YamlTestType.GetInstance());
            YamlTestType actual = (YamlTestType)formatter.Deserialize(stream);
            Assert.AreEqual(expected, actual);
        }

        private class YamlTestType
        {
            private double _privateDouble = 5;

            public string ScalarString
            {
                get;
                set;
            }

            public double ScalarDouble
            {
                get;
                set;
            }

            public int[] IntegerArray
            {
                get;
                set;
            }

            public static YamlTestType GetInstance()
            {
                return new YamlTestType
                {
                    ScalarString = "test string",
                    ScalarDouble = 1.5,
                    IntegerArray = new int[] { 3, 2, 1, 6, 7 }
                };
            }
        }
    }
}
