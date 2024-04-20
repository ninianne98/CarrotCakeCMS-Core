CREATE TABLE [dbo].[carrot_Widget] (
    [Root_WidgetID]   UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [Root_ContentID]  UNIQUEIDENTIFIER NOT NULL,
    [WidgetOrder]     INT              NOT NULL,
    [PlaceholderName] NVARCHAR (256)   NOT NULL,
    [ControlPath]     NVARCHAR (1024)  NOT NULL,
    [WidgetActive]    BIT              NOT NULL,
    [GoLiveDate]      DATETIME         NOT NULL,
    [RetireDate]      DATETIME         NOT NULL,
    CONSTRAINT [PK_carrot_Widget] PRIMARY KEY CLUSTERED ([Root_WidgetID] ASC),
    CONSTRAINT [carrot_RootContent_carrot_Widget_FK] FOREIGN KEY ([Root_ContentID]) REFERENCES [dbo].[carrot_RootContent] ([Root_ContentID])
);


GO
CREATE NONCLUSTERED INDEX [IX_carrot_Widget_Root_ContentID]
    ON [dbo].[carrot_Widget]([Root_ContentID] ASC);

