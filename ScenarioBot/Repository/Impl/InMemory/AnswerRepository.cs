namespace ScenarioBot.Repository.Impl.InMemory
{
    public class AnswerRepository:IAnswerRepository
    {
        private readonly IStorage _storage;

        public AnswerRepository(IStorage storage)
        {
            _storage = storage;
        }
    }
}
