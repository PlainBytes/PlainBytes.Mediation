# PlainBytes.Mediation

YAMI (Yet another mediator implementation).
Opinionated Mediator implementation to serve the authors needs. With the aim to keep it free and open-source.

## Getting Started

### Installation

Add the NuGet package to your project (replace with actual package name if published):

```
dotnet add package PlainBytes.Mediation.Mediator
```

### Usage

#### 1. Setup Dependency Injection and Mediator

```csharp
var services = new ServiceCollection();
services.AddMediator();
```

#### 2. Register Handlers

Register your command, query, and notification handlers:

```csharp
services.AddTransient<IRequestHandler<CreateUserCommand>, CreateUserCommandHandler>();
services.AddTransient<IRequestHandler<CreateUserWithIdCommand, int>, CreateUserWithIdCommandHandler>();
services.AddTransient<IRequestHandler<GetUserNameQuery, string>, GetUserNameQueryHandler>();
services.AddTransient<IRequestHandler<FailingQuery, string>, FailingQueryHandler>();
services.AddTransient<INotificationHandler<UserCreatedNotification>, SendToastNotificationHandler>();
services.AddTransient<INotificationHandler<UserCreatedNotification>, SendWelcomeEmailNotificationHandler>();
```

#### 3. Build Service Provider and Get Mediator

```csharp
var provider = services.BuildServiceProvider();
var mediator = provider.GetRequiredService<IMediator>();
```

#### 4. Example Usages

**Send a command (no return value):**
```csharp
await mediator.Send(new CreateUserCommand { UserName = "Alice" });
```

**Send a command (with return value):**
```csharp
var userId = await mediator.Send(new CreateUserWithIdCommand { UserName = "Bob" });
Console.WriteLine($"Returned user id: {userId}");
```

**Execute a query:**
```csharp
var userName = await mediator.Get(new GetUserNameQuery { UserId = userId });
Console.WriteLine($"Fetched user name: {userName}");
```

**Publish a notification:**
```csharp
await mediator.Publish(new UserCreatedNotification { UserName = userName });
```

#### 5. RequestResult Examples

**TrySend and TryGet return a RequestResult:**
```csharp
var nameResult = await mediator.TrySend(new CreateUserCommand { UserName = "Charlie" });
Console.WriteLine($"TrySend result: {nameResult}");

var idResult = await mediator.TrySend(new CreateUserWithIdCommand { UserName = "Diana" });
Console.WriteLine($"TrySend result: {idResult}");

var userNameResult = await mediator.TryGet(new GetUserNameQuery { UserId = 42 });
Console.WriteLine($"TryGet result: {userNameResult}");

var failedResult = await mediator.TryGet(new FailingQuery());
Console.WriteLine($"TryGet failed result: {failedResult}");
```

#### 6. Adding Logging Pipeline Behaviors

You can enable built-in logging behaviors for requests and notifications. These behaviors use Microsoft.Extensions.Logging.

```csharp
// Basic exception logging for requests & notifications
services.AddLoggingPipelineBehaviors();
```

The basic logging behaviors log errors when a request handler or notification handler throws.
For additional performance timing (logs successful completion time and failures):

```csharp
// Logs execution time and errors for requests, and errors for notifications
services.AddPerformanceLoggingPipelineBehaviors();
```

Choose only one of AddLoggingPipelineBehaviors or AddPerformanceLoggingPipelineBehaviors depending on whether you want timing information.

---

For more examples, see the `SampleApp/Program.cs` file in this repository.

