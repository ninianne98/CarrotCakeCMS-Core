using Microsoft.EntityFrameworkCore;

namespace CarrotCake.CMS.Plugins.EventCalendarModule.Data {

	public partial class CalendarContext : DbContext {

		public CalendarContext() {
		}

		public CalendarContext(DbContextOptions<CalendarContext> options)
		: base(options) {
		}

		//================================

		public static CalendarContext GetDataContext() {
			var optionsBuilder = new DbContextOptionsBuilder<CalendarContext>();

			DataHelper.Configure("CarrotwareCMS", optionsBuilder);

			return new CalendarContext(optionsBuilder.Options);
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
			DataHelper.Configure("CarrotwareCMS", optionsBuilder);
		}

		//================================

		public virtual DbSet<CalendarSingleEvent> CalendarEvents { get; set; } = null!;
		public virtual DbSet<CalendarEventCategory> CalendarEventCategories { get; set; } = null!;
		public virtual DbSet<CalendarEventProfile> CalendarEventProfiles { get; set; } = null!;
		public virtual DbSet<CalendarFrequency> CalendarFrequencies { get; set; } = null!;
		public virtual DbSet<ViewCalendarEvent> ViewCalendarEvents { get; set; } = null!;
		public virtual DbSet<ViewCalendarEventProfile> ViewCalendarEventProfiles { get; set; } = null!;

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.Entity<CalendarSingleEvent>(entity => {
				entity.HasKey(e => e.CalendarEventId);

				entity.ToTable("carrot_CalendarEvent");

				entity.Property(e => e.CalendarEventId)
					.HasColumnName("CalendarEventID")
					.HasDefaultValueSql("(newid())");

				entity.Property(e => e.CalendarEventProfileId).HasColumnName("CalendarEventProfileID");

				entity.Property(e => e.EventDate).HasColumnType("datetime");

				entity.Property(e => e.EventDetail).IsUnicode(false);

				entity.HasOne(d => d.CalendarEventProfile)
					.WithMany(p => p.CalendarEvents)
					.HasForeignKey(d => d.CalendarEventProfileId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_carrot_CalendarEvent_carrot_CalendarEventProfile");
			});

			modelBuilder.Entity<CalendarEventCategory>(entity => {
				entity.HasKey(e => e.CalendarEventCategoryId);

				entity.ToTable("carrot_CalendarEventCategory");

				entity.Property(e => e.CalendarEventCategoryId)
					.HasColumnName("CalendarEventCategoryID")
					.HasDefaultValueSql("(newid())");

				entity.Property(e => e.CategoryBGColor)
					.HasMaxLength(32)
					.IsUnicode(false)
					.HasColumnName("CategoryBGColor");

				entity.Property(e => e.CategoryFGColor)
					.HasMaxLength(32)
					.IsUnicode(false)
					.HasColumnName("CategoryFGColor");

				entity.Property(e => e.CategoryName)
					.HasMaxLength(128)
					.IsUnicode(false);

				entity.Property(e => e.SiteID).HasColumnName("SiteID");
			});

			modelBuilder.Entity<CalendarEventProfile>(entity => {
				entity.HasKey(e => e.CalendarEventProfileId);

				entity.ToTable("carrot_CalendarEventProfile");

				entity.Property(e => e.CalendarEventProfileId)
					.HasColumnName("CalendarEventProfileID")
					.HasDefaultValueSql("(newid())");

				entity.Property(e => e.CalendarEventCategoryId).HasColumnName("CalendarEventCategoryID");

				entity.Property(e => e.CalendarFrequencyId).HasColumnName("CalendarFrequencyID");

				entity.Property(e => e.EventDetail).IsUnicode(false);

				entity.Property(e => e.EventEndDate).HasColumnType("datetime");

				entity.Property(e => e.EventStartDate).HasColumnType("datetime");

				entity.Property(e => e.EventTitle)
					.HasMaxLength(256)
					.IsUnicode(false);

				entity.Property(e => e.SiteID).HasColumnName("SiteID");

				entity.HasOne(d => d.CalendarEventCategory)
					.WithMany(p => p.CalendarEventProfiles)
					.HasForeignKey(d => d.CalendarEventCategoryId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_carrot_CalendarEventProfile_carrot_CalendarEventCategory");

				entity.HasOne(d => d.CalendarFrequency)
					.WithMany(p => p.CalendarEventProfiles)
					.HasForeignKey(d => d.CalendarFrequencyId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_carrot_CalendarEventProfile_carrot_CalendarFrequency");
			});

			modelBuilder.Entity<CalendarFrequency>(entity => {
				entity.HasKey(e => e.CalendarFrequencyId);

				entity.ToTable("carrot_CalendarFrequency");

				entity.Property(e => e.CalendarFrequencyId)
					.HasColumnName("CalendarFrequencyID")
					.HasDefaultValueSql("(newid())");

				entity.Property(e => e.FrequencyName)
					.HasMaxLength(128)
					.IsUnicode(false);

				entity.Property(e => e.FrequencyValue)
					.HasMaxLength(64)
					.IsUnicode(false);
			});

			modelBuilder.Entity<ViewCalendarEvent>(entity => {
				entity.HasNoKey();

				entity.ToView("vw_carrot_CalendarEvent");

				entity.Property(e => e.CalendarEventCategoryId).HasColumnName("CalendarEventCategoryID");

				entity.Property(e => e.CalendarEventId).HasColumnName("CalendarEventID");

				entity.Property(e => e.CalendarEventProfileId).HasColumnName("CalendarEventProfileID");

				entity.Property(e => e.CalendarFrequencyId).HasColumnName("CalendarFrequencyID");

				entity.Property(e => e.CategoryBGColor)
					.HasMaxLength(32)
					.IsUnicode(false)
					.HasColumnName("CategoryBGColor");

				entity.Property(e => e.CategoryFGColor)
					.HasMaxLength(32)
					.IsUnicode(false)
					.HasColumnName("CategoryFGColor");

				entity.Property(e => e.CategoryName)
					.HasMaxLength(128)
					.IsUnicode(false);

				entity.Property(e => e.EventDate).HasColumnType("datetime");

				entity.Property(e => e.EventDetail).IsUnicode(false);

				entity.Property(e => e.EventEndDate).HasColumnType("datetime");

				entity.Property(e => e.EventSeriesDetail).IsUnicode(false);

				entity.Property(e => e.EventStartDate).HasColumnType("datetime");

				entity.Property(e => e.EventTitle)
					.HasMaxLength(256)
					.IsUnicode(false);

				entity.Property(e => e.FrequencyName)
					.HasMaxLength(128)
					.IsUnicode(false);

				entity.Property(e => e.FrequencyValue)
					.HasMaxLength(64)
					.IsUnicode(false);

				entity.Property(e => e.SiteID).HasColumnName("SiteID");
			});

			modelBuilder.Entity<ViewCalendarEventProfile>(entity => {
				entity.HasNoKey();

				entity.ToView("vw_carrot_CalendarEventProfile");

				entity.Property(e => e.CalendarEventCategoryId).HasColumnName("CalendarEventCategoryID");

				entity.Property(e => e.CalendarEventProfileId).HasColumnName("CalendarEventProfileID");

				entity.Property(e => e.CalendarFrequencyId).HasColumnName("CalendarFrequencyID");

				entity.Property(e => e.CategoryBGColor)
					.HasMaxLength(32)
					.IsUnicode(false)
					.HasColumnName("CategoryBGColor");

				entity.Property(e => e.CategoryFGColor)
					.HasMaxLength(32)
					.IsUnicode(false)
					.HasColumnName("CategoryFGColor");

				entity.Property(e => e.CategoryName)
					.HasMaxLength(128)
					.IsUnicode(false);

				entity.Property(e => e.EventDetail).IsUnicode(false);

				entity.Property(e => e.EventEndDate).HasColumnType("datetime");

				entity.Property(e => e.EventStartDate).HasColumnType("datetime");

				entity.Property(e => e.EventTitle)
					.HasMaxLength(256)
					.IsUnicode(false);

				entity.Property(e => e.FrequencyName)
					.HasMaxLength(128)
					.IsUnicode(false);

				entity.Property(e => e.FrequencyValue)
					.HasMaxLength(64)
					.IsUnicode(false);

				entity.Property(e => e.SiteID).HasColumnName("SiteID");
			});

			OnModelCreatingPartial(modelBuilder);
		}

		partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
	}
}