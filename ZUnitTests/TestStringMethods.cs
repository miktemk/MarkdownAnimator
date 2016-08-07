using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZUnitTests
{
    [TestClass]
    public class TestStringMethods
    {
        [TestMethod]
        public void TestTransformWithCallback()
        {
            var sIn = "abc x  x 1blah x2blah x 3blahblah. Hello   x    x  4this is where its tested";
            var sOut = "abc 1blah 2blah 3blahblah. Hello 4this is where its tested";
            var sIndexes = new[] {
                sOut.IndexOf('1'),
                sOut.IndexOf('2'),
                sOut.IndexOf('3'),
                sOut.IndexOf('4'),
            };


        }
    }
}
