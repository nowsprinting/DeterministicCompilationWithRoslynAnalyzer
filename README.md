# Deterministic Compilation with Roslyn Analyzer

This Unity project is a reproducible project about the issue where "[Deterministic Compilation](https://unity.com/releases/2020-2/programmer-tools#improve-compilation-times-deterministic-compilation)" is disabled when Roslyn Analyzer is in the project.

The issue occurred in Unity 2020.3 and below, and has been fixed in Unity 2020.1.



### Assembly structures

- `AsmParent`
- `AsmChild` (Assembly Definition References: `AsmParent`)
- `AsmGrandchild` (Assembly Definition References: `AsmChild`)



### How the DeterministicCompilationTest works

`DeterministicCompilationTest` rewrites a **private** method in the source code in `AsmChild`.
If "Deterministic Compilation" is enabled, only `AsmChild` will be recompiled.



### Test results

#### With an Roslyn Analyzer without asmdef (master)

- Failed run on Unity 2020.3.26f1
- Passed run on Unity 2021.1.0f1

See [workflow log](https://github.com/nowsprinting/DeterministicCompilationWithRoslynAnalyzer/actions/runs/1759030848)


#### With an Roslyn Analyzer under asmdef (use_asmdef_references)

- Failed run on Unity 2020.3.26f1
- Passed run on Unity 2021.1.0f1

See [workflow log](https://github.com/nowsprinting/DeterministicCompilationWithRoslynAnalyzer/actions/runs/1759058793)


#### Without an Roslyn Analyzer (no_roslyn_analyzers)

- Passed run on Unity 2020.3.26f1
- Passed run on Unity 2021.1.0f1

See [Workflow log](https://github.com/nowsprinting/DeterministicCompilationWithRoslynAnalyzer/actions/runs/1759120957)
