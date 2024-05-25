using Carrotware.Web.UI.Components;

namespace Northwind.Code {

	public static class Helper {
		private static Bootstrap.BootstrapColorScheme? _colorScheme;
		private static bool? _useBootstrap5;

		public static bool UseBootstrap5 {
			get {
				return _useBootstrap5.HasValue ? _useBootstrap5.Value : true;
			}
		}

		public static void SetBootstrapColor(Bootstrap.BootstrapColorScheme color) {
			_useBootstrap5 = false;
			_colorScheme = color;
		}

		public static Bootstrap.BootstrapColorScheme BootstrapColorScheme {
			get {
				if (_colorScheme == null) {
					_colorScheme = Bootstrap.BootstrapColorScheme.Blue;
				}

				return _colorScheme.Value;
			}
		}

		private static Bootstrap5.Bootstrap5ColorScheme? _colorScheme5;

		public static void SetBootstrapColor(Bootstrap5.Bootstrap5ColorScheme color) {
			_useBootstrap5 = true;
			_colorScheme5 = color;
		}

		public static Bootstrap5.Bootstrap5ColorScheme Bootstrap5ColorScheme {
			get {
				if (_colorScheme5 == null) {
					_colorScheme5 = Bootstrap5.Bootstrap5ColorScheme.Blue;
				}

				return _colorScheme5.Value;
			}
		}
	}
}