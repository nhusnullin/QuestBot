// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using CoreBot;
using CoreBot.Dialogs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;

namespace Microsoft.BotBuilderSamples
{
    public class MainDialog : ComponentDialog
    {
        protected readonly IConfiguration _configuration;
        protected readonly ILogger _logger;
        private readonly IUserService userService;

        public MainDialog(IConfiguration configuration, ILogger<MainDialog> logger, IScenarioService scenarioService, IUserService userService)
            : base(nameof(MainDialog))
        {
            _configuration = configuration;
            _logger = logger;
            this.userService = userService;
            AddDialog(new ScenarioDialog(scenarioService, userService));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            //AddDialog(new BookingDialog());
            //AddDialog(new PicDialog());
            //AddDialog(new ChoiceDialog());
            //AddDialog(new ChoicePrompt("SelectGroupCardDialog") { Style = ListStyle.None });
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                ScenarioLaunchStepAsync,
                //ShowChoiceDialogAsync,
                //ShowPicDialogAsync,
                //PromptWithContentAttachment
                //Intro2StepAsync
                //ActStepAsync,
                FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }
        

        
        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            var from = stepContext.Context.Activity.From;
            var user = userService.GetBy(stepContext.Context.Activity.ChannelId, stepContext.Context.Activity.From.Id);
            if(user == null)
            {
                user = new User()
                {
                    ChannelId = stepContext.Context.Activity.ChannelId,
                    IsCaptain = true,
                    Name = stepContext.Context.Activity.From.Name,
                    TeamId = stepContext.Context.Activity.From.Id,
                    UserId = stepContext.Context.Activity.From.Id
                };
                userService.InsertOrMerge(user);
            }

            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"{from.Id} {from.Name} {stepContext.Context.Activity.ChannelId}"));

            if (string.IsNullOrEmpty(_configuration["LuisAppId"]) || string.IsNullOrEmpty(_configuration["LuisAPIKey"]) || string.IsNullOrEmpty(_configuration["LuisAPIHostName"]))
            {
                await stepContext.Context.SendActivityAsync(
                    MessageFactory.Text("NOTE: LUIS is not configured. To enable all capabilities, add 'LuisAppId', 'LuisAPIKey' and 'LuisAPIHostName' to the appsettings.json file."), cancellationToken);

                return await stepContext.NextAsync(null, cancellationToken);
            }
            else
            {
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Hello, my hero! Type anything to get started.") }, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> ScenarioLaunchStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userId = stepContext.Context.Activity.From.Id;
            var channelId = stepContext.Context.Activity.ChannelId;

            var user = userService.GetBy(channelId, userId);
            var scenarioDetails = new ScenarioDetails()
            {
                ScenarioId = "Scenario1",
                TeamId = user.TeamId
            };

            return await stepContext.BeginDialogAsync(nameof(ScenarioDialog), scenarioDetails, cancellationToken);
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Call LUIS and gather any potential booking details. (Note the TurnContext has the response to the prompt.)
            var bookingDetails = stepContext.Result != null
                    ?
                await LuisHelper.ExecuteLuisQuery(_configuration, _logger, stepContext.Context, cancellationToken)
                    :
                new BookingDetails();

            // In this sample we only have a single Intent we are concerned with. However, typically a scneario
            // will have multiple different Intents each corresponding to starting a different child Dialog.

            // Run the BookingDialog giving it whatever details we have from the LUIS call, it will fill out the remainder.
            return await stepContext.BeginDialogAsync(nameof(BookingDialog), bookingDetails, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // If the child dialog ("BookingDialog") was cancelled or the user failed to confirm, the Result here will be null.
            //if (stepContext.Result != null)
            //{
            //    var result = (BookingDetails)stepContext.Result;

            //    // Now we have all the booking details call the booking service.

            //    // If the call to the booking service was successful tell the user.

            //    var timeProperty = new TimexProperty(result.TravelDate);
            //    var travelDateMsg = timeProperty.ToNaturalLanguage(DateTime.Now);
            //    var msg = $"I have you booked to {result.Destination} from {result.Origin} on {travelDateMsg}";
            //    await stepContext.Context.SendActivityAsync(MessageFactory.Text(msg), cancellationToken);
            //}
            //else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Thank you."), cancellationToken);
            }
            return await stepContext.EndDialogAsync();
        }
    }
}
