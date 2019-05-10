using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace CoreBot
{
    public class Scenario
    {
        public Scenario()
        {
            Collection = new List<Puzzle>();
        }

        public string ScenarioId { get; set; }

        public IList<Puzzle> Collection { get; set; }

        public Puzzle Add(Puzzle puzzle)
        {
            Collection.Add(puzzle);
            return puzzle;
        }
    }

    public class AnswerToBranch
    {
        public string Answer { get; set; }
        public string GoToId { get; set; }
    }

    /// <summary>
    /// Дерево заданий квеста
    /// </summary>
    public class Puzzle
    {
        public const string RootId = "Root";

        public Puzzle()
        {
            PosibleBranches = new List<AnswerToBranch>();
        }

        public Puzzle(string id):this()
        {
            Id = id;
        }
        
        public Puzzle To(Scenario scenario)
        {
            return scenario.Add(this);
        }

        public Puzzle AddBranch(string answer, string goTo)
        {
            PosibleBranches.Add(new AnswerToBranch()
            {
                Answer = answer,
                GoToId = goTo
            }); 
            return this;
        }

        public Puzzle AddElseBranch(string goTo)
        {
            ElseBranch = goTo;
            return this;
        }


        public string GetNextPossibleBranchId(string answer)
        {
            var answerToBranches = PosibleBranches.FirstOrDefault(x => string.Equals(x.Answer, answer, StringComparison.InvariantCultureIgnoreCase));
            if (answerToBranches != null)
            {
                return answerToBranches.GoToId;
            }

            return ElseBranch;
        }

        public string[] GetPermutations(string sentence)
        {
            var words = sentence.Split(' ')
                .Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x));

            var generatedPermutations = new List<string>();

            return null;

        }

        private static void permute(String str,
            int l, int r)
        {
            if (l == r)
                Console.WriteLine(str);
            else
            {
                for (int i = l; i <= r; i++)
                {
                    str = swap(str, l, i);
                    permute(str, l + 1, r);
                    str = swap(str, l, i);
                }
            }
        }

        /** 
        * Swap Characters at position 
        * @param a string value 
        * @param i position 1 
        * @param j position 2 
        * @return swapped string 
        */
        public static String swap(String a,
            int i, int j)
        {
            char temp;
            char[] charArray = a.ToCharArray();
            temp = charArray[i];
            charArray[i] = charArray[j];
            charArray[j] = temp;
            string s = new string(charArray);
            return s;
        }

        public string Id { get; set; }
        public string Question { get; set; }
        public PuzzleType PuzzleType { get; set; }
      
        /// <summary>
        /// кол-во попыток на получение правильного ответа
        /// </summary>
        public int? NumberOfAttemptsLimit { get; set; }

        /// <summary>
        /// время ожидания перед тем как можно переходить на след шаг
        /// </summary>
        public int? WaitingTime { get; set; }
        
        /// <summary>
        /// ветки развития. key - это ответ пользователя, value - ветка развития
        /// </summary>
        public IList<AnswerToBranch> PosibleBranches { get; set; }

        /// <summary>
        /// ветка развития, когда не подошел ни один из ответов
        /// </summary>
        public string ElseBranch { get; set; }

        /// <summary>
        /// признак что это последний шаг в квесте
        /// </summary>
        public bool IsLastPuzzle { get; set; }
    }
}