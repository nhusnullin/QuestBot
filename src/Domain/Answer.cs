using Core.Domain;

namespace CoreBot.Domain
{
    public class Answer
    {
        public const string EntityName = "answers";

        public Answer()
        {

        }

        public Answer(UserId respondentId)
        {
            RespondentId = respondentId;
        }

        public UserId RespondentId { get; }
        public string ScenarioId { get; set; }
        public string PuzzleId { get; set; }
        public string ScenarioDetails { get; set; }

        public bool IsLastAnswer { get; set; }

        public string GetStorageKey()
        {
            return $"{RespondentId.ChannelId}/users/{RespondentId.Id}";
        }

        public static string GetStorageKey(UserId respondentId)
        {
            return $"{respondentId.ChannelId}/{EntityName}/{respondentId.Id}";
        }
    }

}