// Copyright (c) 2022 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections;
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
        private static string RecompiledAssembliesList => Path.GetFullPath("Temp/RecompiledAssembliesList.txt");
        // Note: Persist to file because domain reloads in test.

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
            File.AppendAllText(RecompiledAssembliesList, $"{assemblyName}\n");
            Debug.Log($"Recompiled {assemblyName}");
        }

        [UnityTest]
        public IEnumerator RecompileChild_ModPrivate_ParentAndGrandchildIsNotRecompile()
        {
            File.Delete(RecompiledAssembliesList);

            const string CompilationCountPattern = "CompilationCount(\\d+)";
            var path = Path.GetFullPath("Assets/AsmChild/Child.cs");
            var source = File.ReadAllText(path);
            var compilationCount = Regex.Match(source, CompilationCountPattern).Groups[1].Value;
            var newCompilationCount = int.Parse(compilationCount) + 1;
            var newSource = Regex.Replace(source, CompilationCountPattern,
                $"CompilationCount{newCompilationCount}");
            File.WriteAllText(path, newSource);

            yield return new RecompileScripts();

            var recompiledAssemblies = File.ReadAllLines(RecompiledAssembliesList);
            Assert.That(recompiledAssemblies, Does.Contain("AsmChild"));
            Assert.That(recompiledAssemblies, Does.Not.Contain("AsmParent"));
            Assert.That(recompiledAssemblies, Does.Not.Contain("AsmGrandchild"));
        }
    }
}
