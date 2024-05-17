﻿using System;
using System.Linq;
using System.Reflection;
#if BUILD_PEANUTBUTTER_DUCKTYPING_INTERNAL
using Imported.PeanutButter.DuckTyping.AutoConversion;
#else
using PeanutButter.DuckTyping.AutoConversion;
#endif

#if BUILD_PEANUTBUTTER_DUCKTYPING_INTERNAL
namespace Imported.PeanutButter.DuckTyping.Shimming
#else
namespace PeanutButter.DuckTyping.Shimming
#endif
{
    /// <summary>
    /// Base class for common shim functionality
    /// </summary>
    // required to be public for source-embedding
    public abstract class ShimShamBase
    {
        private static MethodInfo GetTypeMakerMethod(string name)
        {
            return typeof(TypeMaker)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .First(mi => mi.Name == name &&
                             mi.IsGenericMethod &&
                             mi.GetParameters().Length == 0);
        }
        private readonly MethodInfo _genericMakeType = GetTypeMakerMethod(nameof(TypeMaker.MakeTypeImplementing));
        private readonly MethodInfo _genericFuzzyMakeType = GetTypeMakerMethod(nameof(TypeMaker.MakeFuzzyTypeImplementing));
        private static readonly MethodInfo GetDefaultMethodGeneric =
            typeof(ShimShamBase)
            .GetMethod(nameof(GetDefaultFor), BindingFlags.NonPublic | BindingFlags.Static);
        private TypeMaker _typeMaker;

        /// <summary>
        /// Gets the default value for a type
        /// </summary>
        /// <param name="correctType">Type to find the default value for</param>
        /// <returns>The value that would be returned by default(T) for that type</returns>
        public static object GetDefaultValueFor(Type correctType)
        {
            return GetDefaultMethodGeneric
                .MakeGenericMethod(correctType)
                .Invoke(null, null);
        }

        // ReSharper disable once UnusedMember.Local
#pragma warning disable S1144 // Unused private types or members should be removed
        private static T GetDefaultFor<T>()
        {
            return default(T);
        }
#pragma warning restore S1144 // Unused private types or members should be removed

        /// <summary>
        /// Converts a property value from original type to another type using the provided converter
        /// </summary>
        /// <param name="converter">Converts the value</param>
        /// <param name="propValue">Value to convert</param>
        /// <param name="toType">Required output type</param>
        /// <returns>Value converted to required output type, where possible</returns>
        protected object ConvertWith(
            IConverter converter,
            object propValue,
            Type toType)
        {
            var convertMethod = converter.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Single(mi => mi.Name == "Convert" && mi.ReturnType == toType);
            // ReSharper disable once RedundantExplicitArrayCreation
            return convertMethod.Invoke(converter, new object[] { propValue });
        }

        /// <summary>
        /// Creates a new type to implement the requested interface type
        /// Used internally when fleshing out non-primitive properties
        /// </summary>
        /// <param name="type">Type to implement</param>
        /// <param name="isFuzzy">Flag to allow (or not) approximate / fuzzy ducking</param>
        /// <returns>Type implementing requested interface</returns>
        protected Type MakeTypeToImplement(Type type, bool isFuzzy)
        {
            var typeMaker = _typeMaker ?? (_typeMaker = new TypeMaker());
            var genericMethod = isFuzzy ? _genericFuzzyMakeType : _genericMakeType;
            var specific = genericMethod.MakeGenericMethod(type);
            return specific.Invoke(typeMaker, null) as Type;
        }
    }
}