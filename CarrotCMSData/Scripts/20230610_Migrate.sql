INSERT INTO [dbo].[AspNetUsers] ([Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp],
			[PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) 
	SELECT m.[Id], m.[UserName], UPPER(m.[UserName]), m.[Email], UPPER(m.[Email]), m.[EmailConfirmed], UPPER(m.[PasswordHash]), m.[SecurityStamp],
			 m.[PhoneNumber], m.[PhoneNumberConfirmed], m.[TwoFactorEnabled], m.[LockoutEndDateUtc], m.[LockoutEnabled], m.[AccessFailedCount]
	FROM [dbo].[membership_User] m 
		LEFT JOIN [dbo].[AspNetUsers] a on a.[Id] = m.[Id] 
	where a.[Id] IS NULL

INSERT INTO [dbo].[AspNetRoles] ([Id],[Name],[NormalizedName])
	select m.[Id], m.[Name], UPPER(m.[Name])
	from  [dbo].[membership_Role] m
		LEFT JOIN [dbo].[AspNetRoles] a on a.[Id] = m.[Id] 
	where a.[Id] IS NULL

INSERT INTO [dbo].[AspNetUserRoles] ([UserId],[RoleId])
	select m.[UserId], m.[RoleId]
	from  [dbo].[membership_UserRole] m
		LEFT JOIN [dbo].[AspNetUserRoles] a on a.UserId = m.UserId and a.RoleId = m.RoleId 
	where a.[UserId] IS NULL

GO

IF (NOT EXISTS(SELECT * FROM [__EFMigrationsHistory])) BEGIN
	insert into [__EFMigrationsHistory]([MigrationId],[ProductVersion])
		values ('00000000000000_Initial','6.0.16')
END

--================================================================================

GO

declare @GrpAdminID uniqueidentifier
declare @GrpEditID uniqueidentifier
declare @GrpUserID uniqueidentifier

IF ((select count([Id]) from [dbo].[AspNetRoles] where [Name] = N'CarrotCMS Administrators') < 1) BEGIN

	INSERT [dbo].[AspNetRoles] ([Id], [Name])
		 VALUES (lower(NewID()), N'CarrotCMS Administrators')
	INSERT [dbo].[AspNetRoles] ([Id], [Name])
		  VALUES (lower(NewID()), N'CarrotCMS Editors')
	INSERT [dbo].[AspNetRoles] ([Id], [Name])
		  VALUES (lower(NewID()), N'CarrotCMS Users')

END

GO

-- get rid of ~ as .net code does not use it
UPDATE [dbo].[carrot_Content]
SET [TemplateFile] = REPLACE([TemplateFile],'~/views/','/views/')
WHERE [TemplateFile] like '~/views%' and [IsLatestVersion] = 1

GO

update [dbo].[AspNetRoles]
set [NormalizedName] = upper([Name])

update [dbo].[AspNetUsers]
set [NormalizedUserName] = upper([UserName]),
	[NormalizedEmail] = upper([Email]),
	[EmailConfirmed] = 1

-- ===================================================
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[membership_User]') AND type in (N'U')) BEGIN
	ALTER TABLE [dbo].[membership_UserLogin] DROP CONSTRAINT [FK_dbo.membership_UserLogin_dbo.membership_User_UserId]
	ALTER TABLE [dbo].[membership_UserClaim] DROP CONSTRAINT [FK_dbo.membership_UserClaim_dbo.membership_User_UserId]

	ALTER TABLE [dbo].[carrot_UserData] DROP CONSTRAINT [carrot_UserData_UserKey]
	ALTER TABLE [dbo].[carrot_Content] DROP CONSTRAINT [carrot_Content_CreditUserId_FK]
	ALTER TABLE [dbo].[carrot_Content] DROP CONSTRAINT [carrot_Content_EditUserId_FK]
END

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[membership_UserRole]') AND type in (N'U')) BEGIN
	ALTER TABLE [dbo].[membership_UserRole] DROP CONSTRAINT [FK_dbo.membership_UserRole_dbo.membership_User_UserId]
	ALTER TABLE [dbo].[membership_UserRole] DROP CONSTRAINT [FK_dbo.membership_UserRole_dbo.membership_Role_RoleId]
END
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[membership_UserLogin]') AND type in (N'U')) BEGIN
	DROP TABLE [dbo].[membership_UserLogin]
END

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[membership_UserClaim]') AND type in (N'U')) BEGIN
	DROP TABLE [dbo].[membership_UserClaim]
END

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[membership_UserRole]') AND type in (N'U')) BEGIN
	DROP TABLE [dbo].[membership_UserRole]
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[membership_User]') AND type in (N'U')) BEGIN
	DROP TABLE [dbo].[membership_User]
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[membership_Role]') AND type in (N'U')) BEGIN
	DROP TABLE [dbo].[membership_Role]
END
GO
