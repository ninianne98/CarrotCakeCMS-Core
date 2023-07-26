using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;

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

	public abstract class CarrotWebGridBase : IHtmlContent {
		protected IHtmlHelper _htmlHelper;
		protected SortParm _sortDir;

		protected void StandardInit(IHtmlHelper htmlHelper, PagedDataBase dp) {
			_htmlHelper = htmlHelper;

			this.FooterOuterTag = "ul";
			this.FooterTag = "li";

			this.FieldIdPrefix = string.Empty;
			this.FieldNamePrefix = string.Empty;

			this.SortDescIndicator = "&nbsp;&#9660;";
			this.SortAscIndicator = "&nbsp;&#9650;";

			this.HtmlClientId = "tblDataTable";

			this.Columns = new List<ICarrotGridColumn>();

			this.UseDataPage = true;
			this.PageSizeExternal = false;
			this.PrettifyHeadings = false;

			this.PagedDataBase = dp;
		}

		protected PagedDataBase PagedDataBase { get; set; }

		public List<ICarrotGridColumn> Columns { get; protected set; }

		public Func<Object, IHtmlContent> EmptyDataTemplate { get; set; }

		public string HtmlClientId { get; set; }
		public string HtmlFormId { get; set; }
		public string SortDescIndicator { get; set; }
		public string SortAscIndicator { get; set; }
		protected string FieldIdPrefix { get; set; }
		protected string FieldNamePrefix { get; set; }

		public int RowNumber { get; set; }
		public bool PrettifyHeadings { get; set; }

		public bool UseDataPage { get; set; }
		public bool PageSizeExternal { get; set; }

		public string FooterOuterTag { get; set; }
		public object htmlFootAttrib { get; set; }

		public string FooterTag { get; set; }
		public object htmlFootSel { get; set; }
		public object htmlFootNotSel { get; set; }

		public void ConfigName(HtmlString name) {
			ConfigName(name.ToString());
		}

		public void ConfigName(string name) {
			this.FieldNamePrefix = name;

			if (string.IsNullOrEmpty(this.FieldNamePrefix)) {
				this.FieldNamePrefix = string.Empty;
			} else {
				this.FieldNamePrefix = string.Format("{0}.", this.FieldNamePrefix);
			}

			this.FieldIdPrefix = this.FieldNamePrefix.Replace(".", "_").Replace("]", "_").Replace("[", "_");
		}

		public object TableAttributes { get; set; }
		public object THeadAttributes { get; set; }
		public object TBodyAttributes { get; set; }

		public void SetTableAttributes(object tableAttrib, object headAttrib, object bodyAttrib) {
			this.TableAttributes = InitAttrib(tableAttrib);
			this.THeadAttributes = InitAttrib(headAttrib);
			this.TBodyAttributes = InitAttrib(bodyAttrib);
		}

		protected IDictionary<string, object> InitAttrib(object htmlAttribs) {
			IDictionary<string, object> tblAttrib = new RouteValueDictionary();

			if (htmlAttribs != null) {
				tblAttrib = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttribs);
			}

			return tblAttrib;
		}

		protected void FormHelper(Expression<Func<PagedDataBase, object>> property, StringBuilder sb) {
			PropertyInfo propInfo = this.PagedDataBase.PropInfoFromExpression<PagedDataBase>(property);
			string columnName = ReflectionUtilities.BuildProp(property);
			object val = propInfo.GetValue(this.PagedDataBase, null);

			string fldName = string.Format("{0}{1}", this.FieldNamePrefix, columnName);
			string str = val == null ? string.Empty : val.ToString();

			sb.AppendLine(_htmlHelper.Hidden(fldName, str).RenderToString());
		}

		protected StringBuilder BuildHeadScript(StringBuilder sb) {
			string frm = "form:first";
			if (string.IsNullOrEmpty(this.HtmlFormId)) {
				this.HtmlFormId = this.HtmlClientId + "_form";
			}
			if (!string.IsNullOrEmpty(this.HtmlFormId)) {
				frm = string.Format("#{0}", this.HtmlFormId);
			}

			var btnId = string.Format("submit_{0}", this.HtmlFormId);

			if (this.UseDataPage) {
				sb.AppendLine(string.Empty);
				sb.AppendLine("	<script type=\"text/javascript\">");
				sb.AppendLine("	function __clickHead(fld) {");
				sb.AppendLine(string.Format("		$('#{0}SortByNew').val(fld);", this.FieldIdPrefix));
				sb.AppendLine(string.Format("		$('#{0}').click();", btnId));
				//sb.AppendLine(string.Format("		$('{0}').trigger('submit');", frm));
				sb.AppendLine("	}");
				sb.AppendLine(string.Empty);
				sb.AppendLine("	function __clickPage(nbr, fld) {");
				sb.AppendLine("		$('#' + fld).val(nbr);");
				sb.AppendLine("		$('#' + fld).focus();");
				sb.AppendLine(string.Format("		$('#{0}').click();", btnId));
				//sb.AppendLine(string.Format("		$('{0}').trigger('submit');", frm));
				sb.AppendLine("	}");
				sb.AppendLine("	</script>");
				sb.AppendLine(string.Empty);

				var div = new HtmlTag("div");
				div.MergeAttributes(new { @style = "display: none;" });
				sb.AppendLine(div.OpenTag());

				var btn = new HtmlTag("button");
				btn.InnerHtml = "Save";
				btn.MergeAttributes(new { @id = btnId, @type = "submit", @value = "save" });
				sb.AppendLine(btn.RenderTag());

				FormHelper(x => x.OrderBy, sb);
				FormHelper(x => x.SortByNew, sb);
				if (!this.PageSizeExternal) {
					FormHelper(x => x.PageSize, sb);
				}
				FormHelper(x => x.TotalRecords, sb);
				FormHelper(x => x.MaxPage, sb);
				FormHelper(x => x.PageNumber, sb);

				sb.AppendLine(div.CloseTag());
			}

			return sb;
		}

		protected StringBuilder BuildTableHeadRow(StringBuilder sb) {
			using (new WrappedItem(sb, "thead", this.THeadAttributes)) {
				using (new WrappedItem(sb, "tr", null)) {
					foreach (var col in this.Columns) {
						using (new WrappedItem(sb, "th", col.HeadAttributes)) {
							if (col is ICarrotGridColumnExt) {
								var colExt = (ICarrotGridColumnExt)col;
								if (colExt.Sortable && this.UseDataPage) {
									var js = string.Format("javascript:__clickHead('{0}')", colExt.ColumnName);

									IDictionary<string, object> tagAttrib = InitAttrib(colExt.HeadLinkAttributes);
									tagAttrib.Add("href", js);

									using (new WrappedItem(sb, "a", tagAttrib)) {
										sb.Append(col.HeaderText);

										if (_sortDir.SortField.ToUpperInvariant() == colExt.ColumnName.ToUpperInvariant()) {
											if (_sortDir.SortDirection.ToUpperInvariant() == "ASC") {
												sb.Append(this.SortAscIndicator);
											} else {
												sb.Append(this.SortDescIndicator);
											}
										}
									}
								} else {
									sb.Append(col.HeaderText);
								}
							} else {
								sb.Append(col.HeaderText);
							}
						}
					}
				}
			}

			return sb;
		}

		protected virtual HtmlString CreateBody() {
			return new HtmlString(string.Empty);
		}

		public virtual void SetupFooter(string outer, object outerAttrib, string inner, object selAttrib, object noselAttrib) {
			this.FooterOuterTag = string.IsNullOrEmpty(outer) ? "ul" : outer;

			this.htmlFootAttrib = outerAttrib;

			this.FooterTag = string.IsNullOrEmpty(inner) ? "li" : inner;

			this.htmlFootSel = selAttrib;
			this.htmlFootNotSel = noselAttrib;
		}

		public virtual HtmlString OutputFooter() {
			var sb = new StringBuilder();

			if (this.PagedDataBase.TotalPages > 1) {
				using (new WrappedItem(sb, this.FooterOuterTag, this.htmlFootAttrib)) {
					foreach (var i in this.PagedDataBase.PageNumbers) {
						string clickFn = string.Format("javascript:__clickPage('{0}','{1}PageNumber')", i, this.FieldIdPrefix);

						using (new WrappedItem(_htmlHelper, sb, this.FooterTag, i, this.PagedDataBase.PageNumber, this.htmlFootSel, this.htmlFootNotSel)) {
							using (new WrappedItem(sb, "a", new { @href = clickFn })) {
								sb.Append(string.Format(" {0} ", i));
							}
						}
					}
				}
			}

			return new HtmlString(sb.ToString());
		}

		protected virtual HtmlString EmptyTable() {
			this.PagedDataBase.TotalRecords = 0;
			this.PagedDataBase.PageNumber = 1;

			string cellContents = string.Empty;

			var sb = new StringBuilder();

			FormHelper(x => x.OrderBy, sb);
			FormHelper(x => x.SortByNew, sb);
			if (!this.PageSizeExternal) {
				FormHelper(x => x.PageSize, sb);
			}
			FormHelper(x => x.TotalRecords, sb);
			FormHelper(x => x.MaxPage, sb);
			FormHelper(x => x.PageNumber, sb);

			if ((!this.PagedDataBase.HasData) && this.EmptyDataTemplate != null) {
				cellContents = this.EmptyDataTemplate(new object()).RenderToString();
			}

			sb.AppendLine(cellContents);

			return new HtmlString(sb.ToString());
		}

		public virtual HtmlString OutputHtmlBody() {
			if (this.PagedDataBase.HasData) {
				return CreateBody();
			} else {
				return EmptyTable();
			}
		}

		public string ToHtmlString() {
			var sb = new StringBuilder();
			sb.AppendLine(this.OutputHtmlBody().ToString());
			sb.AppendLine(this.OutputFooter().ToString());
			return sb.ToString();
		}

		public IHtmlContent RenderHtmlString() {
			return new HtmlString(ToHtmlString());
		}

		public void WriteTo(TextWriter writer, HtmlEncoder encoder) {
			writer.Write(this.ToHtmlString());
		}
	}
}