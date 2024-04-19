using Carrotware.CMS.Data.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Data;
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

namespace Carrotware.CMS.Data {

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

				optionsBuilder.UseSqlServer(conn)
					.UseLazyLoadingProxies();
			}
		}

		public static WebApplication MigrateDatabase(this WebApplication app) {
			using (var scope = app.Services.CreateScope()) {
				using (var ctx = scope.ServiceProvider.GetRequiredService<CarrotCakeContext>()) {
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

		public static DataSet ExecDataSet(string connName, string queryText) {
			return ExecDataSet(connName, queryText, null);
		}

		public static DataSet ExecDataSet(string connName, string queryText, List<SqlParameter>? parms) {
			IConfigurationRoot configuration = GetConfig();
			var conn = configuration.GetConnectionString(connName);

			var ds = new DataSet();

			if (parms == null) {
				parms = new List<SqlParameter>();
			}

			using (SqlConnection cn = new SqlConnection(conn)) {
				using (SqlCommand cmd = new SqlCommand(queryText, cn)) {
					cn.Open();
					cmd.CommandType = CommandType.Text;

					if (parms != null) {
						foreach (var p in parms) {
							cmd.Parameters.Add(p);
						}
					}

					using (SqlDataAdapter da = new SqlDataAdapter(cmd)) {
						da.Fill(ds);
					}
				}
				cn.Close();
			}

			return ds;
		}
	}
}