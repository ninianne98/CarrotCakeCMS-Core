using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

namespace Carrotware.Web.UI.Components.SessionData {

	public partial class SessionContext : DbContext {
		public static string DBKey { get; set; } = string.Empty;

		public static bool DBKeyExists { get { return !string.IsNullOrEmpty(DBKey); } }

		public SessionContext() { }

		public SessionContext(DbContextOptions<SessionContext> options) : base(options) {
		}

		public static void CleanExpiredSession() {
			using (var db = new SessionContext()) {
				db.AspNetCaches.Where(c => c.ExpiresAtTime < DateTime.UtcNow.AddHours(-12)).ExecuteDelete();
			}
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
			Configure(DBKey, optionsBuilder);
		}

		internal static IConfigurationRoot GetConfig() {
			return new ConfigurationBuilder()
					.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
					.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();
		}

		internal static WebApplication MigrateDatabase(WebApplication app) {
			using (var scope = app.Services.CreateScope()) {
				using (var ctx = scope.ServiceProvider.GetRequiredService<SessionContext>()) {
					try {
						Initialize(ctx.Database);
					} catch (Exception ex) {
						throw;
					}
				}
			}
			return app;
		}

		internal static void Initialize(DatabaseFacade database) {
			bool pending = database.GetPendingMigrations().Any();
			if (pending) {
				database.Migrate();
			}
		}

		internal static void Configure(string connName, DbContextOptionsBuilder optionsBuilder) {
			if (!optionsBuilder.IsConfigured) {
				IConfigurationRoot configuration = GetConfig();
				var conn = configuration.GetConnectionString(connName);

				optionsBuilder.UseSqlServer(conn)
					.UseLazyLoadingProxies();
			}
		}

		//=========================

		public virtual DbSet<AspNetCache> AspNetCaches { get; set; } = null!;

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.Entity<AspNetCache>(entity => {
				entity.ToTable("AspNetCache");

				entity.HasIndex(e => e.ExpiresAtTime, "Index_ExpiresAtTime");

				entity.Property(e => e.Id)
					.HasMaxLength(512)
					.UseCollation("SQL_Latin1_General_CP1_CS_AS");
			});

			OnModelCreatingPartial(modelBuilder);
		}

		partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
	}
}