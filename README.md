# Advanced Authentication Module

A comprehensive .NET 6 authentication and authorization system with advanced features including JWT token management, email verification with OTP, password reset functionality, and role-based access control.

## 🚀 Features

- **User Registration & Login** - Secure user authentication with JWT tokens
- **JWT Token Management** - Access tokens and refresh tokens with automatic renewal
- **Email Verification** - OTP-based email verification system
- **Password Management** - Change password, forget password, and reset password flows
- **Role-Based Authorization** - Multi-role support with user-role management
- **Profile Management** - Update user profile information
- **Session Management** - Logout functionality with token revocation
- **Database Migrations** - Entity Framework Core migrations for database schema management
- **Email Service** - Integrated email service for OTP and notifications

## 🏗️ Project Structure

The solution follows a clean architecture pattern with four main projects:

```
AdvancAuth/
├── AUTH.API/           # Web API layer - Controllers and API endpoints
├── CORE/               # Domain layer - Entities, DTOs, and Interfaces
├── SERVICES/           # Business logic layer - Service implementations
└── REPOSITORY/         # Data access layer - EF Core, repositories, and configurations
```

### Project Details

#### AUTH.API
- ASP.NET Core Web API
- RESTful endpoints for authentication operations
- Swagger/OpenAPI documentation
- Request/Response DTOs
- Service registration and dependency injection

#### CORE
- Domain entities (User, Role, RefreshToken, OTP, etc.)
- Data Transfer Objects (DTOs)
- Service interfaces
- AutoMapper mapping profiles

#### SERVICES
- Authentication service implementation
- Token service (JWT generation and validation)
- Email service implementation
- Email verification service with OTP

#### REPOSITORY
- Entity Framework Core DbContext
- Generic repository pattern
- Unit of Work pattern
- Entity configurations
- Database migrations
- Data seeding

## 📋 Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- SQL Server (LocalDB, Express, or Full Edition)
- SMTP server credentials for email functionality

## 🔧 Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/KamalElsayedJR/AdvancedAuthModule.git
   cd AdvancAuth
   ```

2. **Configure Database Connection**
   
   Update the connection string in `appsettings.json` in the AUTH.API project:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=.;Database=AdvancedAuthDB;Trusted_Connection=True;"
     }
   }
   ```

3. **Configure Email Settings**
   
   Add your SMTP configuration in `appsettings.json`:
   ```json
   {
     "MailSettings": {
       "Mail": "your-email@example.com",
       "DisplayName": "Your App Name",
       "Password": "your-email-password",
       "Host": "smtp.gmail.com",
       "Port": 587
     }
   }
   ```

4. **Run Database Migrations**
   ```bash
   cd AUTH.API
   dotnet ef database update
   ```

5. **Run the Application**
   ```bash
   dotnet run
   ```

The API will be available at `https://localhost:5001` (or the port specified in launchSettings.json).

## 📚 API Endpoints

### Authentication

- **POST** `/Auth/Register` - Register a new user
- **POST** `/Auth/Login` - Login with email and password
- **POST** `/Auth/RefreshToken` - Refresh access token using refresh token
- **POST** `/Auth/LogOut` - Logout and revoke refresh token

### Email Verification

- **POST** `/Auth/SendOTP` - Send OTP to email for verification
- **POST** `/Auth/VerifiyEmail` - Verify email with OTP
- **POST** `/Auth/OtpVerify` - Verify OTP code

### Password Management

- **POST** `/Auth/ForgetPassword` - Initiate password reset flow
- **POST** `/Auth/ResetPassword` - Reset password with OTP
- **POST** `/Auth/ChangePassword` - Change password (requires authentication)

### Profile Management

- **GET** `/Auth/Me` - Get current user profile (requires authentication)
- **PUT** `/Auth/Me` - Update user profile (requires authentication)
- **GET** `/Auth/gettoken` - Get current authentication token

## 🔐 Authentication Flow

1. **Registration**
   - User registers with email, password, and profile information
   - System generates OTP and sends to email
   - User verifies email with OTP

2. **Login**
   - User logs in with email and password
   - System returns JWT access token and refresh token
   - Access token includes user claims (email, name, roles, etc.)

3. **Token Refresh**
   - When access token expires, use refresh token to get new access token
   - Refresh token is rotated on each use for security

4. **Password Reset**
   - User requests password reset with email
   - System sends OTP to email
   - User verifies OTP and sets new password

## 🛡️ Security Features

- **JWT Authentication** - Secure token-based authentication
- **Password Hashing** - Passwords are hashed using industry-standard algorithms
- **Refresh Token Rotation** - Refresh tokens are rotated to prevent replay attacks
- **OTP Verification** - Time-limited one-time passwords for email verification
- **Role-Based Authorization** - Fine-grained access control based on user roles
- **Token Expiration** - Access tokens have limited lifetime for security

## 🗄️ Database Schema

Key entities:
- **User** - User accounts with email, password, and profile information
- **Roles** - User roles for authorization
- **UserRoles** - Many-to-many relationship between users and roles
- **RefreshToken** - JWT refresh tokens with expiration tracking
- **OTP** - One-time passwords for email verification

## 🧪 Testing

Access the Swagger UI for interactive API testing:
```
https://localhost:5001/swagger
```

## 📦 NuGet Packages

- Microsoft.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Tools
- Microsoft.AspNetCore.Authentication.JwtBearer
- AutoMapper
- Swashbuckle.AspNetCore

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.


## 👨‍💻 Author

**Kamal Elsayed**
- GitHub: [@KamalElsayedJR](https://github.com/KamalElsayedJR)

## 🙏 Acknowledgments

- Built with ASP.NET Core 6
- Follows clean architecture principles
- Implements repository and unit of work patterns
