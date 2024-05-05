using Microsoft.EntityFrameworkCore;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2024, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: April 2024
*/

namespace CarrotCake.CMS.Plugins.FAQ2.Data;

/*
dotnet ef dbcontext scaffold "Data Source=.\SQL2016EXPRESS;Database=CarrotwareCMS;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=False;" Microsoft.EntityFrameworkCore.SqlServer --context FaqContext -o PluginFAQ2Module/Data --table carrot_FaqItem  --table carrot_FaqCategory
dotnet ef migrations add InitialFaq2 --context FaqContext --output-dir PluginFAQ2Module/Data
 */

public partial class FaqContext : DbContext {
	public FaqContext() {
	}


	public FaqContext(DbContextOptions<FaqContext> options)
	: base(options) {
	}

	//================================

	public static FaqContext GetDataContext() {
		var optionsBuilder = new DbContextOptionsBuilder<FaqContext>();

		DataHelper.Configure("CarrotwareCMS", optionsBuilder);

		return new FaqContext(optionsBuilder.Options);
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
		DataHelper.Configure("CarrotwareCMS", optionsBuilder);
	}

	//================================

	public virtual DbSet<CarrotFaqCategory> CarrotFaqCategories { get; set; }

	public virtual DbSet<CarrotFaqItem> CarrotFaqItems { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder) {
		modelBuilder.Entity<CarrotFaqCategory>(entity => {
			entity.HasKey(e => e.FaqCategoryId).HasName("carrot_FaqCategory_PK");

			entity.ToTable("carrot_FaqCategory");

			entity.Property(e => e.FaqCategoryId)
				.HasDefaultValueSql("(newid())")
				.HasColumnName("FaqCategoryID");
			entity.Property(e => e.FaqTitle)
				.HasMaxLength(255)
				.IsUnicode(false)
				.HasColumnName("FAQTitle");
			entity.Property(e => e.SiteId).HasColumnName("SiteID");
		});

		modelBuilder.Entity<CarrotFaqItem>(entity => {
			entity.HasKey(e => e.FaqItemId).HasName("carrot_FaqItem_PK");

			entity.ToTable("carrot_FaqItem");

			entity.Property(e => e.FaqItemId)
				.HasDefaultValueSql("(newid())")
				.HasColumnName("FaqItemID");
			entity.Property(e => e.Answer).IsUnicode(false);
			entity.Property(e => e.Caption)
				.HasMaxLength(128)
				.IsUnicode(false);
			entity.Property(e => e.FaqCategoryId).HasColumnName("FaqCategoryID");
			entity.Property(e => e.IsActive)
				.IsRequired()
				.HasDefaultValueSql("((1))");
			entity.Property(e => e.ItemOrder).HasDefaultValueSql("((1))");
			entity.Property(e => e.Question).IsUnicode(false);

			entity.HasOne(d => d.FaqCategory).WithMany(p => p.CarrotFaqItems)
				.HasForeignKey(d => d.FaqCategoryId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("carrot_FaqCategory_carrot_FaqItem_FK");
		});

		OnModelCreatingPartial(modelBuilder);
	}

	partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
