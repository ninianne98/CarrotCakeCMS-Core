/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

namespace Carrotware.Web.UI.Components {

	public static class EmailHelper {

		public static bool SendMail(string fromEmail, string emailTo, string subjectLine, string bodyText, bool isHTML) {
			List<string> lstTo = new List<string>();
			if (string.IsNullOrEmpty(emailTo)) {
				emailTo = string.Empty;
			}
			//emailTo = emailTo.Replace(",", ";");

			if (emailTo.Contains(";")) {
				lstTo = emailTo.Split(';').Where(x => x.Length > 2 && x.Contains("@")).Select(x => x.Trim()).ToList();
			} else {
				lstTo.Add(emailTo);
			}

			return SendMail(fromEmail, lstTo, null, subjectLine, bodyText, isHTML, null);
		}

		public static bool SendMail(string? fromEmail, List<string> emailTo, List<string> emailCC,
				string? subjectLine, string? bodyText, bool isHTML, List<string> attachments) {
			var mailSettings = SmtpSettings.GetEMailSettings();

			if (string.IsNullOrEmpty(fromEmail) || !fromEmail.Contains("@")) {
				fromEmail = mailSettings.FromEmail;  // try from email first
			}
			if (string.IsNullOrEmpty(fromEmail) || !fromEmail.Contains("@")) {
				fromEmail = mailSettings.SmtpUsername; // use smtp user as fallback if not valid
			}

			if (emailTo != null && emailTo.Any()) {
				var message = new MailRequest(mailSettings) {
					Subject = subjectLine ?? string.Empty,
					Body = bodyText ?? string.Empty,
					HtmlBody = isHTML
				};

				message.Headers = message.DefaultHeaders;

				foreach (var t in emailTo) {
					message.EmailTo.Add(t);
				}

				if (emailCC != null) {
					foreach (var t in emailCC) {
						message.EmailCC.Add(t);
					}
				}

				if (attachments != null) {
					foreach (var attach in attachments) {
						using (var stream = File.OpenRead(attach)) {
							IFormFile file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name)) {
								Headers = new HeaderDictionary(),
								ContentType = "application/octet-stream"
							};

							message.Attachments.Add(file);
						}
					}
				}

				var task = message.SendEmailAsync();
			}

			return true;
		}
	}
}