using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NPatterns.Messaging;
using Ninject;
using System.Threading.Tasks;

namespace NPatterns.Tests
{
    [TestClass]
    public class MessageBusTest
    {
        [TestMethod]
        public void TestMessageBus()
        {
            IMessageBus bus = new MessageBus();

            var msg = new TestMessage();

            //see what will happen if publish right away
            Assert.IsFalse(msg.Handled); //not handled, since no handlers in bus yet

            //register a handler in bus, and dispose after published.
            using (bus.Subscribe<TestMessage>(m => m.HandledBy.Add("anonymous handler")))
                bus.Publish(msg);

            Assert.IsTrue(msg.Handled); //handled by that handler

            msg.HandledBy.Clear(); //reset

            //see what will happen after disposed handlers
            bus.Publish(msg);
            Assert.IsFalse(msg.Handled); //not handled, since that handler removed from bus by disposing
        }

        [TestMethod]
        public void TestMessageBusEx()
        {
            IKernel kernel = new StandardKernel();

            //setup handler factory
            kernel.Bind<Func<Type, IEnumerable<object>>>().ToMethod(ctx => (type) => ctx.Kernel.GetAll(type));
            kernel.Bind<IMessageBus>().To<MessageBusEx>().InSingletonScope(); //make it singleton

            var msg = new TestMessage();

            //get instance of bus
            IMessageBus bus = kernel.Get<IMessageBus>();

            //see what will happen if publish right away
            Assert.IsFalse(msg.Handled); //not handled, since no handlers in bus yet

            //configure the handlers
            kernel.Bind<IHandler<TestMessage>>().To<PrimaryTestMessageHandler>();
            kernel.Bind<IHandler<TestMessage>>().To<SecondaryTestMessageHandler>();
            kernel.Bind<IHandler<TestMessage>>().To<TertiaryTestMessageHandler>();


            Stopwatch sw = Stopwatch.StartNew();
            bus.Publish(msg);
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
            Assert.IsTrue(msg.Handled);

            //also handled sequentially
            Assert.AreEqual(typeof(PrimaryTestMessageHandler).Name, msg.HandledBy[0]);
            Assert.AreEqual(typeof(SecondaryTestMessageHandler).Name, msg.HandledBy[1]);
            Assert.AreEqual(typeof(TertiaryTestMessageHandler).Name, msg.HandledBy[2]);
        }

        [TestMethod]
        public void TestSequentiallyHandling()
        {
            IKernel kernel = new StandardKernel();

            //setup handler factory
            kernel.Bind<Func<Type, IEnumerable<object>>>().ToMethod(ctx => (type) => ctx.Kernel.GetAll(type));
            kernel.Bind<IMessageBus>().To<MessageBusEx>().InSingletonScope(); //make it singleton

            var msg = new TestMessage();

            //get instance of bus
            IMessageBus bus = kernel.Get<IMessageBus>();

            //specify an order for anonymous handler
            bus.Subscribe<TestMessage>(m => m.HandledBy.Add("anonymous1"));
            bus.Subscribe<TestMessage>(m => m.HandledBy.Add("anonymous2"));

            //configure the handlers
            kernel.Bind<IHandler<TestMessage>>().To<PrimaryTestMessageHandler>();
            kernel.Bind<IHandler<TestMessage>>().To<SecondaryTestMessageHandler>();
            kernel.Bind<IHandler<TestMessage>>().To<TertiaryTestMessageHandler>();

            bus.Publish(msg);

            Assert.AreEqual(typeof(PrimaryTestMessageHandler).Name, msg.HandledBy[0]);
            Assert.AreEqual(typeof(SecondaryTestMessageHandler).Name, msg.HandledBy[1]);
            Assert.AreEqual(typeof(TertiaryTestMessageHandler).Name, msg.HandledBy[2]);
            Assert.AreEqual("anonymous1", msg.HandledBy[3]);
            Assert.AreEqual("anonymous2", msg.HandledBy[4]);
        }

        [TestMethod]
        public void TestPublishAsync()
        {
            IMessageBus bus = new MessageBus();

            var msg = new TestMessage();

            bus.Subscribe<TestMessage>(m => m.HandledBy.Add("anonymous handler1"));
            bus.Subscribe<TestMessage>(m => m.HandledBy.Add("anonymous handler2"));
            bus.Subscribe<TestMessage>(m =>
                                           {
                                               Thread.Sleep(100);
                                               m.HandledBy.Add("anonymous handler3");
                                           });

            Stopwatch sw = Stopwatch.StartNew();
            var task = bus.PublishAsync(msg);
            sw.Stop();
            Assert.AreNotEqual(3, msg.HandledBy.Count);
            Assert.IsTrue(sw.ElapsedMilliseconds < 100);

            sw.Restart();
            Task.WaitAll(task);
            sw.Stop();
            Assert.AreEqual(3, msg.HandledBy.Count);
            Assert.IsTrue(sw.ElapsedMilliseconds >= 100);
        }

        [TestMethod]
        public void TestExPublishAsync()
        {
            IKernel kernel = new StandardKernel();

            //setup handler factory
            kernel.Bind<Func<Type, IEnumerable<object>>>().ToMethod(ctx => (type) => ctx.Kernel.GetAll(type));
            kernel.Bind<IMessageBus>().To<MessageBusEx>().InSingletonScope(); //make it singleton

            kernel.Bind<IHandler<TestMessage>>().To<TertiaryTestMessageHandler>();
            kernel.Bind<IHandler<TestMessage>>().To<PrimaryTestMessageHandler>();
            kernel.Bind<IHandler<TestMessage>>().To<SecondaryTestMessageHandler>();

            //test ioc bus
            IMessageBus bus = kernel.Get<IMessageBus>();

            bus.Subscribe<TestMessage>(m => m.HandledBy.Add("anonymous handler1"));
            bus.Subscribe<TestMessage>(m => m.HandledBy.Add("anonymous handler2"));
            bus.Subscribe<TestMessage>(m =>
                                           {
                                               Thread.Sleep(100);
                                               m.HandledBy.Add("anonymous handler3");
                                           });

            var msg = new TestMessage();
            Stopwatch sw = Stopwatch.StartNew();
            var task = bus.PublishAsync(msg);
            sw.Stop();
            Assert.AreNotEqual(6, msg.HandledBy.Count);
            Assert.IsTrue(sw.ElapsedMilliseconds < 100);

            sw.Restart();
            Task.WaitAll(task);
            sw.Stop();
            Assert.AreEqual(6, msg.HandledBy.Count);
            Assert.IsTrue(sw.ElapsedMilliseconds >= 100);
        }

        #region Nested type: PrimaryTestMessageHandler

        public class PrimaryTestMessageHandler : IHandler<TestMessage>
        {
            #region IHandler<TestMessage> Members

            public void Handle(TestMessage message)
            {
                message.HandledBy.Add(GetType().Name);
            }

            public Task HandleAsync(TestMessage message)
            {
                return Task.Run(() => Handle(message));
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

            public Task HandleAsync(TestMessage message)
            {
                return Task.Run(() => Handle(message));
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

            public Task HandleAsync(TestMessage message)
            {
                return Task.Run(() => Handle(message));
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