CREATE TABLE [Tags] (
    [Id] int NOT NULL IDENTITY,
    [Slug] nvarchar(100) NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Tags] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY,
    [Username] nvarchar(640) NOT NULL,
    [Password] nvarchar(300) NOT NULL,
    [ProfileName] nvarchar(100) NOT NULL,
    [DisplayName] nvarchar(200) NOT NULL,
    [Status] tinyint NOT NULL,
    [Role] tinyint NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastChanged] datetime2 NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Articles] (
    [Id] int NOT NULL IDENTITY,
    [Slug] nvarchar(200) NOT NULL,
    [Title] nvarchar(300) NOT NULL,
    [Content] nvarchar(max) NOT NULL,
    [Status] tinyint NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastModifiedAt] datetime2 NOT NULL,
    [PublishedAt] datetime2 NULL,
    [AuthorId] int NOT NULL,
    CONSTRAINT [PK_Articles] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Articles_Users_AuthorId] FOREIGN KEY ([AuthorId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [ArticleTags] (
    [ArticleId] int NOT NULL,
    [TagId] int NOT NULL,
    CONSTRAINT [PK_ArticleTags] PRIMARY KEY ([ArticleId], [TagId]),
    CONSTRAINT [FK_ArticleTags_Articles_ArticleId] FOREIGN KEY ([ArticleId]) REFERENCES [Articles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ArticleTags_Tags_TagId] FOREIGN KEY ([TagId]) REFERENCES [Tags] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_Articles_AuthorId] ON [Articles] ([AuthorId]);
GO

CREATE UNIQUE INDEX [IX_Articles_Slug] ON [Articles] ([Slug]);
GO

CREATE INDEX [IX_ArticleTags_TagId] ON [ArticleTags] ([TagId]);
GO

CREATE INDEX [IX_ArticleTags_ArticleId] ON [ArticleTags] ([ArticleId]);
GO

CREATE UNIQUE INDEX [IX_Tags_Slug] ON [Tags] ([Slug]);
GO

CREATE UNIQUE INDEX [IX_Users_ProfileName] ON [Users] ([ProfileName]);
GO

CREATE UNIQUE INDEX [IX_Users_Username] ON [Users] ([Username]);
GO