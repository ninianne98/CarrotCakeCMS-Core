using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace CarrotCake.CMS.Plugins.PhotoGallery.Data;

public partial class GalleryContext : DbContext {

	public GalleryContext() {
	}

	public GalleryContext(DbContextOptions<GalleryContext> options)
		: base(options) {
	}

	//================================

	public virtual DbSet<Gallery> Galleries { get; set; }

	public virtual DbSet<GalleryImage> GalleryImages { get; set; }

	public virtual DbSet<GalleryImageMetaData> GalleryImageMetaData { get; set; }

	//================================

	public static GalleryContext Create() {
		var optionsBuilder = new DbContextOptionsBuilder<GalleryContext>();

		DataHelper.Configure("CarrotwareCMS", optionsBuilder);

		return new GalleryContext(optionsBuilder.Options);
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
		DataHelper.Configure("CarrotwareCMS", optionsBuilder);
	}

	//================================

	protected override void OnModelCreating(ModelBuilder modelBuilder) {
		modelBuilder.Entity<Gallery>(entity => {
			entity.HasKey(e => e.GalleryId).HasName("tblGallery_PK");

			entity.ToTable("tblGallery");

			entity.Property(e => e.GalleryId)
				.HasDefaultValueSql("(newid())")
				.HasColumnName("GalleryID");
			entity.Property(e => e.GalleryTitle)
				.HasMaxLength(255)
				.IsUnicode(false);
			entity.Property(e => e.SiteId).HasColumnName("SiteID");
		});

		modelBuilder.Entity<GalleryImage>(entity => {
			entity.HasKey(e => e.GalleryImageId).HasName("tblGalleryImage_PK");

			entity.ToTable("tblGalleryImage");

			entity.Property(e => e.GalleryImageId)
				.HasDefaultValueSql("(newid())")
				.HasColumnName("GalleryImageID");
			entity.Property(e => e.GalleryId).HasColumnName("GalleryID");
			entity.Property(e => e.GalleryImageName)
				.HasColumnName("GalleryImage")
				.HasMaxLength(512)
				.IsUnicode(false);

			entity.HasOne(d => d.Gallery).WithMany(p => p.GalleryImages)
				.HasForeignKey(d => d.GalleryId)
				.HasConstraintName("tblGallery_tblGalleryImage_FK");
		});

		modelBuilder.Entity<GalleryImageMetaData>(entity => {
			entity.HasKey(e => e.GalleryImageMetaId).HasName("tblGalleryImageMeta_PK");

			entity.ToTable("tblGalleryImageMeta");

			entity.Property(e => e.GalleryImageMetaId)
				.HasDefaultValueSql("(newid())")
				.HasColumnName("GalleryImageMetaID");
			entity.Property(e => e.GalleryImageName)
				.HasMaxLength(512)
				.IsUnicode(false);
			entity.Property(e => e.ImageMetaData).IsUnicode(false);
			entity.Property(e => e.ImageTitle)
				.HasMaxLength(256)
				.IsUnicode(false);
			entity.Property(e => e.SiteId).HasColumnName("SiteID");
		});

		OnModelCreatingPartial(modelBuilder);
	}

	partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}