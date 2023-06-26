using Carrotware.CMS.Interface;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.IO;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

namespace Carrotware.CMS.Security {

	public class SmtpSettings {
		public string FromEmail { get; set; }
		public string DisplayName { get; set; }
		public string SmtpUsername { get; internal set; }
		public string SmtpPassword { get; set; }
		public string Host { get; set; } = "localhost";
		public int Port { get; set; } = 587;
		public bool UseTls { get; set; } = false;
		public bool UseSpecifiedPickupDirectory { get; set; } = true;
		public string PickupDirectoryLocation { get; set; } = Path.GetTempPath();
	}

	//===============================

	public class MailRequest {

		public MailRequest(SmtpSettings mailSettings) : this() {
			_mailSettings = mailSettings;
		}

		public static MailRequest Create() {
			var mailSettings = CarrotHttpHelper.ServiceProvider.GetService<SmtpSettings>();
			return new MailRequest(mailSettings);
		}

		public MailRequest() {
			this.Emails = new List<string>();
			this.Subject = string.Empty;
			this.Body = string.Empty;
			this.Attachments = null;

			_mailSettings = new SmtpSettings();
		}

		public void ConfigureMessage(string email, string subject, string body) {
			this.Emails.Add(email);
			this.Subject = subject;
			this.Body = body;
		}

		public void ConfigureMessage(string email, string subject, string body, List<IFormFile> attach) {
			ConfigureMessage(email, subject, body);
			this.Attachments = attach;
		}

		public void ConfigureMessage(List<string> emails, string subject, string body) {
			this.Emails = emails;
			this.Subject = subject;
			this.Body = body;
		}

		public void ConfigureMessage(List<string> emails, string subject, string body, List<IFormFile> attach) {
			ConfigureMessage(emails, subject, body);
			this.Attachments = attach;
		}

		public List<string> Emails { get; set; }
		public string Subject { get; set; }
		public string Body { get; set; }
		public bool HtmlBody { get; set; }
		public List<IFormFile>? Attachments { get; set; }

		private SmtpSettings _mailSettings;

		public void SaveToPickupDirectory(MimeMessage msg) {
			var path = Path.Combine(_mailSettings.PickupDirectoryLocation, string.Format("{0}.eml", Guid.NewGuid()).ToLowerInvariant());

			try {
				using (var stream = System.IO.File.Open(path, FileMode.CreateNew)) {
					try {
						using (var filtered = new FilteredStream(stream)) {
							filtered.Add(new SmtpDataFilter());

							var options = FormatOptions.Default.Clone();
							options.NewLineFormat = NewLineFormat.Dos;
							msg.WriteTo(options, filtered);

							filtered.Flush();
							return;
						}
					} catch (Exception ex) {
						System.IO.File.Delete(path);
						throw;
					}
				}
			} catch (IOException ioEx) {
				throw;
			}
		}

		public async Task SendEmailAsync() {
			await SendEmailAsync(this);
		}

		public async Task SendEmailAsync(MailRequest request) {
			var email = new MimeMessage();
			var builder = new BodyBuilder();

			email.Sender = MailboxAddress.Parse(_mailSettings.FromEmail);
			foreach (var addy in request.Emails) {
				email.To.Add(MailboxAddress.Parse(addy));
			}
			email.Subject = request.Subject;

			if (request.Attachments != null) {
				byte[] fileBytes;
				foreach (var file in request.Attachments) {
					if (file.Length > 0) {
						using (var ms = new MemoryStream()) {
							file.CopyTo(ms);
							fileBytes = ms.ToArray();
						}
						builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
					}
				}
			}

			if (request.HtmlBody) {
				builder.HtmlBody = request.Body;
			} else {
				builder.TextBody = request.Body;
			}

			email.Body = builder.ToMessageBody();

			if (_mailSettings.UseSpecifiedPickupDirectory) {
				SaveToPickupDirectory(email);
			} else {
				using (var smtp = new SmtpClient()) {
					if (_mailSettings.UseTls) {
						smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
					} else {
						smtp.Connect(_mailSettings.Host, _mailSettings.Port);
					}
					smtp.Authenticate(_mailSettings.SmtpUsername, _mailSettings.SmtpPassword);
					await smtp.SendAsync(email);
					smtp.Disconnect(true);
				}
			}
		}
	}
}