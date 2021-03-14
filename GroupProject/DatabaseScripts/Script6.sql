USE GroupProject

GO

ALTER TABLE ImageProcessingResult
DROP COLUMN ProcessingResult;

ALTER TABLE ImageProcessingResult
ADD Size NVARCHAR(50) NOT NULL;

ALTER TABLE ImageProcessingResult
ADD RegionsPredictions NVARCHAR(1000) NOT NULL;

ALTER TABLE ImageProcessingResult
ADD ImagePredictions NVARCHAR(1000) NOT NULL;

ALTER TABLE ImageProcessingResult
ADD DiseasesNames NVARCHAR(200) NOT NULL;