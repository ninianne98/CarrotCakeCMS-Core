using CarrotCake.CMS.Plugins.PhotoGallery.Data;
using System;
using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CarrotCake.CMS.Plugins.PhotoGallery {

	public class GalleryImageEntry {

		public GalleryImageEntry() { }

		internal GalleryImageEntry(GalleryImage gal) {
			if (gal != null) {
				this.GalleryId = gal.GalleryId.Value;
				this.GalleryImageID = gal.GalleryImageId;

				this.GalleryImage = gal.GalleryImageName;
				this.ImageOrder = gal.ImageOrder.Value;
			}
		}

		[Key]
		public Guid GalleryId { get; set; }

		public Guid GalleryImageID { get; set; }

		[Required]
		public string GalleryImage { get; set; }

		public int ImageOrder { get; set; }

		public void Save() {
			using (GalleryContext db = new GalleryContext()) {
				GalleryImage gal = (from c in db.GalleryImages
									   where c.GalleryImageId == this.GalleryImageID
									   select c).FirstOrDefault();

				if (gal == null || this.GalleryId == Guid.Empty) {
					gal = new GalleryImage();
					gal.GalleryId = this.GalleryId;
					gal.GalleryImageId = Guid.NewGuid();
				}

				gal.GalleryImageName = this.GalleryImage;
				gal.ImageOrder = this.ImageOrder;

				if (gal.GalleryImageId != this.GalleryImageID) {
					db.GalleryImages.Add(gal);
				}

				db.SaveChanges();

				this.GalleryImageID = gal.GalleryImageId;
			}
		}

		public override string ToString() {
			return GalleryImage;
		}

		public override bool Equals(Object obj) {
			//Check for null and compare run-time types.
			if (obj == null || GetType() != obj.GetType()) return false;
			if (obj is GalleryImageEntry) {
				GalleryImageEntry p = (GalleryImageEntry)obj;
				return (this.GalleryImageID == p.GalleryImageID)
						&& (this.GalleryId == p.GalleryId)
						&& (this.GalleryImage == p.GalleryImage);
			} else {
				return false;
			}
		}

		public override int GetHashCode() {
			return GalleryImageID.GetHashCode() ^ GalleryId.GetHashCode() ^ GalleryImage.GetHashCode();
		}
	}
}