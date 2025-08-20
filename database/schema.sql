Database Schema

Tables Required


1. Users Table
CREATE TABLE Users (
    Id SERIAL PRIMARY KEY,
    Email VARCHAR(100) UNIQUE NOT NULL,
    Password VARCHAR(255) NOT NULL,
    Name VARCHAR(200) NOT NULL,
    Sem INTEGER NULL, -- Only for students (NULL for admin)
    Branch VARCHAR(100) NULL, -- Only for students
    EnrollmentNumber VARCHAR(50) UNIQUE NULL, -- Only for students
    Role VARCHAR(20) NOT NULL DEFAULT 'User', -- 'Admin' or 'User'
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

2. Events Table
CREATE TABLE Events (
    Id SERIAL PRIMARY KEY,
    SrNo INTEGER UNIQUE NOT NULL, -- Serial number for events
    EventName VARCHAR(200) NOT NULL,
    EventDate DATE NOT NULL,
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    TotalSeats INTEGER NOT NULL,
    AvailableSeats INTEGER NOT NULL,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CreatedBy INTEGER REFERENCES Users(Id)
);

3. EventRegistrations Table
CREATE TABLE EventRegistrations (
    Id SERIAL PRIMARY KEY,
    EventId INTEGER REFERENCES Events(Id),
    UserId INTEGER REFERENCES Users(Id),
    RegisteredAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(EventId, UserId)
);