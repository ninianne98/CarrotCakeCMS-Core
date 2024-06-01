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

	internal class CarrotLogWriter {
		private readonly CarrotFileLoggerProvider _logProvider;
		private readonly CarrotLoggerConfig _options;

		private string _fileName = null;
		private Stream _logStream = null;
		private TextWriter _logWriter = null;

		internal CarrotLogWriter(CarrotFileLoggerProvider provider) {
			_logProvider = provider;
			_options = provider.Options;  // for accessibility simplicity

			_fileName = GetFilename();

			Open();
		}

		internal string GenerateFileName() {
			var logDate = _options.UseUtc ? DateTime.UtcNow : DateTime.Now;
			var pattern = _options.FileDatePattern;

			if (!(pattern.Contains("{") && pattern.Contains("}"))) {
				// assume if the braces are left out, simply the string part was provided
				pattern = "{0:" + pattern + "}";
			}

			var fileName = _options.FilePrefix + "_" + string.Format(pattern, logDate) + ".log";

			return Path.Join(_options.FilePath, fileName);
		}

		internal string GetFilename() {
			var workingFileName = GenerateFileName();

			// if file does not exist or no max size, exit, predicted filename OK
			if (!File.Exists(workingFileName) || _options.MaxLogSizeKb <= 0) {
				return workingFileName;
			}

			var workingDirectory = Path.GetDirectoryName(workingFileName);
			var workingFileNameOnly = Path.GetFileNameWithoutExtension(workingFileName);
			var workingExt = Path.GetExtension(workingFileName);

			// if the file exists, make sure it's the latest for the pattern
			if (File.Exists(workingFileName)) {
				if (string.IsNullOrEmpty(workingDirectory)) {
					workingDirectory = AppDomain.CurrentDomain.BaseDirectory;
				}

				var logMask = workingFileNameOnly + "*" + workingExt;

				var otherLogs = Directory.Exists(workingDirectory) ?
								Directory.GetFiles(workingDirectory, logMask, SearchOption.TopDirectoryOnly)
										: Array.Empty<string>();

				if (otherLogs.Length > 0) {
					// most recent file if one or more already exist based on the pattern
					var lastFileInfo = otherLogs.Select(f => new FileInfo(f))
											.OrderBy(f => f.Name)
											.OrderByDescending(f => f.LastWriteTimeUtc).First();

					workingFileName = lastFileInfo.FullName;

					if (_options.MaxLogSizeKb > 0 && lastFileInfo.Length >= (_options.MaxLogSizeKb * 1024)) {
						var newFileIdx = otherLogs.Length + 1;
						var nextFileName = workingFileNameOnly + "." + newFileIdx.ToString("D2") + workingExt;

						workingFileName = Path.Join(workingDirectory, nextFileName);
					}
				}
			}

			return workingFileName;
		}

		private static object _createLock = new object();

		internal void Open() {
			lock (_createLock) {
				if (_options.MinLevel != LogLevel.None) {
					if (_logWriter == null || _logStream == null) {
						try {
							var fileInfo = new FileInfo(_fileName);
							fileInfo.Directory.Create();

							_logStream = new FileStream(_fileName, FileMode.OpenOrCreate, FileAccess.Write);
							_logStream.Seek(0, SeekOrigin.End);
						} catch (Exception ex) {
							// TODO: do some alt behavior
							throw;
						}

						_logWriter = new StreamWriter(_logStream);
					}
				}
			}
		}

		internal void Close() {
			if (_logWriter != null) {
				_logWriter.Dispose();
				_logStream.Dispose();
				_logWriter = null;
				_logStream = null;
			}
		}

		private DateTime _dateCheck = DateTime.MinValue;

		internal void CheckForNewFile() {
			if (_dateCheck < DateTime.UtcNow || string.IsNullOrEmpty(_fileName)) {
				// we don't need to check the file name super frequently
				_dateCheck = DateTime.UtcNow.AddSeconds(30);

				var nextFilename = GetFilename();

				bool fileChanged = nextFilename.ToLowerInvariant() != _fileName.ToLowerInvariant();

				if (fileChanged) {
					Close();
					_fileName = nextFilename;
					Open();
				}
			}
		}

		private static object _writeLock = new object();

		internal void WriteOutput(string message, bool flush) {
			lock (_writeLock) {
				if (_options.MinLevel != LogLevel.None) {
					if (_logWriter != null) {
						CheckForNewFile();
						_logWriter.WriteLine(message);
						if (flush) {
							_logWriter.Flush();
						}
					}
				}
			}
		}
	}
}
