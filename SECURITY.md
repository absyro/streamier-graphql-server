# Security Policy

## Reporting a Vulnerability

We take the security of our GraphQL server seriously. If you believe you've found a security vulnerability, please follow these steps:

1. **DO NOT** disclose the vulnerability publicly until it has been addressed by our team.
2. Submit a detailed report to security@streamier.net
3. Include the following information in your report:
   - Description of the vulnerability
   - Steps to reproduce
   - Potential impact
   - Suggested fixes (if any)

## Security Measures

Our GraphQL server implements several security measures:

- Password hashing using BCrypt
- Rate limiting to prevent brute force attacks
- CORS protection
- Session management
- OTP (One-Time Password) support
- Input validation and sanitization
- Secure headers
- Regular security audits

## Response Time

We aim to:

- Acknowledge receipt of your vulnerability report within 48 hours
- Provide a more detailed response within 7 days
- Keep you informed about our progress in addressing the issue

## Security Updates

Security updates are released as patches to the current version. We recommend always running the latest version of the server.

## Responsible Disclosure

We follow responsible disclosure practices. If you report a vulnerability and follow our guidelines, we will:

- Not take legal action against you
- Work with you to understand and validate the issue
- Publicly acknowledge your responsible disclosure (if you wish)
- Fix the issue in a timely manner
