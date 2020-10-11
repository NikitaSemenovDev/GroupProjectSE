CREATE DATABASE GroupProject;

GO

USE GroupProject;

CREATE TABLE [User]
(
	Id INT PRIMARY KEY IDENTITY,
	FirstName NVARCHAR(100) NOT NULL,
	Surname NVARCHAR(100) NOT NULL,
	Patronym NVARCHAR(100) NULL,
	Username NVARCHAR(100) UNIQUE NOT NULL,
	[Password] NVARCHAR(100) NOT NULL
);

INSERT INTO [User] (FirstName, Surname, Patronym, Username, [Password])
VALUES ('Test', 'Test', 'Test', 'Test', 'Test');
