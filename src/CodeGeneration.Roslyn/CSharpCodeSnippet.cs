// Copyright (c) Andrew Arnott. All rights reserved.
// Licensed under the MS-PL license. See LICENSE.txt file in the project root for full license information.

namespace CodeGeneration.Roslyn
{
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    /// <summary>Represents a C# code snippet.</summary>
    /// <typeparam name="TSnippetArguments">
    /// An argument class that helps transforming the text befor parsing it.
    /// </typeparam>
    public abstract class CSharpCodeSnippet<TSnippetArguments>
        where TSnippetArguments : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpCodeSnippet{TSnippetArguments}"/> class.
        /// </summary>
        /// <param name="text">
        /// The text of the code snippet.
        /// </param>
        /// <param name="fileName">
        /// The file name of the code snippet (optional).
        /// </param>
        /// <param name="parseOptions">
        /// The parse options.
        /// </param>
        protected CSharpCodeSnippet(string text, string fileName, CSharpParseOptions parseOptions)
        {
            Text = text;
            FileName = fileName;
            ParseOptions = parseOptions ?? CSharpParseOptions.Default;
        }

        /// <summary>Gets the (untransformed) text of the code snippet.</summary>
        public string Text { get; }

        /// <summary>Gets the file name of the code snippet.</summary>
        public string FileName { get; }

        /// <summary>Gets the <see cref="CSharpParseOptions"/> to parse the <see cref="Text"/> with.</summary>
        public CSharpParseOptions ParseOptions { get; }

        /// <summary>Parses the code snippet.</summary>
        /// <typeparam name="TSyntax">
        /// The <see cref="SyntaxNode"/> type of the code snippet.
        /// </typeparam>
        /// <returns>
        /// The parsed <see cref="SyntaxNode"/> of the code snippet.
        /// </returns>
        public Task<TSyntax> ParseAysnc<TSyntax>()
            where TSyntax : SyntaxNode
        {
            return ParseAysnc<TSyntax>(default);
        }

        /// <summary>Parses the code snippet.</summary>
        /// <typeparam name="TSyntax">
        /// The <see cref="SyntaxNode"/> type of the code snippet.
        /// </typeparam>
        /// <param name="arguments">
        /// The (optional) arguments to transform the code snippet before parsing.
        /// </param>
        /// <returns>
        /// The parsed <see cref="SyntaxNode"/> of the code snippet.
        /// </returns>
        public async Task<TSyntax> ParseAysnc<TSyntax>(TSnippetArguments arguments)
            where TSyntax : SyntaxNode
        {
            var tranformed = arguments is null ? Text : TransformText(arguments);

            var tree = CSharpSyntaxTree.ParseText(tranformed, ParseOptions, FileName, Encoding.UTF8, default);
            var root = await tree.GetRootAsync(default);

            return root is TSyntax requested
                ? requested
                : root.ChildNodes().FirstOrDefault(ch => ch is TSyntax) as TSyntax;
        }

        /// <inheritdoc />
        public override string ToString() => Text;

        /// <summary>Transforms the text before parsing it.</summary>
        /// <param name="arguments">
        /// The arguments to transform the code snippet before parsing.
        /// </param>
        /// <returns>
        /// The transformed <see cref="Text"/>.
        /// </returns>
        protected abstract string TransformText(TSnippetArguments arguments);
    }
}
