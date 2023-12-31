IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[carrot_CalendarEvent]') AND type in (N'U')) BEGIN

	CREATE TABLE [dbo].[carrot_CalendarFrequency](
		[CalendarFrequencyID] [uniqueidentifier] NOT NULL,
		[FrequencySortOrder] [int] NOT NULL,	
		[FrequencyValue] [varchar](64) NOT NULL,
		[FrequencyName] [varchar](128) NOT NULL,
	 CONSTRAINT [PK_carrot_CalendarFrequency] PRIMARY KEY CLUSTERED 
	(
		[CalendarFrequencyID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]


	CREATE TABLE [dbo].[carrot_CalendarEventCategory](
		[CalendarEventCategoryID] [uniqueidentifier] NOT NULL,
		[CategoryFGColor] [varchar](32) NOT NULL,
		[CategoryBGColor] [varchar](32) NOT NULL,
		[CategoryName] [varchar](128) NOT NULL,
		[SiteID] [uniqueidentifier] NOT NULL,	
	 CONSTRAINT [PK_carrot_CalendarEventCategory] PRIMARY KEY CLUSTERED 
	(
		[CalendarEventCategoryID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]


	CREATE TABLE [dbo].[carrot_CalendarEventProfile](
		[CalendarEventProfileID] [uniqueidentifier] NOT NULL,
		[CalendarFrequencyID] [uniqueidentifier] NOT NULL,
		[CalendarEventCategoryID] [uniqueidentifier] NOT NULL,
		[EventStartDate] [datetime] NOT NULL,
		[EventStartTime] [time](7) NULL,
		[EventEndDate] [datetime] NOT NULL,
		[EventEndTime] [time](7) NULL,
		[EventTitle] [varchar](256) NULL,
		[EventDetail] [varchar](max) NULL,
		[EventRepeatPattern] [int] NULL,
		[IsAllDayEvent] [bit] NOT NULL,
		[IsPublic] [bit] NOT NULL,
		[IsCancelled] [bit] NOT NULL,
		[IsCancelledPublic] [bit] NOT NULL,
		[SiteID] [uniqueidentifier] NOT NULL,
	 CONSTRAINT [PK_carrot_CalendarEventProfile] PRIMARY KEY CLUSTERED 
	(
		[CalendarEventProfileID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]


	CREATE TABLE [dbo].[carrot_CalendarEvent](
		[CalendarEventID] [uniqueidentifier] NOT NULL,
		[CalendarEventProfileID] [uniqueidentifier] NOT NULL,
		[EventDate] [datetime] NOT NULL,
		[EventDetail] [varchar](max) NULL,
		[IsCancelled] [bit] NOT NULL,
	 CONSTRAINT [PK_carrot_CalendarEvent] PRIMARY KEY CLUSTERED 
	(
		[CalendarEventID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]

END

GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_carrot_CalendarEventProfile_carrot_CalendarEventCategory]') ) BEGIN

	ALTER TABLE [dbo].[carrot_CalendarFrequency] ADD  CONSTRAINT [DF_carrot_CalendarFrequency_CalendarFrequencyID]  DEFAULT (newid()) FOR [CalendarFrequencyID]

	ALTER TABLE [dbo].[carrot_CalendarEventCategory] ADD  CONSTRAINT [DF_carrot_CalendarEventCategory_CalendarEventCategoryID]  DEFAULT (newid()) FOR [CalendarEventCategoryID]

	ALTER TABLE [dbo].[carrot_CalendarEventProfile] ADD  CONSTRAINT [DF_carrot_CalendarEvent_CalendarEventProfileID]  DEFAULT (newid()) FOR [CalendarEventProfileID]

	ALTER TABLE [dbo].[carrot_CalendarEvent] ADD  CONSTRAINT [DF_carrot_CalendarEvent_CalendarEventID]  DEFAULT (newid()) FOR [CalendarEventID]


	ALTER TABLE [dbo].[carrot_CalendarEventProfile]  WITH CHECK ADD  CONSTRAINT [FK_carrot_CalendarEventProfile_carrot_CalendarEventCategory] FOREIGN KEY([CalendarEventCategoryID])
	REFERENCES [dbo].[carrot_CalendarEventCategory] ([CalendarEventCategoryID])
	ALTER TABLE [dbo].[carrot_CalendarEventProfile] CHECK CONSTRAINT [FK_carrot_CalendarEventProfile_carrot_CalendarEventCategory]


	ALTER TABLE [dbo].[carrot_CalendarEventProfile]  WITH CHECK ADD  CONSTRAINT [FK_carrot_CalendarEventProfile_carrot_CalendarFrequency] FOREIGN KEY([CalendarFrequencyID])
	REFERENCES [dbo].[carrot_CalendarFrequency] ([CalendarFrequencyID])
	ALTER TABLE [dbo].[carrot_CalendarEventProfile] CHECK CONSTRAINT [FK_carrot_CalendarEventProfile_carrot_CalendarFrequency]


	ALTER TABLE [dbo].[carrot_CalendarEvent]  WITH CHECK ADD  CONSTRAINT [FK_carrot_CalendarEvent_carrot_CalendarEventProfile] FOREIGN KEY([CalendarEventProfileID])
	REFERENCES [dbo].[carrot_CalendarEventProfile] ([CalendarEventProfileID])
	ALTER TABLE [dbo].[carrot_CalendarEvent] CHECK CONSTRAINT [FK_carrot_CalendarEvent_carrot_CalendarEventProfile]


END

GO


IF not exists (select * from [dbo].[carrot_CalendarFrequency]) BEGIN

	INSERT [dbo].[carrot_CalendarFrequency] ([FrequencySortOrder], [FrequencyValue], [FrequencyName]) VALUES (1, N'Once', N'Once')
	INSERT [dbo].[carrot_CalendarFrequency] ([FrequencySortOrder], [FrequencyValue], [FrequencyName]) VALUES (2, N'Daily', N'Daily')
	INSERT [dbo].[carrot_CalendarFrequency] ([FrequencySortOrder], [FrequencyValue], [FrequencyName]) VALUES (3, N'Weekly', N'Weekly')
	INSERT [dbo].[carrot_CalendarFrequency] ([FrequencySortOrder], [FrequencyValue], [FrequencyName]) VALUES (4, N'Monthly', N'Monthly')
	INSERT [dbo].[carrot_CalendarFrequency] ([FrequencySortOrder], [FrequencyValue], [FrequencyName]) VALUES (5, N'Yearly', N'Yearly')

END

GO

--====================================


IF NOT EXISTS( select * from [INFORMATION_SCHEMA].[COLUMNS] 
		where table_name = 'carrot_CalendarEventProfile' and column_name = 'IsHoliday') BEGIN

	ALTER TABLE [dbo].[carrot_CalendarEventProfile] ADD [IsHoliday] bit NULL
	ALTER TABLE [dbo].[carrot_CalendarEventProfile] ADD [IsAnnualHoliday] bit NULL
	
END

IF NOT EXISTS( select * from [INFORMATION_SCHEMA].[COLUMNS] 
		where table_name = 'carrot_CalendarEvent' and column_name = 'EventStartTime') BEGIN

	ALTER TABLE [dbo].[carrot_CalendarEvent] ADD [EventStartTime] [time](7) NULL
	ALTER TABLE [dbo].[carrot_CalendarEvent] ADD [EventEndTime] [time](7) NULL
	
END

IF NOT EXISTS( select * from [INFORMATION_SCHEMA].[COLUMNS] 
		where table_name = 'carrot_CalendarEventProfile' and column_name = 'RecursEvery') BEGIN

	ALTER TABLE [dbo].[carrot_CalendarEventProfile] ADD [RecursEvery] [int] NULL
	
END


GO


UPDATE [carrot_CalendarEventProfile]
SET [IsHoliday] = ISNULL([IsHoliday], 0)
WHERE IsNUll([IsHoliday], 0) = 0

UPDATE [carrot_CalendarEventProfile]
SET [IsAnnualHoliday] = ISNULL([IsAnnualHoliday], 0)
WHERE IsNUll([IsAnnualHoliday], 0) = 0

ALTER TABLE [dbo].[carrot_CalendarEventProfile] 
	ALTER COLUMN  [IsHoliday] [bit] NOT NULL

ALTER TABLE [dbo].[carrot_CalendarEventProfile] 
	ALTER COLUMN  [IsAnnualHoliday] [bit] NOT NULL


UPDATE [carrot_CalendarEventProfile]
SET [RecursEvery] = ISNULL([RecursEvery], 1)
WHERE IsNUll([RecursEvery], -100) = -100

ALTER TABLE [dbo].[carrot_CalendarEventProfile] 
	ALTER COLUMN  [RecursEvery] [int] NOT NULL


GO

--====================================

GO

IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vw_carrot_CalendarEvent]'))
DROP VIEW [dbo].[vw_carrot_CalendarEvent]

IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vw_carrot_CalendarEventProfile]'))
DROP VIEW [dbo].[vw_carrot_CalendarEventProfile]


GO
CREATE VIEW [dbo].[vw_carrot_CalendarEvent]
AS 

SELECT ces.SiteID, ces.CalendarEventProfileID, ces.CalendarFrequencyID, ces.CalendarEventCategoryID, ces.EventStartDate, ces.EventEndDate, 
      ces.EventStartTime, ces.EventEndTime, ces.EventTitle, ces.EventRepeatPattern, ces.EventDetail as EventSeriesDetail, ces.IsCancelledPublic, 
      ces.IsAllDayEvent, ces.IsPublic, ces.IsCancelled AS IsCancelledSeries, ce.IsCancelled AS IsCancelledEvent, ces.IsHoliday, ces.IsAnnualHoliday, ces.RecursEvery,
      ce.CalendarEventID, ce.EventDate, ce.EventDetail, cef.FrequencyValue, cef.FrequencyName, cef.FrequencySortOrder, 
      ce.EventStartTime as EventStartTimeOverride, ce.EventEndTime as EventEndTimeOverride, cec.CategoryFGColor, cec.CategoryBGColor, cec.CategoryName
FROM dbo.carrot_CalendarEventProfile AS ces 
INNER JOIN dbo.carrot_CalendarEvent AS ce ON ces.CalendarEventProfileID = ce.CalendarEventProfileID 
INNER JOIN dbo.carrot_CalendarFrequency AS cef ON ces.CalendarFrequencyID = cef.CalendarFrequencyID 
INNER JOIN dbo.carrot_CalendarEventCategory AS cec ON ces.CalendarEventCategoryID = cec.CalendarEventCategoryID 
		AND ces.SiteID = cec.SiteID

GO
CREATE VIEW [dbo].[vw_carrot_CalendarEventProfile]
AS 

SELECT ces.SiteID, ces.CalendarEventProfileID, ces.CalendarFrequencyID, ces.CalendarEventCategoryID, ces.EventStartDate, ces.EventStartTime, ces.EventEndDate, 
      ces.EventEndTime, ces.EventTitle, ces.EventRepeatPattern, ces.EventDetail, ces.IsCancelled, ces.IsCancelledPublic, ces.IsHoliday, ces.IsAnnualHoliday, ces.RecursEvery,
      ces.IsAllDayEvent, ces.IsPublic, cef.FrequencyValue, cef.FrequencyName, cef.FrequencySortOrder, cec.CategoryFGColor, cec.CategoryBGColor, cec.CategoryName
FROM dbo.carrot_CalendarEventProfile AS ces 
INNER JOIN dbo.carrot_CalendarFrequency AS cef ON ces.CalendarFrequencyID = cef.CalendarFrequencyID 
INNER JOIN dbo.carrot_CalendarEventCategory AS cec ON ces.CalendarEventCategoryID = cec.CalendarEventCategoryID 
		AND ces.SiteID = cec.SiteID

GO
