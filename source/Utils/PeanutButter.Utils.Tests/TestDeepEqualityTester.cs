﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using NExpect;
using NUnit.Framework;
using PeanutButter.RandomGenerators;
using PeanutButter.TestUtils.Generic;
using static PeanutButter.RandomGenerators.RandomValueGen;
using static NExpect.Expectations;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable MemberHidesStaticFromOuterClass
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace PeanutButter.Utils.Tests
{
    [TestFixture]
    public class TestDeepEqualityTester
    {
        [TestFixture]
        public class AreDeepEqual
        {
            // mostly this class is tested through the DeepEquals()
            //  extension method testing. However, I'd like to allow
            //  for a slower operation where discrepencies are recorded
            [Test]
            public void GivenTwoEqualPrimitives_ShouldNotPopulateErrors()
            {
                //--------------- Arrange -------------------
                var sut = Create(1, 1);

                //--------------- Assume ----------------
                Expect(sut.Errors).To.Be.Empty();

                //--------------- Act ----------------------
                Expect(sut.AreDeepEqual()).To.Be.True();

                //--------------- Assert -----------------------
                Expect(sut.Errors).To.Be.Empty();
            }

            [Test]
            public void GivenTwoDifferentPrimitives_ShouldSetExpectedError()
            {
                //--------------- Arrange -------------------
                var sut = Create(true, false);
                //--------------- Assume ----------------

                //--------------- Act ----------------------
                Expect(sut.AreDeepEqual()).To.Be.False();

                //--------------- Assert -----------------------
                Expect(sut.Errors).To.Contain("Primitive values differ");
            }

            [Test]
            public void GivenDifferingComplexObjectsWithOnePropertyOfSameNameAndValue_ShouldRecordError()
            {
                //--------------- Arrange -------------------
                var item1 = new { foo = 1 };
                var item2 = new { foo = 2 };
                var sut = Create(item1, item2);

                //--------------- Assume ----------------

                //--------------- Act ----------------------
                Expect(sut.AreDeepEqual()).To.Be.False();

                //--------------- Assert -----------------------
                var error = sut.Errors.Last();
                Expect(error).To.Contain("foo");
                Expect(error).To.Contain("1");
                Expect(error).To.Contain("2");
            }

            [Test]
            public void WhenBothItemsHaveCollections_ShouldCompareThem_Positive()
            {
                //--------------- Arrange -------------------
                var item1 = new
                {
                    Subs = new[] { 1, 2, }
                };
                var item2 = new ThingWithCollection()
                {
                    Subs = new[] { 1, 2 }
                };
                var sut = Create(item1, item2);

                //--------------- Assume ----------------

                //--------------- Act ----------------------
                Expect(sut.AreDeepEqual()).To.Be.True();

                //--------------- Assert -----------------------
            }

            [Test]
            public void WhenBothItemsHaveCollections_ShouldCompareThem_WithoutCaringAboutOrder()
            {
                //--------------- Arrange -------------------
                var item1 = new ThingWithCollection()
                {
                    Subs = new[] { 2, 1, }
                };
                var item2 = new
                {
                    Subs = new[] { 1, 2 }
                };
                var sut = Create(item1, item2);

                //--------------- Assume ----------------

                //--------------- Act ----------------------
                Expect(sut.AreDeepEqual()).To.Be.True();

                //--------------- Assert -----------------------
            }

            [Test]
            public void WhenBothItemsHaveCollections_ShouldCompareThem_Negative()
            {
                //--------------- Arrange -------------------
                var item1 = new ThingWithCollection()
                {
                    Subs = new[] { 1, 2, 3 }
                };
                var item2 = new ThingWithCollection()
                {
                    Subs = new[] { 1, 2 }
                };
                var sut = Create(item1, item2);

                //--------------- Assume ----------------

                //--------------- Act ----------------------
                Expect(sut.AreDeepEqual()).To.Be.False();

                //--------------- Assert -----------------------
            }

            [Test]
            public void WhenBothItemsHaveCollections_ButOneIsNull_ShouldNotBarf()
            {
                //--------------- Arrange -------------------
                var item1 = new ThingWithCollection()
                {
                    Subs = new[] { 1, 2, 3 }
                };
                var item2 = new ThingWithCollection();
                var sut = Create(item1, item2);

                //--------------- Assume ----------------
                Expect(item2.Subs).To.Be.Null();

                //--------------- Act ----------------------
                Expect(sut.AreDeepEqual()).To.Be.False();

                //--------------- Assert -----------------------
            }

            [Test]
            public void WhenBothItemsHaveCollections_ButOneIsNull_ShouldNotBarfReversed()
            {
                //--------------- Arrange -------------------
                var item1 = new ThingWithCollection();
                var item2 = new ThingWithCollection()
                {
                    Subs = new[] { 1, 2, 3 }
                };
                var sut = Create(item1, item2);

                //--------------- Assume ----------------
                Expect(item1.Subs).To.Be.Null();

                //--------------- Act ----------------------
                Expect(sut.AreDeepEqual()).To.Be.False();

                //--------------- Assert -----------------------
            }

            [Test]
            public void WhenBothItemsHaveCollections_ButBothAreNull_ShouldBehaveAccordingly()
            {
                //--------------- Arrange -------------------
                var item1 = new ThingWithCollection();
                var item2 = new ThingWithCollection();
                var sut = Create(item1, item2);

                //--------------- Assume ----------------
                Expect(item1.Subs).To.Be.Null();
                Expect(item2.Subs).To.Be.Null();

                //--------------- Act ----------------------
                Expect(sut.AreDeepEqual()).To.Be.True();

                //--------------- Assert -----------------------
            }

            [TestFixture]
            public class WithOnlyCompareShapeTrue
            {
                [TestFixture]
                public class DeepSubEquals
                {
                    [Test]
                    public void GivenObjectWithMoreThanOther()
                    {
                        // Arrange
                        var left = new
                        {
                            Id = 2
                        };
                        var right = new
                        {
                            Id = 1,
                            Name = "Moo"
                        };
                        var sut = Create(left, right);
                        sut.FailOnMissingProperties = false;
                        sut.OnlyCompareShape = true;
                        // Pre-Assert
                        // Act
                        Expect(sut.AreDeepEqual()).To.Be.True(sut.PrintErrors());
                        // Assert
                    }

                    public class OurVersion
                    {
                        public int Major { get; set; }
                        public int Minor { get; set; }
                    }

                    public class TheirVersion
                    {
                        public int Major { get; set; }
                        public int Minor { get; set; }
                    }

                    public class OurObject
                    {
                        public OurVersion Version { get; set; }
                    }

                    public class TheirObject
                    {
                        public TheirVersion Version { get; set; }
                    }

                    [Test]
                    public void GivenObjectsWithDifferentTypedButSameShapedProps()
                    {
                        // Arrange
                        var mine = GetRandom<OurObject>();
                        var theirs = GetRandom<TheirObject>();
                        var tester = new DeepEqualityTester(mine, theirs)
                        {
                            OnlyCompareShape = true,
                            RecordErrors = true
                        };
                        // Pre-Assert
                        // Act
                        var result = tester.AreDeepEqual();
                        // Assert
                        Expect(result)
                            .To.Be.True(
                                tester.Errors.JoinWith("\n")
                            );
                    }

                    public class OurSemVer
                    {
                        public int Major { get; set; }
                        public int Minor { get; set; }
                        public int Patch { get; set; }
                    }

                    public class OurObjectWithSemVer
                    {
                        public Guid Id { get; set; }
                        public OurSemVer Version { get; set; }
                    }

                    [Test]
                    public void GivenObjectsWithDifferentTypedButSameSubShapedProps()
                    {
                        // Arrange
                        var mine = GetRandom<OurObjectWithSemVer>();
                        var theirs = GetRandom<TheirObject>();
                        var tester = new DeepEqualityTester(mine, theirs)
                        {
                            OnlyCompareShape = true,
                            FailOnMissingProperties = false,
                            RecordErrors = true
                        };
                        // Pre-Assert
                        // Act
                        var result = tester.AreDeepEqual();
                        // Assert
                        Expect(result)
                            .To.Be.True(
                                tester.Errors.AsText()
                            );
                    }

                    public class LeVersion
                    {
                        public int LeMajor { get; set; }
                        public int LeMinor { get; set; }
                    }

                    public class TheirFrenchObject
                    {
                        public LeVersion Version { get; set; }
                    }

                    [Test]
                    public void
                        GivenObjectsWithDifferentSubShapes_WhenAllowMissingProperties_ButNoMatchedProperties_ShouldThrow()
                    {
                        // Arrange
                        var ours = GetRandom<OurObject>();
                        var theirs = GetRandom<TheirFrenchObject>();
                        var tester = new DeepEqualityTester(ours, theirs)
                        {
                            FailOnMissingProperties = false,
                            RecordErrors = true,
                            OnlyCompareShape = true
                        };
                        // Pre-Assert
                        // Act
                        var result = tester.AreDeepEqual();
                        // Assert
                        Expect(result).To.Be.False();
                        Console.WriteLine(tester.Errors.AsText());
                    }
                }

                [TestFixture]
                public class DeepEquals
                {
                    [Test]
                    public void GivenTwoObjectsOfSameType()
                    {
                        // Arrange
                        var sut = Create(GetRandomString(), GetRandomString());
                        sut.OnlyCompareShape = true;
                        // Pre-Assert
                        // Act
                        var result = sut.AreDeepEqual();
                        // Assert
                        Expect(result).To.Be.True();
                    }

                    [Test]
                    public void GivenTwoObjectsOfSameShape_DifferentType()
                    {
                        // Arrange
                        var left = new
                        {
                            Foo = new
                            {
                                id = 1,
                                Name = "bob",
                                When = DateTime.Now
                            },
                            Stuffs = new[]
                            {
                                new
                                {
                                    Id = 2,
                                    Name = "Billy"
                                },
                                new
                                {
                                    Id = 3,
                                    Name = "Bob"
                                }
                            },
                            Ints = new[] { 1, 2, 3 }
                        };
                        var right = new
                        {
                            Foo = new
                            {
                                id = 1,
                                Name = "bob",
                                When = DateTime.Now
                            },
                            Stuffs = new[]
                            {
                                new
                                {
                                    Id = 2,
                                    Name = "Billy"
                                },
                            },
                            Ints = new List<int>()
                            {
                                0,
                                12,
                                43
                            }
                        };
                        var sut = Create(left, right);
                        sut.OnlyCompareShape = true;
                        // Pre-Assert
                        // Act
                        Expect(sut.AreDeepEqual())
                            .To.Be.True(
                                "* " + sut.Errors.JoinWith("\n *")
                            );
                        // Assert
                    }
                }
            }
        }

        [TestFixture]
        public class UsingAsExtensionMethod
        {
            [TestFixture]
            public class DeepEquals
            {
                [Test]
                public void ShouldNotDerpRoundTwo()
                {
                    // Arrange
                    var left = new ThingWithField() { StringField = GetRandomString() };
                    var right = new ThingWithField() { StringField = GetAnother(left.StringField) };
                    // Act
                    var result = left.DeepEquals(right);
                    // Assert
                    Expect(result).To.Be.False();
                }
            }

            [Test]
            public void ShouldHaveOverloadToLogError()
            {
                // TODO: update this test when markers are added around deep equality failure
                // Arrange
                var left = new { foo = 1 };
                var right = new { foo = 2 };
                var logs = new List<string>();
                // Pre-assert
                // Act
                var result = left.DeepEquals(right, s => logs.Add(s));
                // Assert
                Expect(result).To.Be.False();
                Expect(logs).To.Contain.Only(1)
                    .Matched.By(l => l.Contains(left.Stringify()) && l.Contains(right.Stringify()));
            }

            [Test]
            public void ShouldReturnFalseForTwoArraysWithSameSizeAndDifferentItems()
            {
                // Arrange
                var left = new[] { 1 };
                var right = new[] { 2 };
                // Pre-assert
                // Act
                var result1 = left.DeepEquals(right);
                var result2 = right.DeepEquals(left);
                // Assert
                Expect(result1).To.Be.False();
                Expect(result2).To.Be.False();
            }

            [Test]
            public void ShouldReturnTrueForTwoArraysWithSameSizeAndSameItems()
            {
                // Arrange
                var left = new[] { 1 };
                var right = new[] { 1 };
                // Pre-assert
                // Act
                var result1 = left.DeepEquals(right);
                var result2 = right.DeepEquals(left);
                // Assert
                Expect(result1).To.Be.True();
                Expect(result2).To.Be.True();
            }

            [TestFixture]
            public class ShapeEquals
            {
                [Test]
                public void ShouldBeTrueForMatchingShapes()
                {
                    // Arrange
                    var left = new
                    {
                        Foo = new
                        {
                            id = 1,
                            Name = "bob",
                            When = DateTime.Now
                        },
                        Stuffs = new[]
                        {
                            new
                            {
                                Id = 2,
                                Name = "Billy"
                            },
                            new
                            {
                                Id = 3,
                                Name = "Bob"
                            }
                        },
                        Ints = new[] { 1, 2, 3 }
                    };
                    var right = new
                    {
                        Foo = new
                        {
                            id = 1,
                            Name = "bob",
                            When = DateTime.Now
                        },
                        Stuffs = new[]
                        {
                            new
                            {
                                Id = 2,
                                Name = "Billy"
                            },
                        },
                        Ints = new List<int>()
                        {
                            0,
                            12,
                            43
                        }
                    };
                    // Pre-Assert
                    // Act
                    Expect(left.ShapeEquals(right))
                        .To.Be.True();
                    // Assert
                }
            }

            [TestFixture]
            public class ShapeSubEquals
            {
                [Test]
                public void ShouldBeTrueWhenSourcePropsAreFoundAtTarget()
                {
                    // Arrange
                    var left = new
                    {
                        Id = 1,
                        Enabled = false
                    };
                    var right = new
                    {
                        Id = 123,
                        Name = "Bob",
                        Enabled = true
                    };
                    // Pre-Assert
                    // Act
                    Expect(left.ShapeSubEquals(right)).To.Be.True();
                    // Assert
                }
            }
        }


        [TestFixture]
        public class WildCases
        {
            [TestFixture]
            public class PropertyAssert_AreDeepEqual
            {
                [Test]
                public void ShouldReturnTrueForCaseInWild()
                {
                    // Arrange
                    var original = GetRandom<MooCakesAndStuff>();
                    var encoded = MooCakesAndStuffWithEncodedGraphData.From(original);
                    // Pre-Assert
                    PropertyAssert.AreDeepEqual(original.AuthorData, encoded.AuthorData);
                    PropertyAssert.AreDeepEqual(original.SomeOtherData, encoded.SomeOtherData);
                    // Act
                    var decoded = encoded.Decode();
                    // Assert
                    PropertyAssert.AreDeepEqual(decoded.AuthorData, original.AuthorData);
                    PropertyAssert.AreDeepEqual(decoded.SomeOtherData, original.SomeOtherData);
                    PropertyAssert.AreDeepEqual(decoded.WubWubs, original.WubWubs);
                }

                [Test]
                public void WildCaseExactReplica()
                {
                    // Arrange
                    var thang = GetRandom<MooCakesAndStuff>();
                    var encoded = new MooCakesAndStuffWithEncodedGraphData();
                    thang.CopyPropertiesTo(encoded);
                    encoded.EnkiEnki = DataUriFor(thang.WubWubs);
                    encoded.WubWubs = null;

                    // Pre-assert

                    // Act
                    var result = encoded.Decode();

                    // Assert
                    PropertyAssert.AreDeepEqual(result, thang);
                }
            }
        }

        [TestFixture]
        public class DateTimes
        {
            [Test]
            public void LocalDateTimeShouldNotEqualUtcDateTime()
            {
                // Arrange
                var src = GetRandomDate();
                var local = new DateTime(
                    src.Year,
                    src.Month,
                    src.Day,
                    src.Hour,
                    src.Minute,
                    src.Second,
                    DateTimeKind.Local);
                var utc = new DateTime(
                    src.Year,
                    src.Month,
                    src.Day,
                    src.Hour,
                    src.Minute,
                    src.Second,
                    DateTimeKind.Utc);
                var sut = Create(local, utc);
                // Pre-Assert
                // Act
                var result = sut.AreDeepEqual();
                // Assert
                Expect(result).To.Be.False();
            }
        }

        [TestFixture]
        public class Dictionary
        {
            [Test]
            public void WhenValuesAndKeysAreTheSame_ShouldReturnTrue()
            {
                // Arrange
                var key1 = GetRandomString();
                var key2 = GetRandomString();
                var value1 = GetRandomString();
                var value2 = GetRandomInt();
                var dictionary1 = DictionaryBuilder<string, object>.Create()
                    .WithItem(key1, value1)
                    .WithItem(key2, value2)
                    .Build();
                var dictionary2 = DictionaryBuilder<string, object>.Create()
                    .WithItem(key1, value1)
                    .WithItem(key2, value2)
                    .Build();
                var sut = Create(dictionary1, dictionary2);
                // Pre-Assert
                // Act
                var result = sut.AreDeepEqual();
                // Assert
                Expect(result).To.Be.True();
            }

            [Test]
            [Repeat(100)]
            public void WhenKeysAreTheSame_ButValuesAreDifferent_ShouldReturnFalse()
            {
                // Arrange
                var key1 = GetRandomString();
                var key2 = GetRandomString();
                var value1 = GetRandomString();
                var value2 = GetRandomInt();
                var dictionary1 = DictionaryBuilder<string, object>.Create()
                    .WithItem(key1, value1)
                    .WithItem(key2, value2)
                    .Build();
                var dictionary2 = DictionaryBuilder<string, object>.Create()
                    .WithItem(key1, GetAnother(value1))
                    .WithItem(key2, GetAnother(value2))
                    .Build();
                var sut = Create(dictionary1, dictionary2);
                // Pre-Assert
                // Act
                var result = sut.AreDeepEqual();
                // Assert
                Expect(result).To.Be.False();
            }

            [Test]
            public void WhenValuesAreTheSame_ButKeysAreDifferent_ShouldReturnFalse()
            {
                // Arrange
                var value1 = GetRandomString();
                var value2 = GetRandomInt();
                var dictionary1 = DictionaryBuilder<string, object>.Create()
                    .WithItem(GetRandomString(), value1)
                    .WithItem(GetRandomString(), value2)
                    .Build();
                var dictionary2 = DictionaryBuilder<string, object>.Create()
                    .WithItem(GetRandomString(), value1)
                    .WithItem(GetRandomString(), value2)
                    .Build();
                var sut = Create(dictionary1, dictionary2);
                // Pre-Assert
                // Act
                var result = sut.AreDeepEqual();
                // Assert
                Expect(result).To.Be.False();
            }

            [Test]
            public void WhenDifferentNumberOfEntries_ShouldReturnFalse()
            {
                // Arrange
                var key1 = GetRandomString();
                var key2 = GetRandomString();
                var value1 = GetRandomString();
                var value2 = GetRandomInt();
                var dictionary1 = DictionaryBuilder<string, object>.Create()
                    .WithItem(key1, value1)
                    .Build();
                var dictionary2 = DictionaryBuilder<string, object>.Create()
                    .WithItem(key1, value1)
                    .WithItem(key2, value2)
                    .Build();
                var sut = Create(dictionary1, dictionary2);
                // Pre-Assert
                // Act
                var result = sut.AreDeepEqual();
                // Assert
                Expect(result).To.Be.False();
            }
        }

        [TestFixture]
        public class ArbitraryBehaviors
        {
            [Test]
            public void ComparingTwoSimpleTypesWhereOneIsDecimalAndDifferentValues()
            {
                // Arrange
                var left = "123";
                var right = 124M;
                var sut = Create(left, right);
                // Pre-assert
                // Act
                var result = sut.AreDeepEqual();
                // Assert
                Expect(result).To.Be.False();
            }

            [Test]
            public void ComparingTwoSimpleTypesWhereOneIsDecimalAndSameValuesWhereBothNumeric()
            {
                // Arrange
                var left = 123;
                var right = 123M;
                var sut = Create(left, right);
                // Pre-assert
                // Act
                var result = sut.AreDeepEqual();
                // Assert
                Expect(result).To.Be.True();
            }

            [Test]
            public void ComparingTwoSimpleTypesWhereOneIsDecimalAndSameValuesButOneNotNumeric()
            {
                // Arrange
                var left = "123";
                var right = 123M;
                var sut = Create(left, right);
                // Pre-assert
                // Act
                var result = sut.AreDeepEqual();
                // Assert
                Expect(result).To.Be.True();
            }
        }

        [TestFixture]
        public class Oddities
        {
            [Test]
            public void ComparingInvalidOperationException()
            {
                // Arrange
                var message = GetRandomString(1);
                var ex = new InvalidOperationException(message);
                var sut = Create(ex, ex);
                // Act
                var result = sut.AreDeepEqual();
                // Assert
                Expect(result).To.Be.True();
            }

            [Test]
            public void ShouldNotStackOverflowWhenComparingEnumValues()
            {
                // Arrange
                var left = new { LogLevel = LogLevel.Critical };
                var right = new { LogLevel = LogLevel.Critical };
                var sut = Create(left, right);
                sut.IncludeFields = true;
                // Act
                var result = sut.AreDeepEqual();
                // Assert
                Expect(result)
                    .To.Be.True();
            }

            [Test]
            public void ShouldCompareByNameByDefault()
            {
                // Arrange
                var left = new { LogLevel = LogLevel.Critical };
                var right = new { LogLevel = ExtLogLevel.Critical };
                var sut = Create(left, right);
                // Act
                var result = sut.AreDeepEqual();
                // Assert
                Expect(result)
                    .To.Be.True();
            }

            [Test]
            public void ShouldCompareDirectlyOnRequest()
            {
                // Arrange
                var left = new { LogLevel = LogLevel.Critical };
                var right = new { LogLevel = ExtLogLevel.Critical };
                var sut = Create(left, right);
                sut.EnumComparisonStrategy = DeepEqualityTester.EnumComparisonStrategies.ByObjectEquals;
                // Act
                var result = sut.AreDeepEqual();
                // Assert
                Expect(result)
                    .To.Be.False();
            }

            [Test]
            public void ShouldCompareByIntValueOnRequest()
            {
                // Arrange
                var left = new { LogLevel = LogLevel.Error }; // 4
                var right = new { LogLevel = ExtLogLevel.Warning }; // 4
                var sut = Create(left, right);
                sut.EnumComparisonStrategy = DeepEqualityTester.EnumComparisonStrategies.ByIntegerValue;
                // Act
                var result = sut.AreDeepEqual();
                // Assert
                Expect(result)
                    .To.Be.True();
            }

            [Test]
            public void ShouldBeAbleToCompareTypePropertiesOfSameValue()
            {
                // Arrange
                var left = new
                {
                    T = typeof(Oddities)
                };
                var right = new
                {
                    T = typeof(Oddities)
                };
                var sut = Create(left, right);
                // Act
                var result = sut.AreDeepEqual();
                // Assert
                Expect(result)
                    .To.Be.True();
            }

            [Test]
            public void ShouldBeAbleToCompareTypePropertiesOfDifferentValue()
            {
                // Arrange
                var left = new
                {
                    T = typeof(decimal)
                };

                var right = new
                {
                    T = typeof(int)
                };
                var sut = Create(left, right);
                // Act
                var result = sut.AreDeepEqual();
                // Assert
                Expect(result)
                    .To.Be.False();
            }

            [Test]
            public void ShouldConsiderPropertiesWhichThrowTheExactSameExceptionToBeEqual()
            {
                // Arrange
                var left = new HasThrowingProp();
                var right = new HasThrowingProp();
                left.Id = right.Id = GetRandomInt();
                var sut = new DeepEqualityTester(left, right);
                // Act
                var result = sut.AreDeepEqual();
                // Assert
                Expect(result)
                    .To.Be.True();
            }

            [TestFixture]
            public class ComparingNullableAndNonNullableValues
            {
                [TestFixture]
                public class HappyPaths
                {
                    [Test]
                    public void ShouldBeAbleToCompareNullableInt()
                    {
                        // Arrange
                        var left = new { id = 1 };
                        var right = new { id = (int?) 1 };
                        var sut = Create(left, right);
                        // Act
                        var result = sut.AreDeepEqual();
                        // Assert
                        Expect(result)
                            .To.Be.True();
                    }

                    [Test]
                    public void ShouldBeAbleToCompareNullableLong()
                    {
                        // Arrange
                        var left = new { id = 1 };
                        var right = new { id = (long?) 1 };
                        var sut = Create(left, right);
                        // Act
                        var result = sut.AreDeepEqual();
                        // Assert
                        Expect(result)
                            .To.Be.True();
                    }

                    [Test]
                    public void ShouldBeAbleToCompareNullableDecimal()
                    {
                        // Arrange
                        var left = new { id = 1 };
                        var right = new { id = (decimal?) 1 };
                        var sut = Create(left, right);
                        // Act
                        var result = sut.AreDeepEqual();
                        // Assert
                        Expect(result)
                            .To.Be.True();
                    }

                    [Test]
                    public void ShouldBeAbleToCompareNullableShort()
                    {
                        // Arrange
                        var left = new { id = 1 };
                        var right = new { id = (short?) 1 };
                        var sut = Create(left, right);
                        // Act
                        var result = sut.AreDeepEqual();
                        // Assert
                        Expect(result)
                            .To.Be.True();
                    }

                    [Test]
                    public void ShouldBeAbleToCompareNullableFloat()
                    {
                        // Arrange
                        var left = new { id = 1 };
                        var right = new { id = (float?) 1 };
                        var sut = Create(left, right);
                        // Act
                        var result = sut.AreDeepEqual();
                        // Assert
                        Expect(result)
                            .To.Be.True();
                    }

                    [Test]
                    public void ShouldBeAbleToCompareNullableDouble()
                    {
                        // Arrange
                        var left = new { id = 1 };
                        var right = new { id = (double?) 1 };
                        var sut = Create(left, right);
                        // Act
                        var result = sut.AreDeepEqual();
                        // Assert
                        Expect(result)
                            .To.Be.True();
                    }
                }

                [TestFixture]
                public class SadPaths
                {
                    [Test]
                    public void ShouldBeAbleToCompareNullableInt()
                    {
                        // Arrange
                        var left = new { id = 2 };
                        var right = new { id = (int?) 1 };
                        var sut = Create(left, right);
                        // Act
                        var result = sut.AreDeepEqual();
                        // Assert
                        Expect(result)
                            .To.Be.False();
                    }

                    [Test]
                    public void ShouldBeAbleToCompareNullableIntThatIsNull()
                    {
                        // Arrange
                        var left = new { id = 2 };
                        var right = new { id = (int?) null };
                        var sut = Create(left, right);
                        // Act
                        var result = sut.AreDeepEqual();
                        // Assert
                        Expect(result)
                            .To.Be.False();
                    }

                    [Test]
                    public void ShouldBeAbleToCompareNullableIntThatIsNullReversed()
                    {
                        // Arrange
                        var left = new { id = 2 };
                        var right = new { id = (int?) null };
                        var sut = Create(right, left);
                        // Act
                        var result = sut.AreDeepEqual();
                        // Assert
                        Expect(result)
                            .To.Be.False();
                    }

                    [Test]
                    public void ShouldBeAbleToCompareNullableLong()
                    {
                        // Arrange
                        var left = new { id = 2 };
                        var right = new { id = (long?) 1 };
                        var sut = Create(left, right);
                        // Act
                        var result = sut.AreDeepEqual();
                        // Assert
                        Expect(result)
                            .To.Be.False();
                    }

                    [Test]
                    public void ShouldBeAbleToCompareNullableDecimal()
                    {
                        // Arrange
                        var left = new { id = 1 };
                        var right = new { id = (decimal?) 1.0001M };
                        var sut = Create(left, right);
                        // Act
                        var result = sut.AreDeepEqual();
                        // Assert
                        Expect(result)
                            .To.Be.False();
                    }

                    [Test]
                    public void ShouldBeAbleToCompareNullableShort()
                    {
                        // Arrange
                        var left = new { id = 1 };
                        var right = new { id = (short?) 2 };
                        var sut = Create(left, right);
                        // Act
                        var result = sut.AreDeepEqual();
                        // Assert
                        Expect(result)
                            .To.Be.False();
                    }

                    [Test]
                    public void ShouldBeAbleToCompareNullableFloat()
                    {
                        // Arrange
                        var left = new { id = 1 };
                        var right = new { id = (float?) 1.2 };
                        var sut = Create(left, right);
                        // Act
                        var result = sut.AreDeepEqual();
                        // Assert
                        Expect(result)
                            .To.Be.False();
                    }

                    [Test]
                    public void ShouldBeAbleToCompareNullableDouble()
                    {
                        // Arrange
                        var left = new { id = 1 };
                        var right = new { id = (double?) 1.000001 };
                        var sut = Create(left, right);
                        // Act
                        var result = sut.AreDeepEqual();
                        // Assert
                        Expect(result)
                            .To.Be.False();
                    }
                }
            }

            public class HasThrowingProp
            {
                public int Id { get; set; }
                public int Thrower => throw new ArgumentException("Invalid id", nameof(Id));
            }

            public enum LogLevel
            {
                Trace = 0,
                Debug = 1,
                Information = 2,
                Warning = 3,
                Error = 4,
                Critical = 5,
                None = 6,
            }

            public enum ExtLogLevel
            {
                Trace = 1,
                Debug = 2,
                Information = 3,
                Warning = 4,
                Error = 5,
                Critical = 6,
                None = 7,
            }
        }

        [TestFixture]
        public class CustomEqualityTesting
        {
            [Test]
            public void GivenCustomDateTimeComparer()
            {
                // Arrange
                var left = new { Date = DateTime.Now.AddSeconds(-1) };
                var right = new { Date = left.Date.AddSeconds(1) };
                var sut = Create(left, right);
                sut.AddCustomComparer(new DateComparerByDayOnly());
                // Pre-assert
                // Act
                var result = sut.AreDeepEqual();
                // Assert
                Expect(result).To.Be.True();
            }

            [Test]
            public void GivenCustomDateTimeComparerAsObject()
            {
                // Arrange
                var left = new { Date = DateTime.Now.AddSeconds(-1) };
                var right = new { Date = left.Date.AddSeconds(1) };
                var sut = Create(left, right);
                sut.AddCustomComparer(new DateComparerByDayOnly() as object);
                // Pre-assert
                // Act
                var result = sut.AreDeepEqual();
                // Assert
                Expect(result).To.Be.True();
            }

            [Test]
            public void GivenDisparateNumericTypesAndCustomDecimalComparer()
            {
                // Arrange
                var left = new { id = (long) 1 };
                var right = new { id = 2 };
                var sut = Create(left, right);
                sut.AddCustomComparer(new DecimalComparerWhichAllowsUpTo2Delta());
                // Pre-assert
                // Act
                var result = sut.AreDeepEqual();
                // Assert
                Expect(result).To.Be.True();
            }

            [Test]
            public void GivenCustomComparerForComplexType()
            {
                // Arrange
                var left = GetRandom<Parent>();
                var right = GetRandom<Parent>();
                right.Id = left.Id;
                right.Name = left.Name;
                var sut = Create(left, right);
                sut.RecordErrors = true;
                sut.AddCustomComparer(new ChildEqualityComparer());
                // Pre-assert
                Expect(left.Child).Not.To.Deep.Equal(right.Child);
                // Act
                var result = sut.AreDeepEqual();
                // Assert
                Expect(result).To.Be.True(sut.Errors.JoinWith("\n"));
            }

            public class ChildEqualityComparer : IEqualityComparer<Child>
            {
                public bool Equals(Child x, Child y)
                {
                    return true; // all children are loved just the same!
                }

                public int GetHashCode(Child obj)
                {
                    return 0;
                }
            }

            public class Parent
            {
                public int Id { get; set; }
                public string Name { get; set; }
                public Child Child { get; set; }
            }

            public class Child
            {
                public int Id { get; set; }
                public string Name { get; set; }
            }
        }

        [TestFixture]
        public class Issues
        {
            [Test]
            public void ShouldNotGetConfusedWhenComparingObjectToAnonymousType()
            {
                // Arrange
                var sub = new { id = 1 };
                var container1 = new { sub };
                var container2 = new { sub = (object) sub };
                var sut = Create(container1, container2);
                sut.RecordErrors = true;
                // Act
                var result = sut.AreDeepEqual();
                // Assert
                Expect(result)
                    .To.Be.True(() => sut.Errors.JoinWith("\n"));
            }

            [Test]
            public void ShouldNotIncludeStaticPropsInComparison()
            {
                // Arrange
                var data = new HasAStaticProp()
                {
                    Name = GetRandomString()
                };
                HasAStaticProp.Id = 123;
                var sut = Create(data, new { data.Name, Id = 123 });

                // Act
                var result = sut.AreDeepEqual();

                // Assert
                Expect(result)
                    .To.Be.False();
            }

            public class HasAStaticProp
            {
                public static int Id { get; set; }
                public string Name { get; set; }
            }
        }

        public class DecimalComparerWhichAllowsUpTo2Delta : IEqualityComparer<decimal>
        {
            public bool Equals(decimal x, decimal y)
            {
                return Math.Abs(x - y) < 2;
            }

            public int GetHashCode(decimal obj)
            {
                return 0;
            }
        }

        public class DateComparerByDayOnly : IEqualityComparer<DateTime>
        {
            public bool Equals(DateTime x, DateTime y)
            {
                return x.Year == y.Year &&
                    x.Month == y.Month &&
                    x.Day == y.Day;
            }

            public int GetHashCode(DateTime obj)
            {
                return 0;
            }
        }

        private static string DataUriFor<T>(T input)
        {
            var json = JsonConvert.SerializeObject(input);
            var encoded = json.AsBytes().ToBase64();
            return $"data:application/json;base64,{encoded}";
        }

        public class ThingWithCollection
        {
            public ICollection<int> Subs { get; set; }
        }

        private static DeepEqualityTester Create(object obj1, object obj2)
        {
            var sut = new DeepEqualityTester(obj1, obj2) { RecordErrors = true };
            return sut;
        }

        public class ThingWithField
        {
            public string StringField;
        }

        public class MooCakesAndStuff
        {
            public Guid Id { get; set; }

            public string Name { get; set; }

            public int MajorVersion { get; set; }
            public int MinorVersion { get; set; }

            public SomeOtherDataThing SomeOtherData { get; set; }
            public YetAnotherCollectionOfProperties WubWubs { get; set; }
            public BeerMooCakeCroc AuthorData { get; set; }

            public const int NameMaxLength = 100;
        }

        public class BeerMooCakeCroc
        {
            public DateTime Created { get; set; }
            public DateTime LastModified { get; set; }
            public long ClownId { get; set; }
            public long CreatedById { get; set; }
            public string CreatedBy { get; set; }
            public long LastModifiedById { get; set; }
            public string LastModifiedBy { get; set; }
        }

        public class YetAnotherCollectionOfProperties
        {
            // TODO: generalize down to BiltongSlab
            //  as that type can support things which aren't wait events,
            //  such as, eg (at some point) decision diamonds
            public ThingyMaBob[] ThingyMaBobs { get; set; }

            public WerkelSchmerkel[] WerkelSchmerkels { get; set; }
        }

        public class WerkelSchmerkel
        {
            public string FromId { get; set; }
            public string ToId { get; set; }
        }
    }

    public class ThingyMaBob : BiltongSlab
    {
        // -> backend WaitEvent
        public string ThingType { get; set; }

        public Guid ThingTypeId { get; set; }
        public ThingCondition[] ThingConditions { get; set; }
        public CakesOfMoo[] CakesOfMoos { get; set; }
    }

    public class ThingCondition
    {
        // -> backend: Trigger
        public Option Condition { get; set; }

        public Option Operator { get; set; }
        public Option[] SelectedValues { get; set; }
    }

    public class Option
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }

    public class CakesOfMoo
    {
        // -> backend: StepAction
        public Guid Id { get; set; } // -> this conditional action's id

        public Guid CakeId { get; set; } // -> maps to the id of the source action
        public string CakeName { get; set; }
        public Dictionary<string, object[]> Settings { get; set; }
        public TurkeyInnards[] TurkeyStuffings { get; set; }
    }

    public class TurkeyInnards
    {
        public NamedIdentifier Part1 { get; set; }
        public NamedIdentifier Part2 { get; set; }
    }

    public class NamedIdentifier
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class BiltongSlab
    {
        // -> used at client only
        public string Id { get; set; } // often a guid, but may be a static like "stateBegin"

        public int Top { get; set; }
        public int Left { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string WankelRotaryEngine { get; set; }
    }

    public class SomeOtherDataThing
    {
        public FirstPart PartTheFirst { get; set; }
        public SecondPart PartTheSecond { get; set; }
        public ThirdPart PartTheThird { get; set; }
    }

    public class FirstPart
    {
        public string[] HaveSome { get; set; }
        public string[] HaveNone { get; set; }
    }

    public class SecondPart
    {
        public Flibber Flibber { get; set; }
        public DateTime Nerf { get; set; }

        public DateTime DeNerf { get; set; }
        public bool NerfFreeZone { get; set; }
        public bool CanNerfTheUnNerfable { get; set; }
        public Blargh Blargh { get; set; }
    }

    public class Flibber
    {
        // TODO: complete this
        public Frequency Frequency { get; set; }

        public int Interval { get; set; }
    }

    public enum Frequency
    {
        Hertz,
        HertzMore,
        HertzTheMost
    }

    public class Blargh
    {
        public Salutations Salutations { get; set; }
        public string Exclamation { get; set; }
    }

    public enum Salutations
    {
        Loud,
        Louder
    }

    public class ThirdPart
    {
        public int? SomeNumber1 { get; set; }
        public bool IsSomeNumber1 { get; set; }

        public int? SomeNumber2 { get; set; }
        public bool IsSomeNumber2 { get; set; }

        public int Arb { get; set; }

        public bool Boom { get; set; }
        public bool Stick { get; set; }

        public const int Wombat = 1;
        public const int Feh = 90;
    }

    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class MooCakesAndStuffWithEncodedGraphData : TestDeepEqualityTester.MooCakesAndStuff
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public string EnkiEnki { get; set; }

        private readonly object _lock = new object();

        public TestDeepEqualityTester.MooCakesAndStuff Decode()
        {
            var result = StartMeOffJoe();
            if (EnkiEnki == null)
                return result;
            lock (_lock)
            {
                result.WubWubs = JsonConvert
                    .DeserializeObject<TestDeepEqualityTester.YetAnotherCollectionOfProperties>(
                        Convert.FromBase64String(GetDataPartOf(EnkiEnki))
                            .ToUTF8String());
            }

            return result;
        }

        private readonly Regex _dataUriRegex = new Regex(@"^(data:application/json;base64,)(?<data>.*)");

        private string GetDataPartOf(string barf)
        {
            var match = _dataUriRegex.Match(barf);
            var dataGroup = match.Groups["data"];
            if (!dataGroup.Success)
            {
                throw new InvalidDataException($"Expected to find a base64-encoded data uri, but got:\n${barf}");
            }

            return dataGroup.Value;
        }

        private static readonly PropertyInfo[] _props =
            typeof(TestDeepEqualityTester.MooCakesAndStuff).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        private static readonly PropertyInfo[] _moreProps =
            typeof(MooCakesAndStuffWithEncodedGraphData).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        private TestDeepEqualityTester.MooCakesAndStuff StartMeOffJoe()
        {
            var moo = new TestDeepEqualityTester.MooCakesAndStuff();
            foreach (var propertyInfo in _props)
            {
                var match = _moreProps.First(pi => pi.Name == propertyInfo.Name);
                var myVal = match.GetValue(this);
                propertyInfo.SetValue(moo, myVal);
            }

            return moo;
        }

        public static MooCakesAndStuffWithEncodedGraphData From(TestDeepEqualityTester.MooCakesAndStuff moo)
        {
            var result = new MooCakesAndStuffWithEncodedGraphData();
            foreach (var propertyInfo in _props)
            {
                var match = _moreProps.First(pi => pi.Name == propertyInfo.Name);
                var srcVal = match.GetValue(moo);
                propertyInfo.SetValue(result, srcVal);
            }

            result.EnkiEnki =
                $"data:application/json;base64,{Convert.ToBase64String(JsonConvert.SerializeObject(moo.WubWubs).AsBytes())}";
            return result;
        }
    }

    public class MooBuilder : GenericBuilder<MooBuilder, TestDeepEqualityTester.MooCakesAndStuff>
    {
        public MooBuilder WithCreatedDate(DateTime date)
        {
            return WithProp(dto => dto.AuthorData.Created = date);
        }

        public MooBuilder WithCreatedById(int id)
        {
            return
                WithProp(dto => dto.AuthorData = dto.AuthorData ?? GetRandom<TestDeepEqualityTester.BeerMooCakeCroc>())
                    .WithProp(dto => dto.AuthorData.CreatedById = id);
        }

        public MooBuilder WithName(string name)
        {
            return WithProp(o => o.Name = name);
        }

        public MooBuilder WithInvalidProps()
        {
            return WithRandomProps()
                .WithName("")
                .WithProp(dto => dto.SomeOtherData.PartTheFirst.HaveSome = new string[0])
                .WithProp(dto => dto.SomeOtherData.PartTheSecond.DeNerf = DateTime.MinValue)
                .WithProp(dto => dto.SomeOtherData.PartTheSecond.Nerf = DateTime.MaxValue)
                .WithProp(dto => dto.SomeOtherData.PartTheThird.IsSomeNumber1 = false)
                .WithProp(dto => dto.SomeOtherData.PartTheThird.SomeNumber1 = int.MinValue)
                .WithProp(dto => dto.SomeOtherData.PartTheThird.IsSomeNumber2 = false)
                .WithProp(dto => dto.SomeOtherData.PartTheThird.SomeNumber2 = int.MinValue)
                .WithProp(dto => dto.SomeOtherData.PartTheThird.Arb = int.MinValue);
        }

        public MooBuilder WithAllValidProps()
        {
            return WithRandomProps()
                .WithName(GetRandomString(5, 15))
                .WithProp(dto => dto.SomeOtherData.PartTheFirst.HaveSome = new[] { GetRandomString(5, 11) })
                .WithProp(dto => dto.SomeOtherData.PartTheSecond.Nerf = DateTime.Now)
                .WithProp(dto => dto.SomeOtherData.PartTheSecond.DeNerf = DateTime.Now.AddDays(10))
                .WithProp(dto => dto.SomeOtherData.PartTheThird.IsSomeNumber1 = false)
                .WithProp(dto => dto.SomeOtherData.PartTheThird.SomeNumber1 = 10)
                .WithProp(dto => dto.SomeOtherData.PartTheThird.IsSomeNumber2 = false)
                .WithProp(dto => dto.SomeOtherData.PartTheThird.SomeNumber2 = 1)
                .WithProp(dto => dto.SomeOtherData.PartTheThird.Arb = 5);
        }

        public MooBuilder WithNullGraphData()
        {
            return WithProp(o => o.WubWubs = null);
        }

        public MooBuilder WithNullWidgets()
        {
            return WithProp(o => o.WubWubs.ThingyMaBobs = null);
        }

        public MooBuilder WithEmptyWidgets()
        {
            return WithProp(o => o.WubWubs.ThingyMaBobs = new ThingyMaBob[0]);
        }

        public MooBuilder WithNullConnections()
        {
            return WithProp(o => o.WubWubs.WerkelSchmerkels = null);
        }

        public MooBuilder WithEmptyConnections()
        {
            return WithProp(o => o.WubWubs.WerkelSchmerkels = new TestDeepEqualityTester.WerkelSchmerkel[0]);
        }

        public MooBuilder WithNoInitiatingTrigger()
        {
            return WithProp(
                o => o.WubWubs.WerkelSchmerkels =
                    o.WubWubs.WerkelSchmerkels
                        .EmptyIfNull()
                        .Where(c => c.FromId != "WibbleStix")
                        .ToArray()
            );
        }

        public MooBuilder WithUnconfiguredAction()
        {
            return WithProp(
                o =>
                {
                    var widget = o.WubWubs.ThingyMaBobs.Second();
                    widget.CakesOfMoos.First().Settings["Moo"] = GetRandomBoolean()
                        ? null
                        : new object[] { };
                });
        }

        public MooBuilder WithUnconfiguredCondition()
        {
            return WithProp(
                o =>
                {
                    var widget = o.WubWubs.ThingyMaBobs.Second();
                    widget.ThingConditions.First().SelectedValues = GetRandomBoolean()
                        ? null
                        : new Option[0];
                });
        }

        public MooBuilder WithMultipleEventsButNoActions()
        {
            return WithProp(
                o =>
                {
                    o.WubWubs.ThingyMaBobs.ForEach(
                        w =>
                        {
                            w.CakesOfMoos = null;
                        });
                });
        }

        public class EventConditionBuilder : GenericBuilder<EventConditionBuilder, ThingCondition>
        {
            public override EventConditionBuilder WithRandomProps()
            {
                return base.WithRandomProps()
                    .WithProp(condition => condition.Condition = GetRandom<Option>())
                    .WithProp(condition => condition.Operator = GetRandom<Option>())
                    .WithProp(condition => condition.SelectedValues = GetRandomCollection<Option>(1).ToArray());
            }
        }

        public class ConditionalActionBuilder : GenericBuilder<ConditionalActionBuilder, CakesOfMoo>
        {
            public override ConditionalActionBuilder WithRandomProps()
            {
                return WithProp(o => o.CakeId = Guid.NewGuid())
                    .WithProp(o => o.CakeName = GetRandomString(4))
                    .WithProp(o => o.Id = Guid.NewGuid())
                    .WithProp(
                        o => o.Settings = new Dictionary<string, object[]>()
                        {
                            [GetRandomString(2, 4)] = GetRandomCollection<int>(2).Cast<object>().ToArray()
                        });
            }
        }

        public class WaitEventWidgetDtoBuilder : GenericBuilder<WaitEventWidgetDtoBuilder, ThingyMaBob>
        {
            public override WaitEventWidgetDtoBuilder WithRandomProps()
            {
                return base.WithRandomProps()
                    .WithGuidId()
                    .WithRandomThingConditions()
                    .WithRandomMooCakes();
            }

            public WaitEventWidgetDtoBuilder WithGuidId()
            {
                return WithProp(o => o.Id = $"{Guid.NewGuid()}");
            }

            private readonly string _depositId = Guid.NewGuid().ToString();

            public WaitEventWidgetDtoBuilder AsDeposit()
            {
                return WithGuidId()
                    .WithThingTypeId(_depositId)
                    .WithThingType(
                        "Deposit"); // this is what the client gets; it's only interesting to humans -- we'll use the id
            }

            public WaitEventWidgetDtoBuilder WithThingType(string thingType)
            {
                return WithProp(o => o.ThingType = thingType);
            }

            public WaitEventWidgetDtoBuilder WithThingTypeId(string guid)
            {
                return WithProp(o => o.ThingTypeId = Guid.Parse(guid));
            }

            public WaitEventWidgetDtoBuilder WithRandomMooCakes()
            {
                return WithProp(
                    o =>
                        WithMooCakes(GetRandomCollection<CakesOfMoo>(1, 3).ToArray())
                );
            }

            public WaitEventWidgetDtoBuilder WithId(string id)
            {
                return WithProp(o => o.Id = id);
            }

            public WaitEventWidgetDtoBuilder WithMooCakes(params CakesOfMoo[] actions)
            {
                return WithProp(
                    o =>
                        o.CakesOfMoos = o.CakesOfMoos.EmptyIfNull().And(actions)
                );
            }

            public WaitEventWidgetDtoBuilder WithRandomThingConditions()
            {
                return WithProp(
                    o =>
                        WithThingConditions(GetRandomCollection<ThingCondition>(1, 3).ToArray())
                );
            }

            public WaitEventWidgetDtoBuilder WithThingConditions(params ThingCondition[] conditions)
            {
                return WithProp(o => o.ThingConditions = o.ThingConditions.EmptyIfNull().And(conditions));
            }
        }

        public class YacopBuilder
            : GenericBuilder<YacopBuilder, TestDeepEqualityTester.YetAnotherCollectionOfProperties
            >
        {
            private static readonly ThingyMaBob _start = new ThingyMaBob()
            {
                Id = "stateBegin",
                ThingTypeId = Guid.Empty,
                WankelRotaryEngine = "pjc-state-begin"
            };

            public override YacopBuilder WithRandomProps()
            {
                return base.WithRandomProps()
                    .WithStart()
                    .WithProp(
                        o =>
                        {
                            var waitEventWidgets = GetRandomCollection<ThingyMaBob>(2, 4).ToArray();
                            WithThingies(waitEventWidgets);
                        })
                    .WithSomePlan();
            }

            private YacopBuilder WithSomePlan()
            {
                return WithProp(
                    o =>
                    {
                        var start = o.ThingyMaBobs.FirstOrDefault(w => w.Id == "WibbleStix");
                        if (start == null)
                            return;
                        var others = new Queue<ThingyMaBob>(o.ThingyMaBobs.Except(new[] { start }).ToArray());
                        var last = start;
                        while (others.Count > 0)
                        {
                            var next = others.Dequeue();
                            o.WerkelSchmerkels = o.WerkelSchmerkels.EmptyIfNull().And(ConnectWibbleStix(last, next));
                            last = next;
                        }
                    });

                TestDeepEqualityTester.WerkelSchmerkel ConnectWibbleStix(BiltongSlab fromWidget, BiltongSlab toWidget)
                {
                    return new TestDeepEqualityTester.WerkelSchmerkel()
                    {
                        FromId = fromWidget.Id == "WibbleStix"
                            ? fromWidget.Id
                            : $"success.{fromWidget.Id}",
                        ToId = $"wait-event.{toWidget.Id}"
                    };
                }
            }

            public YacopBuilder WithThingies(params ThingyMaBob[] connections)
            {
                return WithProp(
                    o =>
                        o.ThingyMaBobs = o.ThingyMaBobs.EmptyIfNull().And(connections)
                );
            }

            public YacopBuilder WithWerkels(params TestDeepEqualityTester.WerkelSchmerkel[] connections)
            {
                return WithProp(o => o.WerkelSchmerkels = o.WerkelSchmerkels.EmptyIfNull().And(connections));
            }

            public YacopBuilder WithStart()
            {
                return WithProp(o => o.ThingyMaBobs = o.ThingyMaBobs.And(_start));
            }
        }
    }
}