using System.Text;
using System.Text.Json;

/*
* Carrotware Simple Logger
* http://www.carrotware.com/
*
* Copyright 2023, Samantha Copeland
* Licensed under the MIT License
*
* Date: June 2023
*/

namespace Carrotware.Logging {

	public class CarrotLogMessage : ICarrotLogMessage {
		protected Exception? _exception;
		protected StringBuilder _sb = new StringBuilder();

		public CarrotLogMessage() { }

		public CarrotLogMessage(DateTime date, LogLevel level, string name, EventId eventId,
						string message) {
			this.LogName = name;
			this.DateTime = date;
			this.Message = message ?? string.Empty;
			this.LogLevel = level.LevelToString();
			this.EventId = eventId;
			_exception = null;
		}

		public CarrotLogMessage(DateTime date, LogLevel level, string name, EventId eventId,
						string message, Exception ex)
								: this(date, level, name, eventId, message) {
			_exception = ex;

			if (_exception != null) {
				this.Exception = _exception.ToString();
			}
		}

		public DateTime DateTime { get; set; } = DateTime.UtcNow;
		public EventId EventId { get; set; }
		public string? Exception { get; set; }
		public string LogLevel { get; set; }
		public string LogName { get; set; }
		public string Message { get; set; }

		public virtual string ToJson() {
			_sb.Clear();

			if (_exception != null) {
				_sb.AppendLine(_exception.ToString());
				_sb.AppendLine(_exception.StackTrace?.ToString());
				if (_exception.InnerException != null) {
					_sb.AppendLine("InnerException: " + _exception.InnerException.ToString());
					_sb.AppendLine("InnerException: " + _exception.InnerException.StackTrace?.ToString());
				}
			}
			this.Exception = _sb.ToString().EscapeMessage();
			this.Message = this.Message.EscapeMessage();

			return JsonSerializer.Serialize(this);
		}

		public virtual string ToTabbed() {
			_sb.Clear();

			_sb.Append(this.DateTime.ToString("o") + "\t" + this.LogLevel);
			_sb.Append("\t" + this.LogName);
			_sb.Append("\t[" + this.EventId + "]");
			_sb.Append("\t" + this.Message.EscapeMessage());

			if (_exception != null) {
				_sb.AppendLine("-------------------");
				_sb.AppendLine(_exception.ToString().EscapeMessage());
				if (_exception.InnerException != null) {
					_sb.AppendLine(_exception.InnerException.ToString().EscapeMessage());
				}
				_sb.AppendLine("-------------------");
			}

			return _sb.ToString();
		}
	}

	//=====================================
	public class CarrotLogMessageWeb : CarrotLogMessage {

		public CarrotLogMessageWeb() { }

		public CarrotLogMessageWeb(DateTime date, LogLevel level, string name, EventId eventId,
							string message) : base(date, level, name, eventId, message) {
		}

		public CarrotLogMessageWeb(DateTime date, LogLevel level, string name, EventId eventId,
							string message, IHttpContextAccessor? accessor)
									: base(date, level, name, eventId, message) {
			if (accessor != null && accessor.HttpContext != null) {
				this.Path = accessor.HttpContext.Request.Path.ToString().EscapeMessage();
				this.UserAgent = accessor.HttpContext.Request.Headers.UserAgent.ToString().EscapeMessage();
				this.User = accessor.HttpContext.User?.Identity?.Name?.EscapeMessage();
			}
		}

		public CarrotLogMessageWeb(DateTime date, LogLevel level, string name, EventId eventId,
							string message, IHttpContextAccessor accessor, Exception ex)
									: this(date, level, name, eventId, message, accessor) {
			_exception = ex;

			if (_exception != null) {
				this.Exception = _exception.ToString();
			}
		}

		public string? UserAgent { get; set; }
		public string? Path { get; set; }
		public string? User { get; set; }

		public override string ToTabbed() {
			_sb.Clear();

			_sb.Append(this.DateTime.ToString("o") + "\t" + this.LogLevel);
			_sb.Append("\t" + this.LogName);
			_sb.Append("\t[" + this.EventId + "]");
			_sb.Append("\t[" + this.User + "]");
			_sb.Append("\t[" + this.UserAgent + "]");
			_sb.Append("\t[" + this.Path + "]");
			_sb.Append("\t" + this.Message.EscapeMessage());

			if (_exception != null) {
				_sb.AppendLine("-------------------");
				_sb.AppendLine(_exception.ToString());
				if (_exception.InnerException != null) {
					_sb.AppendLine(_exception.InnerException.ToString());
				}
				_sb.AppendLine("-------------------");
			}

			return _sb.ToString();
		}
	}
}
