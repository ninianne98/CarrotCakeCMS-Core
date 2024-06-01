using System.Web;

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

	public static class CarrotFileLoggerExtensions {

		public static string LevelToString(this LogLevel logLevel) {
			switch (logLevel) {
				case LogLevel.Information:
					return "INFO";

				case LogLevel.Warning:
					return "WARN";

				case LogLevel.Critical:
					return "CRIT";
			}
			return logLevel.ToString().ToUpper();
		}

		internal static string EscapeMessage(this string message) {
			return message != null ? HttpUtility.HtmlEncode(message) : string.Empty;
		}

		internal static CarrotLoggerConfig GetCarrotLoggerConfig(this IConfigurationRoot configuration) {
			return configuration.GetSection("Logging:CarrotLogger").Get<CarrotLoggerConfig>();
		}

		public static ILoggerFactory AddCarrotFileLogger(this IServiceCollection services) {
			var configuration = services.BuildServiceProvider().GetRequiredService<IConfigurationRoot>();
			var options = configuration.GetCarrotLoggerConfig();

			if (options == null) {
				options = new CarrotLoggerConfig();
			}

			options = configuration.ConfigureDefaultLevel(options);

			var factory = services.BuildServiceProvider().GetRequiredService<ILoggerFactory>();
			factory.AddProvider(new CarrotFileLoggerProvider(options));

			return factory;
		}

		public static ILoggerFactory AddCarrotFileLogger(this IServiceCollection services, Action<CarrotLoggerConfig> configure) {
			var options = new CarrotLoggerConfig();
			configure(options);

			if (options.MinLevel == null) {
				var configuration = services.BuildServiceProvider().GetRequiredService<IConfigurationRoot>();
				options = configuration.ConfigureDefaultLevel(options);
			}

			var factory = services.BuildServiceProvider().GetRequiredService<ILoggerFactory>();
			factory.AddProvider(new CarrotFileLoggerProvider(options));

			return factory;
		}

		public static ILoggerFactory AddCarrotFileLogger(this ILoggerFactory factory, IConfigurationRoot configuration, Action<CarrotLoggerConfig> configure = null) {
			var logProvider = configuration.AddCarrotFileLogger(configure);
			if (logProvider == null) {
				return factory;
			}

			factory.AddProvider(logProvider);
			return factory;
		}

		internal static CarrotFileLoggerProvider AddCarrotFileLogger(this IConfigurationRoot configuration, Action<CarrotLoggerConfig> configure) {
			var config = configuration.GetCarrotLoggerConfig();
			if (config == null) {
				config = new CarrotLoggerConfig();
			}

			config = configuration.ConfigureDefaultLevel(config);

			var options = new CarrotLoggerConfig();

			options.MinLevel = config.MinLevel;

			options.FileDatePattern = config.FileDatePattern;
			options.FilePrefix = config.FilePrefix;
			options.FilePath = config.FilePath;

			options.LogFormat = config.LogFormat;
			options.UseUtc = config.UseUtc;
			options.MaxLogSizeKb = config.MaxLogSizeKb;

			if (configure != null) {
				configure(options);
			}

			return new CarrotFileLoggerProvider(options);
		}

		internal static CarrotLoggerConfig ConfigureDefaultLevel(this IConfigurationRoot configuration, CarrotLoggerConfig config) {
			if (config.MinLevel == null) {
				var defLevel = configuration.GetValue<string>("Logging:LogLevel:CarrotLogger");
				if (defLevel != null) {
					try {
						config.MinLevel = (LogLevel)Enum.Parse(typeof(LogLevel), defLevel, true);
					} catch { }
				} else {
					defLevel = configuration.GetValue<string>("Logging:LogLevel:Default");

					if (defLevel != null) {
						try {
							config.MinLevel = (LogLevel)Enum.Parse(typeof(LogLevel), defLevel, true);
						} catch {
							config.MinLevel = LogLevel.Warning;
#if DEBUG
							config.MinLevel = LogLevel.Trace;
#endif
						}
					}
				}
			}

			return config;
		}
	}
}
