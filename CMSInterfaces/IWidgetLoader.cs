using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrotware.CMS.Interface {
	public interface IWidgetLoader {

		void RegisterWidgets(WebApplication app);

		void LoadWidgets(IServiceCollection services);
	}
}
