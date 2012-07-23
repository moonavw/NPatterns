using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NPatterns.Messaging;
using Ninject;
using Microsoft.Practices.ServiceLocation;
using NinjectAdapter;

namespace NPatterns.Tests
{
    [TestClass]
    public class MessageBusTest
    {
        [TestMethod]
        public void TestBasicMessageBus()
        {
            IMessageBus bus = new MessageBus();

            var msg = new UserCreatedEvent { UserName = "abc" };

            //see what will happen if publish right away
            bus.Publish(msg);
            Assert.IsFalse(msg.Handled);//not handled, since no handlers in bus yet

            //register a handler in bus, and dispose after published.
            using (bus.Subscribe<UserCreatedEvent>(m =>
                                                        {
                                                            m.Handled = true;
                                                        }))
            {
                bus.Publish(msg);
            }
            Assert.IsTrue(msg.Handled);//handled by that handler

            msg.Handled = false;//reset

            //OR you may register a handler like this:
            var handler = new UserCreatedEventHandler();
            using (bus.Subscribe<UserCreatedEvent>(handler.Handle))
            {
                bus.Publish(msg);
            }
            Assert.IsTrue(msg.Handled);//handled by that handler

            msg.Handled = false;//reset

            //see what will happen after disposed handlers
            bus.Publish(msg);
            Assert.IsFalse(msg.Handled);//not handled, since that handler removed from bus by disposing
        }

        [TestMethod]
        public void TestIocMessageBus()
        {
            IKernel kernel = new StandardKernel();

            //inital service locator
            ServiceLocator.SetLocatorProvider(() => new NinjectServiceLocator(kernel));

            //we can also use IoC, instead of "new IoC.MessageBus()"
            kernel.Bind<IMessageBus>().To<NPatterns.Messaging.IoC.MessageBus>().InSingletonScope();//make it singleton

            //configure the handlers
            kernel.Bind<IHandler<UserCreatedEvent>>().To<UserCreatedEventHandler>();

            //get instance of bus
            IMessageBus bus = kernel.Get<IMessageBus>();

            var msg = new UserCreatedEvent { UserName = "abc" };
            bus.Publish(msg);
            Assert.IsTrue(msg.Handled);//handled by the instance of UserCreatedEventHandler
        }

        public class UserCreatedEvent
        {
            public string UserName { get; set; }
            public bool Handled { get; set; }
        }

        public class UserCreatedEventHandler : IHandler<UserCreatedEvent>
        {
            public void Handle(UserCreatedEvent message)
            {
                message.Handled = true;
            }
        }
    }
}
