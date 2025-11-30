TaskManagerAPI
A clean and secure Task Management REST API built with ASP.NET Core Web API, Entity Framework Core, and SQL Server.
This project provides a complete backend system with authentication, authorization, user management, and advanced task features.

Features:
🔐 Authentication & Authorization

User registration & login

Secure password hashing (with salt)

JWT authentication

Role-based authorization (Admin / User)

Custom Admin-only access attribute

👤 User Management

Register new users

Login and receive a JWT token

Admin can view, manage, and control user accounts

Rich user profile fields (email, phone, DOB, nationality, etc.)

📝 Task Management

Each user can create and manage their own tasks with:

Title & Description

Priority (Enum)

Status (Enum)

Due date

CreatedAt / UpdatedAt (auto-managed)

Linked user relationship (1-to-many)

💡 API Design Highlights

Uses DTOs for clean responses

Hides nullable fields

Global error handling

EF Core Code-First Migrations

LINQ-based filtering & operations

🛠️ Tech Stack

Backend:

C# — ASP.NET Core Web API

Entity Framework Core

SQL Server

Security:

JWT Authentication

Role-based access

Password hashing + salting
