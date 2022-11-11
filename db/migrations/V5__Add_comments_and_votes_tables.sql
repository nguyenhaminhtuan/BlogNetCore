CREATE TABLE [Comments] (
    [Id] int NOT NULL IDENTITY,
    [Body] nvarchar(max) NOT NULL,
    [IsDeleted] bit NOT NULL,
    [CommentedAt] datetime2 NOT NULL,
    [OwnerId] int NOT NULL,
    [ArticleId] int NOT NULL,
    [ReplyFromId] int NULL,
    [ReplyToId] int NULL,
    CONSTRAINT [PK_Comments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Comments_Users_OwnerId] FOREIGN KEY ([OwnerId]) REFERENCES [Users] ([Id]),
    CONSTRAINT [FK_Comments_Articles_ArticleId] FOREIGN KEY ([ArticleId]) REFERENCES [Articles] ([Id]),
    CONSTRAINT [FK_Comments_Comments_ReplyFromId] FOREIGN KEY ([ReplyFromId]) REFERENCES [Comments] ([Id]),
    CONSTRAINT [FK_Comments_Users_ReplyToId] FOREIGN KEY ([ReplyToId]) REFERENCES [Users] ([Id])
)
GO

CREATE TABLE [Votes] (
    [Id] int NOT NULL IDENTITY,
    [IsPositive] bit NOT NULL,
    [VotedAt] datetime2 NOT NULL,
    [OwnerId] int NOT NULL,
    [ArticleId] int NULL,
    [CommentId] int NULL,
    CONSTRAINT [PK_Votes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Votes_Users_OwnerId] FOREIGN KEY ([OwnerId]) REFERENCES [Users] ([Id]),
    CONSTRAINT [FK_Votes_Articles_ArticleId] FOREIGN KEY ([ArticleId]) REFERENCES [Articles] ([Id]),
    CONSTRAINT [FK_Votes_Comments_CommentId] FOREIGN KEY ([CommentId]) REFERENCES [Comments] ([Id])
)
GO

CREATE INDEX [IX_Comments_OwnerId] ON [Comments] ([OwnerId])
GO

CREATE INDEX [IX_Comments_ArticleId] ON [Comments] ([ArticleId])
GO

CREATE INDEX [IX_Comments_ReplyFromId] ON [Comments] ([ReplyFromId])
GO

CREATE INDEX [IX_Comments_ReplyToId] ON [Comments] ([ReplyToId])
GO

CREATE INDEX [IX_Votes_OwnerId] ON [Votes] ([OwnerId])
GO

CREATE INDEX [IX_Votes_ArticleId] ON [Votes] ([ArticleId])
GO

CREATE INDEX [IX_Votes_CommentId] ON [Votes] ([CommentId])
GO