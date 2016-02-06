using NServiceBus;
using NServiceBus.Testing;
using NUnit.Framework;

namespace Repro
{
    public class Handler : IHandleMessages<Start>
    {
        public IBus Bus { get; set; }

        public void Handle(Start message)
        {
            Bus.Publish<Finished>(e => e.InitialData = message.Data);    
        }
    }

    public interface Finished : IEvent
    {
        string InitialData { get; set; }
    }

    public class Start : ICommand
    {
        public string Data { get; set; }
    }

    [TestFixture]
    public class When_using_intefrace_based_event
    {
        [Test]
        public void Should_work()
        {
            Test.Initialize();
            Test.Handler<Handler>()
                .ExpectPublish<Finished>(e => e.InitialData == "hello")
                .OnMessage<Start>(m => m.Data = "hello");
        }
    }
}
