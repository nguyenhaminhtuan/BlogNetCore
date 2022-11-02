ALTER TABLE [Users]
ALTER COLUMN [Password] nvarchar(200) NOT NULL
GO

DROP INDEX [IX_Users_ProfileName] ON [Users]
GO
    
ALTER TABLE [Users]
ALTER COLUMN [ProfileName] nvarchar(60) NOT NULL
GO

CREATE UNIQUE INDEX [UQ_Users_ProfileName] ON [Users] ([ProfileName]);
GO

ALTER TABLE [Users]
ALTER COLUMN [DisplayName] nvarchar(200) NOT NULL
GO

ALTER TABLE [Articles]
ALTER COLUMN [Title] nvarchar(400) NOT NULL
GO
    
DROP INDEX [IX_Articles_Slug] on [Articles]
GO

ALTER TABLE [Articles]
ALTER COLUMN [Slug] nvarchar(400) NOT NULL
GO

CREATE UNIQUE INDEX [UQ_Articles_Slug] ON [Articles] ([Slug]);
GO

DROP INDEX [IX_Users_Username] ON [Users]
GO

CREATE UNIQUE INDEX [UQ_Users_Username] ON [Users] ([Username]);
GO