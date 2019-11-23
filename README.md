[![Build status](https://dev.azure.com/dotnetquestbot/AwesomeDotNextQuestBot-/_apis/build/status/awesomedotnextquestbot%20-%201%20-%20CI)](https://dev.azure.com/dotnetquestbot/AwesomeDotNextQuestBot-/_build/latest?definitionId=2)

# Бот для проведения квестов. 
Писался под Telegram, с использованием [Bot Framework][20].
Набор заданий для прохождения и возможные ветки развития событий хранится в [json][50].
Использовался для проведения активностей на стенде сообщества [.NetRu][51] на конференциях TechTrain 2019, DotNext 2018 и 2019.

# Для тестирования  можно использовать Bot Framework Emulator 
[Bot Framework Emulator][5]  - это дестктоп приложение, которое помогает тестировать и дебажить бота. Установить можно из [here][6]

## Подключиться к боту используя Bot Framework Emulator
- После запуска приложения ввести в эмуляторе `http://localhost:3978/api/messages`
- Или открыть в эмуляторе [файл][52]

# Так же можно почитать
- [Bot Framework Documentation][20]
- [Bot Basics][32]
- [Prompt types][23]
- [Waterfall dialogs][24]
- [Ask the user questions][26]
- [Activity processing][25]
- [Azure Bot Service Introduction][21]
- [Azure Bot Service Documentation][22]
- [.NET Core CLI tools][23]
- [Azure CLI][7]
- [msbot CLI][9]
- [Azure Portal][10]
- [Language Understanding using LUIS][11]
- [Channels and Bot Connector Service][27]


[1]: https://dev.botframework.com
[4]: https://dotnet.microsoft.com/download
[5]: https://github.com/microsoft/botframework-emulator
[6]: https://github.com/Microsoft/BotFramework-Emulator/releases
[7]: https://docs.microsoft.com/cli/azure/?view=azure-cli-latest
[8]: https://docs.microsoft.com/cli/azure/install-azure-cli?view=azure-cli-latest
[9]: https://github.com/Microsoft/botbuilder-tools/tree/master/packages/MSBot
[10]: https://portal.azure.com
[11]: https://www.luis.ai
[20]: https://docs.botframework.com
[21]: https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0
[22]: https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0
[23]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-prompts?view=azure-bot-service-4.0
[24]: https://docs.microsoft.com/en-us/javascript/api/botbuilder-dialogs/waterfall
[25]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0
[26]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-tutorial-waterfall?view=azure-bot-service-4.0
[27]: https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0
[30]: https://www.npmjs.com/package/restify
[31]: https://www.npmjs.com/package/dotenv
[32]: https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0
[40]: https://aka.ms/azuredeployment
[50]:https://github.com/nhusnullin/QuestBot/blob/master/src/ScenarioBot/raw_data/2019.DotNext.Msk/Robbery.json
[51]:https://dotnet.ru/communities
[52]:https://github.com/nhusnullin/QuestBot/blob/master/emulator.bot
