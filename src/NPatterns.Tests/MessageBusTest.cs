using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            var handler = new PrimaryTestMessageHandler();
            using (bus.Subscribe(handler))
            {
                bus.Publish(msg);
            }
            Assert.IsTrue(msg.Handled); //handled by that handler

            msg.HandledBy.Clear(); //reset

            //see what will happen after disposed handlers
            bus.Publish(msg);
            Assert.IsFalse(msg.Handled); //not handled, since that handler removed from bus by disposing
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
            kernel.Bind<IHandler<TestMessage>>().To<TertiaryTestMessageHandler>();
            kernel.Bind<IHandler<TestMessage>>().To<PrimaryTestMessageHandler>();
            kernel.Bind<IHandler<TestMessage>>().To<SecondaryTestMessageHandler>();

            //get instance of bus
            IMessageBus bus = kernel.Get<IMessageBus>();

            var msg = new TestMessage();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            bus.Publish(msg);
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
            Assert.IsTrue(msg.Handled);

            Assert.AreEqual(typeof(TertiaryTestMessageHandler).Name, msg.HandledBy[0]);
            Assert.AreEqual(typeof(PrimaryTestMessageHandler).Name, msg.HandledBy[1]);
            Assert.AreEqual(typeof(SecondaryTestMessageHandler).Name, msg.HandledBy[2]);
        }

        [TestMethod]
        public void SequentiallyHandling()
        {
            IMessageBus bus = new MessageBus();

            var msg = new TestMessage();

            //specify an order for anonymous handler
            bus.Subscribe<TestMessage>(m => m.HandledBy.Add("anonymous1"), 10);
            bus.Subscribe<TestMessage>(m => m.HandledBy.Add("anonymous2"), 11);

            //ordered handlers
            var handlers = new List<IHandler<TestMessage>>
                               {
                                   new SecondaryTestMessageHandler(),
                                   new TertiaryTestMessageHandler(),
                                   new PrimaryTestMessageHandler()
                               };

            for (int i = 0; i < handlers.Count; i++)
            {
                bus.Subscribe(handlers[i], i);
            }


            bus.Publish(msg);
            for (int i = 0; i < handlers.Count; i++)
            {
                Assert.AreEqual(handlers[i].GetType().Name, msg.HandledBy[i]);
            }
            Assert.AreEqual("anonymous1", msg.HandledBy[3]);
            Assert.AreEqual("anonymous2", msg.HandledBy[4]);
        }

        [TestMethod]
        public void DuplicatedSubscription()
        {
            IMessageBus bus = new MessageBus();

            var msg = new TestMessage();
            //register following handlers: 4 in total
            bus.Subscribe<TestMessage>(m => m.HandledBy.Add("anonymous handler"));
            bus.Subscribe<TestMessage>(m => m.HandledBy.Add("anonymous handler"));
            bus.Subscribe(new PrimaryTestMessageHandler());
            bus.Subscribe(new PrimaryTestMessageHandler()); //duplicate one will be ignored
            bus.Publish(msg);
            Assert.AreEqual(3, msg.HandledBy.Count); //actually handled by 3 handlers
        }

        [TestMethod]
        public void PublishAsync()
        {
            IMessageBus bus = new MessageBus();

            var msg = new TestMessage();

            bus.Subscribe<TestMessage>(m =>
                                           {
                                               Thread.Sleep(300);
                                               m.HandledBy.Add("anonymous handler1");
                                           });
            bus.Subscribe<TestMessage>(m =>
                                           {
                                               Thread.Sleep(200);
                                               m.HandledBy.Add("anonymous handler2");
                                           });
            bus.Subscribe<TestMessage>(m =>
                                           {
                                               Thread.Sleep(100);
                                               m.HandledBy.Add("anonymous handler3");
                                           });

            Stopwatch sw = new Stopwatch();
            sw.Start();
            bus.PublishAsync(msg);
            sw.Stop();
            Assert.IsTrue(sw.ElapsedMilliseconds < 100);

            sw.Restart();
            do
                Thread.Sleep(100);
            while (msg.HandledBy.Count < 3);

            sw.Stop();
            Assert.AreEqual(300, sw.ElapsedMilliseconds, 100);
        }

        #region Nested type: PrimaryTestMessageHandler

        public class PrimaryTestMessageHandler : IHandler<TestMessage>
        {
            #region IHandler<TestMessage> Members

            public void Handle(TestMessage message)
            {
                message.HandledBy.Add(GetType().Name);
            }

            #endregion
        }

        #endregion

        #region Nested type: SecondaryTestMessageHandler

        public class SecondaryTestMessageHandler : IHandler<TestMessage>
        {
            #region IHandler<TestMessage> Members

            public void Handle(TestMessage message)
            {
                message.HandledBy.Add(GetType().Name);
            }

            #endregion
        }

        #endregion

        #region Nested type: TertiaryTestMessageHandler

        public class TertiaryTestMessageHandler : IHandler<TestMessage>
        {
            #region IHandler<TestMessage> Members

            public void Handle(TestMessage message)
            {
                message.HandledBy.Add(GetType().Name);
            }

            #endregion
        }

        #endregion

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
    }
}