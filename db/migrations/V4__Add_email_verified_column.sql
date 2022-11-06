ALTER TABLE [Users]
ADD [EmailVerified] bit NOT NULL DEFAULT 0
GO

UPDATE [Users]
SET [Users].[Status] = 0
GO