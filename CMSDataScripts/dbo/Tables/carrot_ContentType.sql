CREATE TABLE [dbo].[carrot_ContentType] (
    [ContentTypeID]    UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [ContentTypeValue] NVARCHAR (256)   NOT NULL,
    CONSTRAINT [carrot_ContentType_PK] PRIMARY KEY CLUSTERED ([ContentTypeID] ASC)
);

