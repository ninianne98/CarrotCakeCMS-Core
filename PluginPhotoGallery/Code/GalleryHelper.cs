using CarrotCake.CMS.Plugins.PhotoGallery.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CarrotCake.CMS.Plugins.PhotoGallery {

	public class GalleryHelper : IDisposable {
		private GalleryContext _db;
		public Guid SiteID { get; set; }

		public GalleryHelper(Guid? siteID) {
			this.SiteID = siteID ?? Guid.Empty;
			_db = new GalleryContext();
		}

		public GalleryImageEntry? GalleryImageEntryGetByID(Guid galleryImageID) {
			return (from c in _db.GalleryImages
					where c.GalleryImageId == galleryImageID
					select new GalleryImageEntry(c)).FirstOrDefault();
		}

		public List<GalleryImageEntry> GalleryImageEntryListGetByGalleryId(Guid galleryID) {
			return (from c in _db.GalleryImages
					where c.GalleryId == galleryID
					select new GalleryImageEntry(c)).ToList();
		}

		public GalleryImageEntry? GalleryImageEntryGetByFilename(Guid galleryID, string galleryImage) {
			return (from c in _db.GalleryImages
					where c.GalleryId == galleryID
					&& c.GalleryImageName.ToLower() == galleryImage.ToLower()
					orderby c.ImageOrder ascending
					select new GalleryImageEntry(c)).FirstOrDefault();
		}

		public void GalleryImageCleanup(Guid galleryID, List<string> lst) {
			_db.GalleryImages.Where(g => g.GalleryId == galleryID
								 && !lst.Contains(g.GalleryImageName.ToLower())).ExecuteDelete();

			_db.SaveChanges();
		}

		public List<GalleryMetaData> GetGalleryMetaDataListByGalleryId(Guid galleryID) {
			return (from g in _db.GalleryImageMetaData
					join gg in _db.GalleryImages on g.GalleryImageName.ToLower() equals gg.GalleryImageName.ToLower()
					where g.SiteId == this.SiteID
						&& gg.GalleryId == galleryID
					select new GalleryMetaData(g)).ToList();
		}

		public GalleryGroup? GalleryGroupGetByID(Guid galleryID) {
			return (from c in _db.Galleries
					where c.SiteId == this.SiteID
					&& c.GalleryId == galleryID
					select new GalleryGroup(c)).FirstOrDefault();
		}

		public GalleryGroup? GalleryGroupGetByName(string galleryTitle) {
			GalleryGroup? ge = null;

			if (!string.IsNullOrEmpty(galleryTitle)) {
				ge = (from c in _db.Galleries
					  where c.SiteId == this.SiteID
					  && c.GalleryTitle.ToLower() == galleryTitle.ToLower()
					  select new GalleryGroup(c)).FirstOrDefault();
			}

			return ge;
		}

		public IQueryable<Gallery> GalleryGroupListGetBySiteID() {
			return (from c in _db.Galleries
					where c.SiteId == this.SiteID
					select c);
		}

		public int GalleryGroupListGetBySiteIDCount() {
			return GalleryGroupListGetBySiteID().Count();
		}

		public GalleryMetaData? GalleryMetaDataGetByFilename(string galleryImage) {
			GalleryMetaData? ge = null;

			if (!string.IsNullOrEmpty(galleryImage)) {
				ge = (from c in _db.GalleryImageMetaData
					  where c.SiteId == this.SiteID
					  && c.GalleryImageName.ToLower() == galleryImage.ToLower()
					  select new GalleryMetaData(c)).FirstOrDefault();
			}

			return ge;
		}

		public GalleryMetaData? GalleryMetaDataGetByID(Guid galleryImageMetaID) {
			return (from c in _db.GalleryImageMetaData
					where c.SiteId == this.SiteID
					&& c.GalleryImageMetaId == galleryImageMetaID
					select new GalleryMetaData(c)).FirstOrDefault();
		}

		public void Dispose() {
			if (_db != null) {
				_db.Dispose();
			}
		}

		public static string ReadEmbededScript(string resouceName) {
			string ret = null;

			Assembly assembly = Assembly.GetExecutingAssembly();
			using (var stream = new StreamReader(assembly.GetManifestResourceStream(resouceName))) {
				ret = stream.ReadToEnd();
			}

			return ret;
		}
	}
}