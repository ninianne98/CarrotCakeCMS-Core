using System.ComponentModel.DataAnnotations;

namespace CarrotCake.CMS.Plugins.PhotoGallery.Data;

public partial class GalleryImageMetaData {

	[Key]
	public Guid GalleryImageMetaId { get; set; }

	public string? GalleryImageName { get; set; }

	public string? ImageTitle { get; set; }

	public string? ImageMetaData { get; set; }

	public Guid? SiteId { get; set; }
}