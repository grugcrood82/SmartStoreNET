using Autofac;
using Autofac.Integration.Mvc;
using SmartStore.Core.Infrastructure;
using SmartStore.Core.Infrastructure.DependencyManagement;
using SmartStore.PayMark.Filters;
using SmartStore.PayMark.Services;
using SmartStore.Web.Controllers;

namespace SmartStore.PayMark
{
    public class DependencyRegistrar : IDependencyRegistrar
	{
		public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, bool isActiveModule)
		{
			builder.RegisterType<PayMarkService>().As<IPayMarkService>().InstancePerRequest();
		}

		public int Order
		{
			get { return 1; }
		}
	}
}
