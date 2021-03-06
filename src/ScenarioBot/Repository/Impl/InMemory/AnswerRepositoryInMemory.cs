﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain;
using ScenarioBot.Domain;

namespace ScenarioBot.Repository.Impl.InMemory
{
    public class AnswerRepositoryInMemory : IAnswerRepository
    {
        private readonly List<Answer> _store;

        public AnswerRepositoryInMemory()
        {
            _store = new List<Answer>();
        }

        public async Task<IEnumerable<string>> GetCompletedScenarioIds(UserId userId)
        {
            return _store.Where(x => x.IsLastAnswer)
                .GroupBy(x => x.ScenarioId)
                .SelectMany(x => x.ToList())
                .Select(x => x.ScenarioId);
        }


        public async Task AddAnswer(Answer answer)
        {
            _store.Add(answer);
        }

        public dynamic CalcAnswerWeights(int take)
        {
            var calculatedAnswers = _store.Select(x => new
                {
                    x.Weight,
                    x.PuzzleId,
                    x.ScenarioId,
                    x.RespondentId
                })
                .Distinct()
                .GroupBy(x => x.RespondentId)
                .Select(ag => new
                {
                    UserId = ag.Key.Id,
                    Weight = ag.Sum(x => x.Weight)
                })
                .OrderByDescending(x => x.Weight)
                .Take(take)
                .ToList();

            return calculatedAnswers;
        }

        public async Task<bool> IsScenarioCompletedByAsync(UserId userId, string scenarioId)
        {
            return _store.Any(x => x.IsLastAnswer &&
                                   x.ScenarioId == scenarioId &&
                                   x.RespondentId.Id == userId.Id);
        }

        public void RemoveBy(UserId userId)
        {
            _store.RemoveAll(answer => answer.RespondentId.Id == userId.Id);
        }

        public async Task<Answer> GetLastAddedAnswerFromNotCompletedScenario(UserId userId, string scenarioId)
        {
            var completedScenarioIds = _store.Where(x => x.IsLastAnswer && x.RespondentId == userId)
                .Select(x => x.ScenarioId).Distinct();

            return _store
                .Where(x => !x.IsLastAnswer && x.RespondentId == userId &&
                            x.ScenarioId == scenarioId) // не последний ответ сценария
                .Where(x => !completedScenarioIds.Contains(x.ScenarioId)) // из списка не законченных сценариев
                .OrderBy(x => x.Timestamp)
                .FirstOrDefault();
        }
    }
}