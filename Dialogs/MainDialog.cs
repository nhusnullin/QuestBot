using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CoreBot.Service;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CoreBot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        protected readonly IConfiguration _configuration;
        protected readonly ILogger _logger;
        private readonly IScenarioService _scenarioService;
        private readonly IUserService _userService;

        public MainDialog(IConfiguration configuration, 
            ILogger<MainDialog> logger, 
            IScenarioService scenarioService,
            ITeamService teamService,
            IUserService userService)
            : base(nameof(MainDialog))
        {
            _configuration = configuration;
            _logger = logger;
            _scenarioService = scenarioService ?? throw new System.ArgumentNullException(nameof(scenarioService));
            _userService = userService ?? throw new System.ArgumentNullException(nameof(userService));
            AddDialog(new SelectTeamDialog(teamService));
            AddDialog(new ScenarioDialog(scenarioService, userService));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                SelectTeamStepAsync,
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
                user = new User(stepContext.Context.Activity.ChannelId, stepContext.Context.Activity.From.Id)
                {
                    ChannelData = stepContext.Context.Activity.ChannelData != null ? stepContext.Context.Activity.ChannelData.ToString() : string.Empty
                };
                await _userService.InsertOrMergeAsync(user);
            }
            
            return await stepContext.PromptAsync(nameof(TextPrompt),
                new PromptOptions {Prompt = MessageFactory.Text("Hello, my hero! Type anything to get started.")},
                cancellationToken);
        }

        private async Task<DialogTurnResult> SelectTeamStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var user = await _userService.GetByAsync(stepContext.Context.Activity.ChannelId, stepContext.Context.Activity.From.Id);
            return await stepContext.BeginDialogAsync(nameof(SelectTeamDialog), user, cancellationToken);
        }

        private async Task<DialogTurnResult> ScenarioLaunchStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var channelId = stepContext.Context.Activity.ChannelId;
            var teamId = (string)stepContext.Result;
            var scenarioDetails = new ScenarioDetails()
            {
                ScenarioId = "scenario1",
                TeamId = teamId
            };

            return await stepContext.BeginDialogAsync(nameof(ScenarioDialog), scenarioDetails, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Thank you and Goodbay! Have a nice day and hope to see you soon!"), cancellationToken);
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
