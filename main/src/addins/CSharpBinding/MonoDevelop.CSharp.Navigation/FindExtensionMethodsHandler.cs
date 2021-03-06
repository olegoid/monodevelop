//
// FindExtensionMethodHandler.cs
//
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2013 Xamarin Inc. (http://xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using MonoDevelop.Ide;
using MonoDevelop.Ide.FindInFiles;
using MonoDevelop.Ide.TypeSystem;
using MonoDevelop.Core;
using Microsoft.CodeAnalysis;
using MonoDevelop.Components.Commands;
using MonoDevelop.Refactoring;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory6.CSharp;

namespace MonoDevelop.CSharp.Navigation
{
	class FindExtensionMethodsHandler : CommandHandler
	{
		protected override async void Update (CommandInfo info)
		{
			var sym = await GetNamedTypeAtCaret (IdeApp.Workbench.ActiveDocument);
			info.Enabled = sym != null && sym.IsKind (SymbolKind.NamedType);
			info.Bypass = !info.Enabled;
		}

		protected async override void Run ()
		{
			var doc = IdeApp.Workbench.ActiveDocument;
			var sym = await GetNamedTypeAtCaret (doc);
			if (sym != null)
				FindExtensionMethods (await doc.GetCompilationAsync (), sym);
		}

		internal static async System.Threading.Tasks.Task<INamedTypeSymbol> GetNamedTypeAtCaret (Ide.Gui.Document doc)
		{
			if (doc == null)
				return null;
			var info = await RefactoringSymbolInfo.GetSymbolInfoAsync (doc, doc.Editor);
			var sym = info.Symbol ?? info.DeclaredSymbol;
			return sym as INamedTypeSymbol;
		}

		void FindExtensionMethods (Compilation compilation, ISymbol sym)
		{
			var symType = sym as ITypeSymbol;
			if (symType == null)
				return;
			using (var monitor = IdeApp.Workbench.ProgressMonitors.GetSearchProgressMonitor (true, true)) {
				foreach (var type in compilation.Assembly.GlobalNamespace.GetAllTypes ()) {
					if (!type.MightContainExtensionMethods)
						continue;

					foreach (var extMethod in type.GetMembers ().OfType<IMethodSymbol> ().Where (method => method.IsExtensionMethod)) {
						var reducedMethod = extMethod.ReduceExtensionMethod (symType);
						if (reducedMethod != null) {
							var loc = extMethod.Locations.First ();
							monitor.ReportResult (new MemberReference (extMethod, loc.SourceTree.FilePath, loc.SourceSpan.Start, loc.SourceSpan.Length));
						}
					}
				}
			}
		}
	}
}