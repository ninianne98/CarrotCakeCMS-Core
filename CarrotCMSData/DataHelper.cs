﻿using Carrotware.CMS.Data.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
					.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
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
	}
}