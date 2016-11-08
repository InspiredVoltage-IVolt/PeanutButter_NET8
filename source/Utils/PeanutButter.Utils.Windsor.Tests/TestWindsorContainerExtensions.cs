﻿using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NSubstitute;
using NUnit.Framework;
using PeanutButter.Utils.Windsor.Tests.TestClasses;

namespace PeanutButter.Utils.Windsor.Tests
{
    [TestFixture]
    public class TestWindsorContainerExtensions : AssertionHelper
    {
        [Test]
        public void RegisterAllOneToOneResolutionsAsTransientFrom_GivenNoAssemblies_ShouldThrowArgumentException()
        {
            //---------------Set up test pack-------------------
            var container = Substitute.For<IWindsorContainer>();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            Expect(
                () => container.RegisterAllOneToOneResolutionsAsTransientFrom(),
                Throws.Exception.InstanceOf<ArgumentException>()
            );
            //---------------Test Result -----------------------
        }

        [Test]
        public void RegisterAllOneToOneResolutionsAsTransientFrom_ShouldNotRegisterInterfaceWithoutResolution()
        {
            //---------------Set up test pack-------------------
            var container = Create();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            container.RegisterAllOneToOneResolutionsAsTransientFrom(GetType().Assembly);

            //---------------Test Result -----------------------
            Expect(
                () => container.Resolve<IInterfaceWithNoResolutions>(),
                Throws.Exception.InstanceOf<ComponentNotFoundException>()
            );
        }

        [Test]
        public void RegisterAllOneToOneResolutionsAsTransientFrom_ShouldNotRegisterInterfaceWithMultipleResolutions()
        {
            //---------------Set up test pack-------------------
            var container = Create();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            container.RegisterAllOneToOneResolutionsAsTransientFrom(GetType().Assembly);

            //---------------Test Result -----------------------
            Assert.Throws<ComponentNotFoundException>(() => container.Resolve<IInterfaceWithMultipleResolutions>());
        }

        [Test]
        public void RegisterAllOneToOneResolutionsAsTransientFrom_ShouldRegisterOneToOneResolution_IgnoringAbstract()
        {
            //---------------Set up test pack-------------------
            var container = Create();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            container.RegisterAllOneToOneResolutionsAsTransientFrom(GetType().Assembly);

            //---------------Test Result -----------------------
            var result = container.Resolve<IInterfaceWithOneResolution>();
            Expect(result, Is.Not.Null);
            Expect(result, Is.InstanceOf<ImplementsInterfaceWithOneResolution>());
        }


        [Test]
        public void RegisterAllOneToOneResolutionsAsTransientFrom_WhenServiceAlreadyRegistered_ShouldNotAttemptToReRegister()
        {
            //---------------Set up test pack-------------------
            var container = Create();
            container.Register(Component.For<IInterfaceForSingleton>().ImplementedBy<ImplementsInterfaceForSingleton>().LifestyleSingleton());
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            // Castle Windsor chucks if you try to register the same service twice
            Expect(
                () => container.RegisterAllOneToOneResolutionsAsTransientFrom(GetType().Assembly),
                Throws.Nothing
            );

            //---------------Test Result -----------------------
        }

        private interface InterfacePart1
        {
        }

        private interface InterfacePart2
        {
        }

        private class Implementation : InterfacePart1, InterfacePart2
        {
        }


        [Test]
        public void RegisterAllOneToOneResolutionsAsTransientFrom_WhenImplementationAlreadyRegisteredForDifferentService_ShouldNotAttemptToReRegister()
        {
            //---------------Set up test pack-------------------
            var container = Create();
            container.RegisterTransient<InterfacePart1, Implementation>();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            Expect(
                () => container.RegisterAllOneToOneResolutionsAsTransientFrom(GetType().Assembly),
                Throws.Nothing
            );

            //---------------Test Result -----------------------
        }

        [Test]
        public void RegisterAllOneToOneResolutionsAsTransientFrom_ShouldNotRegisterInterfacesImplementingInterfaces()
        {
            //---------------Set up test pack-------------------
            var container = Create();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            container.RegisterAllOneToOneResolutionsAsTransientFrom(GetType().Assembly);

            //---------------Test Result -----------------------
            Expect(
                () => container.Resolve<IInterfaceWithOneResolutionInheritor>(),
                Throws.Exception.InstanceOf<ComponentNotFoundException>()
            );
        }

