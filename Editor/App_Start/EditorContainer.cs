using System.Linq;
using BeeGraph.IoC;
using SimpleInjector;

namespace BeeGraph.Editor
{
    public static class EditorContainer
    {
        public static Container Container;

        static EditorContainer()
        {
            Container = InitContainer();
        }
        private static Container InitContainer()
        {
            var container = new Container();


            RegisterDefaultBindings(container);
            container.Verify();
            return container;
        }

        private static void RegisterDefaultBindings(Container container)
        {
            var defaultBindings = IoC.IoC.Container.GetCurrentRegistrations().Except(container.GetCurrentRegistrations(), InstanceProducerComparer.Instance).ToList();
            defaultBindings.ForEach(b => Register(b, container));
        }

        private static void Register(InstanceProducer instanceProducer, Container container) => container.Register(instanceProducer.ServiceType, instanceProducer.Registration.ImplementationType);
    }
}