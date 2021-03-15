USE GroupProject

GO

TRUNCATE TABLE ImageProcessingResult;

ALTER TABLE ImageProcessingResult
ALTER COLUMN RegionsPredictions NVARCHAR(MAX) NOT NULL;

ALTER TABLE ImageProcessingResult
ALTER COLUMN DiseasesNames NVARCHAR(1000) NOT NULL;

ALTER TABLE ImageProcessingResult
ALTER COLUMN ImagePredictions NVARCHAR(MAX) NOT NULL;