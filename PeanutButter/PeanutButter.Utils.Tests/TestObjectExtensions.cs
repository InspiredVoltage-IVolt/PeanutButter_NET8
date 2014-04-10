﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using PeanutButter.RandomGenerators;
using PeanutButter.Utils;

namespace PenautButter.Utils.Tests
{
    [TestFixture]
    public class TestObjectExtensions
    {
        [Test]
        public void AllPropertiesMatch_GivenSourceWithNoPropsAndDestWithNoProps_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var result = (new object()).AllPropertiesMatch(new object());

            //---------------Test Result -----------------------
            Assert.IsTrue(result);
        }

        [Test]
        public void AllPropertiesMatch_GivenTwoObjectsBothWithTheSamePropertyNameAndValue_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var randomString = RandomValueGen.GetRandomString();
            var result = (new { prop = randomString }).AllPropertiesMatch(new { prop = randomString });

            //---------------Test Result -----------------------
            Assert.IsTrue(result);
        }

        [Test]
        public void AllPropertiesMatch_WhenDestinationHasMorePropertiesButSameNamedOnesMatch_ReturnsTrue()
        {
            //---------------Set up test pack-------------------
            var rs = RandomValueGen.GetRandomString();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var result = (new { prop = rs }).AllPropertiesMatch(new { prop = rs, bar = RandomValueGen.GetRandomString() });

            //---------------Test Result -----------------------
            Assert.IsTrue(result);
        }

        [Test]
        public void AllPropertiesMatch_WhenDestinationIsMissingProperty_ReturnsFalse()
        {
            //---------------Set up test pack-------------------
            
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var result = (new { prop = RandomValueGen.GetRandomString() }).AllPropertiesMatch(new object());

            //---------------Test Result -----------------------
            Assert.IsFalse(result);
        }

        [Test]
        public void AllPropertiesMatch_WhenStringPropertyDoesntMatch_ReturnsFalse()
        {
            //---------------Set up test pack-------------------
            var propVal = RandomValueGen.GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var result = (new { prop = propVal }).AllPropertiesMatch(new { prop = propVal + RandomValueGen.GetRandomString(1, 10) });

            //---------------Test Result -----------------------
            Assert.IsFalse(result);
        }

        [Test]
        public void AllPropertiesMatch_WhenComparingIdenticalPropertiesOfSimpleTypes_ShouldReturnTrue()
        {
            TestPropertyMatchingFor<int>();
            TestPropertyMatchingFor<long>();
            TestPropertyMatchingFor<byte>();
            TestPropertyMatchingFor<char>();
            TestPropertyMatchingFor<float>();
            TestPropertyMatchingFor<double>();
            TestPropertyMatchingFor<decimal>();
            TestPropertyMatchingFor<string>();
            TestPropertyMatchingFor<DateTime>();
            TestPropertyMatchingFor<int?>();
            TestPropertyMatchingFor<long?>();
            TestPropertyMatchingFor<byte?>();
            TestPropertyMatchingFor<char?>();
            TestPropertyMatchingFor<float?>();
            TestPropertyMatchingFor<double?>();
            TestPropertyMatchingFor<decimal?>();
            TestPropertyMatchingFor<DateTime?>();
        }

        private static void TestPropertyMatchingFor<T>()
        {
//---------------Set up test pack-------------------
            var propVal = (T)RandomValueGen.GetRandomValue<T>();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var result = (new {prop = propVal}).AllPropertiesMatch(new {prop = propVal});

            //---------------Test Result -----------------------
            Assert.IsTrue(result);
        }

        [Test]
        public void AllPropertiesMatch_ComplexTypesAreTraversed_HappyCase()
        {
            //---------------Set up test pack-------------------
            var propVal = RandomValueGen.GetRandomString();
            var o1 = new
            {
                prop = new {
                    bar = propVal
                }
            };
            var o2 = new
            {
                prop = new {
                    bar = propVal
                }
            };

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var result = o1.AllPropertiesMatch(o2);

            //---------------Test Result -----------------------
            Assert.IsTrue(result);
        }
        [Test]
        public void AllPropertiesMatch_ComplexTypesAreTraversed_UnhappyCase()
        {
            //---------------Set up test pack-------------------
            var o1 = new
            {
                prop = new {
                    bar = RandomValueGen.GetRandomString(1, 10)
                }
            };
            var o2 = new
            {
                prop = new {
                    bar = RandomValueGen.GetRandomString(11, 20)
                }
            };

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var result = o1.AllPropertiesMatch(o2);

            //---------------Test Result -----------------------
            Assert.IsFalse(result);
        }

        [Test]
        public void CopyPropertiesTo_GivenSimpleObjectDest_DoesNotThrow()
        {
            //---------------Set up test pack-------------------
            var src = new
            {
                prop = RandomValueGen.GetRandomString()
            };
            
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            Assert.DoesNotThrow(() => src.CopyPropertiesTo(new object()));

            //---------------Test Result -----------------------
        }

        public class Simple<T>
        {
            public T prop { get; set; }
            public Simple()
            {
                Randomize();
            }

            public void Randomize()
            {
                prop = (T) RandomValueGen.GetRandomValue<T>();
            }
        }

        [Test]
        public void CopyPropertiesTo_GivenDestWithSameProperty_CopiesValue()
        {
            TestCopyFor<string>();
            TestCopyFor<int>();
            TestCopyFor<byte>();
            TestCopyFor<char>();
            TestCopyFor<long>();
            TestCopyFor<float>();
            TestCopyFor<double>();
            TestCopyFor<decimal>();
            TestCopyFor<DateTime>();
            TestCopyFor<bool>();
        }

        private static void TestCopyFor<T>()
        {
//---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var src = new Simple<T>();
            var dst = new Simple<T>();
            while (dst.prop.Equals(src.prop))
                dst.Randomize();
            src.CopyPropertiesTo(dst);

            //---------------Test Result -----------------------
            Assert.AreEqual(src.prop, dst.prop);
        }

        public class Complex<T>
        {
            public Simple<T> prop { get; set; }
            public Complex()
            {
                prop = new Simple<T>();
            }
        }

        [Test]
        public void CopyPropertiesTo_ComplexTypesAreTraversedButOnlySimplePropertiesAreCopied()
        {
            //---------------Set up test pack-------------------
            var src = new Complex<int>();
            var dst = new Complex<int>();
            while (dst.prop.prop.Equals(src.prop.prop))
                dst.prop.Randomize();

            //---------------Assert Precondition----------------
            Assert.AreNotEqual(src, dst);
            Assert.AreNotEqual(src.prop, dst.prop);
            Assert.AreNotEqual(src.prop.prop, dst.prop.prop);
            //---------------Execute Test ----------------------
            src.CopyPropertiesTo(dst);

            //---------------Test Result -----------------------
            Assert.AreNotEqual(src.prop, dst.prop);
            Assert.AreEqual(src.prop.prop, dst.prop.prop);
        }
        [Test]
        public void CopyPropertiesTo_WhenDeepIsFalse_ComplexTypesAreTraversedAndRefCopied()
        {
            //---------------Set up test pack-------------------
            var src = new Complex<int>();
            var dst = new Complex<int>();
            while (dst.prop.prop.Equals(src.prop.prop))
                dst.prop.Randomize();

            //---------------Assert Precondition----------------
            Assert.AreNotEqual(src, dst);
            Assert.AreNotEqual(src.prop, dst.prop);
            Assert.AreNotEqual(src.prop.prop, dst.prop.prop);
            //---------------Execute Test ----------------------
            src.CopyPropertiesTo(dst, false);

            //---------------Test Result -----------------------
            Assert.AreEqual(src.prop, dst.prop);
            Assert.AreEqual(src.prop.prop, dst.prop.prop);
        }


        [Test]
        public void CopyPropertiesTo_DoesntBarfOnANullTargetThatIsComplex()
        {
            //---------------Set up test pack-------------------
            var o1 = new Complex<string>();
            var o2 = new Complex<string>();
            o2.prop = null;
            
            //---------------Assert Precondition----------------
            Assert.IsNull(o2.prop);

            //---------------Execute Test ----------------------
            Assert.DoesNotThrow(() => o1.CopyPropertiesTo(o2));

            //---------------Test Result -----------------------
        }
        [Test]
        public void CopyPropertiesTo_DoesntBarfOnANullSourceThatIsComplex()
        {
            //---------------Set up test pack-------------------
            var o1 = new Complex<string>();
            var o2 = new Complex<string>();
            o1.prop = null;
            
            //---------------Assert Precondition----------------
            Assert.IsNull(o1.prop);

            //---------------Execute Test ----------------------
            Assert.DoesNotThrow(() => o1.CopyPropertiesTo(o2));

            //---------------Test Result -----------------------
            Assert.IsNull(o2.prop);
        }
    }
}
