# Contributing to SmartifyOS

Thank you for your interest in contributing to my project! Please take a moment to review our guidelines for contributing.

## Commit Message Guidelines

For your commit messages please follow [Conventional Commits](https://www.conventionalcommits.org/).
(If you are using VS Code you can use [this extension](https://marketplace.visualstudio.com/items?itemName=vivaxy.vscode-conventional-commits) to help you follow this convention)

1. **Structure:**
   - Use the following format for commit messages:
     ```
     type: subject
     ```
     or
     ```
     type(scope): subject
     ```

   - **Type:** A brief type of change (e.g., feat, fix, docs, style, refactor, test, chore).
   - **Scope (Optional):** The section of the codebase that the change affects (e.g., component, module).
   - **Subject:** A short description of the change (e.g., "add new login feature").

2. **Examples:**
   - `feat(auth): add user login`
   - `fix(api): handle null values in response`
   - `docs: update installation instructions`

3. **Body (Optional):**
   - Use the body to explain what changes you made and why you made them. Wrap the body at 72 characters per line.

4. **Footer (Optional):**
   - Use the footer to reference any related issues or breaking changes. For example:
     ```
     BREAKING CHANGE: refactor authentication middleware
     Closes #123
     ```
## Pull Request Guidelines
If you modified something in `Assets/SmartifyOS` keep in mind that anything in there should not be (too) project (car/setup) specific.

1. **Title:**
   - Use a clear and descriptive title for the pull request that describes the change being made (also use [Conventional Commits](https://www.conventionalcommits.org/) here!).

2. **Description:**
   - Provide a detailed description of the changes in the pull request.

3. **Referencing Issues:**
   - If the pull request addresses an open issue, link to the issue by using the format `Closes #issue_number`.


## Additional Tips

- Follow the project's coding style and conventions.
- Make sure to pull the latest changes from the main branch before submitting your pull request.
- Don't make multiply different changes that aren't related in one pull request