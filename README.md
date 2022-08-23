# %PROJECTNAME% API

1. [Getting Started](#getting-started)
2. [Configuration](#configuration)
3. [Database](#database)
   1. [Configuring the Database](#configuring-the-database)
   2. [Modifying the Database](#modifying-the-database)
   3. [Data Model Rules and Conventions](#data-model-rules-and-conventions)
   4. [Data Seeding](#data-seeding)
4. [Exception Handling](#exception-handling)
5. [Dependency Injection](#dependency-injection)
6. [Data Transfer Objects (DTOs) and Mapping](#data-transfer-objects-dtos-and-mapping)
7. [C# Coding Standards and Best Practices](#c-coding-standards-and-best-practices)
8. [System Architecture](#system-architecture)
  

## Getting Started
This project has the following system dependencies.
- .NET 6.0 SDK ([Installers](https://dotnet.microsoft.com/en-us/download/dotnet/6.0))
- MS SQL Server ([Windows](https://www.microsoft.com/en-us/sql-server/sql-server-downloads))
    - Mac/Linux Users can run a SQL server instance in a Docker Container [Instructions](https://docs.microsoft.com/en-us/sql/linux/quickstart-install-connect-docker?view=sql-server-ver15&pivots=cs1-bash)

## Configuration

Duplicate the `appsettings.json` file at the root of the `%PROJECTNAME%.Api` project directory. Rename the duplicate as `appsettings.Development.json`. Update the new settings document with the configuration for your local environment. Any JSON block in the `appsettings.Development.json` will override values from `appsetings.json` and may be deleted if no override is required.

##### Example `appsettings.development.json`
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=localhost;Database=%PROJECTNAME%;Trusted_Connection=True;"
  },
  "JwtBearer": {
    "Authority": "https://sts.windows.net/9297e4c4-aec3-4611-bdfe-1a2ee661384d/"
  },
  "Serilog": {
    "WriteTo": [
      {
        "Name": "Console",
        "Theme": "AnsiConsoleTheme.Code"
      }
    ]
  }
}
```

## Database

Interactions with the Database rely on [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/). Download the Entity Framework CLI by running the following terminal command `dotnet tool install --global dotnet-ef`.

### Configuring the Database

On startup, the application will initialize the database(s) using Entity Framework migrations. Providing connection strings for a pre-existing database with mismatched migrations will cause the application to fail on startup.
###### `appsettings.json` connection string block
```json
  "ConnectionStrings": {
    "DefaultConnection": "",
  }
```
###### Connection string example for Windows SQL Server instance
```json
"DefaultConnection": "Data Source=localhost;Database=MIRT;Trusted_Connection=True;"
```
###### Connection string example for Docker Container SQL Server instance
```json
"DefaultConnection": "Server=localhost,1433;Database=MIRT;User=sa;Password=<YOUR PASSWORD>;"
```

### Modifying the Database

All modifications to the data model must go through the EF Migrations API.
- **Never** run raw SQL scripts that modify the database schema
- **Never** use SQL views or any other database-first result sets.
- **Consider** alternatives to SQL triggers, functions or stored procedures. If no alternative is acceptable, add them though EF migrations.
    - Manually added migration operations must include an UP and a DOWN version.

Migrations are added through the `dotnet ef` CLI command. To run the command you must specify the startup assembly using the `--startup` or `-s` flag, and the migrations assembly with the `--project` or `-p` flag.

For this project, the startup assembly is `%PROJECTNAME%.Api` and the migrations assembly is `%PROJECTNAME%.Data`.

You may run the following commands from the root directory of this repository:

##### Add Migration
`dotnet ef migrations add My_Migration_Name -s %PROJECTNAME%.Api -p %PROJECTNAME%.Data`

##### Remove Previous Migration
`dotnet ef migrations remove -s %PROJECTNAME%.Api -p %PROJECTNAME%.Data`

##### Apply All Migrations
`dotnet ef database update -s %PROJECTNAME%.Api -p %PROJECTNAME%.Data`

##### Rollback to a Migration
`dotnet ef database update Target_Migration_Name -s %PROJECTNAME%.Api -p %PROJECTNAME%.Data`

##### Drop Database
`dotnet ef database drop -s %PROJECTNAME%.Api -p %PROJECTNAME%.Data`

### Data Model Rules and Conventions

- **Do** use the name `Id` for all primary keys.
- **Do** use the convention \<EntityName\>Id for all foreign keys
- **Do** provide navigation properties to owned entities and collections in one-to-many relationships.
- **Do not** use attributes or data annotations to configure tables.
- **Do** use join tables to represent many-to-many relationships
    - Use `"{EntityName1}{EntityName2}{EntityName3}..."` naming convention.
- **Do** use Entity Framework's fluent api to configure tables.
    - Create a class for the table in the `%PROJECTNAME%.Data/EntityConfigurations` directory
    - Use the naming convention '\<EntityName\>Configuration'
    - Implement the `IEntityTypeConfiguration<T>` interface
- **Do not** put methods or business logic in your entity classes.
- **Do not** use presentation layer data annotations in entities (e.g `[JsonProperty]`,`[DisplayName]`,`[Required]`)

##### Correct
```csharp
public class MyEntity : IUnique<int>
{
    public int Id { get; set; }
    public string Column1 { get; set; }
    public bool Column2 { get; set; }
    public int RelatedEntityId { get; set; }
    public RelatedEntity RelatedEntity { get; set; }  // Navigation Property will be configured inferred by naming convention
    public int? OptionalRelatedEntityId { get; set; }
    public OptionalRelatedEntity OptionalRelatedEntity { get; set; }   // Navigation Property will be configured inferred by naming convention
    public List<ManyEntity> ManyEntites { get; set; } // One-to-Many
    public List<MyEntityOtherEntity> MyEntityOtherEntites { get; set; } // Many-to Many
}
public class MyEntityConfiguration : IEntityTypeConfiguration<MyEntity>
{
    public void Configure(EntityTypeBuilder<MyEntity> builder)
    {
        builder.HasIndex(x => x.Column1).IsUnique();
    }
}
```
###### Incorrect
```csharp
[Index(nameof(Column1), IsUnique = true)]
public class MyEntity
{
    [Key]
    public int MyEntityId { get; set; }
    public string Column1 { get; set; }
    public bool Column2 { get; set; }
    [ForeignKey(nameof(RelatedEntity))]
    public int RelatedId { get; set; }
    public RelatedEntity RelatedEntity { get; set; } 
}
```

### Data Seeding

Data can be seeded into the database through the Migrations API and a base class has been provided to simplify this process. 
Any data seeds should be placed in the `%PROJECTNAME%.Data/Seeds` directory and implement the `DataSeeder<T>` abstract class. 
Objects that implement this class will be found by the EF model builder and automatically added to the Model Snapshot.

### Special Application Interfaces
- Unless an entity is using a composite PK, all entities should implement the `IUnique<T>` interface.
  - This will enforce the `Id` property, and can be used for equality comparison by consuming frameworks like `AutoMapper`.
- Entities that require timestamps for their creation and last update should implement the `ITracked` interface.
  - Entities that implement this interface will automatically have their `Created` and `Updated` timestamps set when they are added or updated.
  - Entites that are added via `IEntityTypeConfiguration` circumvent this mechanism and will need their `Created` time updated manually
- Entities that need to persist in the database after their 'deletion' should implement `ISoftDelete` interface.
  - Entities that implement this interface can not be permanently deleted through the `DbContext`. Instead they will be flagged as `Deleted` and filtered from all future queries on the table.

## Exception Handling
This application uses custom `ExceptionHandlingMiddleware` to provide a commonly typed result for all error responses. On a system error, all HTTP requests 
will respond with an ExceptionModel.

Note that the StackTrace is only sent while the server is running in a development configuration, and will not be serialized in other environments.
```json
{
  "status" : "BadRequest",
  "statusCode" : 400,
  "message" : "Invalid Request",
  "stackTrace": "...stacktrace of the error"
}
```

Consuming typescript clients can type all server errors using the following interface:
```typescript
interface %PROJECTNAME%ApiException {
  status: string;
  statusCode: number;
  message: string;
  stackTrace?: string;
}
```

## Dependency Injection
.NET Core Applications rely heavily on dependency injection and an object-oriented design pattern called [Inversion of Control](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-6.0).
The *only* objects that should be explicitly instantiated with the `new` keyword are data storage objects. Service objects (Controllers, Managers, Engines etc.) should *never* be created explicitly and must *always* be provided 
by the DI container.

By convention, service classes should have their implementation defined by an interface so that their consumers are decoupled from the interface's implementation. Interfaces and their implementing classes 
should be declaratively paired within the DI container on application startup.

External libraries and core functions should be registered within the `ConfigureServices` method within the `StartUp` class. Core business logic for the %PROJECTNAME% API should be declared within the
`Add%PROJECTNAME%Core` extension method in the `%PROJECTNAME%CoreRegistrar` static class.

The `IServiceCollection` DI container abstraction provides three service lifetimes, and services should be registered per the following rules:

Use `AddSingleton<T1,T2>()` when registering a service that should only be instantiated a single time throughout the entire lifetime of the system. Common use cases for singletons 
include objects that maintain critical global state (like an OIDC configuration) or are computationally expensive to rebuild (like the AutoMapper mapper).

Use `AddTransient<T1,T2>()` when registering a service that should be created `new` *every* time it is requested by a service. Common use cases for transients 
include objects that provide utility methods (like factory classes or value converters). Transient classes are able to be quickly deconstructed by the garbage collector as they 
often fall out of scope shortly after they are used.

Use `AddScoped<T1,T2>()` when registering a service that should have a singleton lifetime within the scope of an HTTP Request. Think of Scoped services like you would think of 
`AsyncLocalStorage` in a javascript application. They provide persisted state across a single asynchronous method chain, and then fall out of scope at then end of the request. 
EntityFramework's `DbContext` is a scoped service by default, as it persists change tracking and uses database transactions across a single request. All core services that are 
used within the vertical slice of your API endpoints should be scoped services (Controllers, Managers, Engines, DataAccess).

## Data Transfer Objects (DTOs) and Mapping
This system maintains an explicit separation between its data-model classes (entities) and its business layer data contracts (DTOs).
The main reasons for this are as follows:
- Entity classes are usually referenced by the EntityFramework change tracker, and manipulating an entity class could result in unintentional modifications to the database.
- Entity classes are often bloated with navigation properties, or may contain sensitive data, both of which generally want to be obscured or protected from being exposed to the business layer.
- Business Layer classes are often designed around the *use case*, whereas entity classes are often designed around *database normalization*. By keeping the classes seperated, they are allowed to be pure in their purpose.

In general, most API use cases are simple CRUD operations that involve request and response objects that closely mirror the shapes of the entities. It is therefore tempting 
to just use the entities directly. To simplify the translation between the two related objects, this application uses the `AutoMapper` library to swap data between the two layers.

To declare a map, create a new class in the `%PROJECTNAME%.Manager/Mapping` directory and give it the `{ENTITYNAME}Profile` naming convention. This class must inherit from `Profile`.

Consider this use case with an entity that contains a private sequential primary key, and a GUID "public id" that it wishes to expose to the frontend.
```csharp
public class MyEntity : IUnique<int>, ITracked, ISoftDelete
{
    public int Id { get; set; }
    public string PublicId { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? Deleted { get; set; }
}
```
```csharp
public class MyEntityModel 
{
    public string Id { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
}
```
##### Mapping Profile
```csharp
public class MyEntityProfile : Profile
{
    public UserProfileProfile()
    {
        CreateMap<MyEntity, MyEntityModel>()
            .ForMember(model => model.Id, opt=> opt.MapFrom(entity => entity.PublicId));
    }
}
```

In this example, the `Created` and `Updated` properties will be mapped automatically, as they can be inferred through naming convention. The `Id` field on the model has 
been explicitly mapped from the `PublicId` field on the entity. All other entity fields will be ignored, guaranteeing proper data sanitization.

## C# Coding Standards and Best Practices
### Naming Styles and Conventions
1. Use Pascal casing for types, method names, constants, protected fields, and properties.
```csharp
public class MyClass
{
   private const string MyConstant = "Public Constant";
   protected int MyProtectedField;
   
   public int MyProperty { get; set; }
   
   public void MyMethod()
   {
      ...
   }
}
```
2. Use Camel casing for local variable names and method arguments
```csharp
public class MyClass
{
   public void MyMethod(int myArgument)
   {
      var myVariable = 12;
   }
}
```
3. Prefix private fields with _. Use camel casing for the rest of the variable name.
```csharp
public class MyClass
{
   private int _myPrivateField;
}
```
4. Prefix interface names with 'I'.
```csharp
public interface IMyInterface
{
}
```
5. Suffix custom attribute classes with `Attribute`.
6. Suffix custom exception classes with `Exception`
7. Name methods using verb-object pair, such as `ShowDialog()`.
8. Methods with return values should have a name describing the value returned, such as `GetObjectState()`.
9. Use descriptive variable names.
- Avoid using single character variable names, such as `i` or `t`. Use `index` or `temp` instead.
- Do not abbreviate words (such as `num` instead of `number`).
10. Always use C# predefined types rather than the aliases in the System namespace.
###### Correct
```csharp
public class MyClass
{
   private int _myNumber;
   private string _myString;
   private object _myObject;
}
```
###### Incorrect
```csharp
public class MyClass
{
   private Int32 _myNumber;
   private String _myString;
   private Object _myObject;
}
```
11. Generic Parameters must always start with 'T'
- **Do** name generic type parameters with descriptive names, unless a single letter name is completely self explanatory and a descriptive name would not add value.
- **Consider** `T` as a parameter name for generics with a single parameter, *except* in cases where it would create a conflict (generic method within a generic class).
###### Correct
```csharp
public interface ISessionChannel<TSession> { /*...*/ }
public delegate TOutput Converter<TInput, TOutput>(TInput from);
public class List<T> { /*...*/ }
public class Tuple<T1, T2>{ /*...*/ } //<-- Acceptable if completely self-explanatory
```
###### Incorrect
```csharp
public class LinkedList<K, T>{ /*...*/ }
public class LinkedList<KeyType, DataType>{ /*...*/ }
```
12. Namespaces/Libraries should follow a `<Company>.(<Product>|<Technology>)[.<Feature>][.<Subnamespace>]` template.
13. Avoid fully qualified type names. Use the `using` statement instead.
14. Avoid putting a `using` statement inside a namespace.
15. Use delegate inference instead of explicit delegate instantiation.

```cs
delegate void MyDelegate();
public void MyMethod() {...}
MyDelegate myDelegate = MyMethod;
```

16. All member variables should be declared at the top, with one line separating them from the properties or methods.

```cs
public class MyClass
{
    public const int MyConstant = 1;
    private int _myPrivateField;
    protected int MyProtectedField;
    
    public int MyProperty { get; set; }
    
    public MyClass() {..} //Constructor 

    public void MyMethod1() {...}
    public void MyMethod2() {...}
    
    protected virtual MyVirtualMethod() {...}
    
    private void MyPrivateMethod() {...}
}
```

17. Declare a local variable as close as possible to its first use.
18. A file name should reflect the class it contains.
19. When using partial types and allocating a part per file, name each file after the logical part that part plays.
20. Always open curly braces ({) in a new line.

### Naming Styles and Conventions
1. **Do not** put multiple classes in a single file.
2. **Do not** have multiple namespaces in the same file.
3. **Avoid** files with more than 500 lines.
4. **Avoid** methods with more than 200 lines.
5. **Avoid** methods with more than 5 arguments. Use structures for passing multiple arguments.
7. **Do not** manually edit any machine-generated code.
8. **Avoid** comments that explain the obvious. Code should be self-explanatory. Good code with readable variable and method names should not require comments.
10. With the exception of zero and one, never hard-code a numeric value; always declare a constant instead.
12. Avoid using const on read-only variables. For that, use the `readonly` directive.

### System Architecture
###### Source: [Righting Software](https://www.amazon.com/Righting-Software-Juval-L%C3%B6wy-ebook/dp/B0822XJZ48/ref=sr_1_1?dchild=1&keywords=righting+software&qid=1614284141&sr=8-1) by Juval Lowy

This system is designed using a layered approach. Layers encapsulate application responsibility and promote
- Consistency
- Scalability
- Fault Isolation
- Security
- Separation of Presentation from logic
- Availability
- Throughput
- Responsiveness
- Synchronization

#### Definition of Layers
##### Client/Presentation
`%PROJECTNAME%.Api`
- Represents a unique interface where application services are exposed to a user or another system.
- Can be any of a variety of client application technologies (Razor/Web API/WPF/Xamarin/etc.)

##### Business
`%PROJECTNAME%.Core`
- Manager namespace encapsulates sequences in use cases and workflows.
  - Each service is a collection of related use cases
- Engine namespace encapsulate business rules
- Managers may use any number of engines
- Engines may be shared between managers

##### Data Access
`%PROJECTNAME%.Data`
- Data namespace encapsulates resource access
- May call into resources stored in external services
- Can be shared across engines and service components

##### Common/Utilities
`%PROJECTNAME%.Utility`
- Common infrastructure across all layers of the application

#### Rules

Between layers, pass only
- Primitives
- Arrays of Primitives
- Data Contracts
- Arrays of Data Contracts

Data Contracts and Entities should not contain business logic, as this logic would cross layers
and break encapsulation.

1. _Clients_ **do not** call multiple _Services_ in a single use case.
2. _Clients_ **do not** call _Engines_
3. _Services_ **do not** queue calls to more than one _Service_ in a single use case.
4. _Engines_ **do not** receive queued calls.
5. _Data Access_ components **do not** receive queued calls.
6. _Clients_ **do not** publish events.
7. _Engines_ **do not** publish events.
8. __Data Access__ components **do not** publish events.
9. _Engines_, and _Data Access_ **do not** subscribe to events.
10. _Engines_ **never** call each other.
11. _Data Access_ components **never** call each other.