        [Test]
        public void RegisterAllOneToOneResulitionsAsTransientFrom_ShouldNotRegisterAbstractClasses()
        {
            //---------------Set up test pack-------------------
            var container = Create();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            container.RegisterAllOneToOneResolutionsAsTransientFrom(GetType().Assembly);

            //---------------Test Result -----------------------
            Expect(
                () => container.Resolve<IInterfaceForAbstractClass>(),
                Throws.Exception.InstanceOf<ComponentNotFoundException>()
            );
        }

        [Test]
        public void RegisterAllMvcControllersFrom_GivenNoAssemblies_ShouldThrow()
        {
            //---------------Set up test pack-------------------
            var container = Create();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            Expect(
                () => container.RegisterAllMvcControllersFrom(),
                Throws.Exception.InstanceOf<ArgumentException>()
            );

            //---------------Test Result -----------------------
        }

        [Test]
        public void RegisterAllMvcControllersFrom_GivenAssemblyWithNoControllers_ShouldNotThrow()
        {
            //---------------Set up test pack-------------------
            var container = Create();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            Expect(
                () => container.RegisterAllMvcControllersFrom(typeof(WindsorContainerExtensions).Assembly),
                Throws.Nothing
            );

            //---------------Test Result -----------------------
        }

        [Test]
        public void RegisterAllMvcControllersFrom_GivenAssemblyContainingControllerClasses_ShouldRegisterThem()
        {
            //---------------Set up test pack-------------------
            var container = Create();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            Assert.DoesNotThrow(() => container.RegisterAllMvcControllersFrom(GetType().Assembly));

            //---------------Test Result -----------------------
            Expect(container.Resolve<HomeController>(), Is.InstanceOf<HomeController>());
            Expect(container.Resolve<AccountController>(), Is.InstanceOf<AccountController>());
        }

        [Test]
        public void RegisterAllApiControllersFrom_GivenNoAssemblies_ShouldThrow()
        {
            //---------------Set up test pack-------------------
            var container = Create();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            Expect(
                () => container.RegisterAllApiControllersFrom(),
                Throws.Exception.InstanceOf<ArgumentException>()
            );

            //---------------Test Result -----------------------
        }

        [Test]
        public void RegisterAllApiControllersFrom_GivenAssemblyWithNoControllers_ShouldNotThrow()
        {
            //---------------Set up test pack-------------------
            var container = Create();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            Expect(
                () => container.RegisterAllApiControllersFrom(typeof(WindsorContainerExtensions).Assembly),
                Throws.Nothing
            );

            //---------------Test Result -----------------------
        }

        [Test]
        public void RegisterAllApiControllersFrom_GivenAssemblyContainingControllerClasses_ShouldRegisterThem()
        {
            //---------------Set up test pack-------------------
            var container = Create();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            Expect(
                () => container.RegisterAllApiControllersFrom(GetType().Assembly),
                Throws.Nothing
            );

            //---------------Test Result -----------------------
            Expect(container.Resolve<SomeApiController>(), Is.InstanceOf<SomeApiController>());
        }


        [Test]
        public void RegisterSingleton_Generic_ShouldRegisterServiceAsSingleton()
        {
            //---------------Set up test pack-------------------
            var container = Create();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            container.RegisterSingleton<ISingletonService, SingletonService>();
            var result1 = container.Resolve<ISingletonService>();
            var result2 = container.Resolve<ISingletonService>();

            //---------------Test Result -----------------------
            Expect(result1, Is.Not.Null);
            Expect(result2, Is.Not.Null);
            Expect(result1, Is.InstanceOf<SingletonService>());
            Expect(result2, Is.InstanceOf<SingletonService>());
            Expect(result1, Is.EqualTo(result2));
        }

        [Test]
        public void RegisterSingleton_GivenTwoTypes_ShouldRegisterServiceAsSingleton()
        {
            //---------------Set up test pack-------------------
            var container = Create();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            container.RegisterSingleton(typeof(ISingletonService), typeof(SingletonService));
            var result1 = container.Resolve<ISingletonService>();
            var result2 = container.Resolve<ISingletonService>();

            //---------------Test Result -----------------------
            Expect(result1, Is.Not.Null);
            Expect(result2, Is.Not.Null);
            Expect(result1, Is.InstanceOf<SingletonService>());
            Expect(result2, Is.InstanceOf<SingletonService>());
            Expect(result1, Is.EqualTo(result2));
        }

