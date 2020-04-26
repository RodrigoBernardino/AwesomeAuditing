using Autofac;
using Autofac.Extras.DynamicProxy;
using AwesomeAuditing.Test.Data.Repositories;
using AwesomeAuditing.Test.Domain.DomainServices;

namespace AwesomeAuditing.Test.Bootstrapper
{
    public class BootstrapperModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(EntityRepository<>))
                .As(typeof(IEntityRepository<>))
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(AwesomeAuditingInterceptor));

            builder.RegisterType<UserDomainService>()
                .As<IUserDomainService>()
                .EnableClassInterceptors()
                .InterceptedBy(typeof(AwesomeAuditingInterceptor));
        }
    }
}
