# TODO

### 🔐 Two-Step Verification (2FA) for User Accounts

- [ ] Implement email-based 2FA for user accounts.
- [ ] Send verification code via email when 2FA is enabled.
- [ ] Return a **boolean** indicating success of email send.
- [ ] Extend login mutation to accept `loginCode` (the verification code).
- [ ] Validate user-submitted 2FA code before completing login.

### 📄 Automatic Expiration of Documents

- [ ] Set up a background job or scheduler to detect expired documents.
- [ ] Invalidate and restrict access to expired documents.
- [ ] Exclude expired documents from database queries and API responses.

### 🔄 Automate Entity Updates

- [ ] Use database triggers or background processes to handle automatic updates on specific entity changes.

### 🧪 Unit Testing

- [ ] Write unit tests for 2FA functionality.
- [ ] Add tests for document expiration logic.
- [ ] Include tests for automated entity updates.
- [ ] Set up test coverage tracking.

### 📚 Documentation

- [ ] Document 2FA setup, flow, and integration details.
- [ ] Add instructions for managing document expiration.
- [ ] Explain database automation and triggers used.
- [ ] Provide setup and usage guidelines for contributors.

Make code cleaner. Some filds from another classes can be assigned to input classes such as email field from User class to SignUpInput class.

Optimize queries

Fix removing sessions from users
