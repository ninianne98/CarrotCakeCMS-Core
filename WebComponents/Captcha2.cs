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

			this.ImageOptions = GetImages();
		}

		public Dictionary<string, string> ImageOptions { get; set; }

		internal static Dictionary<string, string> _imageOptions = new Dictionary<string, string>();

		internal static Dictionary<string, string> GetImages() {
			if (_imageOptions == null) {
				_imageOptions = new Dictionary<string, string>();
			}

			if (_imageOptions.Count < 1) {
				_imageOptions.Add(CreateImageResource("bell.png"), "Bell");
				_imageOptions.Add(CreateImageResource("book.png"), "Book");
				_imageOptions.Add(CreateImageResource("bouquet.png"), "Bouquet");
				_imageOptions.Add(CreateImageResource("candle.png"), "Candle");
				_imageOptions.Add(CreateImageResource("flag.png"), "Flag");
				_imageOptions.Add(CreateImageResource("flower.png"), "Flower");
				_imageOptions.Add(CreateImageResource("pen.png"), "Pen");
				_imageOptions.Add(CreateImageResource("pepper.png"), "Pepper");
				_imageOptions.Add(CreateImageResource("scissors.png"), "Scissors");
				_imageOptions.Add(CreateImageResource("snowflake.png"), "Snowflake");
				_imageOptions.Add(CreateImageResource("web.png"), "Web");
			}

			return _imageOptions.OrderBy(x => Guid.NewGuid()).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
		}

		internal static string CreateImageResource(string imgName) {
			return CarrotWebHelper.GetWebResourceUrl(string.Format("captcha2.{0}", imgName));
		}

		public object? ImageAttributes { get; set; }

		public override string GetHtml() {
			string val = this.SessionKeyValue.Value;

			var imgBuilder = new HtmlTag("img", GetCaptchaImageURI());
			imgBuilder.MergeAttribute("alt", val);
			imgBuilder.MergeAttribute("title", val);
			if (this.ImageAttributes != null) {
				imgBuilder.MergeAttributes(this.ImageAttributes);
			}

			return imgBuilder.RenderSelfClosingTag();
		}

		private string GetCaptchaImageURI() {
			return this.SessionKeyValue.Key;
		}

		public static string SessionKey {
			get {
				return "carrot_captcha2_key";
			}
		}

		internal static KeyValuePair<string, string> GetNewChallengeImage() {
			var choiceKey = SessionKey + "_choices";
			var images = GetImages();
			var ct = images.Count();

			int max = 1;
			if (ct >= 4) {
				max = (max < ct - 4) ? (ct - 4) : max;
			}
			var options = new List<string>();

			if (CarrotWebHelper.HttpContext.Session.GetString(choiceKey) != null) {
				var tmpOpt = CarrotWebHelper.HttpContext.Session.GetString(choiceKey).ToString();
				if (string.IsNullOrEmpty(tmpOpt) == false) {
					tmpOpt = tmpOpt.DecodeBase64();
				}
				options = tmpOpt.Split('|').ToList();
			} else {
				CarrotWebHelper.HttpContext.Session.SetString(choiceKey, string.Empty);
			}

			// logic to prevent the same image mult times in a row by saving the last few presented images
			var randImg = images.Where(x => options.Contains(x.Value) == false)
								.OrderBy(x => Guid.NewGuid()).First();

			var tmpLst = new string[] { randImg.Value }.Union(options).Take(max).ToList();

			CarrotWebHelper.HttpContext.Session.SetString(choiceKey, string.Join("|", tmpLst).EncodeBase64());

			return randImg;
		}

		public KeyValuePair<string, string> SessionKeyValue {
			get {
				var imageName = string.Empty;
				var randImg = new KeyValuePair<string, string>("Fake", "Value");

				if (this.ImageOptions.Any()) {
					try {
						if (CarrotWebHelper.HttpContext.Session.GetString(SessionKey) != null) {
							imageName = CarrotWebHelper.HttpContext.Session.GetString(SessionKey) ?? string.Empty;

							string[] kvp = imageName.Split('|');
							randImg = new KeyValuePair<string, string>(kvp[0], kvp[1]);
						} else {
							randImg = GetNewChallengeImage();
							imageName = string.Format("{0}|{1}", randImg.Key, randImg.Value);
							CarrotWebHelper.HttpContext.Session.SetString(SessionKey, imageName);
						}
					} catch (Exception ex) {
						randImg = GetNewChallengeImage();
						imageName = string.Format("{0}|{1}", randImg.Key, randImg.Value);
						CarrotWebHelper.HttpContext.Session.SetString(SessionKey, imageName);
					}
				}

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
				CarrotWebHelper.HttpContext.Session.Remove(SessionKey);
			}

			return valid;
		}

		public string Instructions { get; set; }

		public string AltValidationFailText { get; set; }
	}
}