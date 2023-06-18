using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

namespace Carrotware.CMS.Data.Models {

	public class AppIdentityDbContext : IdentityDbContext {

		public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options)
			: base(options) {
		}

		public static AppIdentityDbContext Create() {
			var optionsBuilder = new DbContextOptionsBuilder<AppIdentityDbContext>();

			DataHelper.Configure("CarrotwareCMS", optionsBuilder);

			return new AppIdentityDbContext(optionsBuilder.Options);
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
			DataHelper.Configure("CarrotwareCMS", optionsBuilder);
		}
	}
}