﻿using Carrotware.CMS.Core;
using Carrotware.CMS.Interface;
using Carrotware.Web.UI.Components;
using System.ComponentModel.DataAnnotations;
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

namespace Carrotware.CMS.CoreMVC.UI.Admin.Models {

	public class SiteSkinModel {
		protected FileDataHelper helpFile = new FileDataHelper();
		private FileInfo _info = null;

		public SiteSkinModel() {
			this.CreationTime = DateTime.Now.Date;
			this.LastWriteTime = DateTime.Now.Date;
			this.SitePath = CarrotHttpHelper.MapPath("/");
			this.RelatedFiles = new List<FileData>();
		}

		public SiteSkinModel(string encodedPath, string altPath)
			: this(encodedPath) {
			this.AltPath = altPath.DecodeBase64();

			this.EditFile = this.AltPath;
		}

		public SiteSkinModel(string encodedPath)
			: this() {
			this.EncodedPath = encodedPath;
			this.TemplateFile = encodedPath.DecodeBase64();
			this.FullFilePath = CarrotHttpHelper.MapPath(this.TemplateFile);

			using (var cmsHelper = new CMSConfigHelper()) {
				this.Template = cmsHelper.Templates.Where(x => x.TemplatePath.ToLowerInvariant() == this.TemplateFile.ToLowerInvariant()).FirstOrDefault();
			}

			var ifo = new FileInfo(this.TemplateFile);
			this.TemplateFolder = ifo.Directory.FullName;

			this.EditFile = this.TemplateFile;
		}

		protected List<FileData> SetSourceFiles(string templateFileUri) {
			List<FileData> flsWorking = new List<FileData>();
			List<FileData> fldrWorking = new List<FileData>();

			List<string> lstFileExtensions = new List<string>();
			lstFileExtensions.Add(".css");
			lstFileExtensions.Add(".js");
			lstFileExtensions.Add(".cshtml");

			string templateFile = CarrotHttpHelper.MapPath(templateFileUri);

			if (File.Exists(templateFile)) {
				templateFile = templateFile.Replace(@"/", @"\");

				string skinPath = templateFile.Substring(0, templateFile.LastIndexOf(@"\")).ToLowerInvariant();
				string skinName = skinPath.Substring(skinPath.LastIndexOf(@"\") + 1);

				FileData skinFolder = helpFile.GetFolderInfo("/", templateFile);
				skinFolder.FolderPath = FileDataHelper.MakeContentFolderPath(templateFile);
				fldrWorking = helpFile.SpiderDeepFoldersFD(FileDataHelper.MakeContentFolderPath(templateFile));
				fldrWorking.Add(skinFolder);

				try {
					string assetPath = string.Format("/assets/{0}", skinName);

					if (Directory.Exists(CarrotHttpHelper.MapWebPath(assetPath))) {
						FileData incFolder = helpFile.GetFolderInfo("/", CarrotHttpHelper.MapWebPath(assetPath));
						fldrWorking.Add(incFolder);

						var assetFlds = helpFile.SpiderDeepFoldersFD(FileDataHelper.MakeWebFolderPath(incFolder.FolderPath));

						fldrWorking = fldrWorking.Union(assetFlds).ToList();
					}
				} catch (Exception ex) { }

				try {
					if (Directory.Exists(CarrotHttpHelper.MapWebPath("/includes"))) {
						FileData incFolder = helpFile.GetFolderInfo("/", CarrotHttpHelper.MapWebPath("/includes"));
						fldrWorking.Add(incFolder);
					}
					if (Directory.Exists(CarrotHttpHelper.MapWebPath("/js"))) {
						FileData incFolder = helpFile.GetFolderInfo("/", CarrotHttpHelper.MapWebPath("/js"));
						fldrWorking.Add(incFolder);
					}
					if (Directory.Exists(CarrotHttpHelper.MapWebPath("/css"))) {
						FileData incFolder = helpFile.GetFolderInfo("/", CarrotHttpHelper.MapWebPath("/css"));
						fldrWorking.Add(incFolder);
					}

					if (Directory.Exists(CarrotHttpHelper.MapWebPath("/Scripts"))) {
						FileData incFolder = helpFile.GetFolderInfo("/", CarrotHttpHelper.MapWebPath("/Scripts"));
						fldrWorking.Add(incFolder);
					}
					if (Directory.Exists(CarrotHttpHelper.MapWebPath("/Content"))) {
						FileData incFolder = helpFile.GetFolderInfo("/", CarrotHttpHelper.MapWebPath("/Content"));
						fldrWorking.Add(incFolder);
					}
				} catch (Exception ex) { }

				helpFile.IncludeAllFiletypes();

				foreach (FileData f in fldrWorking) {
					List<FileData> fls = helpFile.GetFiles(f.FolderPath);

					flsWorking = (from m in flsWorking.Union(fls).ToList()
								  join e in lstFileExtensions on m.FileExtension.ToLowerInvariant() equals e
								  select m).ToList();
				}

				flsWorking = flsWorking.Where(x => x.MimeType.StartsWith("text")).ToList();
			}

			if (flsWorking == null) {
				flsWorking = new List<FileData>();
			}

			return flsWorking.Distinct().OrderBy(x => x.FileName).OrderBy(x => x.FolderPath).ToList();
		}

		public void ReadFile() {
			string realPath = CarrotHttpHelper.MapPath(this.EditFile);
			if (!File.Exists(realPath)) {
				realPath = CarrotHttpHelper.MapWebPath(this.EditFile);
			}

			if (File.Exists(realPath)) {
				using (var sr = new StreamReader(realPath)) {
					this.FileContents = sr.ReadToEnd();
				}

				ReadRelated();
			}
		}

		public void ReadRelated() {
			string realPath = CarrotHttpHelper.MapPath(this.EditFile);

			if (File.Exists(realPath)) {
				_info = new FileInfo(realPath);

				this.CreationTime = _info.CreationTime;
				this.LastWriteTime = _info.LastWriteTime;
				this.FullName = _info.FullName;
			}

			if (File.Exists(this.FullFilePath)) {
				this.RelatedFiles = SetSourceFiles(this.TemplateFile);
			}
		}

		public void SaveFile() {
			string realPath = CarrotHttpHelper.MapPath(this.EditFile);
			if (!File.Exists(realPath)) {
				realPath = CarrotHttpHelper.MapWebPath(this.EditFile);
			}

			if (File.Exists(realPath)) {
				Encoding encode = System.Text.Encoding.Default;

				using (var oWriter = new StreamWriter(realPath, false, encode)) {
					oWriter.Write(this.FileContents);
					oWriter.Close();
				}
			}
		}

		public string EncodePath(string sIn) {
			if (!(sIn.StartsWith(@"\") || sIn.StartsWith(@"/"))) {
				sIn = @"/" + sIn;
			}
			return CMSConfigHelper.EncodeBase64(sIn.ToLowerInvariant());
		}

		public CMSTemplate Template { get; set; }

		public List<FileData> RelatedFiles { get; set; }

		public string SitePath { get; set; }

		public string TemplateFolder { get; set; }

		public string AltPath { get; set; }

		[Required]
		[Display(Name = "Encoded Path")]
		public string EncodedPath { get; set; }

		[Required]
		[Display(Name = "Edit File")]
		public string EditFile { get; set; }

		public DateTime CreationTime { get; set; }
		public DateTime LastWriteTime { get; set; }
		public string FullName { get; set; }

		public string TemplateFile { get; set; }
		public string FullFilePath { get; set; }

		[Required]
		[Display(Name = "File Contents")]
		public string FileContents { get; set; }
	}
}