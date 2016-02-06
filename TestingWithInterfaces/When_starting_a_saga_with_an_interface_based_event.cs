using NServiceBus;
using NServiceBus.Saga;
using NServiceBus.Testing;
using NUnit.Framework;

namespace Repro2
{
    public class SomeSaga : NServiceBus.Saga.Saga<State>, IAmStartedByMessages<SomethingHappened>
    {
        public void Handle(SomethingHappened message)
        {
            Bus.Publish<Finished>(e => e.InitialData = message.What);
            
            MarkAsComplete();
        }

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<State> mapper)
        {
            mapper.ConfigureMapping<SomethingHappened>(m => m.What).ToSaga(s => s.What);
        }
    }

    public class State : ContainSagaData
    {
        public string What { get; set; }
    }

    public interface SomethingHappened : IEvent
    {
        string What { get; set; }
    }


    public interface Finished : IEvent
    {
        string InitialData { get; set; }
    }


    [TestFixture]
    public class When_starting_a_saga_with_an_interface_based_event
    {
        [Test]
        public void Should_work()
        {
            Test.Initialize();
            Test.Saga<SomeSaga>()
                .WhenHandling<SomethingHappened>(m => m.What = "123")
                .ExpectPublish<Finished>(e => e.InitialData == "123")
                .AssertSagaCompletionIs(true);
        }
    }
}
