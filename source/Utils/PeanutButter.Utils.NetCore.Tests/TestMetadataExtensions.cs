﻿using static PeanutButter.Utils.PyLike;

namespace PeanutButter.Utils.Tests
{
    [TestFixture]
    public class TestMetadataExtensions
    {
        [Test]
        public void ShouldBeAbleToSetAndRetrieveAValue()
        {
            // Arrange
            var target = new { };
            var key = GetRandomString(2);
            var value = GetRandomInt();

            // Pre-Assert

            // Act
            target.SetMetadata(key, value);
            var result = target.GetMetadata<int>(key);

            // Assert
            Expect(result).To.Equal(value);
        }

        [Test]
        public void ShouldBeAbleToUpdateAValue()
        {
            // Arrange
            var target = new { };
            var key = GetRandomString(2);
            var value = GetRandomInt();
            var newValue = GetAnother(value);
            // Pre-assert
            // Act
            target.SetMetadata(key, value);
            var result1 = target.GetMetadata<int>(key);
            target.SetMetadata(key, newValue);
            var result2 = target.GetMetadata<int>(key);
            // Assert
            Expect(result1).To.Equal(value);
            Expect(result2).To.Equal(newValue);
        }

        [Test]
        public void RetrievingMetadataForNonExistingKeys_WhenNoMetadataAtAll_ShouldReturnDefaultForT()
        {
            // Arrange
            var target = new { };
            // Pre-assert
            // Act
            var result = target.GetMetadata<int>(GetRandomString(2));
            // Assert
            Expect(result).To.Equal(default(int));
        }

        [Test]
        public void RetrievingMetadataForNonExistingKeys_WhenHaveOtherMetadata_ShouldReturnDefaultForT()
        {
            // Arrange
            var target = new { };
            var have = GetRandomString(2);
            var test = GetAnother(have);
            var haveValue = GetRandomInt(1);
            target.SetMetadata(have, haveValue);
            var expected = GetAnother(haveValue);
            // Pre-assert
            // Act
            var result = target.GetMetadata(test, expected);
            // Assert
            Expect(result).To.Equal(expected);
        }

        [Test]
        public void RetrievingMetadataForNonExistingKeys_GivenDefaultValue_ShouldReturnThat()
        {
            // Arrange
            var target = new { };
            var expected = !default(bool);
            // Pre-assert
            // Act
            var result = target.GetMetadata(GetRandomString(2), expected);
            // Assert
            Expect(result).To.Equal(expected);
        }

        [Test]
        public void ShouldBeAbleToInformIfThereIsMetadata()
        {
            // Arrange
            var target = new { };
            var key = GetRandomString(2);
            var value = GetRandomString(4);
            target.SetMetadata(key, value);
            // Pre-Assert

            // Act
            var result = target.HasMetadata<string>(key);

            // Assert
            Expect(result).To.Be.True();
        }

        [Test]
        public void ShouldBeAbleToDeleteMetadata()
        {
            // Arrange
            var target = new { };
            var key = GetRandomString(2);
            var value = GetRandomString(4);
            target.SetMetadata(key, value);

            // Act
            Expect(target.GetMetadata<string>(key))
                .To.Equal(value);
            target.DeleteMetadata(key);
            // Assert
            Expect(target.HasMetadata(key))
                .To.Be.False();
        }

        [Test]
        public void ShouldBeAbleToDeleteAllMetadata()
        {
            // Arrange
            var target = new { };
            var key = GetRandomString(2);
            var value = GetRandomString(4);
            target.SetMetadata(key, value);

            // Act
            Expect(target.GetMetadata<string>(key))
                .To.Equal(value);
            target.DeleteMetadata();
            // Assert
            Expect(target.HasMetadata(key))
                .To.Be.False();
        }

        [Test]
        public void ShouldGcMetaData()
        {
            var key = GetRandomString(2);
            ArrangeAndPreAssertForGcTest(key);

            // Act
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);

            // Assert
            Expect(MetadataExtensions.TrackedObjectCount()).To.Equal(0);
        }

        private static void ArrangeAndPreAssertForGcTest(string key)
        {
            // this code needs to be in a different scope to force
            //  the loss of reference to target
            // Arrange
            GC.Collect();
            var target = new { foo = "bar" };
            var value = GetRandomBoolean();
            target.SetMetadata(key, value);

            // Pre-Assert
            Expect(target.HasMetadata<bool>(key)).To.Be.True();
            Expect(MetadataExtensions.TrackedObjectCount()).To.Equal(1);
        }

        [Test]
        public void ShouldBeAbleToCopyAllMetadataToAnotherObject()
        {
            // Arrange
            var obj1 = new object();
            var obj2 = new object();
            var id = GetRandomInt();
            var name = GetRandomString();
            obj1.SetMetadata("id", id);
            obj1.SetMetadata("name", name);

            // Act
            obj1.CopyAllMetadataTo(obj2);
            // Assert
            Expect(obj2.GetMetadata<int>("id"))
                .To.Equal(id);
            Expect(obj2.GetMetadata<string>("name"))
                .To.Equal(name);
        }

        [Test]
        public void ShouldBeThreadSafe()
        {
            // Arrange
            var target = new { };
            var keys = Range(0, 256).Select(
                _ => GetRandomString(32)
            ).ToArray();
            // Act
            Expect(() =>
            {
                Parallel.For(
                    0, keys.Length, (i, state) => target.SetMetadata(
                        keys[i],
                        GetRandomString(10)
                    )
                );
            }).Not.To.Throw();
            // Assert
            keys.ForEach(k =>
                Expect(target.HasMetadata(k))
                    .To.Be.True()
            );
        }
    }
}