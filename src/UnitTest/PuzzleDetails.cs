using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScenarioBot.Domain;

namespace UnitTestProject
{
    [TestClass]
    public class PuzzleDetailsTests
    {
//        [TestMethod]
//        [DataRow("xss", "xss", true)]
//        [DataRow("Андрей Дятлов", "Дятлов Андрей", true)]
//        [DataRow("Андрей Дятлов", "  андрей     дятлов    ", true)]
//        [DataRow("xss", "ssx", false)]
//        [DataRow("xss", null, false)]
//        [DataRow(null, "ssx", false)]
//        [DataRow(null, null, false)]
//        public void IsRightTest(string expected, string actual, bool isEqual)
//        {
//            var details = new PuzzleDetails();
//            details.PossibleAnswers = expected;
//            details.ActualAnswer = actual;
//            Assert.AreEqual(isEqual, details.IsRight);
//        }


        [TestMethod]
        [DataRow(@"async-await", "async await")]
        [DataRow(@"async\await", "async await")]
        [DataRow(@"async  await", "async await")]
        public void VanshTest(string input, string expectedOutput)
        {
            var actualOutput = PuzzleDetails.VanishAnswer(input);
            Assert.AreEqual(expectedOutput, actualOutput);
        }
    }
}