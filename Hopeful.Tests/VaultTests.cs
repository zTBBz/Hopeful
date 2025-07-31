using Hopeful.Injection;
using System.Diagnostics;
using System.Reflection;

namespace Hopeful.Tests;

[TestFixture]
public class VaultTests
{
    public class InjectionsTests
    {
        [Test]
        public void LoadInjections_FromAssembly()
        {
            var vault = new Vault();
            vault.LoadInjections(Assembly.GetExecutingAssembly());
            var injections = vault.Injections;

            Assert.That(injections.Count(), Is.EqualTo(3));
        }
    }

    public class ServiceTests
    {
        [Service]
        public class ServiceWithInjection
        {
            [Inject]
            private readonly InnerService inner = null!;
            public void Test() => inner.Test();
        }

        [Service]
        public class InnerService
        {
            public void Test() => Assert.Pass();
        }

        [Test]
        public void LoadServices_InjectServiceInService()
        {
            var vault = new Vault();
            vault.LoadInjections(Assembly.GetExecutingAssembly());
            vault.LoadServices(Assembly.GetExecutingAssembly());

            vault.InjectService<ServiceWithInjection>().Test();
        }

        [Test]
        public void LoadServices()
        {
            var vault = new Vault();
            vault.LoadServices(Assembly.GetExecutingAssembly());
            var services = vault.Services;

            Assert.That(services.Count(), Is.EqualTo(5));
        }

        [Test]
        public void ExtractService_ResolveService()
        {
            var vault = new Vault();
            var service = new MyService();

            vault.ExtractService<IMyService>(service);
            var resolved = vault.InjectService<IMyService>();

            Assert.That(resolved, Is.SameAs(service));
        }

        [Test]
        public void ExtractKeyedServices_ResolveKeyedServices()
        {
            var vault = new Vault();
            var service1 = new MyService();
            var service2 = new MyService2();
            var service3 = new MyService3();

            vault.ExtractService<IMyService>(service1, 1);
            vault.ExtractService<IMyService>(service2, 2);
            vault.ExtractService<IMyService>(service3, 3);

            var resolved1 = vault.InjectService<IMyService>(1);
            var resolved2 = vault.InjectService<IMyService>(2);
            var resolved3 = vault.InjectService<IMyService>(3);

            Assert.Multiple(() =>
            {
                Assert.That(resolved1, Is.SameAs(service1));
                Assert.That(resolved2, Is.SameAs(service2));
                Assert.That(resolved3, Is.SameAs(service3));
            });

            try
            {
                vault.InjectService(typeof(IMyService));
            }
            catch
            {
                Assert.Pass();
            }
        }
    }

    public class DecoratorTests
    {
        [Test]
        public void LoadDecorators()
        {
            var vault = new Vault();
            vault.LoadServices(Assembly.GetExecutingAssembly());
            vault.LoadDecorators(Assembly.GetExecutingAssembly());

            Assert.That(vault.Decorators.Count(), Is.EqualTo(3));
        }

        [Test]
        public void InvokeDecorator()
        {
            var vault = new Vault();
            vault.ExtractService<IMyService, MyService>();
            vault.ExtractDecorator<IMyService, MyDecorator>();

            var service = vault.InjectService<IMyService>();
            service.Test();
        }

        [Test]
        public void InvokeChainDecorators()
        {
            var vault = new Vault();

            vault.ExtractService<IMyService, MyService>();
            vault.ExtractDecorator<IMyService, MyDecorator1>();
            vault.ExtractDecorator<IMyService, MyDecorator2>();
            vault.ExtractDecorator<IMyService, MyDecorator3>();

            var service = vault.InjectService<IMyService>();
            service.Test();
        }

        [Test]
        public void InvokeTargetDecorator()
        {
            var vault = new Vault();

            vault.ExtractService<IMyService, MyService>();
            vault.ExtractDecorator<IMyService, MyDecorator1>();
            vault.ExtractDecorator<IMyService, MyDecorator2>();
            vault.ExtractDecorator<IMyService, MyDecorator3>();

            var service = vault.InjectService<IMyService, MyDecorator3>();
            service.Test();
        }

        [Test]
        public void InvokeKeyedTargetDecorator()
        {
            var vault = new Vault();

            vault.ExtractService<IMyService, MyService>(1);
            vault.ExtractDecorator<IMyService, MyDecorator1>();
            vault.ExtractDecorator<IMyService, MyDecorator2>();
            vault.ExtractDecorator<IMyService, MyDecorator3>();
            vault.ExtractDecorator<IMyService, MyDecorator3>(1);

            var service = vault.InjectService<IMyService, MyDecorator3>(1);
            service.Test();
        }

        [Test]
        public void InvokeGenericDecorator()
        {
            var vault = new Vault();

            vault.ExtractService<IMyService2, MyService2>();
            vault.ExtractDecorator<IMyService2, MyGenericDecorator>();

            var service = vault.InjectService<IMyService2, MyGenericDecorator>();
            service.Test();
        }

        [Decorator(typeof(IMyService))]
        public class MyDecorator(IMyService inner) : IMyService
        {
            private readonly IMyService inner = inner;

            public void Test()
            {
                inner.Test();

                Assert.Pass();
            }
        }

        [Decorator(typeof(IMyService))]
        public class MyDecorator1(IMyService inner) : IMyService
        {
            private readonly IMyService inner = inner;

            public void Test()
            {
                inner.Test();
                Debug.Write("Called Test in MyDecorator1!");
            }
        }
        public class MyDecorator2(IMyService inner) : IMyService
        {
            private readonly IMyService inner = inner;

            public void Test()
            {
                inner.Test();
                Debug.Write("Called Test in MyDecorator2!");
            }
        }
        public class MyDecorator3(IMyService inner) : IMyService
        {
            private readonly IMyService inner = inner;

            public void Test()
            {
                inner.Test();

                Assert.Pass();
            }
        }

        public sealed class MyGenericDecorator(IMyService2 inner) : Decorator<IMyService2>(inner), IMyService2
        {
            public void Test()
            {
                Inner.Test();

                Assert.Pass();
            }
        }

        public class MyService2 : IMyService2
        {
            public void Test()
            {
                Debug.Write("Called Test in MyService!");
            }
        }
        public interface IMyService2
        {
            void Test();
        }
    }

    public interface IMyService
    {
        void Test();
    }

    [Service]
    public class MyService : IMyService
    {
        public void Test()
        {
            Debug.Write("Called Test in MyService!");
        }
    }

    [Service]
    public class MyService2 : IMyService
    {
        public void Test() { }
    }

    [Service]
    public class MyService3 : IMyService
    {
        public void Test() { }
    }

    public class MyInjection
    {
        [Inject]
        public MyService2 service = null!;
    }

    public class MyInjection2
    {
        [Inject]
        public MyService3 service = null!;
    }
}
