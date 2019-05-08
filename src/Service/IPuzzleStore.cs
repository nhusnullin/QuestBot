using System;
using System.Collections.Generic;
using Shields.GraphViz.Components;
using Shields.GraphViz.Models;
using Shields.GraphViz.Services;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Immutable;

namespace CoreBot.Service
{
    public interface IScenarioStore
    {

    }

    public class InMemoryScenarioStore
    {
        public static Scenario GetScenario1()
        {
            var scenario = new Scenario() { ScenarioId = "scenario1" };
            var root = new Puzzle("xss")
            {
                Question = "Ночь. Тишина. Тебе снится странный сон: кто-то проник в ЦОД... Добро пожаловать в рабство." +
            "... Миша Щербаков... Что такого потенциального могло случиться? Что это была за атака?",
            }.To(scenario)
                .AddBranch("xss", "zero gc")
                .AddElseBranch("xss--")
                ;


            var yesQuestion2 = new Puzzle()
            {
                Id = "zero gc",
                Question = "Кажется - это тот самый gc, о котором нам рассказывал Кондрад. Как он назывался?",
            }.To(scenario)
                .AddBranch("zero gc", "andrei");

            var noQuestion2 = new Puzzle()
            {
                Id = "xss--",
                Question = "попроси помощи у Миши",
            }.To(scenario);

            var yesQuestion3 = new Puzzle()
            {
                Id = "andrei",
                Question = "Душно... Кажется ребята из JB ужерешали эту проблему. В решарпере она точно решена. Кто мне может с этим помочь? ",
            }.To(scenario)
                .AddBranch("Андрей Дятлов", "hell");


            var noQuestion33 = new Puzzle()
            {
                Id = "timeot 30 min",
                Question = "Я не могу дышать, сейчас будет timeot 30 min",
                WaitnigTime = 30
            }.To(scenario);

            var yesQuestion5 = new Puzzle()
            {
                Id = "hell",
                Question = "Андрей рассказывает коротко... Можно \\ Нужно бежать. Песня. Кажется я вспомнил где находится ЦОД",
//                Answer = "hell",
            }.To(scenario);

            var noQuestion5 = new Puzzle()
            {
                Id = "andrei2",
                Question = "Кажется, кажется. К кому бежать? ",
//                Answer = "Андрей Дятлов"
            }.To(scenario);

            var noQuestion55 = new Puzzle()
            {
                Id = "hell-",
                Question = "Душно. Обморок. Сон.",
//                Answer = "hell",
                WaitnigTime = 30,
                NumberOfAttemptsLimit = 5
            }.To(scenario);

            var gameOver = new Puzzle()
            {
                Id = "Game over",
                Question = "Game over!"
            }.To(scenario);



            return scenario;
        }


        public static async Task<string> Generate(Scenario scenario)
        {
            var statements = new List<Statement>();
            foreach (var item in scenario.Collection)
            {
                foreach (var branch in item.PosibleBranches)
                {
                    statements.Add(new EdgeStatement(item.Id, branch.GoToId, new Dictionary<Id, Id> { { "label", $"{branch.Answer}" } }.ToImmutableDictionary()));
                }
                statements.Add(new EdgeStatement(item.Id, item.ElseBranch, new Dictionary<Id, Id> { { "label", "else" } }.ToImmutableDictionary()));
            }

            var graph = Graph.Undirected.AddRange(statements);
            IRenderer renderer = new Renderer(@"C:\projects\bot\graphviz-2.38\release\bin");
            var cProjectsBotOutGraphPng = $@"C:\projects\bot\out\{Guid.NewGuid()}.png";
            using (Stream file = File.Create(cProjectsBotOutGraphPng))
            {
                await renderer.RunAsync(
                    graph, file,
                    RendererLayouts.Dot,
                    RendererFormats.Png,
                    CancellationToken.None);
            }

            return cProjectsBotOutGraphPng;
        }
    }
}