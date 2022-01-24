// Copyright (c) 2022 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEditor.Compilation;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class DeterministicCompilationTest
    {
        private readonly HashSet<string> _compilationAssemblies = new HashSet<string>();

        [SetUp]
        public void SetUp()
        {
            CompilationPipeline.assemblyCompilationFinished += OnAssemblyCompilationFinished;
        }

        [TearDown]
        public void TearDown()
        {
            CompilationPipeline.assemblyCompilationFinished -= OnAssemblyCompilationFinished;
        }

        private void OnAssemblyCompilationFinished(string assemblyPath, CompilerMessage[] messages)
        {
            var assemblyName = Regex.Match(assemblyPath, "Library/ScriptAssemblies/(.+)\\.dll").Groups[1].Value;
            _compilationAssemblies.Add(assemblyName);
            Debug.Log($"Recompiled {assemblyName}");
        }

        [UnityTest]
        public IEnumerator RecompileChild_ModPrivate_ParentAndGrandchildIsNotRecompile()
        {
            const string CompilationCountPattern = "CompilationCount(\\d+)";
            var path = Path.GetFullPath("Assets/AsmChild/Child.cs");
            var source = File.ReadAllText(path);
            var compilationCount = Regex.Match(source, CompilationCountPattern).Groups[1].Value;
            var newCompilationCount = int.Parse(compilationCount) + 1;
            var newSource = Regex.Replace(source, CompilationCountPattern,
                $"CompilationCount{newCompilationCount}");
            File.WriteAllText(path, newSource);

            // AssetDatabase.Refresh();
            yield return new RecompileScripts();
            // TODO: ここでreloadが走るため、たとえstaticにしても_compilationAssembliesはクリアされる
            // おそらく同梱の理由で、コンパイル時のエラーログで失敗されることもLogAssert.Expectも無効

            Assert.That(_compilationAssemblies, Does.Contain("AsmChild"));
            Assert.That(_compilationAssemblies, Does.Not.Contain("AsmParent"));
            Assert.That(_compilationAssemblies, Does.Not.Contain("AsmGrandchild"));
        }
    }
}