        [Test]
        public void RegisterTransient_ShouldRegisterServiceAsTransient()
        {
            //---------------Set up test pack-------------------
            var container = Create();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            container.RegisterTransient<IInterfaceWithMultipleResolutions, ImplementsInterfaceWithMultipleResolutions1>();
            var result1 = container.Resolve<IInterfaceWithMultipleResolutions>();
            var result2 = container.Resolve<IInterfaceWithMultipleResolutions>();

            //---------------Test Result -----------------------
            Expect(result1, Is.Not.Null);
            Expect(result2, Is.Not.Null);
            Expect(result1, Is.InstanceOf<ImplementsInterfaceWithMultipleResolutions1>());
            Expect(result2, Is.InstanceOf<ImplementsInterfaceWithMultipleResolutions1>());
            Expect(result1, Is.Not.EqualTo(result2));
        }

        [Test]
        public void RegisterPerWebRequest_ShouldRegisterPerWebRequest()
        {
            //---------------Set up test pack-------------------
            var container = Substitute.For<IWindsorContainer>();
            var registrations = new List<IRegistration>();
            container.Register(Arg.Any<IRegistration>())
                .Returns(ci =>
                {
                    registrations.Add((ci.Args()[0] as IRegistration[])[0]);
                    return container;
                });

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            container.RegisterPerWebRequest<ISingletonService, SingletonService>();

            //---------------Test Result -----------------------
            var registration = registrations.Single();
            Expect(registration, Is.Not.Null);

            var lifestyle = registration.GetOrDefault<Castle.MicroKernel.Registration.Lifestyle.LifestyleGroup<ISingletonService>>("LifeStyle");
            Expect(lifestyle, Is.Not.Null);
            // TODO: actually prove PerWebRequest lifestyle...
        }

        private interface IDependencyForSingleInstanceBase
        {
        }
        private interface IDependencyForSingleInstance : IDependencyForSingleInstanceBase
        {
        }

        private class DependencyForSingleInstance : IDependencyForSingleInstance
        {
        }

        [Test]
        public void RegisterInstance_ShouldRegisterSingleProvidedInstanceForResolution()
        {
            //---------------Set up test pack-------------------
            var container = new WindsorContainer();
            var instance = new DependencyForSingleInstance();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            container.RegisterInstance<IDependencyForSingleInstance>(instance);
            var result1 = container.Resolve<IDependencyForSingleInstance>();
            var result2 = container.Resolve<IDependencyForSingleInstance>();

            //---------------Test Result -----------------------
            Expect(result1, Is.Not.Null);
            Expect(result2, Is.Not.Null);
            Expect(result1, Is.EqualTo(result2));
            Expect(result1, Is.EqualTo(instance));
            Expect(result2, Is.EqualTo(instance));
        }

        [Test]
        public void RegisterInstance_ShouldRegisterSingleProvidedInstanceForResolutionOnMultipleAttemptsWithDifferentInterface()
        {
            //---------------Set up test pack-------------------
            var container = new WindsorContainer();
            var instance = new DependencyForSingleInstance();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            container.RegisterInstance<IDependencyForSingleInstance>(instance);
            container.RegisterInstance<IDependencyForSingleInstanceBase>(instance);
            var result1 = container.Resolve<IDependencyForSingleInstance>();
            var result2 = container.Resolve<IDependencyForSingleInstanceBase>();

            //---------------Test Result -----------------------
            Expect(result1, Is.Not.Null);
            Expect(result2, Is.Not.Null);
            Expect(result1, Is.EqualTo(result2));
            Expect(result1, Is.EqualTo(instance));
            Expect(result2, Is.EqualTo(instance));
        }

        [Test]
        public void RegisterInstance_ShouldNotThrowIfTryingToRegisterTwiceForTheSameInterfaceAndImplementation()
        {
            //---------------Set up test pack-------------------
            var container = new WindsorContainer();
            var instance = new DependencyForSingleInstance();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            container.RegisterInstance<IDependencyForSingleInstance>(instance);

            Expect(() =>
                container.RegisterInstance<IDependencyForSingleInstance>(instance),
                Throws.Exception.InstanceOf<ComponentRegistrationException>()
            );
            //---------------Test Result -----------------------
        }



        private static IWindsorContainer Create()
        {
            return new WindsorContainer();
        }
    }
}
