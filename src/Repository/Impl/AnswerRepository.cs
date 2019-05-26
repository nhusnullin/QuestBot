using CoreBot.Storage;

namespace CoreBot.Repository.Impl
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
