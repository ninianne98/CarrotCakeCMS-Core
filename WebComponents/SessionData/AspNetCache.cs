/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

namespace Carrotware.Web.UI.Components.SessionData {

	public partial class AspNetCache {
		public string Id { get; set; } = null!;
		public byte[] Value { get; set; } = null!;
		public DateTimeOffset ExpiresAtTime { get; set; }
		public long? SlidingExpirationInSeconds { get; set; }
		public DateTimeOffset? AbsoluteExpiration { get; set; }
	}
}