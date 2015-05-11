using Microsoft.Framework.DependencyInjection;
using System.Collections.Generic;

namespace SampleService
{
	public static class ServiceExtensions
	{
		public static IServiceCollection AddSampleService(this IServiceCollection services)
		{
			services.TryAdd(GetDefaultServices());
			return services;
		}

		private static IEnumerable<ServiceDescriptor> GetDefaultServices()
		{
			yield return ServiceDescriptor.Transient<SampleApplication.IService, SomeService>();
		}
	}
}
