using NServiceBus;
using NServiceBus.Saga;
using NServiceBus.Testing;
using NUnit.Framework;

namespace Repro3
{
    public class SomeSaga : NServiceBus.Saga.Saga<State>, IAmStartedByMessages<Kickoff>
    {
        public void Handle(Kickoff message)
        {
            Bus.Publish<Finished>(e => e.InitialData = message.What);
            Data.What = message.What;
            MarkAsComplete();
        }

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<State> mapper)
        {
            mapper.ConfigureMapping<Kickoff>(m => m.What).ToSaga(s => s.What);
        }
    }

    public class State : ContainSagaData
    {
        public string What { get; set; }
    }

    public class Kickoff : ICommand
    {
        public string What { get; set; }
    }


    public interface Finished : IEvent
    {
        string InitialData { get; set; }
    }


    [TestFixture]
    public class When_testing_saga_for_state
    {
        [Test]
        public void Should_work()
        {
            Test.Initialize();
            Test.Saga<SomeSaga>()
                .ExpectPublish<Finished>(e => e.InitialData == "123")
                .When(saga =>
                {
                    saga.Handle(new Kickoff { What = "123" });
                    Assert.That(saga.Data.What, Is.EqualTo("123"));
                })
                .AssertSagaCompletionIs(true);
        }
    }
}
