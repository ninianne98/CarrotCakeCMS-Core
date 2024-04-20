CREATE TABLE [dbo].[carrot_ContentSnippet] (
    [ContentSnippetID]      UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [Root_ContentSnippetID] UNIQUEIDENTIFIER NOT NULL,
    [IsLatestVersion]       BIT              NOT NULL,
    [EditUserId]            UNIQUEIDENTIFIER NOT NULL,
    [EditDate]              DATETIME         NOT NULL,
    [ContentBody]           NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_carrot_ContentSnippet] PRIMARY KEY CLUSTERED ([ContentSnippetID] ASC),
    CONSTRAINT [FK_carrot_ContentSnippet_Root_ContentSnippetID] FOREIGN KEY ([Root_ContentSnippetID]) REFERENCES [dbo].[carrot_RootContentSnippet] ([Root_ContentSnippetID])
);


GO
CREATE NONCLUSTERED INDEX [IX_carrot_ContentSnippet_Root_ContentSnippetID]
    ON [dbo].[carrot_ContentSnippet]([Root_ContentSnippetID] ASC);

