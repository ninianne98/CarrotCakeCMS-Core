using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
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

	public static class CaptchaImage {

		public static string BGColorDef {
			get {
				string s = "#EEEEEE";
				try { s = CarrotWebHelper.QueryString("bgcolor"); } catch { }
				return ((s == null) ? "#EEEEEE" : CarrotWebHelper.DecodeColorString(s));
			}
		}

		public static string NColorDef {
			get {
				string s = "#C46314";
				try { s = CarrotWebHelper.QueryString("ncolor"); } catch { }
				return ((s == null) ? "#C46314" : CarrotWebHelper.DecodeColorString(s));
			}
		}

		public static string FGColorDef {
			get {
				string s = "#69785F";
				try { s = CarrotWebHelper.QueryString("fgcolor"); } catch { }
				return ((s == null) ? "#69785F" : CarrotWebHelper.DecodeColorString(s));
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
				guid = Guid.NewGuid().ToString().Substring(0, 6);
				CarrotWebHelper.HttpContext.Session.SetString(SessionKey, guid);
			}
			return bValid;
		}

		public static Bitmap GetCachedCaptcha() {
			Color medGreen = ColorTranslator.FromHtml("#69785F");
			Color medOrange = ColorTranslator.FromHtml("#C46314");
			return GetCaptchaImage(medGreen, Color.White, medOrange);
		}

		public static string SessionKeyValue {
			get {
				string guid = "ABCXYZ";
				if (CarrotWebHelper.HttpContext != null) {
					try {
						if (CarrotWebHelper.HttpContext.Session.GetString(SessionKey) != null) {
							guid = CarrotWebHelper.HttpContext.Session.GetString(SessionKey);
						} else {
							guid = Guid.NewGuid().ToString().Substring(0, 6);
							CarrotWebHelper.HttpContext.Session.SetString(SessionKey, guid);
						}
					} catch {
						guid = Guid.NewGuid().ToString().Substring(0, 6);
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