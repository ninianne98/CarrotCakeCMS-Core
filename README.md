# CarrotCakeCMS (MVC Core)
Source code for CarrotCakeCMS (MVC - Core), .Net Core 8

[SITE_CT]: http://www.carrotware.com/contact?from=github-core
[REPO_SF]: http://sourceforge.net/projects/carrotcakecmscore/
[REPO_GH]: https://github.com/ninianne98/CarrotCakeCMS-Core/

[DOC_PDF]: http://www.carrotware.com/fileassets/CarrotCakeCoreDevNotes.pdf?from=github-core
[DOC]: http://www.carrotware.com/carrotcake-download?from=github-core "CarrotCakeCMS User Documentation"
[TMPLT]: http://www.carrotware.com/carrotcake-templates?from=github-core
[IDE]: https://visualstudio.microsoft.com/
[VS2022C]: https://visualstudio.microsoft.com/vs/community/
[SQL]: https://www.microsoft.com/en-us/sql-server/sql-server-downloads
[SSMS]: https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms

Welcome to the GitHub project for CarrotCake CMS MVC Core, an open source c# project. CarrotCake is a [template-based][TMPLT] MVC .Net Core CMS (content management system) built with C#, SQL server, jQueryUI, and TinyMCE. This content management system supports multi-tenant webroots with shared databases. 

## If you have found this tool useful please [contact us][SITE_CT].

Source code and [documentation][DOC_PDF] is available on [GitHub][REPO_GH] and [SourceForge][REPO_SF]. Documentation and assemblies can be found [here][DOC].

Some features include: blogging engine, configurable date based blog post URLs, blog post content association with categories and tags, assignment/customization of category and tag URL patterns, simple content feedback collection and review, blog post pagination/indexes (with templating support), designation of default listing blog page (required to make search, category links, or tag links function), URL date formatting patterns, RSS feed support for posts and pages, import and export of site content, and import of content from WordPress XML export files.

Other features also include date based release and retirement of content - allowing you to queue up content to appear or disappear from your site on a pre-arranged schedule, site time-zone designation, site search, and ability to rename the administration folder. Supports the use of layout views to provide re-use when designing content view templates.

---

## CarrotCakeCMS (MVC Core) Developer Quick Start Guide

Copyright (c) 2011, 2015, 2023, 2024 Samantha Copeland
Licensed under the MIT or GPL v3 License

CarrotCakeCMS (MVC Core) is maintained by Samantha Copeland

### Install Development Tools

1. **[Visual Studio Community/Pro/Enterprise][IDE]** ([VS 2022 Community][VS2022C])  Typically being developed on VS 2022 Enterprise.  Requires patch version 17.8 or later, for .Net 8 support.
1. **[SQL Server Express 2016 (or higher/later)][SQL]** - currently vetted on 2012 Express and 2016 Express.  Entity Framework Core 7 does not work with older versions of  SQL Server, such as 2008/2008R2 and earlier.
Eventually, the project will use EF 8, which will mean a minimum of SQL 2016, until then, SQL 2012 is supported.
1. **[SQL Server Management Studio (SSMS)][SSMS]** - required for managing the database

### Get the Source Code

1. Go to the repository ([GitHub][REPO_GH] or [SourceForge][REPO_SF]) in a browser

1. Download either a GIT or ZIP archive or connect using either a GIT or SVN client

### Open the Project

1. Start **Visual Studio**

1. Open **CarrotCakeCoreMVC.sln** solution in the root of the repository

	Note: If your file extensions are hidden, you will not see the ".sln"
	Other SLN files are demo widgets for how to wire in custom code/extensions

1. Edit **appsettings.json** under **CMSAdmin** root directory (this corresponds to the **CMSAdminCore** project)

	- In the ConnectionStrings section, configure the CarrotwareCMS value to point to your server and the name of your database.
		Note: the credentials require database owner/dbo level as it will create the database artifacts for you.
	- In the SmtpSettings, configure the pickupDirectoryLocation to a directory on your development machine (for testing purposes).

1. Right-click on **CMSAdminCore** and select **Set as StartUp Project**

1. Right-click on **CMSAdminCore** and select **Rebuild**. The project should download all required NuGet packages and compile successfully

	There may be some warnings, you can ignore them

1. SQL Server should be running with an empty database matching the one specified in the connection string. If you are running the code a second or later time, it will auto update if there are schema changes (see dbo note above).  
	- Do not share a database between the Core, MVC 5, and WebForms editions.  You can update the schema if you want to upgrade and take your existing data to the newer version.  
	- If you manually add the first EF migration to an existing MVC5 version of this CMS, it will automatically migrate the data.  
	- Password hashes will not be valid when upgrading MVC 5 (or MVC Core 6) to MVC Core 8, so perform a password recovery to set valid ones.

### Make a backup FIRST when upgrading!

```sql
-- if you plan to use an existing database from the MVC 5 version, you will need to have some entries in the migrations table
-- password hashes from MVC 5 will be invalid, perform a password recovery to set valid ones

-- to create the migrations table:

CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- main CMS MVC 5-> MVC Core 8 - create the ef table (if needed) and execute the insert for 00000000000000_Initial
-- the password hashes will be incorrect, so perform a password reset once the DB has been upgraded
IF (NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] where [MigrationId]='00000000000000_Initial')
			AND EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[membership_User]') AND type in (N'U'))) BEGIN
	insert into [__EFMigrationsHistory]([MigrationId],[ProductVersion])
		values ('00000000000000_Initial','7.0.0')
END

-- photo gallery widget - create the ef table (if needed) and execute the insert for 20230625212349_InitialGallery
IF (NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] where [MigrationId]='20230625212349_InitialGallery')
			AND EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblGallery]') AND type in (N'U'))) BEGIN
	insert into [__EFMigrationsHistory]([MigrationId],[ProductVersion])
		values ('20230625212349_InitialGallery','7.0.0')
END

-- simple calendar widget - create the ef table (if needed) and execute the insert for 20230709210325_InitialCalendar
IF (NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] where [MigrationId]='20230709210325_InitialCalendar')
			AND EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblGallery]') AND type in (N'U'))) BEGIN
	insert into [__EFMigrationsHistory]([MigrationId],[ProductVersion])
		values ('20230709210325_InitialCalendar','7.0.0')
END

-- event calendar widget - create the ef table (if needed) and execute the insert for 20230723225354_InitialEventCalendar
IF (NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] where [MigrationId]='20230723225354_InitialEventCalendar')
			AND EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblGallery]') AND type in (N'U'))) BEGIN
	insert into [__EFMigrationsHistory]([MigrationId],[ProductVersion])
		values ('20230723225354_InitialEventCalendar','7.0.0')
END

-- to validate
select * from [__EFMigrationsHistory] where [MigrationId] like '%Initial%'
```

1. if the database is empty or has pending database changes, the EF migrations will be automatically applied.

1. The first time you start up the website, it will create the required artifacts in the database (tables/views/sprocs etc.)

1. Click the **Play** button (or hit F5) in the main toolbar to launch CarrotCakeCMS

1. When you run the website with an empty user database, you will be prompted to create the first user

1. Once you have created a user, you can go to the login screen, enter the credentials

1. After successfully logging in, you can create and manage your new website

### Using CarrotCakeCMS Core

For additional information on how to use CarrotCakeCMS, please see the **[CarrotCakeCMS Documentation][DOC]**.
