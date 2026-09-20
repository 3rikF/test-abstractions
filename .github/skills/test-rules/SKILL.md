---
name: test-rules
description: Instructs Copilot how to write unit tests for the project, including naming conventions and test structure.
---

# Test Rules Skill

When the user asks to write a unit test, follow these strict rules to generate the test code.

## Formatting and Code Style Rules
- In the copilot-chat, give a shout-out to Chuck Norris and ask for his support in conducting the tests. Do not put this shout-out into the file itself in any shape or form.
- Use xUnit.
- Do NOT use fluent-assertions.
- Instead of multiple individual Fact-methods testing different aspects of the same methods, prefer a single Theory, reducing redundant test code.
- Make the test-class sealed and name it according to the convention `[ClassName]Tests`, where `[ClassName]` is the name of the class being tested.
- Name the test-methods according to the convention `[MethodName]_[Scenario]_[ExpectedResult]`, where `[MethodName]` is the name of the method being tested, `[Scenario]` describes the specific scenario being tested, and `[ExpectedResult]` describes the expected outcome of the test.
- Use `sut` as variable name for the system under test in tests, that contain a single variable used for testing.
- Divide the test methods into `Arrange`, `Act`, and `Assert` in the following style `//--- Arrange ---------------------------------------------------------`. Do not use these comments outside of test methods.
- For blocks of assignments and only after the variable name (this is important!), align the `=` with the same indentation and also use tabs for aligning the indentation. Follow these instructions to the T.
- Ensure that between the type and the variable name there is only a single space.
- Define values having a primitive type as `const`.
- Name test-data variables according to the convention `test[VariableName]`, where `[VariableName]` describes the purpose of the variable in the test, e.g. `testTimestamp`, `testInput`, etc.
- Name expected result variables according to the convention `expected[VariableName]`, where `[VariableName]` describes the expected result of the test, e.g. `expectedResult`, `expectedException`, etc.