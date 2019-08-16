using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ScenarioBot.Domain
{
    /// <summary>
    /// Класс для передачи данных из диалога в диалог 
    /// </summary>
    public class PuzzleDetails
    {
        private readonly Puzzle _puzzle;

        // пустой конструктор необходим для дессериализации bot framework
        public PuzzleDetails()
        {
            PossibleAnswers = new List<string>();
        }

        public PuzzleDetails(Puzzle puzzle)
        {
            _puzzle = puzzle;
            
            PuzzleId = puzzle.Id;
            Question = puzzle.Question;
            PossibleAnswers = _puzzle.PosibleBranches.Select(x => x.Answer).ToList();
            NumberOfAttemptsLimit = puzzle.NumberOfAttemptsLimit;
            WaitnigTime = puzzle.WaitingTime;
            IsLastPuzzle = puzzle.IsLastPuzzle;

            // для режима ожидания у нас свой диалог
            PuzzleType = WaitnigTime.HasValue ? PuzzleType.WaitTextPuzzleDialog : puzzle.PuzzleType;
        }

        public bool IsLastPuzzle { get; set; }

        public PuzzleType PuzzleType { get; set; }


        /// <summary>
        /// Время когда задали вопрос (UTC)
        /// </summary>
        public DateTime? QuestionAskedAt { get; set; }


        public void SetQuestionAskedAt(DateTime value)
        {
            QuestionAskedAt = value;

            if (!WaitnigTime.HasValue)
            {
                return;
            }

            AnswerTimeNoLessThan = QuestionAskedAt.Value.AddMinutes(WaitnigTime.Value);
        }

        /// <summary>
        /// Время когда можно дать ответ (UTC)
        /// </summary>
        public DateTime AnswerTimeNoLessThan { get; set; }

        public int GetRemainMinutesToAnswer(DateTime now)
        {
#if DEBUG
            return 1;
#endif
            
            return (AnswerTimeNoLessThan - now).Minutes;
        }

        public int PuzzleWeight => _puzzle.Weight;

        public string PuzzleId { get; set; }
        public string Question { get; set; }
        public IList<string> PossibleAnswers { get; set; }
        public string ActualAnswer { get; set; }
        public int? WaitnigTime { get; set; }
        /// <summary>
        /// сколько раз пользователь вводил ответ 
        /// </summary>
        public int NumberOfAttempts { get; set; }

        /// <summary>
        /// сколько раз пользователь может вводить ответ 
        /// </summary>
        public int? NumberOfAttemptsLimit { get; set; }

        //public bool IsRight
        //{
        //    get
        //    {
        //        if(string.IsNullOrEmpty(ActualAnswer) ||
        //            string.IsNullOrEmpty(PossibleAnswers))
        //        {
        //            return false;
        //        }

        //        var actuals = ActualAnswer.Split(' ')
        //            .Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x));

        //        var expected = PossibleAnswers.Split(' ')
        //            .Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToArray();

        //        foreach(var aItem in actuals)
        //        {
        //            bool isFound = false;
        //            for(var i = 0; i < expected.Length; i++)
        //            {
        //                var isEqual = String.Equals(expected[i], aItem, StringComparison.CurrentCultureIgnoreCase);
        //                if (isEqual)
        //                {
        //                    isFound = true;
        //                    expected[i] = null;
        //                    break;
        //                }
        //            }

        //            if (!isFound)
        //            {
        //                return false;
        //            }
        //        }

        //        return true;
        //    }
        //}

        public bool IsRight
        {
            get
            {
                if (PossibleAnswers == null || !PossibleAnswers.Any())
                {
                    return false;
                }

                foreach (var answer in PossibleAnswers)
                {
                    if (string.Equals(answer, ActualAnswer, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public void SetAnswer(string answer)
        {
            ActualAnswer = VanishAnswer(answer);
            NumberOfAttempts++;
        }

        public static string VanishAnswer(string input)
        {
            string pattern = " *[\\\\~#%&*{}/:<>?|\"-]+ *";
            string replacement = " ";

            Regex regEx = new Regex(pattern);
            string sanitized = Regex.Replace(regEx.Replace(input, replacement), @"\s+", " ");

            return sanitized;
        }


        
    }
}