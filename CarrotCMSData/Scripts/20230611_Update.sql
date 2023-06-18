SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER VIEW [dbo].[vw_carrot_EditHistory]
-- select top 10 * from  [dbo].[vw_carrot_EditHistory]
AS 

SELECT  rc.SiteID, c.ContentID, c.Root_ContentID, c.IsLatestVersion, c.TitleBar, c.NavMenuText, c.PageHead, c.CreditUserId, 
	c.EditDate, rc.CreateDate, rc.[FileName], ct.ContentTypeID, ct.ContentTypeValue, rc.PageActive, rc.GoLiveDate, rc.RetireDate, 
	c.EditUserId, m.UserName as EditUserName, m.Email as EditEmail, 
	rc.CreateUserId, m2.UserName as CreateUserName, m2.Email as CreateEmail
FROM [dbo].carrot_RootContent AS rc
	INNER JOIN [dbo].carrot_Content AS c ON rc.Root_ContentID = c.Root_ContentID 
	INNER JOIN [dbo].carrot_ContentType AS ct ON rc.ContentTypeID = ct.ContentTypeID
	INNER JOIN [dbo].carrot_UserData AS u ON c.EditUserId = u.UserId 
	INNER JOIN [dbo].AspNetUsers AS m ON u.UserKey = m.Id
	INNER JOIN [dbo].carrot_UserData AS u2 ON rc.CreateUserId = u2.UserId 
	INNER JOIN [dbo].AspNetUsers AS m2 ON u2.UserKey = m2.Id
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER VIEW [dbo].[vw_carrot_UserData]
-- select top 10 * from  [dbo].[vw_carrot_UserData]
AS 

SELECT mu.Id, mu.Email, mu.EmailConfirmed, mu.PasswordHash, mu.SecurityStamp, mu.PhoneNumber, mu.PhoneNumberConfirmed, 
		mu.TwoFactorEnabled,  CAST(mu.LockoutEnd AS datetime) LockoutEndDateUtc, mu.LockoutEnabled, mu.AccessFailedCount, mu.UserName, ud.UserId, ud.UserKey, 
		ud.UserNickName, ud.FirstName, ud.LastName, ud.UserBio
FROM dbo.AspNetUsers AS mu 
LEFT JOIN carrot_UserData AS ud ON ud.UserKey = mu.Id
GO


