# MyFirstApi - Pro Secure Todo Application

A modern, full-stack todo management application built with **ASP.NET Core 9.0** backend and a **beautiful responsive frontend** with authentication, real-time updates, and dark mode support.

---

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Installation & Setup](#installation--setup)
- [Configuration](#configuration)
- [API Endpoints](#api-endpoints)
- [Frontend Features](#frontend-features)
- [Database](#database)
- [Security](#security)
- [Usage Guide](#usage-guide)

---

## 🎯 Overview

**MyFirstApi** is a secure, feature-rich todo application that demonstrates modern web development practices. It combines a robust ASP.NET Core backend with an elegant frontend, featuring JWT authentication, SQLite database, and a stunning UI with dark mode support.

The application allows users to:
- Register and authenticate securely
- Create, read, update, and delete todos
- Toggle task completion status
- Search and filter tasks
- Enjoy a beautiful dark/light theme UI

---

## ✨ Features

### Backend Features
- **JWT Authentication**: Secure token-based authentication with 1-day expiration
- **User Registration & Login**: BCrypt password hashing for security
- **RESTful API**: Clean, well-organized API endpoints
- **Entity Framework Core**: ORM for database operations with SQLite
- **CORS Support**: Configured to allow frontend communication
- **Swagger/OpenAPI**: Interactive API documentation
- **Authorization Middleware**: Protected endpoints requiring valid JWT

### Frontend Features
- **Modern UI/UX**: Glass-morphism design with animated gradients
- **Dark Mode**: Toggle between light and dark themes with localStorage persistence
- **Responsive Design**: Works seamlessly on desktop, tablet, and mobile devices
- **Real-time Task Management**: Add, edit, delete, and complete tasks instantly
- **Search & Filter**: Find tasks quickly with built-in search functionality
- **XSS Protection**: HTML escaping to prevent security vulnerabilities
- **Animated Backgrounds**: Floating orbs and gradient animations
- **Form Validation**: Client-side validation for auth and task creation

---

## 🛠 Technology Stack

### Backend
- **Framework**: ASP.NET Core 9.0
- **Language**: C#
- **Database**: SQLite
- **ORM**: Entity Framework Core 9.0.0
- **Authentication**: JWT Bearer Token
- **Password Hashing**: BCrypt.Net-Next 4.0.3
- **API Documentation**: Swashbuckle.AspNetCore 7.2.0

### Frontend
- **HTML5**: Semantic markup
- **CSS3**: Advanced styling with CSS Grid, Flexbox, animations, and gradients
- **Vanilla JavaScript**: No framework dependencies (pure ES6+)
- **APIs**: Fetch API for HTTP requests
- **Storage**: LocalStorage for token and theme persistence

---

## 📁 Project Structure

```
MyFirstApi/
├── Controllers/
│   ├── AuthController.cs       # User authentication (register, login)
│   └── TodosController.cs      # Todo CRUD operations
├── Program.cs                  # Application startup configuration
├── MyFirstApi.csproj           # Project file with NuGet dependencies
├── MyFirstApi.sln              # Visual Studio solution file
├── MyFirstApi.http             # REST client test file (VS Code REST extension)
├── index.html                  # Frontend UI
├── app.js                       # Frontend JavaScript logic
├── styles.css                  # Frontend styling
├── appsettings.json            # Application configuration
├── appsettings.Development.json # Development-specific configuration
├── Migrations/                 # EF Core database migrations
├── bin/                        # Compiled binaries
├── obj/                        # Intermediate build objects
└── Properties/
    └── launchSettings.json     # Launch configuration
```

---

## 🚀 Installation & Setup

### Prerequisites
- **.NET 9.0 SDK** or later
- **Visual Studio Code** or **Visual Studio 2022** (recommended)
- **Git** (optional)

### Step 1: Clone/Navigate to Project
```bash
cd c:\projects\.net-dev\MyFirstApi
```

### Step 2: Restore NuGet Packages
```bash
dotnet restore
```

### Step 3: Create Database (if not exists)
```bash
dotnet ef database update
```

This will create a `todo.db` SQLite database with the initial schema.

### Step 4: Run the Application
```bash
dotnet run
```

The application will start on `http://localhost:5063`

### Step 5: Access the Application
Open your browser and navigate to:
```
http://localhost:5063
```

---

## ⚙️ Configuration

### appsettings.json

```json
{
  "Logging": { 
    "LogLevel": { 
      "Default": "Information", 
      "Microsoft.AspNetCore": "Warning" 
    } 
  },
  "AllowedHosts": "*",
  "AppSettings": {
    "Token": "THIS_IS_MY_SUPER_SECRET_KEY_THAT_IS_LONG_ENOUGH_FOR_HMAC_SHA512_SECURITY_123456789"
  }
}
```

#### Key Configuration Details:
- **Token**: JWT signing key (must be at least 32 characters for HMAC SHA512)
- **Logging**: Set logging levels for different components
- **AllowedHosts**: Specifies which hosts can access the API (using "*" allows all)

### Database Connection
SQLite database is configured in `Program.cs`:
```csharp
builder.Services.AddDbContext<TodoDb>(opt => 
    opt.UseSqlite("Data Source=todo.db")
);
```

The `todo.db` file will be created in the project root directory.

---

## 🔌 API Endpoints

All API endpoints are prefixed with `/api/` and are accessible at `http://localhost:5063/api/`

### Authentication Endpoints

#### POST `/auth/register`
Register a new user account.

**Request Body:**
```json
{
  "username": "john_doe",
  "password": "securePassword123"
}
```

**Response (201 Created):**
```json
{
  "message": "Registered"
}
```

**Errors:**
- `400 Bad Request`: User already exists

---

#### POST `/auth/login`
Authenticate and receive JWT token.

**Request Body:**
```json
{
  "username": "john_doe",
  "password": "securePassword123"
}
```

**Response (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9..."
}
```

**Errors:**
- `400 Bad Request`: Invalid username or password

---

### Todo Endpoints
**⚠️ All endpoints require Authorization header with valid JWT token**

#### GET `/todos`
Retrieve all todos for the authenticated user.

**Headers:**
```
Authorization: Bearer <JWT_TOKEN>
```

**Response (200 OK):**
```json
[
  {
    "id": 1,
    "name": "Buy groceries",
    "isComplete": false,
    "userId": 1
  },
  {
    "id": 2,
    "name": "Complete project",
    "isComplete": true,
    "userId": 1
  }
]
```

---

#### POST `/todos`
Create a new todo task.

**Headers:**
```
Authorization: Bearer <JWT_TOKEN>
```

**Query Parameters:**
- `taskName` (required): Name of the task

**Example:**
```
POST /api/todos?taskName=Buy%20groceries
Authorization: Bearer <JWT_TOKEN>
```

**Response (200 OK):**
```json
{
  "id": 3,
  "name": "Buy groceries",
  "isComplete": false,
  "userId": 1
}
```

---

#### PUT `/todos/{id}/toggle`
Toggle the completion status of a todo.

**Headers:**
```
Authorization: Bearer <JWT_TOKEN>
```

**Path Parameters:**
- `id` (required): Todo ID

**Response (200 OK):**
```json
{
  "id": 1,
  "name": "Buy groceries",
  "isComplete": true,
  "userId": 1
}
```

**Errors:**
- `404 Not Found`: Todo doesn't exist or doesn't belong to user
- `401 Unauthorized`: Invalid or missing token

---

#### DELETE `/todos/{id}`
Delete a todo task.

**Headers:**
```
Authorization: Bearer <JWT_TOKEN>
```

**Path Parameters:**
- `id` (required): Todo ID

**Response (204 No Content)**

**Errors:**
- `404 Not Found`: Todo doesn't exist or doesn't belong to user
- `401 Unauthorized`: Invalid or missing token

---

## 🎨 Frontend Features

### Authentication System
- **Login Page**: Clean, modern login interface with validation
- **Register Link**: Toggle between login and registration modes
- **Error Messages**: User-friendly error notifications
- **Token Storage**: JWT token stored in browser's localStorage for persistence

### Main App Interface
- **Header**: Displays "My Tasks" with logout button
- **Search Bar**: Real-time task filtering as you type
- **Add Task Section**: Input field with "Add Task" button
- **Task List Grid**: Responsive card-based layout showing all tasks
- **Task Actions**: 
  - Checkbox to toggle completion status
  - Delete button with confirmation dialog
  - Visual strikethrough for completed tasks

### UI Components
- **Theme Toggle**: Moon/Sun button (top-right corner) for dark/light mode
- **Animated Background**: Floating orbs and gradient animations
- **Empty State**: Friendly message when no tasks exist
- **Responsive Design**: Adapts to all screen sizes

### Frontend Security
- **XSS Protection**: HTML escaping using `escapeHtml()` function
- **Token Management**: Automatic logout on 401 responses
- **Secure Headers**: Authorization headers for all API requests

---

## 💾 Database

### Schema

#### Users Table
```
Id (int) - Primary Key
Username (string) - Unique username
PasswordHash (string) - BCrypt hashed password
```

#### Todos Table
```
Id (int) - Primary Key
Name (string) - Task name/description
IsComplete (bool) - Completion status
UserId (int) - Foreign key to Users table
```

### Entity Framework Models

**User Model:**
```csharp
public class User 
{ 
    public int Id { get; set; } 
    public string Username { get; set; } = ""; 
    public string PasswordHash { get; set; } = ""; 
}
```

**Todo Model:**
```csharp
public class Todo 
{ 
    public int Id { get; set; } 
    public string? Name { get; set; } 
    public bool IsComplete { get; set; } 
    public int UserId { get; set; } 
}
```

**Database Context:**
```csharp
public class TodoDb : DbContext 
{
    public TodoDb(DbContextOptions<TodoDb> options) : base(options) { }
    public DbSet<Todo> Todos => Set<Todo>();
    public DbSet<User> Users => Set<User>();
}
```

### Database Migrations
Located in `Migrations/` folder:
- `20260107124656_InitialSetup.cs` - Initial schema creation

To create new migrations:
```bash
dotnet ef migrations add MigrationName
dotnet ef database update
```

---

## 🔐 Security

### Password Security
- **BCrypt Hashing**: Passwords are hashed using BCrypt.Net-Next with automatic salt generation
- **Never Stored in Plain Text**: Only password hashes are stored in database
- **Secure Verification**: `BCrypt.Verify()` safely compares passwords

### JWT Authentication
- **Token-Based**: Stateless authentication using JWT tokens
- **Expiration**: Tokens expire after 1 day (86400 seconds)
- **HMAC SHA512**: Tokens are signed with a 512-bit key
- **Claims**: User ID stored as NameIdentifier claim
- **Bearer Scheme**: Standard HTTP Bearer token authentication

### CORS Configuration
```csharp
app.UseCors(policy => policy
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());
```

**⚠️ Note**: Current configuration allows all origins. For production, restrict to specific domains:
```csharp
.WithOrigins("https://yourdomain.com")
```

### Authorization
- **[Authorize]**: Todos controller requires valid JWT token
- **User Isolation**: Users can only access their own todos via `GetUserId()` helper
- **XSS Prevention**: Frontend escapes HTML to prevent injection attacks

---

## 📖 Usage Guide

### First Time Setup

1. **Create Account**
   - Click "Need an account? Register"
   - Enter desired username and password
   - Click "Register"
   - Switch back to login and enter credentials

2. **Login**
   - Enter your username and password
   - Click "Login"
   - If successful, you'll be directed to the main app

### Managing Tasks

1. **Add Task**
   - Type task name in input field
   - Press Enter or click "Add Task"
   - Task appears in the task list

2. **Complete Task**
   - Check the checkbox next to the task
   - Task text will show strikethrough
   - Uncheck to mark as incomplete

3. **Delete Task**
   - Click the delete button (🗑️)
   - Confirm deletion when prompted
   - Task is removed from your list

4. **Search Tasks**
   - Type in the search bar
   - Tasks are filtered in real-time
   - Clear search to see all tasks

5. **Toggle Theme**
   - Click the moon (🌙) or sun (☀️) button in top-right
   - Theme preference is saved in browser

### Logout
- Click "Logout" in the header
- You'll be returned to the login screen

---

## 🧪 Testing API Endpoints

### Using REST Client in VS Code

The `MyFirstApi.http` file contains test requests. Install the "REST Client" extension and use it to test endpoints:

```
GET http://localhost:5063/api/todos
Authorization: Bearer <your_jwt_token>
```

### Using Curl

```bash
# Register
curl -X POST http://localhost:5063/api/auth/register \
  -H "Content-Type: application/json" \
  -d "{\"username\":\"test\",\"password\":\"password123\"}"

# Login
curl -X POST http://localhost:5063/api/auth/login \
  -H "Content-Type: application/json" \
  -d "{\"username\":\"test\",\"password\":\"password123\"}"

# Get Todos (replace TOKEN with actual JWT)
curl http://localhost:5063/api/todos \
  -H "Authorization: Bearer TOKEN"
```

---

## 📦 Dependencies

### NuGet Packages
- **BCrypt.Net-Next** (4.0.3): Password hashing library
- **Microsoft.AspNetCore.Authentication.JwtBearer** (9.0.0): JWT authentication
- **Microsoft.EntityFrameworkCore.Sqlite** (9.0.0): SQLite database provider
- **Microsoft.EntityFrameworkCore.Design** (9.0.0): EF Core tools
- **Swashbuckle.AspNetCore** (7.2.0): Swagger/OpenAPI integration

---

## 🚀 Deployment

### Build for Production
```bash
dotnet publish -c Release -o ./publish
```

### Environment Variables
Set these in production:
- `AppSettings:Token`: Use a secure, random 32+ character string
- `ASPNETCORE_ENVIRONMENT`: Set to "Production"

### Database
For production, consider migrating from SQLite to:
- **SQL Server**
- **PostgreSQL**
- **MySQL**

Update the connection string and DbContext configuration accordingly.

---

## 📝 License

This project is provided as-is for educational and development purposes.

---

## 💡 Notes & Future Enhancements

### Current Limitations
- SQLite database is file-based (not suitable for multi-instance deployments)
- CORS allows all origins (restrict in production)
- Token stored in localStorage (vulnerable to XSS)

### Suggested Improvements
- Add task categories and tags
- Implement task priority levels
- Add due dates and reminders
- Enable task sharing between users
- Add task comments/notes
- Implement refresh token rotation
- Add request rate limiting
- Create admin dashboard
- Add email notifications

---

**Happy Coding! 🚀**
