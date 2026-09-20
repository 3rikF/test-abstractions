---
name: documentation-rules
description: Instructs Copilot how to write documentation for the codebase, including rules for formatting, style, and content.
---

# Documentation Generator Skill

When the user asks for documentation, follow these strict rules.

## Formatting Rules
1. **Language:**
  Always write XML documentation comments (`///`) in English, regardless of the language used in the prompt or surrounding UI text.
  Always write normal comments (`//`) in English too, regardless of the language used in the prompt or surrounding UI text.

2. **Format:**
  Follow standard .NET XML tag conventions (`<summary>`, `<remarks>`, `<param>`, `<returns>`, `<exception>`, `<inheritdoc />`).
  For type references in XML documentation, use the `cref` attribute, e.g. `<see cref="MyClass"/>`.
  For property references in XML documentation, use the `cref` attribute, e.g. `<see cref="MyClass.MyProperty"/>`.
  For parameter references in XML documentation, use the `name` attribute, e.g. `<param name="myParameter"/>`.

3. **Style:**
  Use concise, professional technical English.