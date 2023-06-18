using System.Collections.Generic;
using System.Linq;
using System;
using Carrotware.Web.UI.Components;
using Microsoft.AspNetCore.Http;

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

	public class Captcha2 : BaseWebComponent, IValidateHuman {

		public Captcha2() {
			this.AltValidationFailText = "Failed to validate image";
			this.Instructions = "Select the name of the item shown in the image above from the list below.";

			this.ImageOptions = new Dictionary<string, string>();
			string imgPfx = "captcha2";

			this.ImageOptions.Add(CarrotWebHelper.GetWebResourceUrl(string.Format("{0}.bouquet.png", imgPfx)), "Bouquet");
			this.ImageOptions.Add(CarrotWebHelper.GetWebResourceUrl(string.Format("{0}.pen.png", imgPfx)), "Pen");
			this.ImageOptions.Add(CarrotWebHelper.GetWebResourceUrl(string.Format("{0}.pepper.png", imgPfx)), "Pepper");
			this.ImageOptions.Add(CarrotWebHelper.GetWebResourceUrl(string.Format("{0}.scissors.png", imgPfx)), "Scissors");
			this.ImageOptions.Add(CarrotWebHelper.GetWebResourceUrl(string.Format("{0}.snowflake.png", imgPfx)), "Snowflake");
			this.ImageOptions.Add(CarrotWebHelper.GetWebResourceUrl(string.Format("{0}.web.png", imgPfx)), "Web");
		}

		public Dictionary<string, string> ImageOptions { get; set; }

		public object ImageAttributes { get; set; }

		public override string GetHtml() {
			string val = this.SessionKeyValue.Value;

			var imgBuilder = new HtmlTag("img", GetCaptchaImageURI());
			imgBuilder.MergeAttribute("alt", val);
			imgBuilder.MergeAttribute("title", val);
			imgBuilder.MergeAttributes(this.ImageAttributes);

			return imgBuilder.RenderSelfClosingTag();
		}

		private string GetCaptchaImageURI() {
			if (this.IsWebView) {
				return this.SessionKeyValue.Key;
			} else {
				return string.Empty;
			}
		}

		public static string SessionKey {
			get {
				return "carrot_captcha2_key";
			}
		}

		public KeyValuePair<string, string> SessionKeyValue {
			get {
				string imageName = string.Empty;
				KeyValuePair<string, string> randImg = new KeyValuePair<string, string>("Fake", "Value");
				if (this.IsWebView && this.ImageOptions.Any()) {
					var rand = new Random();
					randImg = this.ImageOptions.ElementAt(rand.Next(0, this.ImageOptions.Count));

					try {
						if (CarrotWebHelper.Current.Session.GetString(SessionKey) != null) {
							imageName = CarrotWebHelper.Current.Session.GetString(SessionKey).ToString();
						} else {
							imageName = string.Format("{0}|{1}", randImg.Key, randImg.Value);
							CarrotWebHelper.Current.Session.SetString(SessionKey, imageName);
						}
					} catch {
						imageName = string.Format("{0}|{1}", randImg.Key, randImg.Value);
						CarrotWebHelper.Current.Session.SetString(SessionKey, imageName);
					}
				}

				string[] kvp = imageName.Split('|');
				randImg = new KeyValuePair<string, string>(kvp[0], kvp[1]);
				return randImg;
			}
		}

		//=============================
		public bool ValidateValue(string testValue) {
			if (string.IsNullOrEmpty(testValue)) {
				return false;
			}

			bool valid = this.SessionKeyValue.Value.ToLowerInvariant().Trim() == testValue.ToLowerInvariant().Trim();

			if (valid) {
				CarrotWebHelper.Current.Session.SetString(SessionKey, null);
			}

			return valid;
		}

		public string Instructions { get; set; }

		public string AltValidationFailText { get; set; }
	}
}