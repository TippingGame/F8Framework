using System.Collections.Generic;
using NUnit.Framework;

namespace F8Framework.Core.Editor.Tests
{
    public sealed class ConfigDataSourceRegistryTests
    {
        private const string LowSourceId = "tests.low";
        private const string HighSourceId = "tests.high";
        private const string BrokenSourceId = "tests.broken";

        [TearDown]
        public void TearDown()
        {
            ConfigDataSourceRegistry.Unregister(LowSourceId);
            ConfigDataSourceRegistry.Unregister(HighSourceId);
            ConfigDataSourceRegistry.Unregister(BrokenSourceId);
        }

        [Test]
        public void HighestPriorityAvailableSourceWins()
        {
            ConfigDataSourceRegistry.Register(
                new TestSource(LowSourceId, int.MaxValue - 1, "low"));
            ConfigDataSourceRegistry.Register(
                new TestSource(HighSourceId, int.MaxValue, "high"));

            bool loaded = ConfigDataSourceRegistry.TryLoadAll(
                out Dictionary<string, object> data);

            Assert.IsTrue(loaded);
            Assert.AreEqual("high", data["value"]);
        }

        [Test]
        public void UnavailableSourceFallsBackToNextSource()
        {
            ConfigDataSourceRegistry.Register(
                new TestSource(BrokenSourceId, int.MaxValue, "broken", isAvailable: false));
            ConfigDataSourceRegistry.Register(
                new TestSource(LowSourceId, int.MaxValue - 1, "fallback"));

            bool loaded = ConfigDataSourceRegistry.TryLoadAll(
                out Dictionary<string, object> data);

            Assert.IsTrue(loaded);
            Assert.AreEqual("fallback", data["value"]);
        }

        private sealed class TestSource : IConfigDataSource
        {
            private readonly string value;
            private readonly bool isAvailable;

            public TestSource(
                string id,
                int priority,
                string value,
                bool isAvailable = true)
            {
                Id = id;
                Priority = priority;
                this.value = value;
                this.isAvailable = isAvailable;
            }

            public string Id { get; }
            public int Priority { get; }
            public bool IsAvailable => isAvailable;

            public void LoadAll(IDictionary<string, object> destination)
            {
                destination["value"] = value;
            }
        }
    }
}
