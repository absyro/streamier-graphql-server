## TODO

1. **Redis Caching for Frequently Accessed Data**

Implement Redis caching to store and quickly retrieve frequently accessed data, improving performance and reducing database load.

2. **Two-Step Verification for Accounts**

- Implement two-step verification (2FA) for accounts that have enabled this feature.
- The 2FA process will be conducted via email.
  - When a user requests to create a session, the response type should be a **union GraphQL** type.
  - If 2FA is enabled on the account, send a verification code to the user's email and return a **boolean** value indicating the success of this step.
- After the user receives the code, they will need to insert the code in the request.
- The same mutation should allow submitting a **login code** as a parameter to complete the login process.
- This implementation can be integrated with **Temporary Codes** for added flexibility.

3. **Expiration**

Documents should be invalidated once their expiration date has passed. (automated)

4. **Updating Entities**

- To update entities, use database triggers. (automated)
- All entities should have a "updated_at" column. (automated)

5. **Created At Column**

All entities should have a "created_at" column. (automated)
