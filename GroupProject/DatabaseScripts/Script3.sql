USE GroupProject;

GO

ALTER TABLE ImageProcessingResult
DROP CONSTRAINT FK_ImageProcessingResult_Person;

GO

ALTER TABLE ImageProcessingResult
DROP COLUMN PersonId;

GO

ALTER TABLE ImageProcessingResult
ADD AccountId INT NULL;

GO

ALTER TABLE ImageProcessingResult
ADD CONSTRAINT FK_ImageProcessingResult_Account FOREIGN KEY (AccountId) REFERENCES Account (Id) ON DELETE SET NULL;