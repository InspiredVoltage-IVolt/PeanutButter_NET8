﻿using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using static PeanutButter.Utils.PyLike;
using static PeanutButter.RandomGenerators.RandomValueGen;
using NExpect;
using static NExpect.Expectations;

// ReSharper disable ExpressionIsAlwaysNull
// ReSharper disable RedundantArgumentDefaultValue

namespace PeanutButter.Utils.Tests
{
    [TestFixture]
    public class TestPyLike
    {
        [TestFixture]
        public class Range
        {
            [Test]
            public void GivenCeilingOf0_ShouldYieldNoResults()
            {
                // Arrange
                // Pre-Assert
                // Act
                var result = Range(0).ToArray();
                // Assert
                Expect(result).To.Be.Empty();
            }

            [Test]
            public void GivenCeilingOf1_ShouldReturnCollectionWithOnlyZero()
            {
                // Arrange
                // Pre-Assert
                // Act
                var result = Range(1).ToArray();
                // Assert
                Expect(result).To.Equal(new[] { 0 });
            }

            [Test]
            public void GivenCeilingOfSomeNumber_ShouldReturnCollectionWithNumbersFromZeroToJustBelowCeiling()
            {
                // Arrange
                var ceiling = GetRandomInt(6, 12);
                var expected = new List<int>();
                for (var i = 0; i < ceiling; i++)
                {
                    expected.Add(i);
                }

                // Pre-Assert
                // Act
                var result = Range(ceiling).ToArray();
                // Assert
                Expect(result).To.Equal(expected);
            }

            [Test]
            public void
                GivenStartAndCeiling_WhenCeilingGreaterThanStart_ShouldResultInRangeFromStartToJustUnderCeiling()
            {
                // Arrange
                var start = GetRandomInt(5, 8);
                var ceiling = GetRandomInt(12, 24);
                var expected = new List<int>();
                for (var i = start; i < ceiling; i++)
                {
                    expected.Add(i);
                }

                // Pre-Assert
                // Act
                var result = Range(start, ceiling).ToArray();
                // Assert
                Expect(result).To.Equal(expected);
            }

            [Test]
            public void GivenStartAndCeilingAndStep_ShouldProduceSteppedResult()
            {
                // Arrange
                var start = GetRandomInt(5, 18);
                var ceiling = GetRandomInt(22, 34);
                var step = GetRandomInt(2, 3);
                var expected = new List<int>();
                for (var i = start; i < ceiling; i += step)
                {
                    expected.Add(i);
                }

                // Pre-Assert
                // Act
                var result = Range(start, ceiling, step).ToArray();
                // Assert
                Expect(result).To.Equal(expected);
            }

            [Test]
            public void GivenNegativeProgression_ShouldReturnEmptyCollection()
            {
                // Arrange
                var start = GetRandomInt(5, 10);
                var ceiling = GetRandomInt(-5, 2);
                // Pre-Assert
                // Act
                var result = Range(start, ceiling).ToArray();
                // Assert
                Expect(result).To.Be.Empty();
            }
        }

        [TestFixture]
        public class Zip
        {
            [TestFixture]
            public class WhenCollectionsHaveTheSameNumberOfItems
            {
                [TestFixture]
                public class TwoCollections
                {
                    [Test]
                    public void ShouldReturnItemPairs()
                    {
                        // Arrange
                        var left = new[] { 1, 2, 3 };
                        var right = new[] { "a", "b", "c" };
                        // Pre-Assert
                        // Act
                        var result = Zip(left, right).ToArray();
                        // Assert
                        Expect(result).To.Contain.Exactly(3).Items();
                        Expect(result)
                            .To.Equal(
                                new[]
                                {
                                    Tuple.Create(left[0], right[0]),
                                    Tuple.Create(left[1], right[1]),
                                    Tuple.Create(left[2], right[2])
                                });
                    }
                }

                [TestFixture]
                public class ThreeCollections
                {
                    [Test]
                    public void ShouldReturnItemPairs()
                    {
                        // Arrange
                        var left = new[] { 1, 2, 3 };
                        var middle = GetRandomCollection<bool>(3, 3).ToArray();
                        var right = new[] { "a", "b", "c" };
                        // Pre-Assert
                        // Act
                        var result = Zip(left, middle, right).ToArray();
                        // Assert
                        Expect(result).To.Contain.Exactly(3).Items();
                        Expect(result)
                            .To.Equal(new[]
                            {
                                Tuple.Create(left[0], middle[0], right[0]),
                                Tuple.Create(left[1], middle[1], right[1]),
                                Tuple.Create(left[2], middle[2], right[2])
                            });
                    }
                }

