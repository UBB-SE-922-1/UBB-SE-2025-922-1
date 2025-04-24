# Duolingo Project Coding Standards

## 1. Naming Conventions

- Use PascalCase for:
  - Class names
  - Method names
  - Public properties
  - Public fields
  - Namespaces
- Use camelCase for:
  - Method parameters
  - Local variables
  - Private fields (prefixed with underscore)
  - Private properties

## 2. File Organization

- One class per file
- File name must match the class name
- Group related files in appropriate folders (e.g., Models, Views, Controllers)
- Keep files under 500 lines of code

## 3. Class Structure

- Fields should be declared at the top of the class
- Properties should follow fields
- Constructors should follow properties
- Public methods should come before private methods
- Use regions to group related members (optional)

## 4. Method Design

- Methods should have a single responsibility
- Keep methods under 20 lines of code
- Maximum of 4 parameters per method
- Use meaningful parameter names
- Return early when possible instead of deep nesting

## 5. Comments and Documentation

- Use XML documentation for public APIs
- Use // for single-line comments
- Use /\* \*/ for multi-line comments
- Comments should explain why, not what
- Remove commented-out code before committing

## 6. Error Handling

- Use try-catch blocks only for exceptional cases
- Log exceptions with appropriate context
- Never catch general Exception
- Use specific exception types
- Include meaningful error messages

## 7. Async/Await

- Use async/await instead of Task.ContinueWith
- Suffix async methods with "Async"
- Avoid async void methods
- ConfigureAwait(false) for library code
- Handle cancellation tokens properly

## 8. LINQ Usage

- Use method syntax for complex queries
- Use query syntax for simple joins
- Avoid multiple enumerations of the same query
- Use FirstOrDefault() instead of First() when appropriate
- Consider performance implications of LINQ queries

## 9. Dependency Injection

- Use constructor injection
- Register dependencies in Startup.cs
- Avoid service locator pattern
- Use interfaces for dependencies
- Keep constructors simple

## 10. Testing

- Write unit tests for business logic
- Use meaningful test names
- Follow Arrange-Act-Assert pattern
- One assertion per test when possible
- Use mocking frameworks appropriately

## 11. Git Practices

- Write meaningful commit messages
- Keep commits focused and small
- Use feature branches
- Review code before merging
- Resolve merge conflicts properly

## 12. Security

- Never store secrets in code
- Use environment variables for configuration
- Validate all user input
- Use parameterized queries
- Implement proper authentication and authorization

## 13. Performance

- Use StringBuilder for string concatenation in loops
- Implement IDisposable properly
- Use async I/O for database operations
- Cache frequently used data
- Profile code before optimization

## 14. Code Formatting

- Use 4 spaces for indentation
- Maximum line length of 120 characters
- Use braces for all control statements
- Place braces on new lines
- Use consistent spacing around operators

## 15. Entity Framework

- Use navigation properties for relationships
- Implement proper indexing
- Use async methods for database operations
- Keep DbContext scope minimal
- Use proper data types for columns
