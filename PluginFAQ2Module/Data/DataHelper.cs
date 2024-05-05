using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Reflection;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2024, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: April 2024
*/

namespace CarrotCake.CMS.Plugins.FAQ2.Data {

	public static class DataHelper {

		public static IConfigurationRoot GetConfig() {
			return new ConfigurationBuilder()
					.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
					.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();
		}

		public static void Configure(string connName, DbContextOptionsBuilder optionsBuilder) {
			if (!optionsBuilder.IsConfigured) {
				IConfigurationRoot configuration = GetConfig();
				var conn = configuration.GetConnectionString(connName);

				optionsBuilder.UseSqlServer(conn);
			}
		}

		public static WebApplication MigrateDatabase(this WebApplication app) {
			using (var scope = app.Services.CreateScope()) {
				using (var ctx = scope.ServiceProvider.GetRequiredService<FaqContext>()) {
					try {
						ctx.Database.Initialize();
					} catch (Exception ex) {
						throw;
					}
				}
			}
			return app;
		}

		public static void Initialize(this DatabaseFacade database) {
			bool pending = database.GetPendingMigrations().Any();
			if (pending) {
				database.Migrate();
			}
		}

		public static string GetScript(string resource) {
			string resourceName = $"{typeof(DataHelper).Namespace}.Scripts.{resource}";
			string script = string.Empty;
			var assembly = Assembly.GetExecutingAssembly();
			using (var stream = assembly.GetManifestResourceStream(resourceName)) {
				using (var sr = new StreamReader(stream)) {
					script = sr.ReadToEnd();
				}
			}
			return script;
		}
	}
}