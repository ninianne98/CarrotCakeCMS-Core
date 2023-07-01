using CarrotCake.CMS.Plugins.PhotoGallery.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CarrotCake.CMS.Plugins.PhotoGallery {

	public class GalleryGroup {

		public GalleryGroup() { }

		internal GalleryGroup(Gallery gal) {
			if (gal != null) {
				this.GalleryId = gal.GalleryId;
				this.SiteID = gal.SiteId.Value;

				this.GalleryTitle = gal.GalleryTitle;

				GalleryHelper gh = new GalleryHelper(SiteID);

				this.GalleryImages = gh.GalleryImageEntryListGetByGalleryId(this.GalleryId);
			}
		}

		[Key]
		[Display(Name = "ID")]
		public Guid GalleryId { get; set; }

		[Display(Name = "Site ID")]
		public Guid SiteID { get; set; }

		[Required]
		[Display(Name = "Gallery Title")]
		public string GalleryTitle { get; set; }

		[Display(Name = "Images")]
		public List<GalleryImageEntry> GalleryImages { get; set; }

		public void Save() {
			using (GalleryContext db = new GalleryContext()) {
				Gallery gal = (from c in db.Galleries
							   where c.GalleryId == this.GalleryId
							   select c).FirstOrDefault();

				if (gal == null || this.GalleryId == Guid.Empty) {
					gal = new Gallery();
					gal.SiteId = this.SiteID;
					gal.GalleryId = Guid.NewGuid();
				}

				gal.GalleryTitle = this.GalleryTitle;

				if (gal.GalleryId != this.GalleryId) {
					db.Galleries.Add(gal);
				}

				db.SaveChanges();

				this.GalleryId = gal.GalleryId;
			}
		}

		public override string ToString() {
			return GalleryTitle;
		}

		public override bool Equals(Object obj) {
			//Check for null and compare run-time types.
			if (obj == null || GetType() != obj.GetType()) return false;
			if (obj is GalleryGroup) {
				GalleryGroup p = (GalleryGroup)obj;
				return (this.GalleryId == p.GalleryId)
						&& (this.SiteID == p.SiteID)
						&& (this.GalleryTitle == p.GalleryTitle);
			} else {
				return false;
			}
		}

		public override int GetHashCode() {
			return GalleryId.GetHashCode() ^ SiteID.GetHashCode() ^ GalleryTitle.GetHashCode();
		}
	}
}