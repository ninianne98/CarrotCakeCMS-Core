CREATE TABLE [dbo].[carrot_WidgetData] (
    [WidgetDataID]      UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [Root_WidgetID]     UNIQUEIDENTIFIER NOT NULL,
    [IsLatestVersion]   BIT              NOT NULL,
    [EditDate]          DATETIME         DEFAULT (getdate()) NOT NULL,
    [ControlProperties] NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_carrot_WidgetData] PRIMARY KEY CLUSTERED ([WidgetDataID] ASC),
    CONSTRAINT [carrot_WidgetData_Root_WidgetID_FK] FOREIGN KEY ([Root_WidgetID]) REFERENCES [dbo].[carrot_Widget] ([Root_WidgetID])
);


GO
CREATE NONCLUSTERED INDEX [IX_carrot_WidgetData_Root_WidgetID]
    ON [dbo].[carrot_WidgetData]([Root_WidgetID] ASC);

