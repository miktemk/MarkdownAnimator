using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miktemk;
using System.Collections.Generic;

namespace ZUnitTests
{
    [TestClass]
    public class TestLinqSplitEnumerable
    {
        char[] input = new[] { 'a', 'b', 'c', '1', 'd', 'e', 'f', '1', 'x', 'y', 'z' };
        char[][] outputEnding = new[]
        {
            new [] { 'a', 'b', 'c', '1' },
            new [] { 'd', 'e', 'f', '1' },
            new [] { 'x', 'y', 'z' }
        };
        char[][] outputStaring = new[]
        {
            new [] { 'a', 'b', 'c' },
            new [] { '1', 'd', 'e', 'f' },
            new [] { '1', 'x', 'y', 'z' }
        };

        [TestMethod]
        public void TestSplitEnumerable()
        {
            var result = input.SplitEnumerableEndingWith(x => x == '1');
            AssertEnumerablesAreEqual(outputEnding, result, (subExp, subAct) =>
            {
                AssertEnumerablesAreEqual(subExp, subAct, (e, a) => e == a);
                return true;
            });
        }
        [TestMethod]
        public void TestSplitEnumerable2()
        {
            var result = input.SplitEnumerableStartingWith(x => x == '1');
            AssertEnumerablesAreEqual(outputStaring, result, (subExp, subAct) =>
            {
                AssertEnumerablesAreEqual(subExp, subAct, (e, a) => e == a);
                return true;
            });
        }


        private void AssertEnumerablesAreEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual, Func<T,T,bool> compareElements)
        {
            var arrAct = actual.ToArray();
            Assert.AreEqual(expected.Count(), actual.Count());
            expected.EnumerateWith(actual, (exp, act, index) => {
                Assert.AreEqual(true, compareElements(exp, act));
            });
        }
    }
}
