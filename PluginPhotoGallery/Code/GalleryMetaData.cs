using CarrotCake.CMS.Plugins.PhotoGallery.Data;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

namespace CarrotCake.CMS.Plugins.PhotoGallery {

	public class GalleryMetaData {

		public GalleryMetaData() { }

		internal GalleryMetaData(GalleryImageMetaData gal) {
			if (gal != null) {
				this.GalleryImageMetaID = gal.GalleryImageMetaId;
				this.SiteID = gal.SiteId.Value;

				this.GalleryImageName = gal.GalleryImageName ?? string.Empty;
				this.ImageTitle = gal.ImageTitle ?? string.Empty;
				this.ImageMetaData = gal.ImageMetaData;

				this.ValidateGalleryImage();
			}
		}

		public Guid GalleryImageMetaID { get; set; }
		public Guid SiteID { get; set; }

		public string GalleryImageName { get; set; }
		public string ImageTitle { get; set; }
		public string? ImageMetaData { get; set; }

		public void ValidateGalleryImage() {
			if (string.IsNullOrWhiteSpace(this.GalleryImageName)) {
				throw new Exception("Image path must be provided.");
			}
			if (!string.IsNullOrEmpty(this.GalleryImageName)) {
				if (this.GalleryImageName.Contains("../") || this.GalleryImageName.Contains(@"..\")) {
					throw new Exception("Cannot use relative paths.");
				}
				if (this.GalleryImageName.Contains(":")) {
					throw new Exception("Cannot specify drive letters or other protocols.");
				}
				if (this.GalleryImageName.Contains("//") || this.GalleryImageName.Contains(@"\\")) {
					throw new Exception("Cannot use UNC paths.");
				}
				if (this.GalleryImageName.Contains("<") || this.GalleryImageName.Contains(">")) {
					throw new Exception("Cannot include html tags.");
				}
			}
		}

		public void Save() {
			if (!string.IsNullOrEmpty(this.GalleryImageName)) {
				this.ValidateGalleryImage();

				using (GalleryContext db = new GalleryContext()) {
					var gal = (from c in db.GalleryImageMetaData
							   where c.GalleryImageName.ToLower() == this.GalleryImageName.ToLower()
							   select c).FirstOrDefault();

					if (gal == null || this.GalleryImageMetaID == Guid.Empty) {
						gal = new GalleryImageMetaData();
						gal.SiteId = this.SiteID;
						gal.GalleryImageMetaId = Guid.NewGuid();
						gal.GalleryImageName = this.GalleryImageName;
					}

					gal.ImageTitle = this.ImageTitle;
					gal.ImageMetaData = this.ImageMetaData;

					if (gal.GalleryImageMetaId != this.GalleryImageMetaID) {
						db.GalleryImageMetaData.Add(gal);
					}

					db.SaveChanges();

					this.GalleryImageMetaID = gal.GalleryImageMetaId;
				}
			}
		}

		public override string ToString() {
			return this.ImageTitle;
		}

		public override bool Equals(object? obj) {
			//Check for null and compare run-time types.
			if (obj == null || this.GetType() != obj.GetType()) return false;
			if (obj is GalleryMetaData) {
				GalleryMetaData p = (GalleryMetaData)obj;
				return (this.GalleryImageMetaID == p.GalleryImageMetaID)
						&& (this.SiteID == p.SiteID)
						&& (this.ImageTitle == p.ImageTitle);
			} else {
				return false;
			}
		}

		public override int GetHashCode() {
			return this.GalleryImageMetaID.GetHashCode() ^ this.SiteID.GetHashCode() ^ this.ImageTitle.GetHashCode();
		}
	}
}