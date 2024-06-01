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

	public interface ICarrotLogMessage {
		DateTime DateTime { get; set; }
		EventId EventId { get; set; }
		string? Exception { get; set; }
		string LogLevel { get; set; }
		string LogName { get; set; }
		string Message { get; set; }

		string ToJson();

		string ToTabbed();
	}
}
