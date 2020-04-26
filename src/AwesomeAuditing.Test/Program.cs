using Autofac;
using AwesomeAuditing.Data;
using AwesomeAuditing.Test.Auditing;
using AwesomeAuditing.Test.Bootstrapper;
using AwesomeAuditing.Test.Data.Repositories;
using AwesomeAuditing.Test.Domain.DomainServices;
using AwesomeAuditing.Test.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AwesomeAuditing.Test
{
    class Program
    {
        private static IAuditingRepository _auditingRepository;
        private static IUserDomainService _userDomainService;
        private static IEntityRepository<User> _userRepository;
        private static IEntityRepository<Role> _roleRepository;

        static void Main(string[] args)
        {
            ConfigureCurrentThreadPrincipal();

            var builder = new ContainerBuilder();
            builder.RegisterModule(AuditingConfig.CreateAuditingModule("TestContext", new TestAuditingHandler(), true));
            builder.RegisterModule(new BootstrapperModule());

            IContainer container = builder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                _auditingRepository = scope.Resolve<IAuditingRepository>();
                _userDomainService = scope.Resolve<IUserDomainService>();
                _userRepository = scope.Resolve<IEntityRepository<User>>();
                _roleRepository = scope.Resolve<IEntityRepository<Role>>();

                User newUser = new User
                {
                    Name = "rodrigo.test2"
                };

                newUser = _userDomainService.Add(newUser);

                //User user = _userRepository.FindAll().FirstOrDefault();
                //user.Name = "updatedName";
                //_userDomainService.Update(user);

                //User user = _userRepository.FindAll().FirstOrDefault();
                //_userDomainService.Remove(user);

                Role newRole = new Role
                {
                    Name = "Operators2"
                };

                newRole = _roleRepository.Add(newRole);

                IEnumerable<AuditRecord> auditRecords = _auditingRepository.FindAll();

                Console.ReadKey();
            }
        }

        static void ConfigureCurrentThreadPrincipal()
        {
            GenericIdentity userIdentity = new GenericIdentity("rodrigo.bernardino");
            GenericPrincipal userPrincipal = new GenericPrincipal(userIdentity, new string[] { "Admin" });
            Thread.CurrentPrincipal = userPrincipal;
        }
    }
}
