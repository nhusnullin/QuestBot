using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CoreBot.Dialogs
{
    public class MainDialog : CancelAndHelpDialog
    {
        protected readonly IConfiguration _configuration;
        protected readonly ILogger _logger;
        private readonly IScenarioService _scenarioService;
        private readonly IUserService _userService;

        public MainDialog(IConfiguration configuration, ILogger<MainDialog> logger, IScenarioService scenarioService, IUserService userService)
            : base(nameof(MainDialog), scenarioService, userService)
        {
            _configuration = configuration;
            _logger = logger;
            this._scenarioService = scenarioService;
            _userService = userService;
            AddDialog(new ScenarioDialog(scenarioService, userService));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                ScenarioLaunchStepAsync,
                FinalStepAsync,
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            // это на тот случай что человек уже себе поставил бота, но пользователя нет у нас в БД
            var user = await _userService.GetByAsync(stepContext.Context.Activity.ChannelId, stepContext.Context.Activity.From.Id);
            if (user == null)
            {
                user = new User(stepContext.Context.Activity.ChannelId, stepContext.Context.Activity.From.Id) {
                    ChannelData = stepContext.Context.Activity.ChannelData.ToString()
                };
                await _userService.InsertOrMergeAsync(user);
            }
            
            return await stepContext.PromptAsync(nameof(TextPrompt),
                new PromptOptions {Prompt = MessageFactory.Text("Hello, my hero! Type anything to get started.")},
                cancellationToken);
        }

        private async Task<DialogTurnResult> ScenarioLaunchStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userId = stepContext.Context.Activity.From.Id;
            var channelId = stepContext.Context.Activity.ChannelId;

            var user = await _userService.GetByAsync(channelId, userId);
            var scenarioDetails = new ScenarioDetails()
            {
                ScenarioId = "testScenario",
                TeamId = user.TeamId
            };

            return await stepContext.BeginDialogAsync(nameof(ScenarioDialog), scenarioDetails, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Квест окончен!"), cancellationToken);
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
