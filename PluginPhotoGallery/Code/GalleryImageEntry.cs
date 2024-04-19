using CarrotCake.CMS.Plugins.PhotoGallery.Data;
using System.ComponentModel.DataAnnotations;
using System.Web;

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

	public class GalleryImageEntry {

		public GalleryImageEntry() { }

		internal GalleryImageEntry(GalleryImage gal) {
			if (gal != null) {
				this.GalleryId = gal.GalleryId;
				this.GalleryImageId = gal.GalleryImageId;

				this.GalleryImageName = gal.GalleryImageName ?? string.Empty;
				this.ImageOrder = gal.ImageOrder ?? 0;

				this.ValidateGalleryImage();
			}
		}

		[Key]
		public Guid? GalleryId { get; set; }

		public Guid GalleryImageId { get; set; }

		[Required]
		public string GalleryImageName { get; set; }

		public int ImageOrder { get; set; }

		public void ValidateGalleryImage() {
			if (!string.IsNullOrEmpty(this.GalleryImageName)) {
				if (this.GalleryImageName.Contains("../") || this.GalleryImageName.Contains(@"..\")) {
					throw new Exception("Cannot use relative paths.");
				}
				if (this.GalleryImageName.Contains(":")) {
					throw new Exception("Cannot specify drive letters.");
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
			using (var db = new GalleryContext()) {
				var gal = (from c in db.GalleryImages
						   where c.GalleryImageId == this.GalleryImageId
						   select c).FirstOrDefault();

				if (gal == null || this.GalleryId == Guid.Empty) {
					gal = new GalleryImage();
					gal.GalleryId = this.GalleryId;
					gal.GalleryImageId = Guid.NewGuid();
				}

				gal.GalleryImageName = this.GalleryImageName;
				gal.ImageOrder = this.ImageOrder;

				if (gal.GalleryImageId != this.GalleryImageId) {
					db.GalleryImages.Add(gal);
				}

				db.SaveChanges();

				this.GalleryImageId = gal.GalleryImageId;
			}
		}

		public override string ToString() {
			return HttpUtility.HtmlEncode(this.GalleryImageName ?? string.Empty);
		}

		public override bool Equals(object? obj) {
			//Check for null and compare run-time types.
			if (obj == null || this.GetType() != obj.GetType()) return false;
			if (obj is GalleryImageEntry) {
				GalleryImageEntry p = (GalleryImageEntry)obj;
				return (this.GalleryImageId == p.GalleryImageId)
						&& (this.GalleryId == p.GalleryId)
						&& (this.GalleryImageName == p.GalleryImageName);
			} else {
				return false;
			}
		}

		public override int GetHashCode() {
			return this.GalleryImageId.GetHashCode() ^ this.GalleryId.GetHashCode() ^ this.GalleryImageName.GetHashCode();
		}
	}
}