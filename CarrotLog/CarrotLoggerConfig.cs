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

	public enum LogFormat {
		Tabbed,
		Json,
	}

	//=====================

	public class CarrotLoggerConfig {
		public LogLevel? MinLevel { get; set; } // = LogLevel.Debug;
		public string FilePrefix { get; set; } = AppDomain.CurrentDomain.FriendlyName;
		public string FileDatePattern { get; set; } = "{0:yyyyMMdd}";
		public string FilePath { get; set; } = AppDomain.CurrentDomain.BaseDirectory;
		public bool UseAccessor { get; set; } = true;
		public bool UseUtc { get; set; } = true;
		public long MaxLogSizeKb { get; set; } = 4096;
		public LogFormat LogFormat { get; set; } = LogFormat.Json;
	}
}
