using System.ComponentModel.DataAnnotations;

namespace CarrotCake.CMS.Plugins.PhotoGallery.Data;

public partial class Gallery {

	[Key]
	public Guid GalleryId { get; set; }

	public string? GalleryTitle { get; set; }

	public Guid? SiteId { get; set; }

	public virtual ICollection<GalleryImage> GalleryImages { get; set; } = new List<GalleryImage>();
}