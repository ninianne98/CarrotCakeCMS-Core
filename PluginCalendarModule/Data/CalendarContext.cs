using Microsoft.EntityFrameworkCore;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

namespace CarrotCake.CMS.Plugins.CalendarModule.Data {

	public partial class CalendarContext : DbContext {

		public CalendarContext() {
		}

		public CalendarContext(DbContextOptions<CalendarContext> options)
				: base(options) {
		}

		//================================

		public static CalendarContext Create() {
			var optionsBuilder = new DbContextOptionsBuilder<CalendarContext>();

			DataHelper.Configure("CarrotwareCMS", optionsBuilder);

			return new CalendarContext(optionsBuilder.Options);
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
			DataHelper.Configure("CarrotwareCMS", optionsBuilder);
		}

		//================================

		public virtual DbSet<CalendarEntry> CalendarDates { get; set; }

		//================================

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.Entity<CalendarEntry>(entity => {
				entity.HasKey(e => e.CalendarID)
					.HasName("tblCalendar_PK");

				entity.ToTable("tblCalendar");

				entity.Property(e => e.CalendarID)
					.HasDefaultValueSql("(newid())");

				entity.Property(e => e.EventDate)
					.HasColumnType("datetime");

				entity.Property(e => e.EventDetail)
					.IsUnicode(true);

				entity.Property(e => e.EventTitle)
					.HasMaxLength(255)
					.IsUnicode(true);

				entity.Property(e => e.SiteID);
			});

			OnModelCreatingPartial(modelBuilder);
		}

		partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
	}
}