# 🎟 Event Management System

A full-stack **Event Management** app for a seminar hall (Room 121), built with **Angular**, **ASP.NET Core**, and **PostgreSQL**.  
Includes **JWT auth**, role-based access (Admin/User), registration workflow, and **Admin PDF download** of an event’s details (with attendee list).

## 🚀 Tech Stack
- **Frontend:** Angular, TypeScript
- **Backend:** ASP.NET Core Web API (.NET 7/8)
- **Database:** PostgreSQL
- **Auth:** JWT
- **Password Hashing:** BCrypt

## 🔑 Features
### User
- Register/Login
- View **today & upcoming** events
- Register for events
- See **My Registrations**

### Admin
- CRUD events (with validation: date in future, start < end, time-conflict checks)
- View all users
- View event details & registrations
- **Download event details as PDF** (event info + registered users)

## 🗄 Database
- `Users(Id, Email, PasswordHash, Name, Role, Sem, Branch, EnrollmentNumber, CreatedAt)`
- `Events(Id, SrNo, EventName, EventDate, StartTime, EndTime, TotalSeats, AvailableSeats, CreatedBy, CreatedAt)`
- `EventRegistrations(Id, EventId, UserId, RegisteredAt)`

## 🛠 Local Setup

### 1) Clone
```bash
git clone https://github.com/<your-username>/EventManagementSystem.git
cd EventManagementSystem