                [TestFixture]
                public class FourCollections
                {
                    [Test]
                    public void ShouldReturnItemPairs()
                    {
                        // Arrange
                        var first = new[] { 1, 2, 3 };
                        var second = GetRandomArray<bool>(3, 3);
                        var third = new[] { "a", "b", "c" };
                        var fourth = GetRandomArray<DateTime>(10);
                        // Pre-Assert
                        // Act
                        var result = Zip(first, second, third, fourth).ToArray();
                        // Assert
                        Expect(result).To.Contain.Exactly(3).Items();
                        Expect(result)
                            .To.Equal(new[]
                            {
                                Tuple.Create(first[0], second[0], third[0], fourth[0]),
                                Tuple.Create(first[1], second[1], third[1], fourth[1]),
                                Tuple.Create(first[2], second[2], third[2], fourth[2])
                            });
                    }
                }
            }

            [TestFixture]
            public class WhenCollectionsHaveDifferentNumberOfItems
            {
                [TestFixture]
                public class TwoCollections
                {
                    [Test]
                    public void ShouldOnlyReturnFullPairs()
                    {
                        // Arrange
                        var left = new[] { 1, 2 };
                        var right = new[] { "a", "b", "c" };
                        // Pre-Assert
                        // Act
                        var result = Zip(left, right).ToArray();
                        // Assert
                        Expect(result).To.Contain.Exactly(2).Items();
                        Expect(result)
                            .To.Equal(new[]
                            {
                                Tuple.Create(left[0], right[0]),
                                Tuple.Create(left[1], right[1])
                            });
                    }
                }

                [TestFixture]
                public class ThreeCollections
                {
                    [Test]
                    public void ShouldOnlyReturnFullPairs()
                    {
                        // Arrange
                        var left = new[] { 1, 2 };
                        var middle = GetRandomCollection<bool>(5, 7).ToArray();
                        var right = new[] { "a", "b", "c" };
                        // Pre-Assert
                        // Act
                        var result = Zip(left, middle, right).ToArray();
                        // Assert
                        Expect(result).To.Contain.Exactly(2).Items();
                        Expect(result)
                            .To.Equal(new[]
                            {
                                Tuple.Create(left[0], middle[0], right[0]),
                                Tuple.Create(left[1], middle[1], right[1])
                            });
                    }
                }

                [TestFixture]
                public class FourCollections
                {
                    [Test]
                    public void ShouldOnlyReturnFullPairs()
                    {
                        // Arrange
                        var left = new[] { 1, 2 };
                        var middle = GetRandomArray<bool>(5, 7);
                        var right = new[] { "a", "b", "c" };
                        var last = GetRandomArray<DateTime>(10);
                        // Pre-Assert
                        // Act
                        var result = Zip(left, middle, right, last).ToArray();
                        // Assert
                        Expect(result).To.Contain.Exactly(2).Items();
                        Expect(result)
                            .To.Equal(new[]
                            {
                                Tuple.Create(left[0], middle[0], right[0], last[0]),
                                Tuple.Create(left[1], middle[1], right[1], last[1])
                            });
                    }
                }
            }

            [TestFixture]
            public class WhenOneOrMoreCollectionsAreNull
            {
                [TestFixture]
                public class TwoCollections
                {
                    [Test]
                    public void ShouldReturnEmptyCollection()
                    {
                        // Arrange
                        var left = new[] { 1, 2 };
                        int[] right = null;
                        // Pre-Assert
                        // Act
                        var result = Zip(left, right).ToArray();
                        // Assert
                        Expect(result).To.Be.Empty();
                    }
                }

                [TestFixture]
                public class ThreeCollections
                {
                    [Test]
                    public void ShouldReturnEmptyCollection()
                    {
                        // Arrange
                        var left = new[] { 1, 2 };
                        bool[] middle = null;
                        int[] right = null;
                        // Pre-Assert
                        // Act
                        var result = Zip(left, middle, right).ToArray();
                        // Assert
                        Expect(result).To.Be.Empty();
                    }
                }

                [TestFixture]
                public class FourCollections
                {
                    [Test]
                    public void ShouldReturnEmptyCollection()
                    {
                        // Arrange
                        var left = new[] { 1, 2 };
                        bool[] middle = null;
                        int[] right = null;
                        // Pre-Assert
                        // Act
                        var result = Zip(left, middle, right, GetRandomArray<DateTime>()).ToArray();
                        // Assert
                        Expect(result).To.Be.Empty();
                    }
                }
            }
        }
    }
}