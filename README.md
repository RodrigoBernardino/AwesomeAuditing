# AwesomeAuditing
This is an auditing project that helps and simplifies auditing on any kind of project in .NET Framework. It is based in the **AOP (aspect-oriented programming)** paradigm.

.NET Core version coming soon!

## Depencencies
Autofac (https://autofac.org/)

Castle DynamicProxy (http://www.castleproject.org/projects/dynamicproxy/)

Entity Framework v6.2.0 (https://www.nuget.org/packages/EntityFramework/6.2.0)

## Instalation
This project will be added to NuGet gallery soon. For now, download this solution and add its projects to your target solution.

## How to Use
This auditing project depends on Autofac DI container. So first you need to register a new module to your ContainerBuilder to enable the AwesomeAuditing depency injection in your project.

```C#
var builder = new Autofac.ContainerBuilder();
builder.RegisterModule(AuditingConfig.CreateAuditingModule("CustomContext", new CustomAuditingHandler()));
```

The **AuditingConfig.CreateAuditingModule** method has three parameters:

The **first** one is the name of the EntityFramework DbContext of your project. This is required because AwesomeAuditing creates its own table in your project database automatically. All auditing records will be saved in this table.

The **second** one is a class that should inherit from **AwesomeAuditingHandler**. This class must implement the **GetAuditingUser** method to give the auditing interceptor the user's name of each operation.

```C#
public class TestAuditingHandler : AwesomeAuditing.AwesomeAuditingHandler
{
    public override string GetAuditingUser()
    {
        return Thread.CurrentPrincipal.Identity.Name;
    }
}
```

The **third** one is optional and determines if the audited info will be serialized using the CamelCase pattern or not. The default value is **false**.

To enable auditing it is necessary to add the **Audit** attribute to your method, informing the action type.

```C#
[Audit(AuditOperation.Create), MethodImpl(MethodImplOptions.NoInlining)]
public virtual User Add(User newUser, [MethodIsAuditableParam]bool methodIsAuditable = true)
{
    return _userRepository.Add(newUser);
}
```

 The return object will be audited, so it must inherit from the **AwesomeAuditing.IAuditableEntity** interface.
 
```C#
public class User : AwesomeAuditing.IAuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; }

    public ICollection<Role> Roles { get; set; }
}
```

It is necessary to always add the **Audit** attribute with the **MethodImpl(MethodImplOptions.NoInlining)** attribute. With this, the AwesomeAuditing project can work with multi-layer auditing. For example, if you have a generic repository class, you should add the **Audit** attribute to the generic **Add**, **Update** and **Remove** methods. This will enable auditing for these methods automatically. However, if you have a custom service, you can have an auditable method that calls one of these generic repository methods. AwesomeAuditing will not audit twice, because it has the capability to check if an auditable method is already inside of another auditable method.

It is necessary to configure right your DI container using **Castle DynamicProxy** to enable AwesomeAuditing usage. You need to register properly the classes and interfaces that contains auditable methods.

```C#
builder.RegisterGeneric(typeof(EntityRepository<>))
.As(typeof(IEntityRepository<>))
.EnableInterfaceInterceptors()
.InterceptedBy(typeof(AwesomeAuditingInterceptor));
                
builder.RegisterType<UserDomainService>()
.As<IUserDomainService>()
.EnableClassInterceptors()
.InterceptedBy(typeof(AwesomeAuditingInterceptor));
```

The auditable methods must be virtual and public (or protected). This way, **Castle DynamicProxy** will be able to create a decorator to your auditable class.

This repository has a **test project** that illustrates a simple use of AwesomeAuditing. It's worth taking a look over there to see how things work, specially the **AwesomeAuditingHandler**.
