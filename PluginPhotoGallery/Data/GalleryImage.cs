using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarrotCake.CMS.Plugins.PhotoGallery.Data;

public partial class GalleryImage {

	[Key]
	public Guid GalleryImageId { get; set; }

	public string? GalleryImageName { get; set; }

	public int? ImageOrder { get; set; }

	public Guid? GalleryId { get; set; }

	public virtual Gallery? Gallery { get; set; }
}