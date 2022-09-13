﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using NUnit.Framework;
using PeanutButter.TestUtils.AspNetCore.Builders;
using NExpect;
using PeanutButter.TestUtils.AspNetCore.Utils;
using static NExpect.Expectations;
using static NExpect.AspNetCoreExpectations;

namespace PeanutButter.TestUtils.AspNetCore.Tests
{
    [TestFixture]
    public class TestModelBindingContextBuilder
    {
        [TestFixture]
        public class DefaultBuild
        {
            [Test]
            public void ShouldSetActionContextToANewControllerContext()
            {
                // Arrange
                // Act
                var result1 = BuildDefault();
                var result2 = BuildDefault();
                // Assert
                Expect(result1.ActionContext)
                    .Not.To.Be.Null()
                    .And
                    .To.Be.An.Instance.Of<ControllerContext>();
                Expect(result1.ActionContext)
                    .Not.To.Be(result2.ActionContext);
            }

            [TestCase("Model")]
            public void ShouldSetBinderModelNameTo_(string expected)
            {
                // Arrange
                // Act
                var result = BuildDefault();
                // Assert
                Expect(result.ModelName)
                    .To.Equal(expected);
            }

            [Test]
            public void ShouldSetBindingSourceBody()
            {
                // Arrange
                // Act
                var result = BuildDefault();
                // Assert
                Expect(result.BindingSource)
                    .To.Equal(BindingSource.Body);
            }

            [TestCase("Field")]
            public void ShouldSetTheFieldNameTo_(string expected)
            {
                // Arrange
                // Act
                var result = BuildDefault();
                // Assert
                Expect(result.FieldName)
                    .To.Equal(expected);
            }

            [Test]
            public void ShouldSetIsTopLevelObjectTrue()
            {
                // Arrange
                // Act
                var result = BuildDefault();
                // Assert
                Expect(result.IsTopLevelObject)
                    .To.Be.True();
            }

            [Test]
            public void ShouldSetEmptyModelMetadata()
            {
                // Arrange
                var defaultCompositeMetadataDetailsProvider = new DefaultCompositeMetadataDetailsProvider(
                    new IMetadataDetailsProvider[0]
                );
                var defaultModelMetadataProvider = new DefaultModelMetadataProvider(
                    defaultCompositeMetadataDetailsProvider,
                    new DefaultOptions()
                );
                var defaultMetadataDetails = new DefaultMetadataDetails(
                    ModelMetadataIdentity.ForType(typeof(object)),
                    ModelAttributes.GetAttributesForType(typeof(object))
                );
                var expectedMetaData = new DefaultModelMetadata(
                    defaultModelMetadataProvider, defaultCompositeMetadataDetailsProvider,
                    defaultMetadataDetails
                );
                // Act
                var result = BuildDefault();
                // Assert
                var metaData = result.ModelMetadata;
                Expect(metaData)
                    .Not.To.Be.Null();
                Expect(metaData)
                    .To.Deep.Equal(
                        expectedMetaData
                    );
            }

            [Test]
            public void ShouldSetModelToEmptyObject()
            {
                // Arrange
                // Act
                var result = BuildDefault();
                // Assert
                Expect(result.Model)
                    .To.Deep.Equal(new { });
            }

            [Test]
            public void ShouldSetEmptyModelDictionary()
            {
                // Arrange
                // Act
                var result = BuildDefault();
                // Assert
                Expect(result.ModelState)
                    .Not.To.Be.Null();
                Expect(result.ModelState)
                    .To.Be.Empty();
            }

            [Test]
            public void ShouldSetAlwaysTruePropertyFilter()
            {
                // Arrange
                // Act
                var result = BuildDefault();
                // Assert
                Expect(result.PropertyFilter)
                    .Not.To.Be.Null();
                Expect(result.PropertyFilter(null))
                    .To.Be.True();
            }

            [Test]
            public void ShouldSetValidationState()
            {
                // Arrange
                // Act
                var result = BuildDefault();
                // Assert
                Expect(result.ValidationState)
                    .Not.To.Be.Null();
                // TODO: validate that it's empty
            }

            [Test]
            [Ignore("WIP")]
            public void ShouldSetReflectiveValueProvider()
            {
                // Arrange
                
                // Act
                // Assert
            }

            [Test]
            [Ignore("WIP")]
            public void ShouldSetEmptyModelBindingResult()
            {
                // Arrange
                
                // Act
                // Assert
            }

            private static ModelBindingContext BuildDefault()
            {
                return ModelBindingContextBuilder.BuildDefault();
            }
        }
    }
}