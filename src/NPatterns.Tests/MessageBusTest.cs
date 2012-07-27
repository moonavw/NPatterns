using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NPatterns.Messaging;
using Ninject;
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

            var msg = new TestMessage();

            //see what will happen if publish right away
            bus.Publish(msg);
            Assert.IsFalse(msg.Handled); //not handled, since no handlers in bus yet

            //register a handler in bus, and dispose after published.
            using (bus.Subscribe<TestMessage>(m => m.HandledBy.Add("anonymous handler")))
            {
                bus.Publish(msg);
            }
            Assert.IsTrue(msg.Handled); //handled by that handler

            msg.HandledBy.Clear(); //reset

            //OR you may register a handler like this:
            var handler = new TestMessageHandler();
            using (bus.Subscribe(handler))
            {
                bus.Publish(msg);
            }
            Assert.IsTrue(msg.Handled); //handled by that handler

            msg.HandledBy.Clear(); //reset

            //see what will happen after disposed handlers
            bus.Publish(msg);
            Assert.IsFalse(msg.Handled); //not handled, since that handler removed from bus by disposing

            msg.HandledBy.Clear(); //reset

            //register following handlers: 5 in total
            bus.Subscribe<TestMessage>(m => m.HandledBy.Add("anonymous handler1"));
            bus.Subscribe<TestMessage>(m => m.HandledBy.Add("anonymous handler2"));
            bus.Subscribe<TestMessage>(m => m.HandledBy.Add("anonymous handler3"));
            bus.Subscribe(handler);
            bus.Subscribe(handler);//duplicate one will be ignored
            bus.Publish(msg);
            Assert.AreEqual(4, msg.HandledBy.Count);//actually handled by 4 handlers
        }

        [TestMethod]
        public void TestIocMessageBus()
        {
            IKernel kernel = new StandardKernel();

            //inital service locator
            ServiceLocator.SetLocatorProvider(() => new NinjectServiceLocator(kernel));

            //we can also use IoC, instead of "new IoC.MessageBus()"
            kernel.Bind<IMessageBus>().To<Messaging.IoC.MessageBus>().InSingletonScope(); //make it singleton

            //configure the handlers
            kernel.Bind<IHandler<TestMessage>>().To<TestMessageHandler>();

            //get instance of bus
            IMessageBus bus = kernel.Get<IMessageBus>();

            var msg = new TestMessage();
            bus.Publish(msg);
            Assert.IsTrue(msg.Handled); //handled by the instance of UserCreatedEventHandler
        }

        [TestMethod]
        public void PublishAsync()
        {
            IMessageBus bus = new MessageBus();

            var msg = new TestMessage();

            bus.Subscribe<TestMessage>(m =>
            {
                Thread.Sleep(500);
                m.HandledBy.Add("anonymous handler1");
            });
            bus.Subscribe<TestMessage>(m =>
            {
                Thread.Sleep(250);
                m.HandledBy.Add("anonymous handler2");
            });
            bus.Subscribe<TestMessage>(m =>
            {
                Thread.Sleep(100);
                m.HandledBy.Add("anonymous handler3");
            });

            bus.PublishAsync(msg);
            Thread.Sleep(300);
            Assert.AreEqual(2, msg.HandledBy.Count);
            Thread.Sleep(300);
            Assert.AreEqual(3, msg.HandledBy.Count);
        }

        #region Nested type: TestMessage

        public class TestMessage
        {
            public TestMessage()
            {
                HandledBy = new List<string>();
            }

            public List<string> HandledBy { get; set; }

            public bool Handled
            {
                get { return HandledBy.Any(); }
            }
        }

        #endregion

        #region Nested type: TestMessageHandler

        public class TestMessageHandler : IHandler<TestMessage>
        {
            #region IHandler<TestMessage> Members

            public void Handle(TestMessage message)
            {
                message.HandledBy.Add(GetType().Name);
            }

            #endregion
        }

        #endregion
    }
}