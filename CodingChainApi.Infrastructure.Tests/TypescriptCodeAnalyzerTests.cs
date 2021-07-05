using CodingChainApi.Infrastructure.Services.Parser;
using NUnit.Framework;

namespace NeosCodingApi
{
    public class TypescriptCodeAnalyzerTests
    {
        private TypescriptCodeAnalyzer? _analyzer;

        private string GetOutputValidator(string type)
        {
            return $@"
export function outputValidator(test: {type}): boolean {{
    return 1;
}}";
        }

        private string GetInputGenerator(string type)
        {
            return $@"
export function inputGenerator(): {type} {{
    return ""1"";
}}";
        }


        [SetUp]
        public void Setup()
        {
            _analyzer = new TypescriptCodeAnalyzer();
        }

        [Test]
        [TestCase("string")]
        [TestCase("{test:boolean, test2: string}")]
        [TestCase("string[]")]
        public void simple_input_type_should_works(string expectedType)
        {
            var res = _analyzer?.GetInputType(GetOutputValidator(expectedType));
            Assert.AreEqual(expectedType, res);
        }

        [Test]
        [TestCase("string")]
        [TestCase("{test:boolean, test2: string}")]
        [TestCase("string[]")]
        public void simple_output_type_should_works(string expectedType)
        {
            var res = _analyzer?.GetReturnType(GetInputGenerator(expectedType));
            Assert.AreEqual(expectedType, res);
        }
    }
}