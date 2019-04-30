using System.Threading;
using System.Threading.Tasks;
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
        private readonly IUserService _userService;

        public MainDialog(IConfiguration configuration, ILogger<MainDialog> logger, IScenarioService scenarioService, IUserService userService)
            : base(nameof(MainDialog))
        {
            _configuration = configuration;
            _logger = logger;
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
            var user = _userService.GetBy(stepContext.Context.Activity.ChannelId, stepContext.Context.Activity.From.Id);
            if (user == null)
            {
                user = new User()
                {
                    ChannelId = stepContext.Context.Activity.ChannelId,
                    IsCaptain = true,
                    Name = stepContext.Context.Activity.From.Name,
                    TeamId = stepContext.Context.Activity.From.Id,
                    UserId = stepContext.Context.Activity.From.Id
                };
                _userService.InsertOrMerge(user);
            }

//            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"{from.Id} {from.Name} {stepContext.Context.Activity.ChannelId}"));

            return await stepContext.PromptAsync(nameof(TextPrompt),
                new PromptOptions {Prompt = MessageFactory.Text("Hello, my hero! Type anything to get started.")},
                cancellationToken);
        }

        private async Task<DialogTurnResult> ScenarioLaunchStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userId = stepContext.Context.Activity.From.Id;
            var channelId = stepContext.Context.Activity.ChannelId;

            var user = _userService.GetBy(channelId, userId);
            var scenarioDetails = new ScenarioDetails()
            {
                ScenarioId = "Scenario1",
                TeamId = user.TeamId
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
