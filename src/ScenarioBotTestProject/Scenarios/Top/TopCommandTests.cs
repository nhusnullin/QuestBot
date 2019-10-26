using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.BotCommands;
using Core.Service;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Adapters;
using Microsoft.Bot.Builder.Dialogs;
using NSubstitute;
using ScenarioBot.BotCommands;
using ScenarioBot.Dialogs;
using ScenarioBot.Service;
using Xunit;

namespace ScenarioBotTestProject.Scenarios.Top
{
    public class TopCommandTests
    {
        private readonly IUserService _userServiceMock;
        private readonly IScenarioService _scenarioServiceStub = Substitute.For<IScenarioService>();
        private readonly INotificationService _notificationServiceStub = Substitute.For<INotificationService>();
        private readonly Dictionary<string, string> _testCases;

        public TopCommandTests()
        {
            _userServiceMock = Substitute.For<IUserService>();
            _userServiceMock
                .CalcUserWeightsAsync(Arg.Any<int>())
                .Returns(callInfo => Enumerable.Range(1, callInfo.ArgAt<int>(0))
                    .ToDictionary(n => $"User_{n}", n => n));
            
            _testCases = ResourceHelper.GetDeserializedResourceValue<Dictionary<string, string>>(
                "ScenarioBotTestProject.Scenarios.Top.TestData.json");
        }

        [Fact]
        public async Task Top_command_cases()
        {
            var testFlow = CreateTestFlow();
            foreach (var testCase in _testCases)
            {
                testFlow = testFlow.Send(testCase.Key)
                    .AssertReply(testCase.Value);
            }

            await testFlow.StartTestAsync();
        }

        private TestFlow CreateTestFlow()
        {
            var conversationState = new ConversationState(new MemoryStorage());

            var adapter = new TestAdapter()
                .Use(new AutoSaveStateMiddleware(conversationState));

            var dialogState = conversationState.CreateProperty<DialogState>("dialogState");

            var dialogs = new DialogSet(dialogState);
            dialogs.Add(new ScenarioDialog(_scenarioServiceStub,
                _userServiceMock,
                _notificationServiceStub,
                new List<IBotCommand>
                {
                    new TopCommand(_userServiceMock)
                }));

            return new TestFlow(adapter, async (turnContext, cancellationToken) =>
            {
                var dc = await dialogs.CreateContextAsync(turnContext, cancellationToken);
                await dc.ContinueDialogAsync(cancellationToken);
                if (!turnContext.Responded)
                    await dc.BeginDialogAsync(nameof(ScenarioDialog), null, cancellationToken);
            });
        }
    }
}