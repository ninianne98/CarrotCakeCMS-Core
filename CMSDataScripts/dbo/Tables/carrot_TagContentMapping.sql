CREATE TABLE [dbo].[carrot_TagContentMapping] (
    [TagContentMappingID] UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [ContentTagID]        UNIQUEIDENTIFIER NOT NULL,
    [Root_ContentID]      UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_carrot_TagContentMapping] PRIMARY KEY NONCLUSTERED ([TagContentMappingID] ASC),
    CONSTRAINT [FK_carrot_TagContentMapping_ContentTagID] FOREIGN KEY ([ContentTagID]) REFERENCES [dbo].[carrot_ContentTag] ([ContentTagID]),
    CONSTRAINT [FK_carrot_TagContentMapping_Root_ContentID] FOREIGN KEY ([Root_ContentID]) REFERENCES [dbo].[carrot_RootContent] ([Root_ContentID])
);


GO
CREATE NONCLUSTERED INDEX [IX_carrot_TagContentMapping_Root_ContentID]
    ON [dbo].[carrot_TagContentMapping]([Root_ContentID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_carrot_TagContentMapping_ContentTagID]
    ON [dbo].[carrot_TagContentMapping]([ContentTagID] ASC);

