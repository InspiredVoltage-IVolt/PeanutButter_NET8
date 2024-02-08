using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using PeanutButter.TestUtils.AspNetCore.Builders;
using PeanutButter.TestUtils.AspNetCore.Fakes;
using PeanutButter.Utils;

namespace PeanutButter.TestUtils.AspNetCore.Tests;

[TestFixture]
public class TestFormBuilder
{
    [TestFixture]
    public class DefaultBuild
    {
        [Test]
        public void ShouldProduceForm()
        {
            // Arrange
            // Act
            var result = FormBuilder.BuildDefault();
            // Assert
            Expect(result)
                .Not.To.Be.Null();
            Expect(result.Keys)
                .To.Be.Empty();
        }
    }

    [Test]
    public void ShouldBeAbleToAddFields()
    {
        // Arrange
        var field1 = GetRandomString(10);
        var value1 = GetRandomString();
        var field2 = GetRandomString(10);
        var value2 = GetRandomInt();
        var expected = new Dictionary<string, StringValues>()
        {
            [field1] = value1,
            [field2] = $"{value2}"
        };
        // Act
        var result = FormBuilder.Create()
            .WithField(field1, value1)
            .WithField(field2, value2)
            .Build() as FakeFormCollection;
        // Assert
        Expect(result.FormValues)
            .To.Deep.Equal(expected);
    }

    [Test]
    public void ShouldBeAbleToAddFiles()
    {
        // Arrange
        var name = GetRandomString(10);
        var fileName = GetRandomFileName();
        var contents = GetRandomWords();

        // Act
        var result = FormBuilder.Create()
            .WithFile(contents, name, fileName)
            .Build();
        // Assert
        Expect(result.Files.Count)
            .To.Equal(1);
        var file = result.Files[0];
        Expect(file.Name)
            .To.Equal(name);
        Expect(file.FileName)
            .To.Equal(fileName);
        using var s = file.OpenReadStream();
        Expect(s.ReadAllText())
            .To.Equal(contents);
    }

    [TestFixture]
    public class RandomFormCollection
    {
        [Test]
        public void FakeShouldHaveAtLeastOneFieldAndNoFiles()
        {
            // Arrange
            // Act
            var result = GetRandom<FakeFormCollection>();
            // Assert
            Expect(result.Files.Count)
                .To.Equal(0);
            Expect(result.FormValues.Keys)
                .Not.To.Be.Empty();
        }

        [Test]
        public void InterfaceShouldHaveAtLeastOneFieldAndNoFiles()
        {
            // Arrange
            // Act
            var result = GetRandom<IFormCollection>();
            // Assert
            Expect(result.Files.Count)
                .To.Equal(0);
            Expect(result.Keys)
                .Not.To.Be.Empty();
        }
    }
}