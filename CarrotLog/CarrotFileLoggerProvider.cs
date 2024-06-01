using System.Collections.Concurrent;

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

	[ProviderAlias("CarrotLogger")]
	public class CarrotFileLoggerProvider : ILoggerProvider {
		private readonly Task _outputThread;
		private readonly CarrotLogWriter _writer;

		private readonly BlockingCollection<string> _messageQueue = new BlockingCollection<string>(2048);
		private readonly ConcurrentDictionary<string, CarrotFileLogger> _loggers = new ConcurrentDictionary<string, CarrotFileLogger>();

		private readonly IHttpContextAccessor _httpContextAccessor;

		internal IHttpContextAccessor ContextAccessor { get { return _httpContextAccessor; } }

		internal CarrotLoggerConfig Options { get; private set; }

		public CarrotFileLoggerProvider() : this(new CarrotLoggerConfig()) { }

		public CarrotFileLoggerProvider(CarrotLoggerConfig options) {
			this.Options = options;
			if (this.Options.UseAccessor) {
				_httpContextAccessor = new HttpContextAccessor();
			}

			_writer = new CarrotLogWriter(this);
			_outputThread = Task.Factory.StartNew(LoggingTask, this, TaskCreationOptions.LongRunning);
		}

		public ILogger CreateLogger(string categoryName) {
			// only make one logger per consuming category
			return _loggers.TryGetValue(categoryName, out CarrotFileLogger? logger) ?
					logger : _loggers.GetOrAdd(categoryName, new CarrotFileLogger(categoryName, this));
		}

		internal static void LoggingTask(object action) {
			var fileProvider = (CarrotFileLoggerProvider)action;
			fileProvider.WriteQueue();
		}

		public void Dispose() {
			_messageQueue.CompleteAdding();

			try {
				_outputThread.Wait(1500);
			} catch (ThreadStateException) { }

			_writer.Close();
		}

		internal void AppendEntry(string message) {
			if (!_messageQueue.IsAddingCompleted) {
				try {
					_messageQueue.Add(message);
					return;
				} catch (InvalidOperationException) { }
			}
		}

		private static object _writeLock = new object();

		private void WriteQueue() {
			lock (_writeLock) {
				try {
					_writer.Open();
					foreach (var logEntry in _messageQueue.GetConsumingEnumerable()) {
						_writer.WriteOutput(logEntry, _messageQueue.Count == 0);
					}
				} catch {
					try {
						_messageQueue.CompleteAdding();
					} catch { }
				}
			}
		}
	}
}
