using System.Drawing;
using System.Text;
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

	public class Calendar : BaseTwoPartWebComponent {

		public Calendar() {
			this.ElementId = "cal";

			this.CellColor = ColorTranslator.FromHtml("#ffffff");
			this.CellBackground = ColorTranslator.FromHtml("#00509F");
			this.WeekdayColor = ColorTranslator.FromHtml("#000000");
			this.WeekdayBackground = ColorTranslator.FromHtml("#00C87F");

			this.TodayColor = ColorTranslator.FromHtml("#FFFF80");
			this.TodayBackground = ColorTranslator.FromHtml("#800080");
			this.TodaySelectBorder = ColorTranslator.FromHtml("#FFFF80");
			this.NormalColor = ColorTranslator.FromHtml("#004040");

			this.NormalBackground = ColorTranslator.FromHtml("#D8D8EB");
			this.NormalSelectBorder = ColorTranslator.FromHtml("#FF0080");
			this.TodayLink = ColorTranslator.FromHtml("#FFFF80");
			this.NormalLink = ColorTranslator.FromHtml("#004040");

			this.JavascriptForDate = string.Empty;
			this.OverrideCssFile = string.Empty;

			this.HilightDateList = new List<DateTime>();
			this.CalendarDate = DateTime.Now.Date;
		}

		public Calendar(string wc, string wb, string cc, string cb,
							string tc, string tb, string tsb, string tl,
							string nc, string nb, string nsb, string nl) : this() {

			this.WeekdayColor = CarrotWebHelper.DecodeColor(wc);
			this.WeekdayBackground = CarrotWebHelper.DecodeColor(wb);
			this.CellColor = CarrotWebHelper.DecodeColor(cc);
			this.CellBackground = CarrotWebHelper.DecodeColor(cb);

			this.TodayColor = CarrotWebHelper.DecodeColor(tc);
			this.TodayBackground = CarrotWebHelper.DecodeColor(tb);
			this.TodaySelectBorder = CarrotWebHelper.DecodeColor(tsb);
			this.TodayLink = CarrotWebHelper.DecodeColor(tl);

			this.NormalColor = CarrotWebHelper.DecodeColor(nc);
			this.NormalBackground = CarrotWebHelper.DecodeColor(nb);
			this.NormalSelectBorder = CarrotWebHelper.DecodeColor(nsb);
			this.NormalLink = CarrotWebHelper.DecodeColor(nl);
		}

		public Color CellColor { get; set; }
		public Color CellBackground { get; set; }
		public Color WeekdayColor { get; set; }
		public Color WeekdayBackground { get; set; }
		public Color TodayColor { get; set; }
		public Color TodayBackground { get; set; }
		public Color TodaySelectBorder { get; set; }
		public Color NormalColor { get; set; }
		public Color NormalBackground { get; set; }
		public Color NormalSelectBorder { get; set; }
		public Color TodayLink { get; set; }
		public Color NormalLink { get; set; }
		public string JavascriptForDate { get; set; }
		public string OverrideCssFile { get; set; }
		public int MonthNumber { get; private set; }
		public int YearNumber { get; private set; }

		private DateTime _date = DateTime.Today;

		public DateTime CalendarDate {
			get {
				this.YearNumber = _date.Year;
				this.MonthNumber = _date.Month;
				return _date;
			}
			set {
				_date = value;
				this.YearNumber = _date.Year;
				this.MonthNumber = _date.Month;
			}
		}

		public string HilightDates { get; set; } = string.Empty;

		public string ElementId { get; set; }

		public List<DateTime> HilightDateList { get; set; } = new List<DateTime>();

		public override string GetBody() {
			StringBuilder sb = new StringBuilder();
			sb.AppendLine();

			string CtrlID = this.ElementId;

			DateTime today = DateTime.Today.Date;
			DateTime thisMonth = DateTime.Today.Date;

			try {
				thisMonth = new DateTime(YearNumber, MonthNumber, 15);
			} catch { }

			DateTime firstOfMonth = thisMonth.AddDays(1 - thisMonth.Day);

			DateTime nextMonth = thisMonth.AddMonths(1);
			DateTime prevMonth = thisMonth.AddMonths(-1);

			int iFirstDay = (int)firstOfMonth.DayOfWeek;
			TimeSpan ts = firstOfMonth.AddMonths(1) - firstOfMonth;
			int iDaysInMonth = ts.Days;

			string MonthName = thisMonth.ToString("MMMM") + "  " + thisMonth.Year.ToString();

			int dayOfWeek = 6;
			int dayOfMonth = 1;
			dayOfMonth -= iFirstDay;

			List<DateTime> dates = new List<DateTime>();

			if (this.HilightDateList != null && this.HilightDateList.Any()) {
				dates = this.HilightDateList;
			} else {
				List<string> lstDates = new List<string>();

				if (!string.IsNullOrEmpty(this.HilightDates)) {
					lstDates = this.HilightDates.Split(';').AsEnumerable().ToList();
				}
				dates = (from dd in lstDates select Convert.ToDateTime(dd)).ToList();
			}

			sb.AppendLine("<table  id=\"" + CtrlID + "\" class=\"calendarGrid\" cellspacing=\"0\" cellpadding=\"3\" align=\"center\" border=\"1\">");
			sb.AppendLine("	<tr class=\"calendarheadrow\">");
			sb.AppendLine("		<td class=\"head\" colspan=\"7\">");
			sb.AppendLine("			<table class=\"innerhead\" cellspacing=\"0\" cellpadding=\"0\" width=\"100%\" border=\"0\">");
			sb.AppendLine("				<tr> <td class=\"head normaltext\"> &nbsp; </td> </tr>");
			sb.AppendLine("				<tr> <td class=\"head headtext\"> " + MonthName + " </td> </tr>");
			sb.AppendLine("				<tr> <td class=\"head normaltext\"> &nbsp; </td> </tr>");
			sb.AppendLine("			</table>");
			sb.AppendLine("		</td>");
			sb.AppendLine("	</tr>");
			sb.AppendLine();
			sb.AppendLine("	<tr class=\"weekday\">");
			sb.AppendLine("		<td class=\"weekday\" width=\"38\"> SU </td>");
			sb.AppendLine("		<td class=\"weekday\" width=\"38\"> M </td>");
			sb.AppendLine("		<td class=\"weekday\" width=\"38\"> TU </td>");
			sb.AppendLine("		<td class=\"weekday\" width=\"38\"> W </td>");
			sb.AppendLine("		<td class=\"weekday\" width=\"38\"> TR </td>");
			sb.AppendLine("		<td class=\"weekday\" width=\"38\"> F </td>");
			sb.AppendLine("		<td class=\"weekday\" width=\"38\"> SA </td>");
			sb.AppendLine("	</tr>");
			sb.AppendLine();

			int WeekNumber = 1;

			while ((dayOfMonth <= iDaysInMonth) && (dayOfMonth <= 31) && (dayOfMonth >= -7)) {
				for (int DayIndex = 0; DayIndex <= dayOfWeek; DayIndex++) {
					if (DayIndex == 0) {
						sb.Append("<tr class=\"weekdayrow\" id=\"" + CtrlID + "-weekRow" + WeekNumber.ToString() + "\">\n");
						WeekNumber++;
					}

					string strCaption = "&nbsp;";
					string sClass = "normal";
					DateTime cellDate = DateTime.MinValue;

					if ((dayOfMonth >= 1) && (dayOfMonth <= iDaysInMonth)) {
						cellDate = new DateTime(YearNumber, MonthNumber, dayOfMonth);
						if (!string.IsNullOrEmpty(JavascriptForDate)) {
							strCaption = "&nbsp;<a href=\"javascript:" + JavascriptForDate + "('" + cellDate.ToString("yyyy-MM-dd") + "')\">" + dayOfMonth.ToString() + "&nbsp;";
						} else {
							strCaption = "&nbsp;" + dayOfMonth.ToString() + "&nbsp;";
						}
					}

					if (strCaption != "&nbsp;") {
						cellDate = new DateTime(YearNumber, MonthNumber, dayOfMonth);
						if (cellDate == today) {
							sClass = "today";
						}

						IEnumerable<DateTime> copyRows = (from c in dates
														  where c == cellDate.Date
														  select c);
						if (copyRows.Any()) {
							sClass = sClass + "sel";
						}
					}

					dayOfMonth++;

					string cell = "\t<td id=\"" + CtrlID + "-cellDay" + dayOfMonth.ToString() + "\" class=\"" + sClass + "\">" + strCaption + "</td>\n";
					sb.Append(cell);

					if (DayIndex == dayOfWeek) {
						sb.Append("</tr>\n");
					}
				}
			}

			sb.AppendLine("</table>");

			return CarrotWebHelper.HtmlFormat(sb);
		}

		public string GenerateCSS() {
			var sb = new StringBuilder();
			sb.Append(CarrotWebHelper.GetManifestResourceText("calendar.txt"));

			sb.Replace("[[TIMESTAMP]]", DateTime.UtcNow.ToString("u"));

			sb.Replace("{WEEKDAY_CHEX}", ColorTranslator.ToHtml(this.WeekdayColor));
			sb.Replace("{WEEKDAY_BGHEX}", ColorTranslator.ToHtml(this.WeekdayBackground));
			sb.Replace("{CELL_CHEX}", ColorTranslator.ToHtml(this.CellColor));
			sb.Replace("{CELL_BGHEX}", ColorTranslator.ToHtml(this.CellBackground));

			sb.Replace("{TODAY_CHEX}", ColorTranslator.ToHtml(this.TodayColor));
			sb.Replace("{TODAY_BGHEX}", ColorTranslator.ToHtml(this.TodayBackground));
			sb.Replace("{TODAYSEL_BDR}", ColorTranslator.ToHtml(this.TodaySelectBorder));
			sb.Replace("{TODAY_LNK}", ColorTranslator.ToHtml(this.TodayLink));

			sb.Replace("{NORMAL_CHEX}", ColorTranslator.ToHtml(this.NormalColor));
			sb.Replace("{NORMAL_BGHEX}", ColorTranslator.ToHtml(this.NormalBackground));
			sb.Replace("{NORMALSEL_BDR}", ColorTranslator.ToHtml(this.NormalSelectBorder));
			sb.Replace("{NORMAL_LNK}", ColorTranslator.ToHtml(this.NormalLink));

			sb.Replace("{CALENDAR_ID}", string.Format("#{0}", this.ElementId));

			return sb.ToString();
		}

		public override string GetHead() {
			if (string.IsNullOrEmpty(this.OverrideCssFile)) {
				var sb = new StringBuilder();

				sb.Append(string.Format("{0}?el={1}", UrlPaths.CalendarStylePath, HttpUtility.HtmlEncode(this.ElementId.EncodeBase64())));
				sb.Append(string.Format("&wc={0}&wb={1}&cc={2}&cb={3}", CarrotWebHelper.EncodeColor(this.WeekdayColor), CarrotWebHelper.EncodeColor(this.WeekdayBackground), CarrotWebHelper.EncodeColor(this.CellColor), CarrotWebHelper.EncodeColor(this.CellBackground)));
				sb.Append(string.Format("&tc={0}&tb={1}&tsb={2}&tl={3}", CarrotWebHelper.EncodeColor(this.TodayColor), CarrotWebHelper.EncodeColor(this.TodayBackground), CarrotWebHelper.EncodeColor(this.TodaySelectBorder), CarrotWebHelper.EncodeColor(this.TodayLink)));
				sb.Append(string.Format("&nc={0}&nb={1}&nsb={2}&nl={3}", CarrotWebHelper.EncodeColor(this.NormalColor), CarrotWebHelper.EncodeColor(this.NormalBackground), CarrotWebHelper.EncodeColor(this.NormalSelectBorder), CarrotWebHelper.EncodeColor(this.NormalLink)));
				sb.Append(string.Format("&ts={0}", CarrotWebHelper.DateKey()));

				return UrlPaths.CreateCssTag(string.Format("Calendar CSS: {0}", this.ElementId), sb.ToString());
			} else {
				return UrlPaths.CreateCssTag(this.OverrideCssFile);
			}
		}
	}
}