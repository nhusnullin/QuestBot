using System;
using System.Linq;

namespace CoreBot
{
    /// <summary>
    /// Класс для передачи данных из диалога в диалог 
    /// </summary>
    public class PuzzleDetails
    {
        // пустой конструктор необходим для дессериализации bot framework
        public PuzzleDetails()
        {

        }

        public PuzzleDetails(Puzzle puzzle, string expectedAnswer)
        {
            ScenarioId = "1";
            PuzzleId = puzzle.Id;
            Question = puzzle.Question;
            ExpectedAnswer = expectedAnswer;
            WaitUntilReceiveRightAnswer = puzzle.WaitUntilReceiveRightAnswer;
            NumberOfAttemptsLimit = puzzle.NumberOfAttemptsLimit;
        }


        public string ScenarioId { get; set; }
        public string PuzzleId { get; set; }
        public string Question { get; set; }
        public string ExpectedAnswer { get; set; }
        public string ActualAnswer { get; set; }
        public bool? WaitUntilReceiveRightAnswer { get; set; }

        /// <summary>
        /// сколько раз пользователь вводил ответ 
        /// </summary>
        public int NumberOfAttempts { get; set; }

        /// <summary>
        /// сколько раз пользователь может вводить ответ 
        /// </summary>
        public int? NumberOfAttemptsLimit { get; set; }

        public bool IsRight
        {
            get
            {
                if(string.IsNullOrEmpty(ActualAnswer) ||
                    string.IsNullOrEmpty(ExpectedAnswer))
                {
                    return false;
                }

                var actuals = ActualAnswer.Split(' ')
                    .Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x));

                var expected = ExpectedAnswer.Split(' ')
                    .Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToArray();

                foreach(var aItem in actuals)
                {
                    bool isFound = false;
                    for(var i = 0; i < expected.Length; i++)
                    {
                        var isEqual = String.Equals(expected[i], aItem, StringComparison.CurrentCultureIgnoreCase);
                        if (isEqual)
                        {
                            isFound = true;
                            expected[i] = null;
                            break;
                        }
                    }

                    if (!isFound)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public void SetAnswer(string answer)
        {
            ActualAnswer = answer;
            NumberOfAttempts++;
        }

        
    }
}