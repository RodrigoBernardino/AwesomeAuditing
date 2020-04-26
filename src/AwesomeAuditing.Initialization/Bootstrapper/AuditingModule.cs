using Autofac;
using AwesomeAuditing.Data;
using AwesomeAuditing.Domain;
using AwesomeAuditing.Domain.DomainServices;

namespace AwesomeAuditing.Initialization.Bootstrapper
{
    /// <summary>
    /// AwesomeAuditing Autofac module class.
    /// </summary>
    public class AuditingModule : Module
    {
        /// <summary>
        /// The AwesomeAuditing user handler.
        /// </summary>
        private readonly AwesomeAuditingHandler _auditingUserHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditingModule"/> class.
        /// </summary>
        /// <param name="auditingUserHandler">The AwesomeAuditing user handler.</param>
        internal AuditingModule(AwesomeAuditingHandler auditingUserHandler)
        {
            _auditingUserHandler = auditingUserHandler;
        }

        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be
        /// registered.</param>
        /// <remarks>
        /// Note that the ContainerBuilder parameter is unique to this module.
        /// </remarks>
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AuditingRepository>()
                .As<IAuditingRepository>();

            builder.Register(c => new AuditingHandler(_auditingUserHandler))
                .As<IAuditingHandler>();

            builder.RegisterType<AuditingDomainService>()
                .AsSelf();

            builder.Register(c =>
                new AwesomeAuditingInterceptor(c.Resolve<IAuditingHandler>()
                    , c.Resolve<AuditingDomainService>()));
        }
    }
}
