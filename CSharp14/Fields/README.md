# The C# 14 `field` Keyword

The `field` keyword in C# 14 represents an interesting addition to property syntax in C# versions. This contextual keyword provides direct access to compiler-synthesized backing fields, enabling developers to write cleaner property implementations without the traditional boilerplate of explicit backing fields.

## Fundamentals

The `field` keyword is a **contextual keyword** introduced in C# 14 that allows direct access to the compiler-generated backing field of a property. It bridges the gap between the simplicity of auto-implemented properties and the flexibility of fully custom properties.

### Core Concepts

- **Compiler-Synthesized Backing Field**: The `field` keyword refers to an unnamed backing field automatically created by the compiler
- **Property-Scoped Access**: The backing field is only accessible within the property's accessors, maintaining proper encapsulation
- **Mixed Accessor Patterns**: You can combine auto-implemented accessors (`get;`) with custom logic accessors (`set => field = value`)
- **Type Inference**: The backing field has the same type as the property

## Demo: Four Essential Use Cases

The following sections explore four fundamental patterns demonstrated in this project's sample code.

### Property Validation ([View Implementation](01-PropertyValidation.cs))

Property validation represents one of the most common and powerful applications of the `field` keyword. This pattern enables robust input validation while maintaining clean, readable code.

This approach eliminates the traditional pattern of declaring private backing fields and manually implementing both accessors for validation scenarios.

### Property Caching and Lazy Computation ([View Implementation](02-PropertyCache.cs))

The caching pattern showcases how the `field` keyword enables elegant lazy computation and performance optimization strategies.

### Data Transformation on Set ([View Implementation](03-TrimmingOnSet.cs))

Data transformation demonstrates how the `field` keyword enables automatic data normalization and formatting during property assignment.

### Property Change Notification ([View Implementation](04-Notification.cs))

The notification pattern illustrates how the `field` keyword integrates with the `INotifyPropertyChanged` interface for data binding scenarios.
