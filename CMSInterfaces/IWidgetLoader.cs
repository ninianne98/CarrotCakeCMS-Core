namespace Carrotware.CMS.Interface {

	public interface IWidgetLoader {
		string AreaName { get; }

		void RegisterWidgets(WebApplication app);

		void LoadWidgets(IServiceCollection services);
	}
}