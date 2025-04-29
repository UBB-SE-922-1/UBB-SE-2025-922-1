# Duolingo Project Coding Standards

## 1. Naming Conventions

- Use PascalCase for:
  - Class names
  - Method names
- Use camelCase for:
  - Method parameters
  - Local variables

## 2. File Organization

- One class per file
- File name must match the class name
- Group related files in appropriate folders (e.g., Models, Views, Controllers)
- Keep files under 500 lines of code

## 3. Class Structure

- Fields should be declared at the top of the class

## 4. Method Design

- Methods should have a single responsibility
- Maximum of 6 parameters per method
- Use meaningful parameter names

## 5. Comments and Documentation

- Use // for single-line comments
- Use /\* \*/ for multi-line comments
- Comments should explain why, not what
- Remove commented-out code before committing

## 6. Error Handling

- Use try-catch blocks only for exceptional cases
- Log exceptions with appropriate context

## 7. Async/Await

- Use async/await instead of Task.ContinueWith
- Suffix async methods with "Async"

## 8. LINQ Usage

- Use FirstOrDefault() instead of First() when appropriate
- Consider performance implications of LINQ queries

## 9. Dependency Injection

- Keep constructors simple

## 10. Testing

- Write unit tests for business logic
- Use meaningful test names

## 11. Git Practices

- Write meaningful commit messages
- Keep commits focused and small
- Use feature branches
- Review code before merging
- Resolve merge conflicts properly

## 12. Security

- Validate all user input
- Use parameterized queries
- Implement proper authentication and authorization

## 13. Performance

- Use async I/O for database operations
- Cache frequently used data

## 14. Code Formatting

- Use 4 spaces for indentation
- Maximum line length of 120 characters
- Use braces for all control statements

## 15. Entity Framework

- Keep DbContext scope minimal
- Use proper data types for columns
