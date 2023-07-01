using CarrotCake.CMS.Plugins.PhotoGallery.Models;
using Carrotware.CMS.Interface.Controllers;

namespace CarrotCake.CMS.Plugins.PhotoGallery.Controllers {

	public class BasePublicController : BaseDataWidgetController {

		protected GalleryModel BuildModel(GallerySettings settings) {
			var model = new GalleryModel();

			if (settings != null) {
				model.GalleryId = settings.GalleryId;
				model.ShowHeading = settings.ShowHeading;
				model.ScaleImage = settings.ScaleImage;
				model.ThumbSize = settings.ThumbSize;
				model.PrettyPhotoSkin = settings.PrettyPhotoSkin;

				model.InstanceId = settings.WidgetClientID;

				GalleryHelper gh = new GalleryHelper(settings.SiteID);

				var gal = gh.GalleryGroupGetByID(model.GalleryId);

				if (gal != null) {
					model.Gallery = gal;
					model.Images = (from g in gal.GalleryImages
									where g.GalleryId == model.GalleryId
									orderby g.ImageOrder ascending
									select g).ToList();
				} else {
					model.Gallery = new GalleryGroup();
					model.Images = new List<GalleryImageEntry>();
				}
			}

			return model;
		}
	}
}