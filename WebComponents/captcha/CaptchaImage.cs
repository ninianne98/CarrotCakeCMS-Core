using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Security.Cryptography;
using System.Text;

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

	public static class CaptchaImage {

		public static string BGColorDef {
			get {
				string d = "#EEEEEE";
				var v = CarrotWebHelper.QueryString("bgcolor");
				return (string.IsNullOrWhiteSpace(v) ? d : CarrotWebHelper.DecodeColorString(v));
			}
		}

		public static string NColorDef {
			get {
				string d = "#C46314";
				var v = CarrotWebHelper.QueryString("ncolor");
				return (string.IsNullOrWhiteSpace(v) ? d : CarrotWebHelper.DecodeColorString(v));
			}
		}

		public static string FGColorDef {
			get {
				string d = "#69785F";
				var v = CarrotWebHelper.QueryString("fgcolor");
				return (string.IsNullOrWhiteSpace(v) ? d : CarrotWebHelper.DecodeColorString(v));
			}
		}

		public static string SessionKey {
			get {
				return "carrot_captcha_key";
			}
		}

		public static bool Validate(string testValue) {
			if (string.IsNullOrEmpty(testValue)) {
				return false;
			}

			bool bValid = false;
			string guid = SessionKeyValue;

			if (testValue.ToLowerInvariant() == guid.ToLowerInvariant()) {
				bValid = true;
			}

			if (CarrotWebHelper.HttpContext != null) {
				guid = GetNewChallengeText();
				CarrotWebHelper.HttpContext.Session.SetString(SessionKey, guid);
			}
			return bValid;
		}

		public static Bitmap GetCachedCaptcha() {
			Color medGreen = ColorTranslator.FromHtml("#69785F");
			Color medOrange = ColorTranslator.FromHtml("#C46314");
			return GetCaptchaImage(medGreen, Color.White, medOrange);
		}

		internal static string GetNewChallengeText() {
			int length = 6;
			var tmp = Guid.NewGuid().ToString("N")
					+ Guid.NewGuid().ToString("N")
					+ Guid.NewGuid().ToString("N");
			byte[] inputBytes = Encoding.UTF8.GetBytes(tmp);
			byte[] hashBytes;

			using (var sha256 = SHA256.Create()) {
				hashBytes = sha256.ComputeHash(inputBytes);
			}

			int number = BitConverter.ToInt32(hashBytes, 0) & 0x7FFFFFFF;

			int modulus = (int)Math.Pow(10, length);
			int pinValue = number % modulus;

			return pinValue.ToString(new string('0', length));
		}

		public static string SessionKeyValue {
			get {
				string guid = "ABCXYZ";
				if (CarrotWebHelper.HttpContext != null) {
					try {
						if (CarrotWebHelper.HttpContext.Session.GetString(SessionKey) != null) {
							guid = CarrotWebHelper.HttpContext.Session.GetString(SessionKey);
						} else {
							guid = GetNewChallengeText();
							CarrotWebHelper.HttpContext.Session.SetString(SessionKey, guid);
						}
					} catch (Exception ex) {
						guid = GetNewChallengeText();
						CarrotWebHelper.HttpContext.Session.SetString(SessionKey, guid);
					}
				}
				return guid.ToUpperInvariant();
			}
		}

		public static Bitmap GetCaptchaImage(Color fg, Color bg, Color n) {
			int topPadding = 2; // top and bottom padding in pixels
			int sidePadding = 3; // side padding in pixels

			SolidBrush textBrush = new SolidBrush(fg);
			Font font = new Font(FontFamily.GenericSansSerif, 32, FontStyle.Bold);

			string guid = SessionKeyValue;

			Bitmap bmpCaptcha = new Bitmap(500, 500);
			Graphics graphics = Graphics.FromImage(bmpCaptcha);
			SizeF textSize = graphics.MeasureString(guid, font);

			bmpCaptcha.Dispose();
			graphics.Dispose();

			int bitmapWidth = sidePadding * 2 + (int)textSize.Width;
			int bitmapHeight = topPadding * 2 + (int)textSize.Height;
			bmpCaptcha = new Bitmap(bitmapWidth, bitmapHeight);
			graphics = Graphics.FromImage(bmpCaptcha);

			Rectangle rect = new Rectangle(0, 0, bmpCaptcha.Width, bmpCaptcha.Height);

			HatchBrush hatch1 = new HatchBrush(HatchStyle.SmallGrid, n, bg);

			HatchBrush hatch2 = new HatchBrush(HatchStyle.DiagonalCross, bg, Color.Transparent);

			graphics.FillRectangle(hatch1, rect);
			graphics.DrawString(guid, font, textBrush, sidePadding, topPadding);
			graphics.FillRectangle(hatch2, rect);

			CarrotWebHelper.HttpContext.Response.ContentType = "image/x-png";

			using (MemoryStream memStream = new MemoryStream()) {
				bmpCaptcha.Save(memStream, ImageFormat.Png);
			}

			textBrush.Dispose();
			font.Dispose();
			hatch1.Dispose();
			hatch2.Dispose();
			graphics.Dispose();

			return bmpCaptcha;
		}
	}
}