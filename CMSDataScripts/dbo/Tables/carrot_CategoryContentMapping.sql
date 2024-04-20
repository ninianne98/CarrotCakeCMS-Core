CREATE TABLE [dbo].[carrot_CategoryContentMapping] (
    [CategoryContentMappingID] UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [ContentCategoryID]        UNIQUEIDENTIFIER NOT NULL,
    [Root_ContentID]           UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_carrot_CategoryContentMapping] PRIMARY KEY NONCLUSTERED ([CategoryContentMappingID] ASC),
    CONSTRAINT [FK_carrot_CategoryContentMapping_ContentCategoryID] FOREIGN KEY ([ContentCategoryID]) REFERENCES [dbo].[carrot_ContentCategory] ([ContentCategoryID]),
    CONSTRAINT [FK_carrot_CategoryContentMapping_Root_ContentID] FOREIGN KEY ([Root_ContentID]) REFERENCES [dbo].[carrot_RootContent] ([Root_ContentID])
);


GO
CREATE NONCLUSTERED INDEX [IX_carrot_CategoryContentMapping_Root_ContentID]
    ON [dbo].[carrot_CategoryContentMapping]([Root_ContentID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_carrot_CategoryContentMapping_ContentCategoryID]
    ON [dbo].[carrot_CategoryContentMapping]([ContentCategoryID] ASC);

