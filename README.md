# HCM App

A web-based Human Capital Management (HCM) system built with ASP.NET Core MVC. The system provides:

- 🔐 Authentication and authorization via `AuthAPIHCM`
- 🗂️ CRUD operations for managing employees via `CRUDHCM_API`
- 👤 A user-facing front-end with role-specific views (`Employee`, `Manager`, `HrAdmin`)
- ✅ Input sanitization and XSS protection using Ganss.Xss
- 🧪 Unit and integration tests

---

## 📂 Project Structure

/HCM_app - ASP.NET Core MVC front-end (UI + controller logic)

/AuthAPIHCM - Authentication microservice (JWT-based login/register)

/CRUDHCM_API - CRUD microservice (users, updates, roles)

/SharedModels - Shared DTOs/ViewModels between services

/Tests - Integration and unit tests for APIs

README.md - This file

## 🚀 Getting Started

### Prerequisites

- [.NET 9](https://dotnet.microsoft.com/en-us/download)
- Visual Studio or VS Code
- SQL Server

### For first time usage - 
- #### You need to have a database with the name - HCMdb!
- #### Type Update-Database in Package Manager Console!
- Locate Constants.cs in SharedModels
- Change usersPerPage depending on how many users you want to view per page (default is 2, users are ordered alphabetically of their first names)
- You can use the current secret key if you only want to test the project (it's hardcoded everywhere)
- If you want to change the secret key then you would need to change it in every Program.cs file and in AuthService.cs
- If you want to run the integration tests, make sure the CRUDAPI is running
#### Seeded-ready accounts for testing (all emails are just for testing):
Email: admin@email.com
Password: 123123
Role: HrAdmin

Email: manager@email.com
Password: 123123
Role: Manager

Email: terry@email.com
Password: 123123
Role: Employee

#### There are more seeded accounts which you can see in HCM-app/Data/ApplicationDbContext.cs

## Features

#### 🔐 Authentication & Authorization

- Login / Register via AuthAPI HCM
  
- JWT token stored in session
  
- Role-based authorization (Employee, Manager, HrAdmin)

#### 🧑‍💼 User Management

- Admin and Manager dashboards with paging

- User role and salary updates

- User deletion, restricted from deleting self

- Secure password change (with hashing and validation)

#### 🛡️ Security

- All user inputs are sanitized with Ganss.Xss

- Anti-forgery tokens via [ValidateAntiForgeryToken]

- Passwords hashed with BCrypt.Net

- Security against MIME sniffing and clickjacking

- Security against IDOR through validation checks

- Security against SQL injections

## 🙋 Roles
#### Role - Permissions
Employee -	View profile only

Manager -	View & update users in their department

HrAdmin -	View, update, and delete users across all departments

## 📝 Author Notes

### This project was built using ASP.NET Core MVC with a focus on:

- Clean separation between services

- Testability via DI and layered architecture

- Frontend security and input validation
