using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.IO;
using System.Diagnostics;
using System.Reflection;

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

	public class MailRequest {

		public MailRequest() {
			this.Subject = string.Empty;
			this.Body = string.Empty;
			this.Attachments = new List<IFormFile>();

			_mailSettings = new SmtpSettings();
		}

		public MailRequest(SmtpSettings mailSettings) : this() {
			_mailSettings = mailSettings;
		}

		public static MailRequest Create() {
			var mailSettings = SmtpSettings.GetEMailSettings();
			return new MailRequest(mailSettings);
		}

		private static bool? _debug = null;
		private static FileVersionInfo _fileversion = null;
		private static Version _version = null;

		private static Version CurrentVersion {
			get {
				if (_version == null) {
					_version = Assembly.GetExecutingAssembly().GetName().Version;
				}
				return _version;
			}
		}

		private static void LoadFileInfo() {
			if (_fileversion == null) {
				_debug = false;
#if DEBUG
				_debug = true;
#endif
				var assembly = Assembly.GetExecutingAssembly();
				_fileversion = FileVersionInfo.GetVersionInfo(assembly.Location);
			}
		}

		public static string CurrentDLLVersion {
			get {
				LoadFileInfo();
				return _fileversion.FileVersion;
			}
		}

		public static string CurrentDLLMajorMinorVersion {
			get {
				Version v = CurrentVersion;
				return v.Major.ToString() + "." + v.Minor.ToString();
			}
		}

		public void ConfigureMessage(string email, string subject, string body) {
			this.EmailTo.Add(email);
			this.Subject = subject;
			this.Body = body;
		}

		public void ConfigureMessage(string email, string subject, string body, List<IFormFile> attach) {
			ConfigureMessage(email, subject, body);
			this.Attachments = attach;
		}

		public void ConfigureMessage(List<string> emails, string subject, string body) {
			this.EmailTo = emails;
			this.Subject = subject;
			this.Body = body;
		}

		public void ConfigureMessage(List<string> emails, string subject, string body, List<IFormFile> attach) {
			ConfigureMessage(emails, subject, body);
			this.Attachments = attach;
		}

		public List<string> EmailTo { get; set; } = new List<string>();
		public List<string> EmailCC { get; set; } = new List<string>();
		public string Subject { get; set; }
		public string Body { get; set; }
		public bool HtmlBody { get; set; }
		public List<IFormFile> Attachments { get; set; } = new List<IFormFile>();
		public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

		private SmtpSettings _mailSettings;

		public Dictionary<string, string> DefaultHeaders {
			get {
				var head = new Dictionary<string, string>();
				head.Add("X-Computer", Environment.MachineName);
				head.Add("X-Originating-IP", CarrotWebHelper.HttpContext.Connection.RemoteIpAddress.ToString());
				head.Add("X-Application", "Carrotware Web " + CurrentDLLVersion);
				head.Add("User-Agent", "Carrotware Web " + CurrentDLLMajorMinorVersion);
				head.Add("Message-ID", "<" + Guid.NewGuid().ToString().ToLowerInvariant() + "@" + _mailSettings.Host + ">");
				return head;
			}
		}

		public void SaveToPickupDirectory(MimeMessage msg) {
			var path = Path.Join(_mailSettings.PickupDirectoryLocation, string.Format("{0}.eml", Guid.NewGuid()).ToLowerInvariant());

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

			// fallback to smtp username in case from is empty
			var fromEmail = !string.IsNullOrEmpty(_mailSettings.FromEmail) ? _mailSettings.FromEmail : _mailSettings.SmtpUsername;

			if (string.IsNullOrEmpty(_mailSettings.DisplayName)) {
				email.Sender = MailboxAddress.Parse(fromEmail);
			} else {
				email.Sender = new MailboxAddress(_mailSettings.DisplayName, fromEmail);
			}

			if (email.From.Any() == false) {
				email.From.Add(email.Sender);
			}

			foreach (var h in this.DefaultHeaders) {
				if (!request.Headers.ContainsKey(h.Key)) {
					email.Headers.Add(h.Key, h.Value);
				}
			}

			foreach (var addy in request.EmailTo) {
				email.To.Add(MailboxAddress.Parse(addy));
			}
			foreach (var addy in request.EmailCC) {
				email.Cc.Add(MailboxAddress.Parse(addy));
			}

			email.Subject = request.Subject;

			foreach (var h in request.Headers) {
				email.Headers.Add(h.Key, h.Value);
			}

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