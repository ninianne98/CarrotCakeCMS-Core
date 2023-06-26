using System.ComponentModel.DataAnnotations;

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

	public class FileUpModel {

		public FileUpModel() { }

		[DataType(DataType.Upload)]
		[Display(Name = "Posted File")]
		public IFormFile PostedFile { get; set; }
	}
}