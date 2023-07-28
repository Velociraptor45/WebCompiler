﻿using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebCompiler;

namespace WebCompilerTest
{
    [TestClass]
    public class LessTest
    {
        private ConfigFileProcessor _processor;

        [TestInitialize]
        public void Setup()
        {
            _processor = new ConfigFileProcessor();
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            File.Delete("../../../../WebCompilerTest/artifacts/less/test.css");
            File.Delete("../../../../WebCompilerTest/artifacts/less/test.min.css");
            File.Delete("../../../../WebCompilerTest/artifacts/less/error.css");
            File.Delete("../../../../WebCompilerTest/artifacts/less/relative.css");
            File.Delete("../../../../WebCompilerTest/artifacts/less/relative.min.css");
            File.Delete("../../../../WebCompilerTest/artifacts/less/circrefa.css");
            File.Delete("../../../../WebCompilerTest/artifacts/less/circrefa.min.css");
        }

        [TestMethod, TestCategory("LESS")]
        public void CompileLess()
        {
            var result = _processor.Process("../../../../WebCompilerTest/artifacts/lessconfig.json");
            Assert.IsTrue(result.All(r => !r.HasErrors));
            Assert.IsTrue(File.Exists("../../../../WebCompilerTest/artifacts/less/test.css"));
            Assert.IsTrue(File.Exists("../../../../WebCompilerTest/artifacts/less/test.min.css"));
            Assert.IsTrue(result.ElementAt(1).CompiledContent.Contains("url(foo.png)"));
            Assert.IsTrue(result.ElementAt(1).CompiledContent.Contains("-webkit-animation"), "AutoPrefix");

            Assert.IsTrue(File.ReadAllText("../../../../WebCompilerTest/artifacts/less/test.min.css").Contains("important comment"), "Default options");

            string sourceMap = ScssTest.DecodeSourceMap(result.ElementAt(1).CompiledContent);
            Assert.IsTrue(sourceMap.Contains("\"relative.less\""), "Source map paths");

            string compiled = result.First().CompiledContent;
            int top = compiled.IndexOf("top");
            int pos = compiled.IndexOf("position");
            Assert.IsTrue(pos < top, "CSS Comb ordering");
        }

        [TestMethod, TestCategory("LESS")]
        public void CompileLessWithError()
        {
            var result = _processor.Process("../../../../WebCompilerTest/artifacts/lessconfigerror.json");
            Assert.IsTrue(result.Any(r => r.HasErrors));
            Assert.IsTrue(result.Count() == 1);
            Assert.IsTrue(result.ElementAt(0).HasErrors);
        }

        [TestMethod, TestCategory("LESS")]
        public void CompileLessWithParsingExceptionError()
        {
            var result = _processor.Process("../../../../WebCompilerTest/artifacts/lessconfigParseerror.json");
            Assert.IsTrue(result.Any(r => r.HasErrors));
            Assert.IsTrue(result.Count() == 1);
            Assert.IsTrue(result.ElementAt(0).HasErrors);
            Assert.AreNotEqual(0, result.ElementAt(0).Errors.ElementAt(0).LineNumber, "LineNumber is set when engine.TransformToCss generate a ParsingException");
            Assert.AreNotEqual(0, result.ElementAt(0).Errors.ElementAt(0).ColumnNumber, "ColumnNumber is set when engine.TransformToCss generate a ParsingException");
        }

        [TestMethod, TestCategory("LESS")]
        public void CompileLessWithOptions()
        {
            var result = ConfigHandler.GetConfigs("../../../../WebCompilerTest/artifacts/lessconfig.json");
            Assert.IsTrue(result.First().Options.Count == 2);
        }

        [TestMethod, TestCategory("LESS")]
        public void AssociateExtensionSourceFileChangedTest()
        {
            var result = _processor.SourceFileChanged("../../../../WebCompilerTest/artifacts/lessconfig.json", "less/test.less", null);
            Assert.IsTrue(result.All(r => !r.HasErrors));
            Assert.AreEqual(2, result.Count<CompilerResult>());
        }

        [TestMethod, TestCategory("LESS")]
        public void OtherExtensionTypeSourceFileChangedTest()
        {
            var result = _processor.SourceFileChanged("../../../../WebCompilerTest/artifacts/lessconfig.json", "scss/test.scss", null);
            Assert.IsTrue(result.All(r => !r.HasErrors));
            Assert.AreEqual(0, result.Count<CompilerResult>());
        }

        [TestMethod, TestCategory("LESS")]
        public void CompileCircularReference()
        {
            // Subtract one second from the last write time to ensure the less files are older than the css files
            // otherwise the tests are flaky because the css files are created in the same millisecond as the
            // last write time of the less files
            var lastWriteTime = DateTime.UtcNow.AddSeconds(-1);

            // Set the last write time and create outputs in a way that Config.CheckForNewerDependenciesRecursively will be called
            File.SetLastWriteTimeUtc("../../../../WebCompilerTest/artifacts/less/circrefa.less", lastWriteTime);
            File.SetLastWriteTimeUtc("../../../../WebCompilerTest/artifacts/less/circrefb.less", lastWriteTime);
            File.WriteAllText("../../../../WebCompilerTest/artifacts/less/circrefa.css", string.Empty);
            File.WriteAllText("../../../../WebCompilerTest/artifacts/less/circrefa.min.css", string.Empty);

            // Since the outputs were generated after the inputs, no compilation should have occurred
            var result = _processor.Process("../../../../WebCompilerTest/artifacts/lessconfigCircRef.json");
            Assert.AreEqual(0, result.Count<CompilerResult>());
        }

        public void CompileLessLegacyStrictMath()
        {
            var result = _processor.Process("../../../../WebCompilerTest/artifacts/lessconfigLegacyStrictMath.json");
            Assert.IsTrue(result.All(r => !r.HasErrors || r.Errors.All(e => e.IsWarning)));
        }
    }
}
