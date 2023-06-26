using System;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

namespace Carrotware.CMS.Core {

	public class InfoKVP {

		public InfoKVP() { }

		public InfoKVP(string k, string t) {
			this.InfoKey = k;
			this.InfoLabel = t;
		}

		public string InfoLabel { get; set; }
		public string InfoKey { get; set; }

		public override string ToString() {
			return InfoKey + " : " + InfoLabel;
		}

		public override bool Equals(object obj) {
			//Check for null and compare run-time types.
			if (obj == null || GetType() != obj.GetType()) return false;
			if (obj is InfoKVP) {
				InfoKVP p = (InfoKVP)obj;
				return (this.InfoKey == p.InfoKey);
			} else {
				return false;
			}
		}

		public override int GetHashCode() {
			return InfoLabel.GetHashCode() ^ InfoKey.GetHashCode();
		}
	}
}