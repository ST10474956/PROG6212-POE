/* RaceDay database creation script
   Based on the Users/Roles, Events, Categories, Enrolments and Results
   schema included in the supplied Part 1 PDF and implemented by Part 2 API.
   Register real accounts through the RaceDay application; do not use sample
   password hashes copied from planning documents.
*/
IF DB_ID(N'RaceDayDB') IS NULL
    CREATE DATABASE RaceDayDB;
GO
USE RaceDayDB;
GO

IF OBJECT_ID(N'dbo.Roles', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Roles (
        RoleID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Roles PRIMARY KEY,
        RoleName VARCHAR(20) NOT NULL CONSTRAINT UQ_Roles_RoleName UNIQUE
    );
END;
GO
IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users (
        UserID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Users PRIMARY KEY,
        FullName VARCHAR(100) NOT NULL,
        Email VARCHAR(150) NOT NULL CONSTRAINT UQ_Users_Email UNIQUE,
        PasswordHash VARCHAR(255) NOT NULL,
        RoleID INT NOT NULL,
        ProfilePictureURL VARCHAR(500) NULL,
        CreatedAt DATETIME NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT GETDATE(),
        CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleID) REFERENCES dbo.Roles(RoleID)
    );
END;
GO
IF OBJECT_ID(N'dbo.Events', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Events (
        EventID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Events PRIMARY KEY,
        Name VARCHAR(150) NOT NULL,
        Description VARCHAR(500) NULL,
        EventDate DATETIME NOT NULL,
        Location VARCHAR(150) NOT NULL,
        DistanceKm DECIMAL(5,2) NOT NULL,
        EventType VARCHAR(10) NOT NULL,
        BannerImageURL VARCHAR(500) NULL,
        OrganiserID INT NOT NULL,
        CreatedAt DATETIME NOT NULL CONSTRAINT DF_Events_CreatedAt DEFAULT GETDATE(),
        CONSTRAINT FK_Events_Users FOREIGN KEY (OrganiserID) REFERENCES dbo.Users(UserID),
        CONSTRAINT CK_Events_EventType CHECK (EventType IN ('Run','Walk','Cycle')),
        CONSTRAINT CK_Events_Distance CHECK (DistanceKm > 0)
    );
END;
GO
IF OBJECT_ID(N'dbo.Categories', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Categories (
        CategoryID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Categories PRIMARY KEY,
        EventID INT NOT NULL,
        Name VARCHAR(100) NOT NULL,
        DistanceKm DECIMAL(5,2) NULL,
        MaxParticipants INT NOT NULL CONSTRAINT DF_Categories_MaxParticipants DEFAULT 100,
        CONSTRAINT FK_Categories_Events FOREIGN KEY (EventID) REFERENCES dbo.Events(EventID),
        CONSTRAINT CK_Categories_MaxParticipants CHECK (MaxParticipants > 0)
    );
END;
GO
IF OBJECT_ID(N'dbo.Enrolments', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Enrolments (
        EnrolmentID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Enrolments PRIMARY KEY,
        UserID INT NOT NULL,
        CategoryID INT NOT NULL,
        EnrolmentDate DATETIME NOT NULL CONSTRAINT DF_Enrolments_Date DEFAULT GETDATE(),
        Status VARCHAR(20) NOT NULL CONSTRAINT DF_Enrolments_Status DEFAULT 'Confirmed',
        CONSTRAINT FK_Enrolments_Users FOREIGN KEY (UserID) REFERENCES dbo.Users(UserID),
        CONSTRAINT FK_Enrolments_Categories FOREIGN KEY (CategoryID) REFERENCES dbo.Categories(CategoryID),
        CONSTRAINT UQ_Enrolments_User_Category UNIQUE (UserID, CategoryID)
    );
END;
GO
IF OBJECT_ID(N'dbo.Results', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Results (
        ResultID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Results PRIMARY KEY,
        EnrolmentID INT NOT NULL CONSTRAINT UQ_Results_Enrolment UNIQUE,
        FinishTime TIME NULL,
        Position INT NULL,
        Status VARCHAR(20) NOT NULL CONSTRAINT DF_Results_Status DEFAULT 'Finished',
        CONSTRAINT FK_Results_Enrolments FOREIGN KEY (EnrolmentID) REFERENCES dbo.Enrolments(EnrolmentID),
        CONSTRAINT CK_Results_Position CHECK (Position IS NULL OR Position > 0)
    );
END;
GO
IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE RoleName='Organiser') INSERT dbo.Roles(RoleName) VALUES ('Organiser');
IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE RoleName='Participant') INSERT dbo.Roles(RoleName) VALUES ('Participant');
GO
PRINT 'RaceDayDB schema is ready. Register Organiser and Participant accounts through the RaceDay website.';
GO
