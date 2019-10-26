using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScenarioBot.BotCommands;

namespace UnitTestProject
{
    [TestClass]
    public class TopCommandTests
    {
        [TestMethod]
        [DataRow("/top5", 5)]
        [DataRow("/top", 10)]
        [DataRow("/topN", 10)]
        public void GetUserCountTest(string input, int expectedOutput)
        {
            var actualOutput = TopCommand.GetUserCount(input);
            Assert.AreEqual(expectedOutput, actualOutput);
        }
        
        [TestMethod]
        [DataRow("top5", true)]
        [DataRow("top", true)]
        [DataRow("topN", false)]
        [DataRow("top99", true)]
        [DataRow("top100", false)]
        public void IsApplicableTest(string input, bool expectedOutput)
        {
            var actualOutput = new TopCommand(null).IsApplicable(input, null);
            Assert.AreEqual(expectedOutput, actualOutput);
        }
    }
}