using CarrotCake.CMS.Plugins.PhotoGallery.Data;
using System;
using System.Collections.Generic;
using System.Linq;


namespace CarrotCake.CMS.Plugins.PhotoGallery {

	public class GalleryMetaData {

		public GalleryMetaData() { }

		internal GalleryMetaData(GalleryImageMetaData gal) {
			if (gal != null) {
				this.GalleryImageMetaID = gal.GalleryImageMetaId;
				this.SiteID = gal.SiteId.Value;

				this.GalleryImageName = gal.GalleryImageName;
				this.ImageTitle = gal.ImageTitle;
				this.ImageMetaData = gal.ImageMetaData;
			}
		}

		public Guid GalleryImageMetaID { get; set; }
		public Guid SiteID { get; set; }

		public string GalleryImageName { get; set; }
		public string ImageTitle { get; set; }
		public string ImageMetaData { get; set; }

		public void Save() {
			if (!String.IsNullOrEmpty(this.GalleryImageName)) {
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

		public override bool Equals(Object obj) {
			//Check for null and compare run-time types.
			if (obj == null || GetType() != obj.GetType()) return false;
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