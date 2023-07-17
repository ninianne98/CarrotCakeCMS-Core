using Microsoft.AspNetCore.Html;
using System.Text.Encodings.Web;
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

namespace Carrotware.Web.UI.Components {

	public class ImageSizer : IHtmlContent {

		public ImageSizer() {
			this.ThumbSize = 150;
			this.ScaleImage = true;
			this.ImageUrl = string.Empty;
			this.Title = string.Empty;
		}

		public string HandlerURL {
			get {
				return UrlPaths.ThumbnailPath;
			}
		}

		public string ImageUrl { get; set; }

		public string ImageThumbUrl {
			get {
				string imgURL = this.ImageUrl;
				if (!imgURL.StartsWith(this.HandlerURL)) {
					imgURL = string.Format("{0}?thumb={1}&square={2}&scale={3}", this.HandlerURL, HttpUtility.UrlEncode(this.ImageUrl), this.ThumbSize, this.ScaleImage);
				}
				return imgURL;
			}
		}

		public string ThumbUrl {
			get {
				string imgURL = this.ImageUrl;
				if (!imgURL.StartsWith(this.HandlerURL)) {
					imgURL = string.Format("{0}?square={1}&scale={2}&thumb=", this.HandlerURL, this.ThumbSize, this.ScaleImage);
				}
				return imgURL;
			}
		}

		public string Title { get; set; }

		// allow alt to be different from Title, but set alt to title if not directly set
		private string _alt = null;

		public string Alt {
			get {
				if (string.IsNullOrWhiteSpace(_alt)) {
					_alt = this.Title;
				}
				return _alt;
			}
			set {
				_alt = value;
			}
		}

		public int ThumbSize { get; set; }
		public bool ScaleImage { get; set; }
		public object ImageAttributes { get; set; }

		public string ToHtmlString() {
			var imgBuilder = new HtmlTag("img", this.ImageThumbUrl);
			imgBuilder.MergeAttributes(this.ImageAttributes);
			imgBuilder.MergeAttribute("alt", this.Alt);
			imgBuilder.MergeAttribute("title", this.Title);

			return imgBuilder.RenderSelfClosingTag();
		}

		public void WriteTo(TextWriter writer, HtmlEncoder encoder) {
			writer.Write(this.ToHtmlString());
		}
	}
}