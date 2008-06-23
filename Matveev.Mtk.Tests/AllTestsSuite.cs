using System;
using System.Collections.Generic;

using NUnit.Core;
using NUnit.Framework;

using Matveev.Common;
using Matveev.Mtk.Core;
using Matveev.Mtk.Library;
using Matveev.Mtk.Library.Fields;
using System.Reflection;

namespace Matveev.Mtk.Tests
{
    public class AllTestsSuite
    {
        [Suite]
        public static TestSuite Suite
        {
            get
            {
                TestSuite suite = new TestSuite("All tests");

                Assembly thatAssembly = Assembly.GetExecutingAssembly();
                foreach (var type in thatAssembly.GetTypes())
                {
                    object[] attributes = type.GetCustomAttributes(typeof(TestFixtureAttribute), true);
                    if (attributes.Length != 0)
                        suite.Add(type.GetConstructor(System.Type.EmptyTypes).Invoke(null));
                }

                suite.Add(PolygonizationSuite.Suite);
                return suite;
            }
        }
    }
}
