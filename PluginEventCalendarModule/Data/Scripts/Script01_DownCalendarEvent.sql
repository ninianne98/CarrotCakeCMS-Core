IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vw_carrot_CalendarEvent]'))
DROP VIEW [dbo].[vw_carrot_CalendarEvent]

IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vw_carrot_CalendarEventProfile]'))
DROP VIEW [dbo].[vw_carrot_CalendarEventProfile]

GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[carrot_CalendarEvent]') AND type in (N'U')) BEGIN
	ALTER TABLE [dbo].[carrot_CalendarEventProfile] DROP CONSTRAINT [FK_carrot_CalendarEventProfile_carrot_CalendarFrequency]

	ALTER TABLE [dbo].[carrot_CalendarEventProfile] DROP CONSTRAINT [FK_carrot_CalendarEventProfile_carrot_CalendarEventCategory]

	ALTER TABLE [dbo].[carrot_CalendarEvent] DROP CONSTRAINT [FK_carrot_CalendarEvent_carrot_CalendarEventProfile]
END

GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[carrot_CalendarFrequency]') AND type in (N'U'))
DROP TABLE [dbo].[carrot_CalendarFrequency]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[carrot_CalendarEventProfile]') AND type in (N'U'))
DROP TABLE [dbo].[carrot_CalendarEventProfile]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[carrot_CalendarEventCategory]') AND type in (N'U'))
DROP TABLE [dbo].[carrot_CalendarEventCategory]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[carrot_CalendarEvent]') AND type in (N'U'))
DROP TABLE [dbo].[carrot_CalendarEvent]
GO
