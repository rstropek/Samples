# C# 14 Samples

This project contains comprehensive samples demonstrating the new features introduced in C# 14. Each sample focuses on a specific language enhancement and includes detailed explanations, code examples, and practical use cases.

## Sample Projects

| Project | Feature | Description |
|---------|---------|-------------|
| [CompoundAssignment](./CompoundAssignment/) | User-Defined Compound Assignment Operators | Demonstrates how to implement compound operators (`+=`, `-=`, etc.) as instance methods with `void` return type for in-place mutation and zero allocations. Perfect for performance-sensitive structs and large classes. |
| [ExtensionMembers](./ExtensionMembers/) | Extension Members | Comprehensive guide to the new extension member syntax that enables extension properties, static extension members, and better organization of related functionality. Includes examples of traditional vs. new syntax, generic extensions, and IL inspection tools. |
| [Fields](./Fields/) | The `field` Keyword | Explores the contextual `field` keyword that provides direct access to compiler-synthesized backing fields. Covers property validation, caching, data transformation, and change notification patterns. |
| [LambdaModifiers](./LambdaModifiers/) | Lambda Parameter Modifiers | Shows how to use parameter modifiers (`ref`, `in`, `out`, `scoped`, `ref readonly`) on lambda parameters without requiring explicit type declarations, enabling more concise and readable lambda expressions. |
| [NameofGenerics](./NameofGenerics/) | `nameof` with Unbound Generic Types | Demonstrates using the `nameof` operator with unbound generic types (e.g., `nameof(IList<>)`), providing a type-safe way to reference generic type names without specifying type arguments. |
| [NullConditional](./NullConditional/) | Null-Conditional Assignment | Demonstrates the ability to use null-conditional operators (`?.`, `?[]`) on the left-hand side of assignments and compound assignments, reducing the need for explicit null checks. |
| [SpanConversions](./SpanConversions/) | First-Class Span Conversions | Covers the new implicit conversions for `Span<T>` and `ReadOnlySpan<T>` types, including array-to-span, span-to-readonly-span, covariant spans, and string-to-readonly-span conversions for allocation-free operations. |
