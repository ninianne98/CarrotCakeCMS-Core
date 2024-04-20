CREATE TABLE [dbo].[carrot_RootContentSnippet] (
    [Root_ContentSnippetID] UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [SiteID]                UNIQUEIDENTIFIER NOT NULL,
    [ContentSnippetName]    NVARCHAR (256)   NOT NULL,
    [ContentSnippetSlug]    NVARCHAR (128)   NOT NULL,
    [CreateUserId]          UNIQUEIDENTIFIER NOT NULL,
    [CreateDate]            DATETIME         NOT NULL,
    [GoLiveDate]            DATETIME         NOT NULL,
    [RetireDate]            DATETIME         NOT NULL,
    [ContentSnippetActive]  BIT              NOT NULL,
    [Heartbeat_UserId]      UNIQUEIDENTIFIER NULL,
    [EditHeartbeat]         DATETIME         NULL,
    CONSTRAINT [PK_carrot_RootContentSnippet] PRIMARY KEY CLUSTERED ([Root_ContentSnippetID] ASC),
    CONSTRAINT [FK_carrot_RootContentSnippet_SiteID] FOREIGN KEY ([SiteID]) REFERENCES [dbo].[carrot_Sites] ([SiteID])
);


GO
CREATE NONCLUSTERED INDEX [IX_carrot_RootContentSnippet_SiteID]
    ON [dbo].[carrot_RootContentSnippet]([SiteID] ASC);

