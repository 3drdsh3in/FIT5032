-- Manually written DDL Script for the code first approach

CREATE TABLE Student (
	[Id] int IDENTITY(1,1) NOT NULL,
    [FirstName] nvarchar(max)  NOT NULL,
    [LastName] nvarchar(max)  NOT NULL,
    PRIMARY KEY (Id),

);
GO

CREATE TABLE [dbo].[Units] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [StudentId] int  NOT NULL,
    PRIMARY KEY (Id),
    FOREIGN KEY (StudentId) REFERENCES Student(Id) -- FK Pointing to the student id in student table
);
GO