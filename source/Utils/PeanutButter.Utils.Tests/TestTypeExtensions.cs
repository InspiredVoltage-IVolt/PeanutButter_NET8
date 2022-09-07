﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NExpect;
using NUnit.Framework;
using static NExpect.Expectations;
using static PeanutButter.RandomGenerators.RandomValueGen;
using static PeanutButter.Utils.Benchmark;

// ReSharper disable UnusedParameter.Local
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace PeanutButter.Utils.Tests
{
    [TestFixture]
    public class TestTypeExtensions
    {
        [TestFixture]
        public class Ancestry
        {
            [Test]
            public void WorkingOnTypeOfObject_ShouldReturnOnlyObject()
            {
                //---------------Set up test pack-------------------
                var type = typeof(object);
                //---------------Assert Precondition----------------

                //---------------Execute Test ----------------------
                var result = type.Ancestry();

                //---------------Test Result -----------------------
                Expect(result)
                    .To.Contain.Only(1)
                    .Equal.To(typeof(object));
            }

            [Test]
            public void WorkingOnSimpleType_ShouldReturnItAndObject()
            {
                //---------------Set up test pack-------------------
                var type = typeof(SimpleType);

                //---------------Assert Precondition----------------

                //---------------Execute Test ----------------------
                var result = type.Ancestry();

                //---------------Test Result -----------------------
                Expect(result)
                    .To.Equal(
                        new[]
                        {
                            typeof(object),
                            typeof(SimpleType)
                        });
            }

            [Test]
            public void WorkingOnInheritedType_ShouldReturnItAndObject()
            {
                //---------------Set up test pack-------------------
                var type = typeof(InheritedType);

                //---------------Assert Precondition----------------

                //---------------Execute Test ----------------------
                var result = type.Ancestry();

                //---------------Test Result -----------------------
                Expect(result)
                    .To.Equal(
                        new[]
                        {
                            typeof(object),
                            typeof(SimpleType),
                            typeof(InheritedType)
                        });
            }

            [TestFixture]
            public class IsAncestorOf
            {
                [Test]
                public void ShouldReturnTrueWhenOperatingTypeIsImmediateAncestorOfOtherType()
                {
                    // Arrange
                    var t = typeof(SimpleType);
                    // Act
                    var result = t.IsAncestorOf(typeof(InheritedType));
                    // Assert
                    Expect(result)
                        .To.Be.True();
                }

                [Test]
                public void ShouldReturnTrueWhenOperatingTypeIsSomeAncestorOfOtherType()
                {
                    // Arrange
                    var t = typeof(object);
                    // Act
                    var result = t.IsAncestorOf(typeof(InheritedType));
                    // Assert
                    Expect(result)
                        .To.Be.True();
                }

                [Test]
                public void ShouldReturnFalseWhenTypesAreNotRelated()
                {
                    // Arrange
                    var t = typeof(SimpleType);
                    // Act
                    var result = t.IsAncestorOf(typeof(AnotherType));
                    // Assert
                    Expect(result)
                        .To.Be.False();
                }

                [Test]
                public void ShouldReturnFalseWhenTypesAreRelatedInverted()
                {
                    // Arrange
                    var t = typeof(SimpleType);
                    // Act
                    var result = t.IsAncestorOf(typeof(object));
                    // Assert
                    Expect(result)
                        .To.Be.False();
                }

                [Test]
                public void ShouldReturnFalseWhenOperatingTypeIsNull()
                {
                    // Arrange
                    var t = null as Type;
                    // Act
                    var result = t.IsAncestorOf(typeof(object));
                    // Assert
                    Expect(result)
                        .To.Be.False();
                }
            }

            [TestFixture]
            public class Inherits
            {
                [Test]
                public void ShouldReturnTrueWhenOperatingTypeImmediatelyInheritsOtherType()
                {
                    // Arrange
                    var t = typeof(InheritedType);

                    // Act
                    var result = t.Inherits(typeof(SimpleType));

                    // Assert
                    Expect(result)
                        .To.Be.True();
                }

                [Test]
                public void ShouldReturnTrueWhenOperatingTypeEventuallyInheritsOtherType()
                {
                    // Arrange
                    var t = typeof(GrandChildType);
                    // Act
                    var result = t.Inherits(typeof(SimpleType));
                    // Assert
                    Expect(result)
                        .To.Be.True();
                }

                [Test]
                public void ShouldReturnTrueWhenOperatingTypeIsNotNullAndTestTypeIsObject()
                {
                    // Arrange
                    var t = typeof(GrandChildType);
                    // Act
                    var result = t.Inherits(typeof(object));
                    // Assert
                    Expect(result)
                        .To.Be.True();
                }

                [Test]
                public void ShouldReturnFalseWhenTypesAreNotRelated()
                {
                    // Arrange
                    var t = typeof(AnotherType);
                    // Act
                    var result = t.Inherits(typeof(SimpleType));
                    // Assert
                    Expect(result)
                        .To.Be.False();
                }

                [Test]
                public void ShouldReturnFalseWhenOperatingTypeIsNull()
                {
                    // Arrange
                    var t = null as Type;
                    // Act
                    var result = t.Inherits(typeof(object));
                    // Assert
                    Expect(result)
                        .To.Be.False();
                }

                [TestFixture]
                public class Generics
                {
                    [Test]
                    public void ShouldReturnTrueWhenComparingRelatedGenerics()
                    {
                        // Arrange
                        var baseType = typeof(BaseGenericType<>);
                        var derivedType = typeof(DerivedGenericType<>);
                        // Act
                        var result = derivedType.Inherits(baseType);
                        // Assert
                        Expect(result)
                            .To.Be.True();
                    }
                }
            }

            public class BaseGenericType<T>
            {
            }

            public class DerivedGenericType<T>: BaseGenericType<T>
            {
            }

            public class SimpleType
            {
            }

            public class InheritedType : SimpleType
            {
            }

            public class GrandChildType : InheritedType
            {
            }

            public class AnotherType
            {
            }
        }

        public class ClassWithConstants
        {
            public const string CONSTANT1 = "Constant1";
            public const string CONSTANT2 = "Constant2";
            public const int CONSTANT3 = 1;
            public const bool CONSTANT4 = false;
        }

        [TestFixture]
        public class GetAllConstants
        {
            [Test]
            public void OperatingOnType_ShouldReturnDictionaryOfConstants()
            {
                //---------------Set up test pack-------------------
                var type = typeof(ClassWithConstants);
                var expected = new Dictionary<string, object>()
                {
                    ["CONSTANT1"] = ClassWithConstants.CONSTANT1,
                    ["CONSTANT2"] = ClassWithConstants.CONSTANT2,
                    ["CONSTANT3"] = ClassWithConstants.CONSTANT3,
                    ["CONSTANT4"] = ClassWithConstants.CONSTANT4,
                };

                //---------------Assert Precondition----------------

                //---------------Execute Test ----------------------
                var result = type.GetAllConstants();

                //---------------Test Result -----------------------
                Expect(result)
                    .To.Equal(expected);
            }

            [Test]
            public void OperatingOnType_WithGenericParameter_ShouldReturnDictionaryOfConstantsOfThatType()
            {
                //---------------Set up test pack-------------------
                var type = typeof(ClassWithConstants);
                var expected = new Dictionary<string, string>()
                {
                    ["CONSTANT1"] = ClassWithConstants.CONSTANT1,
                    ["CONSTANT2"] = ClassWithConstants.CONSTANT2,
                };
                //---------------Assert Precondition----------------

                //---------------Execute Test ----------------------
                var result = type.GetAllConstants<string>();

                //---------------Test Result -----------------------
                Expect(result)
                    .To.Equal(expected);
            }
        }

        [TestFixture]
        public class GetAllConstantValues
        {
            [Test]
            public void ShouldListAllConstantsOfThatType()
            {
                //---------------Set up test pack-------------------
                var type = typeof(ClassWithConstants);
                var expected = new object[]
                {
                    "Constant1",
                    "Constant2",
                    1,
                    false
                };

                //---------------Assert Precondition----------------

                //---------------Execute Test ----------------------
                var result = type.GetAllConstantValues();

                //---------------Test Result -----------------------
                Expect(result)
                    .To.Be.Equivalent.To(expected);
            }

            [Test]
            public void OfGenericType_ShouldListAllConstantsOfThatType()
            {
                //---------------Set up test pack-------------------
                var type = typeof(ClassWithConstants);
                var expected = new[]
                {
                    "Constant1",
                    "Constant2"
                };

                //---------------Assert Precondition----------------

                //---------------Execute Test ----------------------
                var result = type.GetAllConstantValues<string>();

                //---------------Test Result -----------------------
                Expect(result)
                    .To.Be.Equivalent.To(expected);
            }
        }

        [TestFixture]
        public class CanBeAssignedNull
        {
            [TestCase(typeof(int))]
            [TestCase(typeof(bool))]
            [TestCase(typeof(SomeEnums))]
            [TestCase(typeof(SomeStruct))]
            public void OperatingOnNonNullableType_ShouldReturnFalse(Type t)
            {
                //---------------Set up test pack-------------------

                //---------------Assert Precondition----------------

                //---------------Execute Test ----------------------
                var result = t.CanBeAssignedNull();

                //---------------Test Result -----------------------
                Expect(result)
                    .To.Be.False();
            }

            [TestCase(typeof(ClassWithConstants))]
            [TestCase(typeof(int[]))]
            [TestCase(typeof(int?))]
            [TestCase(typeof(string))]
            public void OperatingOnNullableType_ShouldReturnTrue(Type t)
            {
                //---------------Set up test pack-------------------

                //---------------Assert Precondition----------------

                //---------------Execute Test ----------------------
                var result = t.CanBeAssignedNull();

                //---------------Test Result -----------------------
                Expect(result)
                    .To.Be.True();
            }

            public enum SomeEnums
            {
            }

            public struct SomeStruct
            {
            }
        }

        [TestFixture]
        public class HasDefaultConstructor
        {
            public class TypeWithDefaultConstructor
            {
            }

            [Test]
            public void OperatingOnTypeWithDefaultConstructorOnly_ShouldReturnTrue()
            {
                //---------------Set up test pack-------------------
                var type = typeof(TypeWithDefaultConstructor);

                //---------------Assert Precondition----------------

                //---------------Execute Test ----------------------
                var result = type.HasDefaultConstructor();

                //---------------Test Result -----------------------
                Expect(result)
                    .To.Be.True();
            }

            public class TypeWithoutDefaultConstructor
            {
                public TypeWithoutDefaultConstructor(int id)
                {
                }
            }

            [Test]
            public void OperatingOnTypeWithoutDefaultConstructor_ShouldReturnFalse()
            {
                //---------------Set up test pack-------------------
                var type = typeof(TypeWithoutDefaultConstructor);

                //---------------Assert Precondition----------------

                //---------------Execute Test ----------------------
                var result = type.HasDefaultConstructor();

                //---------------Test Result -----------------------
                Expect(result)
                    .To.Be.False();
            }

            public class TypeWithDefaultAndParameteredConstructor
            {
                public TypeWithDefaultAndParameteredConstructor()
                {
                }

                public TypeWithDefaultAndParameteredConstructor(int id)
                {
                }
            }

            [Test]
            public void OperatingOnTypeWithDefaultConstructorAndParameteredConstructor_ShouldReturnTrue()
            {
                //---------------Set up test pack-------------------
                var type = typeof(TypeWithDefaultAndParameteredConstructor);

                //---------------Assert Precondition----------------

                //---------------Execute Test ----------------------
                var result = type.HasDefaultConstructor();

                //---------------Test Result -----------------------
                Expect(result)
                    .To.Be.True();
            }
        }

        [TestFixture]
        public class GetAllImplementedInterfaces
        {
            public interface ILevel4Again
            {
            }

            public interface ILevel4
            {
            }

            public interface ILevel3 : ILevel4, ILevel4Again
            {
            }

            public interface ILevel2 : ILevel3
            {
            }

            public interface ILevel1 : ILevel2
            {
            }

            public class Level1 : ILevel1
            {
            }

            [Test]
            public void ShouldReturnAllInterfacesAllTheWayDown()
            {
                //--------------- Arrange -------------------

                //--------------- Assume ----------------

                //--------------- Act ----------------------
                var result = typeof(Level1).GetAllImplementedInterfaces();

                //--------------- Assert -----------------------
                Expect(result)
                    .To.Be.Equivalent.To(
                        new[]
                        {
                            typeof(ILevel1),
                            typeof(ILevel2),
                            typeof(ILevel3),
                            typeof(ILevel4),
                            typeof(ILevel4Again)
                        });
            }
        }

        [TestFixture]
        public class IsDisposable
        {
            public class NotDisposable
            {
                public void Dispose()
                {
                    /* just to prevent mickey-mousery */
                }
            }

            public class DisposableTeen : IDisposable
            {
                public void Dispose()
                {
                    throw new NotImplementedException();
                }
            }

            [Test]
            public void IsDisposable_OperatingOnNonDisposableType_ShouldReturnFalse()
            {
                //--------------- Arrange -------------------
                var sut = typeof(NotDisposable);

                //--------------- Assume ----------------

                //--------------- Act ----------------------
                var result = sut.IsDisposable();

                //--------------- Assert -----------------------
                Expect(result)
                    .To.Be.False();
            }

            [Test]
            public void IsDisposable_OperatingOnDisposableType_ShouldReturnTrue()
            {
                //--------------- Arrange -------------------
                var sut = typeof(DisposableTeen);

                //--------------- Assume ----------------

                //--------------- Act ----------------------
                var result = sut.IsDisposable();

                //--------------- Assert -----------------------
                Expect(result)
                    .To.Be.True();
            }
        }

        [TestFixture]
        public class CanImplicitlyUpcastTo
        {
            [TestCase(typeof(byte), typeof(long))]
            [TestCase(typeof(byte), typeof(ushort))]
            [TestCase(typeof(byte), typeof(ulong))]
            [TestCase(typeof(short), typeof(long))]
            [TestCase(typeof(int), typeof(long))]
            [TestCase(typeof(float), typeof(double))]
            public void ShouldReturnTrueFor_(Type src, Type target)
            {
                // Arrange
                // Pre-Assert
                // Act
                var result = src.CanImplicitlyCastTo(target);
                // Assert
                Expect(result)
                    .To.Be.True();
            }

            [TestCase(typeof(int), typeof(string))]
            public void ShouldReturnFalseFor_(Type src, Type target)
            {
                // Arrange
                // Act
                var result = src.CanImplicitlyCastTo(target);
                // Assert
                Expect(result)
                    .To.Be.False();
            }
        }

        [TestFixture]
        public class DefaultValue
        {
            [TestCase(typeof(object), null)]
            [TestCase(typeof(string), null)]
            [TestCase(typeof(bool), false)]
            [TestCase(typeof(int), 0)]
            public void ShouldReturnDefaultValueForType(
                Type t,
                object expected)
            {
                // Arrange
                // Pre-assert
                // Act
                var result = t.DefaultValue();
                // Assert
                Expect(result).To.Equal(expected);
            }
        }

        [TestFixture]
        public class TryGetEnumerableItemType
        {
            [Test]
            public void ShouldBeAbleToGetEnumerableTypeOfArray()
            {
                // Arrange
                var arr = new int[0];
                // Pre-assert
                // Act
                var result = arr.GetType().TryGetEnumerableItemType();
                // Assert
                Expect(result).To.Equal(typeof(int));
            }

            [Test]
            public void ShouldReturnNullForNonCollection()
            {
                // Arrange
                // Act
                var result = typeof(int).TryGetEnumerableItemType();
                // Assert
                Expect(result)
                    .To.Be.Null();
            }
        }

        [TestFixture]
        public class Implements
        {
            public interface IInterface
            {
            }

            public class NotImplemented
            {
            }

            public class Implemented : IInterface
            {
            }

            public class DerivedImplemented : Implemented
            {
            }

            [Test]
            public void ShouldReturnFalseIfInterfaceNotImplemented()
            {
                // Arrange
                // Act
                var result = typeof(NotImplemented).Implements<IInterface>();
                // Assert
                Expect(result)
                    .To.Be.False();
            }

            [Test]
            public void ShouldReturnTrueIfInterfaceImmediatelyImplemented()
            {
                // Arrange
                // Act
                var result = typeof(Implemented).Implements<IInterface>();
                // Assert
                Expect(result)
                    .To.Be.True();
            }

            [Test]
            public void ShouldReturnTrueIfInterfaceImplementedByAncestor()
            {
                // Arrange
                // Act
                var result = typeof(DerivedImplemented).Implements<IInterface>();
                // Assert
                Expect(result)
                    .To.Be.True();
            }

            [Test]
            public void ShouldReturnTrueForInheritedInterface()
            {
                // Arrange
                var sut = typeof(Concrete<int>);
                // Act
                var result = sut.Implements(typeof(IContract));
                // Assert
                Expect(result)
                    .To.Be.True();
            }

            [Test]
            public void ShouldReturnTrueForInheritedGeneric()
            {
                // Arrange
                var sut = typeof(Concrete<int>);
                // Act
                var result = sut.Implements(typeof(IContract<int>));
                // Assert
                Expect(result)
                    .To.Be.True();
            }

            [Test]
            public void ShouldReturnTrueForGenerics()
            {
                // Arrange
                var sut = typeof(Concrete<>);
                // Act
                var result = sut.Implements(typeof(IContract<>));
                // Assert
                Expect(result)
                    .To.Be.True();
            }

            public class Concrete<T> : IContract<T>
            {
            }

            public interface IContract<T> : IContract
            {
            }

            public interface IContract
            {
            }
        }

        [TestFixture]
        public class SetStatic
        {
            [Test]
            public void ShouldSetStaticProperty()
            {
                // Arrange
                var t = typeof(HasStatics);
                var expected = GetRandomInt();
                // Act
                t.SetStatic(nameof(HasStatics.Id), expected);
                // Assert
                Expect(HasStatics.Id)
                    .To.Equal(expected);
            }

            [Test]
            public void ShouldSetStaticField()
            {
                // Arrange
                var t = typeof(HasStatics);
                var expected = GetRandomString();
                // Act
                t.SetStatic("_name", expected);
                // Assert
                Expect(new HasStatics().Name)
                    .To.Equal(expected);
            }

            [Test]
            public void ShouldImplicitlyUpCastValue()
            {
                // Arrange
                var t = typeof(HasStatics);
                var expected = (byte) GetRandomInt(1, 10);
                // Act
                t.SetStatic("Id", expected);
                // Assert
                Expect(HasStatics.Id)
                    .To.Equal(expected);
            }

            public class HasStatics
            {
                public string Name => _name;
                private static string _name = Guid.NewGuid().ToString();

                public static int Id { get; set; }
            }
        }

        [TestFixture]
        public class GetStatic
        {
            [Test]
            public void ShouldRetrieveStaticProperty()
            {
                // Arrange
                var t = typeof(HasStatics);
                var expected = GetRandomInt();
                // Act
                t.SetStatic("Id", expected);
                var result = t.GetStatic<int>("Id");
                // Assert
                Expect(result)
                    .To.Equal(expected);
            }

            [Test]
            public void ShouldRetrieveStaticField()
            {
                // Arrange
                var t = typeof(HasStatics);
                var expected = GetRandomString();
                // Act
                t.SetStatic("_name", expected);
                var result = t.GetStatic<string>("_name");
                // Assert
                Expect(result)
                    .To.Equal(expected);
            }

            public class HasStatics
            {
                public string Name => _name;
                private static string _name = Guid.NewGuid().ToString();

                private static int Id { get; set; }
            }
        }

        [TestFixture]
        public class TestingVirtualAndAbstract
        {
            [Test]
            public void ShouldFindVirtualProperty()
            {
                // Arrange
                var prop = typeof(Poco)
                    .GetProperty(nameof(Poco.Id));
                // Act
                var result = prop.IsVirtualOrAbstract();
                // Assert
                Expect(result)
                    .To.Be.True();
            }

            [Test]
            public void ShouldFindAbstractProperty()
            {
                // Arrange
                var prop = typeof(Poco)
                    .GetProperty(nameof(Poco.Foo));
                // Act
                var result = prop.IsVirtualOrAbstract();
                // Assert
                Expect(result)
                    .To.Be.True();
            }

            [Test]
            public void ShouldFindConcreteProperty()
            {
                // Arrange
                var prop = typeof(Poco)
                    .GetProperty(nameof(Poco.Name));
                // Act
                var result = prop.IsVirtualOrAbstract();
                // Assert
                Expect(result)
                    .To.Be.False();
            }

            [Test]
            public void ShouldFindAbstractMethod()
            {
                // Arrange
                var method = typeof(Poco)
                    .GetMethod(nameof(Poco.AbstractMethod));
                // Act
                var result = method.IsVirtualOrAbstract();
                // Assert
                Expect(result)
                    .To.Be.True();
            }

            [Test]
            public void ShouldFindVirtualMethod()
            {
                // Arrange
                var method = typeof(Poco)
                    .GetMethod(nameof(Poco.VirtualMethod));
                // Act
                var result = method.IsVirtualOrAbstract();
                // Assert
                Expect(result)
                    .To.Be.True();
            }

            [Test]
            public void ShouldFindConcreteMethod()
            {
                // Arrange
                var method = typeof(Poco)
                    .GetMethod(nameof(Poco.ConcreteMethod));
                // Act
                var result = method.IsVirtualOrAbstract();
                // Assert
                Expect(result)
                    .To.Be.False();
            }

            public abstract class Poco
            {
                public virtual int Id { get; set; }
                public string Name { get; set; }

                public abstract string Foo { get; set; }

                public abstract void AbstractMethod();

                public virtual void VirtualMethod()
                {
                }

                public void ConcreteMethod()
                {
                }
            }
        }

        [TestFixture]
        public class HasAttribute
        {
            public class ClassAttribute : Attribute
            {
            }

            public class BaseClassAttribute : Attribute
            {
            }

            public class MethodAttribute : Attribute
            {
            }

            public class BaseMethodAttribute : Attribute
            {
            }

            public class PropertyAttribute : Attribute
            {
            }

            public class BasePropertyAttribute : Attribute
            {
            }

            [BaseClass]
            public abstract class TestClassBase
            {
                [BaseProperty]
                public virtual int Id { get; set; }

                [BaseMethod]
                public virtual void DoNothing()
                {
                }
            }

            [Class]
            public class TestClass : TestClassBase
            {
                [Property]
                public override int Id { get; set; }

                [Method]
                public override void DoNothing()
                {
                }
            }

            [TestFixture]
            public class OperatingOnType
            {
                [Test]
                public void ShouldFindAttributeWhenPresent()
                {
                    // Arrange
                    var sut = typeof(TestClass);
                    // Act
                    Expect(sut.HasAttribute<ClassAttribute>())
                        .To.Be.True();
                    Expect(sut.HasAttribute<PropertyAttribute>())
                        .To.Be.False();
                    Expect(sut.HasAttribute<MethodAttribute>())
                        .To.Be.False();
                    // Assert
                }

                [Test]
                public void ShouldFindBaseClassAttribute()
                {
                    // Arrange
                    var sut = typeof(TestClass);
                    // Act
                    Expect(sut.HasAttribute<BaseClassAttribute>())
                        .To.Be.True();
                    Expect(sut.HasAttribute<BaseClassAttribute>(false))
                        .To.Be.False();
                    // Assert
                }
            }

            [TestFixture]
            public class OperatingOnMethodInfo
            {
                [Test]
                public void ShouldFindAttributeWhenPresent()
                {
                    // Arrange
                    var sut = typeof(TestClass)
                        .GetMethod(nameof(TestClass.DoNothing));
                    // Act
                    Expect(sut.HasAttribute<ClassAttribute>())
                        .To.Be.False();
                    Expect(sut.HasAttribute<PropertyAttribute>())
                        .To.Be.False();
                    Expect(sut.HasAttribute<MethodAttribute>())
                        .To.Be.True();
                    // Assert
                }

                [Test]
                public void ShouldFindBaseMethodAttribute()
                {
                    // Arrange
                    var sut = typeof(TestClass)
                        .GetMethod(nameof(TestClass.DoNothing));
                    // Act
                    Expect(sut.HasAttribute<BaseMethodAttribute>())
                        .To.Be.True();
                    Expect(sut.HasAttribute<BaseMethodAttribute>(false))
                        .To.Be.False();
                    // Assert
                }
            }

            [TestFixture]
            public class OperatingOnPropertyInfo
            {
                [Test]
                public void ShouldFindAttributeWhenPresent()
                {
                    // Arrange
                    var sut = typeof(TestClass)
                        .GetProperty(nameof(TestClass.Id));
                    // Act
                    Expect(sut.HasAttribute<ClassAttribute>())
                        .To.Be.False();
                    Expect(sut.HasAttribute<PropertyAttribute>())
                        .To.Be.True();
                    Expect(sut.HasAttribute<MethodAttribute>())
                        .To.Be.False();
                    // Assert
                }

                [Test]
                public void ShouldFindBaseAttribute()
                {
                    // Arrange
                    var sut = typeof(TestClass)
                        .GetProperty(nameof(TestClass.Id));

                    // Act
                    Expect(sut.HasAttribute<BasePropertyAttribute>())
                        .To.Be.True();
                    Expect(sut.HasAttribute<BasePropertyAttribute>(false))
                        .To.Be.False();
                    // Assert
                }
            }

            [Test]
            [Explicit("Speed test: proves that the non-generic path is _slightly_ more performant")]
            public void Speed()
            {
                // Arrange
                var sut = typeof(TestClass);
                var iterations = 1000000;
                bool foo;
                var classAttrib = typeof(ClassAttribute);
                var methodAttrib = typeof(MethodAttribute);
                var nonGeneric = Time(() =>
                {
                    foo = sut.GetCustomAttributes()
                        .Any(a => a?.GetType() == classAttrib);
                    foo = sut.GetCustomAttributes()
                        .Any(a => a?.GetType() == methodAttrib);
                }, iterations);
                var generic = Time(() =>
                {
                    foo = sut.GetCustomAttributes()
                        .OfType<ClassAttribute>()
                        .Any();
                    foo = sut.GetCustomAttributes()
                        .OfType<MethodAttribute>()
                        .Any();
                }, iterations);
                // Act
                // Assert
                Console.WriteLine($"Generic : {generic}");
                Console.WriteLine($"!Generic: {nonGeneric}");
            }
        }
    }
}