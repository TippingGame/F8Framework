using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using F8Framework.Core;
using NUnit.Framework;
using UnityEngine;

namespace F8Framework.ExcelData.Editor.Tests
{
    public sealed class ExcelDataToolTests
    {
        private string testParentRoot;
        private string testRoot;

        [SetUp]
        public void SetUp()
        {
            testParentRoot = Path.Combine(
                Path.GetFullPath(Path.Combine(Application.dataPath, "..")),
                "Library/F8ExcelDataToolTests");
            testRoot = Path.Combine(testParentRoot, Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(testRoot);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(testRoot))
            {
                Directory.Delete(testRoot, true);
            }

            if (Directory.Exists(testParentRoot) &&
                Directory.GetFileSystemEntries(testParentRoot).Length == 0)
            {
                Directory.Delete(testParentRoot);
            }
        }

        [Test]
        public void CommitGeneratedDirectoryReplacesOldConfiguration()
        {
            string outputPath = Path.Combine(testRoot, "BinConfigData");
            string stagingPath = Path.Combine(testRoot, "BinConfigData.f8-staging-test");
            Directory.CreateDirectory(outputPath);
            Directory.CreateDirectory(stagingPath);
            File.WriteAllText(Path.Combine(outputPath, "Old.bytes"), "old");
            File.WriteAllText(Path.Combine(stagingPath, "New.bytes"), "new");

            InvokePrivate("CommitGeneratedDirectory", stagingPath, outputPath);

            Assert.IsFalse(Directory.Exists(stagingPath));
            Assert.IsFalse(File.Exists(Path.Combine(outputPath, "Old.bytes")));
            Assert.AreEqual("new", File.ReadAllText(Path.Combine(outputPath, "New.bytes")));
        }

        [Test]
        public void MissingStagingDirectoryKeepsOldConfiguration()
        {
            string outputPath = Path.Combine(testRoot, "BinConfigData");
            string stagingPath = Path.Combine(testRoot, "missing-staging");
            Directory.CreateDirectory(outputPath);
            File.WriteAllText(Path.Combine(outputPath, "Old.bytes"), "old");

            TargetInvocationException exception = Assert.Throws<TargetInvocationException>(() =>
                InvokePrivate("CommitGeneratedDirectory", stagingPath, outputPath));

            Assert.IsInstanceOf<DirectoryNotFoundException>(exception.InnerException);
            Assert.AreEqual("old", File.ReadAllText(Path.Combine(outputPath, "Old.bytes")));
        }

        [Test]
        public void ExistingNonConfigurationFileStopsReplacement()
        {
            string outputPath = Path.Combine(testRoot, "BinConfigData");
            string stagingPath = Path.Combine(testRoot, "BinConfigData.f8-staging-test");
            Directory.CreateDirectory(outputPath);
            Directory.CreateDirectory(stagingPath);
            File.WriteAllText(Path.Combine(outputPath, "Keep.cs"), "user code");
            File.WriteAllText(Path.Combine(stagingPath, "New.bytes"), "new");

            TargetInvocationException exception = Assert.Throws<TargetInvocationException>(() =>
                InvokePrivate("CommitGeneratedDirectory", stagingPath, outputPath));

            Assert.IsInstanceOf<InvalidOperationException>(exception.InnerException);
            Assert.AreEqual("user code", File.ReadAllText(Path.Combine(outputPath, "Keep.cs")));
        }

        [Test]
        public void ProjectAssetsDirectoryIsRejectedAsOutput()
        {
            TargetInvocationException exception = Assert.Throws<TargetInvocationException>(() =>
                InvokePrivate("ValidateOutputDirectory", Application.dataPath, null));

            Assert.IsInstanceOf<InvalidOperationException>(exception.InnerException);
        }

        [Test]
        public void ExcelDiscoverySupportsNestedUppercaseExtension()
        {
            string nestedPath = Path.Combine(testRoot, "Nested");
            Directory.CreateDirectory(nestedPath);
            string excelPath = Path.Combine(nestedPath, "Config.XLSX");
            File.WriteAllText(excelPath, "test");
            File.WriteAllText(Path.Combine(nestedPath, "~$Ignored.xlsx"), "test");

            string[] files = (string[])InvokePrivate("GetExcelFiles", testRoot);

            CollectionAssert.AreEqual(new[] { excelPath }, files);
        }

        [Test]
        public void CommandLineRejectsUnsupportedExportFormat()
        {
            string[] arguments =
            {
                "ConvertExcelToOtherFormats-",
                "xml",
            };

            Assert.Throws<ArgumentException>(() =>
                ExcelDataSettings.ApplyCommandLineArguments(arguments));
        }

