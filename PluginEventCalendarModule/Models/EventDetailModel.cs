using CarrotCake.CMS.Plugins.EventCalendarModule.Data;
using Carrotware.CMS.Core;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CarrotCake.CMS.Plugins.EventCalendarModule.Models {

	public class EventDetailModel : IValidatableObject {

		public EventDetailModel() {
			this.SiteID = Guid.Empty;
			this.ItemID = Guid.Empty;

			this.Frequencies = CalendarFrequencyHelper.GetCalendarFrequencies();
			this.DaysOfTheWeek = CalendarHelper.DaysOfTheWeek.Select(x => new SelectListItem() { Text = x.Value, Value = x.Key.ToString(), Selected = false }).ToList();

			this.ItemData = new CalendarEventProfile();
			this.ItemData.EventStartDate = SiteData.CurrentSite.Now.Date;
			this.ItemData.EventEndDate = SiteData.CurrentSite.Now.Date;
			this.ItemData.RecursEvery = 1;
			this.ItemData.IsAllDayEvent = true;
			this.ItemData.IsCancelledPublic = true;
			this.ItemData.CalendarFrequencyId = CalendarFrequencyHelper.GetIDByFrequencyType(CalendarFrequencyHelper.FrequencyType.Once);
		}

		public EventDetailModel(Guid siteId, Guid itemId) : this() {
			this.SiteID = siteId;
			this.ItemID = itemId;

			Load();
		}

		public ModelStateDictionary ClearOptionalItems(ModelStateDictionary modelState) {
			// these child objects are for display only, and their validation is not needed
			foreach (var ms in modelState.ToArray()) {
				if (ms.Key.ToLowerInvariant().Contains("calendareventcategory") || ms.Key.ToLowerInvariant().Contains("calendarfrequency")) {
					modelState.Remove(ms.Key);
				}
			}

			return modelState;
		}

		public void Load() {
			this.Categories = CalendarHelper.GetCalendarCategories(this.SiteID);

			this.Colors = (from c in CalendarHelper.GetCalendarCategories(this.SiteID)
						   select new KeyValuePair<Guid, string>(c.CalendarEventCategoryId, string.Format("{0}|{1}", c.CategoryBGColor, c.CategoryFGColor)))
							.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

			if (this.ItemID != Guid.Empty) {
				this.ItemData = CalendarHelper.GetProfile(this.ItemID);

				if (this.ItemData.EventStartTime.HasValue) {
					this.EventStartTime = CalendarHelper.GetFullDateTime(this.ItemData.EventStartTime);
				}
				if (this.ItemData.EventEndTime.HasValue) {
					this.EventEndTime = CalendarHelper.GetFullDateTime(this.ItemData.EventEndTime);
				}

				foreach (var d in this.DaysOfTheWeek) {
					d.Selected = GetSelectedDays(int.Parse(d.Value));
				}
			}

			// in case of post, fill in the name
			var dow = CalendarHelper.DaysOfTheWeek.Select(x => new SelectListItem() { Text = x.Value, Value = x.Key.ToString(), Selected = false }).ToList();
			foreach (var d in this.DaysOfTheWeek) {
				var day = dow.Where(x => x.Value == d.Value).First();
				d.Text = day.Text;
			}
		}

		public bool GetSelectedDays(int dayInt) {
			if (this.ItemData.EventRepeatPattern.HasValue) {
				if ((this.ItemData.EventRepeatPattern.Value & dayInt) != 0) {
					return true;
				} else {
					return false;
				}
			} else {
				return false;
			}
		}

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
			List<ValidationResult> errors = new List<ValidationResult>();
			var props = this.ItemData.GetType().GetProperties()
							.Where(p => p.GetCustomAttributes(typeof(RequiredAttribute), true).Any());

			foreach (var prop in props) {
				var val = prop.GetValue(this.ItemData).ToString();

				if (String.IsNullOrWhiteSpace(val)) {
					ValidationResult err = new ValidationResult(string.Format("{0} is required", prop.Name), new string[] { string.Format("ItemData.{0}", prop.Name) });
					errors.Add(err);
				}
			}

			return errors;
		}

		public string Operation { get; set; }
		public Guid SiteID { get; set; }
		public Guid ItemID { get; set; }

		public DateTime? EventStartTime { get; set; }
		public DateTime? EventEndTime { get; set; }

		public CalendarEventProfile ItemData { get; set; }

		public List<SelectListItem> DaysOfTheWeek { get; set; }

		public Dictionary<Guid, string>? Colors { get; set; }

		public List<CalendarFrequency>? Frequencies { get; set; }
		public List<CalendarEventCategory>? Categories { get; set; }
	}
}