//
// IMetadataProperty.cs
//
// Author:
//       Lluis Sanchez Gual <lluis@xamarin.com>
//
// Copyright (c) 2015 Xamarin, Inc (http://www.xamarin.com)
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
using System.Xml;
using Microsoft.Build.BuildEngine;
using System.Xml.Linq;
using MonoDevelop.Core;
using System.Globalization;

namespace MonoDevelop.Projects.MSBuild
{

	public interface IMetadataProperty
	{
		string Name { get; }

		string Value { get; }

		string UnevaluatedValue { get; }

		T GetValue<T> ();

		object GetValue (Type t);

		FilePath GetPathValue (bool relativeToProject = true, FilePath relativeToPath = default(FilePath));

		bool TryGetPathValue (out FilePath value, bool relativeToProject = true, FilePath relativeToPath = default(FilePath));

		void SetValue (string value, bool preserveCase = false, bool mergeToMainGroup = false, MSBuildValueType valueType = null);

		void SetValue (FilePath value, bool relativeToProject = true, FilePath relativeToPath = default(FilePath), bool mergeToMainGroup = false);

		void SetValue (object value, bool mergeToMainGroup = false);
	}
	
}
