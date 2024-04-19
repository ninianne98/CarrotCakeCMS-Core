using CarrotCake.CMS.Plugins.PhotoGallery.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CarrotCake.CMS.Plugins.PhotoGallery {

	public class GalleryHelper {

		public GalleryHelper() { }

		public GalleryHelper(Guid? siteID) {
			this.SiteID = siteID ?? Guid.Empty;
		}

		public Guid SiteID { get; set; }

		public GalleryImageEntry? GalleryImageEntryGetByID(Guid galleryImageID) {
			using (GalleryContext db = new GalleryContext()) {
				var ge = (from c in db.GalleryImages
						  where c.GalleryImageId == galleryImageID
						  select new GalleryImageEntry(c)).FirstOrDefault();

				return ge;
			}
		}

		public List<GalleryImageEntry> GalleryImageEntryListGetByGalleryId(Guid galleryID) {
			using (GalleryContext db = new GalleryContext()) {
				List<GalleryImageEntry> ge = (from c in db.GalleryImages
											  where c.GalleryId == galleryID
											  select new GalleryImageEntry(c)).ToList();

				return ge;
			}
		}

		public GalleryImageEntry? GalleryImageEntryGetByFilename(Guid galleryID, string galleryImage) {
			using (GalleryContext db = new GalleryContext()) {
				var ge = (from c in db.GalleryImages
						  where c.GalleryId == galleryID
						  && c.GalleryImageName.ToLower() == galleryImage.ToLower()
						  orderby c.ImageOrder ascending
						  select new GalleryImageEntry(c)).FirstOrDefault();

				return ge;
			}
		}

		public void GalleryImageCleanup(Guid galleryID, List<string> lst) {
			using (GalleryContext db = new GalleryContext()) {
				db.GalleryImages.Where(g => g.GalleryId == galleryID
									 && !lst.Contains(g.GalleryImageName.ToLower())).ExecuteDelete();

				db.SaveChanges();
			}
		}

		public List<GalleryMetaData> GetGalleryMetaDataListByGalleryId(Guid galleryID) {
			using (GalleryContext db = new GalleryContext()) {
				List<GalleryMetaData> imageData = (from g in db.GalleryImageMetaData
												   join gg in db.GalleryImages on g.GalleryImageName.ToLower() equals gg.GalleryImageName.ToLower()
												   where g.SiteId == this.SiteID
													   && gg.GalleryId == galleryID
												   select new GalleryMetaData(g)).ToList();

				return imageData;
			}
		}

		public GalleryGroup? GalleryGroupGetByID(Guid galleryID) {
			using (GalleryContext db = new GalleryContext()) {
				var ge = (from c in db.Galleries
						  where c.SiteId == this.SiteID
						  && c.GalleryId == galleryID
						  select new GalleryGroup(c)).FirstOrDefault();

				return ge;
			}
		}

		public GalleryGroup? GalleryGroupGetByName(string galleryTitle) {
			GalleryGroup? ge = null;

			using (GalleryContext db = new GalleryContext()) {
				if (!string.IsNullOrEmpty(galleryTitle)) {
					ge = (from c in db.Galleries
						  where c.SiteId == this.SiteID
						  && c.GalleryTitle.ToLower() == galleryTitle.ToLower()
						  select new GalleryGroup(c)).FirstOrDefault();
				}
			}

			return ge;
		}

		public List<GalleryGroup> GalleryGroupListGetBySiteID() {
			using (GalleryContext db = new GalleryContext()) {
				List<GalleryGroup> ge = (from c in db.Galleries
										 where c.SiteId == this.SiteID
										 select new GalleryGroup(c)).ToList();

				return ge;
			}
		}

		public GalleryMetaData? GalleryMetaDataGetByFilename(string galleryImage) {
			GalleryMetaData? ge = null;

			using (GalleryContext db = new GalleryContext()) {
				if (!string.IsNullOrEmpty(galleryImage)) {
					ge = (from c in db.GalleryImageMetaData
						  where c.SiteId == this.SiteID
						  && c.GalleryImageName.ToLower() == galleryImage.ToLower()
						  select new GalleryMetaData(c)).FirstOrDefault();
				}
			}

			return ge;
		}

		public GalleryMetaData? GalleryMetaDataGetByID(Guid galleryImageMetaID) {
			using (GalleryContext db = new GalleryContext()) {
				var ge = (from c in db.GalleryImageMetaData
						  where c.SiteId == this.SiteID
						  && c.GalleryImageMetaId == galleryImageMetaID
						  select new GalleryMetaData(c)).FirstOrDefault();

				return ge;
			}
		}

		public static string ReadEmbededScript(string resouceName) {
			string ret = null;

			Assembly _assembly = Assembly.GetExecutingAssembly();
			using (var stream = new StreamReader(_assembly.GetManifestResourceStream(resouceName))) {
				ret = stream.ReadToEnd();
			}

			return ret;
		}
	}
}