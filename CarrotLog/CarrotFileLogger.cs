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

	public class CarrotFileLogger : ILogger {
		private readonly string _logName;
		private readonly CarrotFileLoggerProvider _logProvider;
		private readonly CarrotLoggerConfig _options;

		public CarrotFileLogger(string logName, CarrotFileLoggerProvider provider) {
			_logName = logName;
			_logProvider = provider;
			_options = provider.Options;  // for accessibility simplicity
		}

		public IDisposable BeginScope<TState>(TState state) {
			return null;
		}

		public bool IsEnabled(LogLevel logLevel) {
			return logLevel >= (_options.MinLevel ?? LogLevel.Debug);
		}

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
						Func<TState, Exception, string> formatter) {
			if (formatter == null) {
				throw new ArgumentNullException(nameof(formatter));
			}

			if (!IsEnabled(logLevel)) {
				return;
			}

			ICarrotLogMessage entry;
			string message = formatter(state, exception);
			var timeStamp = _options.UseUtc ? DateTime.UtcNow : DateTime.Now;

			if (!string.IsNullOrEmpty(message) || exception != null) {
				if (_options.UseAccessor) {
					entry = new CarrotLogMessageWeb(timeStamp, logLevel, _logName, eventId, message, _logProvider.ContextAccessor, exception);
				} else {
					entry = new CarrotLogMessage(timeStamp, logLevel, _logName, eventId, message, exception);
				}

				if (_options.LogFormat == LogFormat.Tabbed) {
					_logProvider.AppendEntry(entry.ToTabbed());
				}

				if (_options.LogFormat == LogFormat.Json) {
					_logProvider.AppendEntry(entry.ToJson());
				}
			}
		}
	}
}
