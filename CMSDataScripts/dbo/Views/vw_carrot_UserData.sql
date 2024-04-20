CREATE VIEW [dbo].[vw_carrot_UserData]
-- select top 10 * from  [dbo].[vw_carrot_UserData]
AS 

SELECT mu.Id, mu.Email, mu.EmailConfirmed, mu.PasswordHash, mu.SecurityStamp, mu.PhoneNumber, mu.PhoneNumberConfirmed, 
		mu.TwoFactorEnabled,  CAST(mu.LockoutEnd AS datetime) LockoutEndDateUtc, mu.LockoutEnabled, mu.AccessFailedCount, mu.UserName, ud.UserId, ud.UserKey, 
		ud.UserNickName, ud.FirstName, ud.LastName, ud.UserBio
FROM dbo.AspNetUsers AS mu 
LEFT JOIN carrot_UserData AS ud ON ud.UserKey = mu.Id