USE GroupProject;

GO

ALTER TABLE Person
ADD MedicalRecordNumber INT NULL;

ALTER TABLE Person
ADD WorkExperience INT NULL;

GO

CREATE TABLE DoctorPatient
(
	Id INT PRIMARY KEY IDENTITY,
	AccountId INT NULL,
	LinkedAccountId INT NULL,
	CONSTRAINT FK_Account_Account FOREIGN KEY (AccountId) REFERENCES Account (Id) ON DELETE SET NULL,
	CONSTRAINT FK_LinkedAccount_Account FOREIGN KEY (LinkedAccountId) REFERENCES Account (Id) ON DELETE NO ACTION
);