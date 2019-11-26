// Copyright (c) Andrew Arnott. All rights reserved.
// Licensed under the MS-PL license. See LICENSE.txt file in the project root for full license information.

namespace CodeGeneration.Roslyn.Tests
{
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Xunit;

    public class CSharpCodeSnippetTest
    {
        [Fact]
        public async Task ParseAsync_StructDefinition_Parsed()
        {
            var snippet = new SimpleCodeSnippet(@"
public struct @Name
{
    public @Name(int count) => Count = count;
    public int Count { get; }
}");

            var syntax = await snippet.ParseAysnc<StructDeclarationSyntax>(new TransformArguments { Name = "TestStruct" });

            var expected = @"
public struct TestStruct
{
    public TestStruct(int count) => Count = count;
    public int Count { get; }
}";
            Assert.Equal(expected, syntax.ToFullString());
        }

        [Fact]
        public async Task ParseAsync_MethodDeclaration_Parsed()
        {
            var snippet = new SimpleCodeSnippet(@"public string ToDebug(string par) => par.ToUpper();");
            var syntax = await snippet.ParseAysnc<MethodDeclarationSyntax>();

            var expected = @"public string ToDebug(string par) => par.ToUpper();";
            Assert.Equal(expected, syntax.ToFullString());
        }
    }

    internal class SimpleCodeSnippet : CSharpCodeSnippet<TransformArguments>
    {
        public SimpleCodeSnippet(string text)
            : base(text, "test.cs", null) { }

        protected override string TransformText(TransformArguments arguments)
        {
            return Text.Replace("@Name", arguments.Name);
        }
    }

    internal class TransformArguments
    {
        public string Name { get; set; }
    }
}
