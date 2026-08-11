using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace F8Framework.Core.Editor.Tests
{
    public sealed class F8EditorPipelineTests
    {
        [SetUp]
        public void SetUp()
        {
            F8EditorPipeline.CancelPending();
            CompletedTestStep.ExecutionCount = 0;
        }

        [TearDown]
        public void TearDown()
        {
            F8EditorPipeline.CancelPending();
        }

        [Test]
        public void BuilderOrdersByOrderThenInsertionSequence()
        {
            F8EditorPipelineBuilder builder = new F8EditorPipelineBuilder("Order Test")
                .Add("third", 30)
                .Add("first-a", 10)
                .Add("first-b", 10)
                .Add("second", 20);

            string[] orderedIds = builder.GetOrderedSteps().Select(step => step.Id).ToArray();

            CollectionAssert.AreEqual(
                new[] { "first-a", "first-b", "second", "third" },
                orderedIds);
        }

        [Test]
        public void CompletedPipelineRunsAndClearsPersistentState()
        {
            F8EditorPipelineBuilder builder = new F8EditorPipelineBuilder("Completion Test")
                .Add(CompletedTestStep.StepId, 0, "Complete");

            F8EditorPipeline.Start(builder);

            Assert.AreEqual(1, CompletedTestStep.ExecutionCount);
            Assert.IsFalse(F8EditorPipeline.HasPendingPipeline);
        }

        [Test]
        public void BuildWithoutExtensionsContainsOnlyRequestedCoreSteps()
        {
            F8BuildRequest request = new F8BuildRequest
            {
                DisplayName = "Core Only",
                IncludeExtensions = false,
                GenerateHotUpdateDll = true,
                BuildAssetBundles = true,
            };

            IReadOnlyList<F8EditorPipelineStepDefinition> steps =
                F8BuildPipeline.CreateBuilder(request).GetOrderedSteps();

            Assert.AreEqual(2, steps.Count);
            Assert.IsFalse(steps.Any(step => step.Id.StartsWith("f8.excel.")));
            Assert.Less(steps[0].Order, steps[1].Order);
        }

        [Test]
        public void PlayerAndUpdateBuildCannotBeRequestedTogether()
        {
            F8BuildRequest request = new F8BuildRequest
            {
                BuildPlayer = true,
                BuildUpdate = true,
            };

            Assert.Throws<System.InvalidOperationException>(() =>
                F8BuildPipeline.CreateBuilder(request));
        }

        [Test]
        public void RequiredCommandLineValueRejectsMissingValue()
        {
            string[] arguments = { "Platform-", "BuildPath-", "C:/Build" };

            Assert.Throws<System.ArgumentException>(() =>
                F8EditorCommandLine.GetRequiredValue(arguments, "Platform-"));
        }

        [Test]
        public void CorruptStateCanStillBeDetectedAndCancelled()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(StatePath));
            File.WriteAllText(StatePath, "{ invalid json");

            Assert.IsTrue(F8EditorPipeline.HasPendingPipeline);
            Assert.AreEqual("无效的流水线状态", F8EditorPipeline.PendingPipelineName);
            Assert.IsTrue(F8EditorPipeline.CancelPending());
            Assert.IsFalse(F8EditorPipeline.HasPendingPipeline);
        }

        [Test]
        public void TemporaryStateCanStillBeDetectedAndCancelled()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(StatePath));
            File.WriteAllText(StateTempPath, "{ invalid json");

            Assert.IsTrue(F8EditorPipeline.HasPendingPipeline);
            Assert.AreEqual("无效的流水线状态", F8EditorPipeline.PendingPipelineName);
            Assert.IsTrue(F8EditorPipeline.CancelPending());
            Assert.IsFalse(File.Exists(StateTempPath));
        }

        [Test]
        public void ValidTemporaryStateIsUsedWhenMainStateIsCorrupt()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(StatePath));
            File.WriteAllText(StatePath, "{ invalid json");
            File.WriteAllText(
                StateTempPath,
                "{\"Version\":1,\"PipelineId\":\"test\"," +
                "\"DisplayName\":\"Temporary State\",\"Steps\":[]," +
                "\"NextStepIndex\":0,\"Failed\":true}");

            Assert.AreEqual("Temporary State", F8EditorPipeline.PendingPipelineName);
            Assert.IsTrue(F8EditorPipeline.CancelPending());
        }

        private static string StatePath => Path.GetFullPath(Path.Combine(
            Application.dataPath,
            "../Library/F8EditorPipeline/state.json"));

        private static string StateTempPath => StatePath + ".tmp";

        public sealed class CompletedTestStep : IF8EditorPipelineStep
        {
            public const string StepId = "f8.tests.completed";
            public static int ExecutionCount;

            public string Id => StepId;

            public F8EditorPipelineStepResult Execute(F8EditorPipelineContext context)
            {
                ExecutionCount++;
                return F8EditorPipelineStepResult.Completed;
            }
        }
    }
}