        [Test]
        public void LocalizedStringsGeneratorImplementsLocalizationItemContract()
        {
            ScriptGenerator generator = new ScriptGenerator(
                testRoot,
                "LocalizedStrings",
                new List<ReadExcel.ConfigData>
                {
                    CreateConfigData("int", "id"),
                    CreateConfigData("str", "TextID"),
                    CreateConfigData("str", "ChineseSimplified"),
                    CreateConfigData("str", "English"),
                });

            string source = generator.Generate();

            StringAssert.Contains(
                "public class LocalizedStringsItem : ILocalizationItem",
                source);
            StringAssert.Contains(
                "IReadOnlyList<string> ILocalizationItem.LanguageNames",
                source);
            StringAssert.Contains(
                "IReadOnlyList<string> ILocalizationItem.LanguageValues",
                source);
            StringAssert.Contains("nameof(ChineseSimplified)", source);
            StringAssert.Contains("nameof(English)", source);
            Assert.Less(
                source.IndexOf("nameof(ChineseSimplified)", StringComparison.Ordinal),
                source.IndexOf("nameof(English)", StringComparison.Ordinal));
        }

        [Test]
        public void LocalizedStringsGeneratorRequiresTextIdField()
        {
            ScriptGenerator generator = new ScriptGenerator(
                testRoot,
                "LocalizedStrings",
                new List<ReadExcel.ConfigData>
                {
                    CreateConfigData("int", "id"),
                    CreateConfigData("str", "ChineseSimplified"),
                });

            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
                generator.Generate());

            StringAssert.Contains("id 和 TextID", exception.Message);
        }

        [Test]
        public void LocalizedStringsGeneratorRequiresCanonicalSheetName()
        {
            ScriptGenerator generator = new ScriptGenerator(
                testRoot,
                "localizedstrings",
                new List<ReadExcel.ConfigData>
                {
                    CreateConfigData("int", "id"),
                    CreateConfigData("str", "TextID"),
                    CreateConfigData("str", "English"),
                });

            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
                generator.Generate());

            StringAssert.Contains("必须精确为 LocalizedStrings", exception.Message);
        }

        [Test]
        public void LocalizedStringsGeneratorRejectsAmbiguousReservedFields()
        {
            ScriptGenerator generator = new ScriptGenerator(
                testRoot,
                "LocalizedStrings",
                new List<ReadExcel.ConfigData>
                {
                    CreateConfigData("int", "id"),
                    CreateConfigData("int", "ID"),
                    CreateConfigData("str", "TextID"),
                    CreateConfigData("str", "English"),
                });

            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
                generator.Generate());

            StringAssert.Contains("都只能出现一次", exception.Message);
        }

        [Test]
        public void LocalizedStringsGeneratorDoesNotReserveLanguageFieldNames()
        {
            ScriptGenerator generator = new ScriptGenerator(
                testRoot,
                "LocalizedStrings",
                new List<ReadExcel.ConfigData>
                {
                    CreateConfigData("int", "id"),
                    CreateConfigData("str", "TextID"),
                    CreateConfigData("str", "LocalizationLanguageNames"),
                });

            string source = generator.Generate();

            StringAssert.Contains(
                "public System.String LocalizationLanguageNames;",
                source);
            StringAssert.Contains(
                "nameof(LocalizationLanguageNames)",
                source);
            StringAssert.DoesNotContain(
                "private static readonly string[] LocalizationLanguageNames",
                source);
        }

        [Test]
        public void LocalizationLoadsItemsThroughCoreContract()
        {
            const string testLanguage = "ContractTestLanguage";
            string previousLanguage = F8EditorPrefs.GetString(
                LocalizationConst.CurrentLanguageKey,
                string.Empty);
            Localization localization = Localization.EditorInstance;
            localization.OnTermination();
            localization = new Localization();

            try
            {
                F8EditorPrefs.SetString(LocalizationConst.CurrentLanguageKey, testLanguage);
                localization.Load(new Dictionary<int, TestLocalizationItem>
                {
                    {
                        1,
                        new TestLocalizationItem
                        {
                            Id = "1",
                            TextId = "Greeting",
                            LanguageNames = new[] { testLanguage, "English" },
                            LanguageValues = new[] { "测试", "Hello" },
                        }
                    },
                });

                CollectionAssert.AreEqual(
                    new[] { testLanguage, "English" },
                    localization.LanguageList);
                Assert.AreEqual(
                    "测试",
                    localization.GetTextFromIdLanguage("Greeting", testLanguage));
                Assert.AreEqual(
                    "Hello",
                    localization.GetTextFromIdLanguage("Greeting", "English"));
            }
            finally
            {
                localization.OnTermination();
                F8EditorPrefs.SetString(
                    LocalizationConst.CurrentLanguageKey,
                    previousLanguage);
            }
        }

        private static ReadExcel.ConfigData CreateConfigData(string type, string name)
        {
            return new ReadExcel.ConfigData
            {
                Type = type,
                Name = name,
            };
        }

        private sealed class TestLocalizationItem : ILocalizationItem
        {
            public string Id { get; set; }
            public string TextId { get; set; }
            public IReadOnlyList<string> LanguageNames { get; set; }
            public IReadOnlyList<string> LanguageValues { get; set; }
        }

        private static object InvokePrivate(string methodName, params object[] arguments)
        {
            MethodInfo method = typeof(ExcelDataTool).GetMethod(
                methodName,
                BindingFlags.Static | BindingFlags.NonPublic);
            Assert.IsNotNull(method, "找不到待测试方法：" + methodName);
            return method.Invoke(null, arguments);
        }
    }
}
