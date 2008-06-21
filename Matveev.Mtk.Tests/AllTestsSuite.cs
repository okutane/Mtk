using System;
using System.Collections.Generic;

using NUnit.Core;
using NUnit.Framework;

using Matveev.Common;
using Matveev.Mtk.Core;
using Matveev.Mtk.Library;
using Matveev.Mtk.Library.Fields;
using System.Collections;

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
                suite.Add(new YamlSerializerTest());

                suite.Add(PolygonizationSuite.Suite);
                return suite;
            }
        }
    }
}
