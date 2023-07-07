using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Carrotware.CMS.Interface {

	public interface IWidgetLoader {
		string AreaName { get; }

		void RegisterWidgets(WebApplication app);

		void LoadWidgets(IServiceCollection services);
	}
}