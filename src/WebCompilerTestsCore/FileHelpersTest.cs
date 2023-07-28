namespace WebCompilerTestsCore
{
    [TestClass]
    public class FileHelpersTest
    {
        public static IEnumerable<object[]> MakeRelativeData
        {
            get
            {
                yield return new object[] { "\\a", "/a", $".{Path.DirectorySeparatorChar}a" };
                yield return new object[] { "\\a", "\\a", $".{Path.DirectorySeparatorChar}a" };
                yield return new object[] { "/a", "/a", $".{Path.DirectorySeparatorChar}a" };
                yield return new object[] { "/a", "/a/b", $".{Path.DirectorySeparatorChar}b" };
                yield return new object[] { "/a", "/a/b/c", $".{Path.DirectorySeparatorChar}b{Path.DirectorySeparatorChar}c" };
            }
        }

        [TestMethod]
        [DynamicData(nameof(MakeRelativeData))]
        public void TestCases( string basePath, string filePath, string expectedResult)
        {
            var result = WebCompiler.FileHelpers.MakeRelative(basePath, filePath);
            Assert.AreEqual( expectedResult, result );
        }
    }
}

