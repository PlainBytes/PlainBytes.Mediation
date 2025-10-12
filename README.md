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

## Benchmarks

Can be found in the benchmarks project, while highly depends on hardware here is a set of results for example:

BenchmarkDotNet v0.14.0, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD Ryzen 9 5900X, 1 CPU, 24 logical and 12 physical cores
.NET SDK 9.0.110
[Host]     : .NET 9.0.9 (9.0.925.41916), X64 RyuJIT AVX2
DefaultJob : .NET 9.0.9 (9.0.925.41916), X64 RyuJIT AVX2

### Commands and queries

All handlers are registered as transients.

Single and multiple execution of the same command, queries.

| Method          | Mean         | Error      | StdDev     | Gen0   | Allocated |
|---------------- |-------------:|-----------:|-----------:|-------:|----------:|
| Send_Command    |     99.35 ns |   1.949 ns |   1.823 ns | 0.0076 |     128 B |
| Get_Query       |    109.20 ns |   0.532 ns |   0.472 ns | 0.0076 |     128 B |
| Send_Command_1K | 82,010.86 ns | 411.569 ns | 364.846 ns | 7.5684 |  128000 B |
| Get_Query_1K    | 89,398.72 ns | 243.521 ns | 203.351 ns | 7.5684 |  128000 B |

### Notifications

All handlers are registered as transients.

1K notifications executed by the different strategies with 3 handlers, there is no work done, thus parallel executions overhead is visible as expected.

| Method                                     | Mean     | Error   | StdDev  | Gen0    | Allocated |
|------------------------------------------- |---------:|--------:|--------:|--------:|----------:|
| Publish_DefaultStrategy_MultipleHandlers   | 171.7 us | 0.50 us | 0.44 us | 15.6250 | 257.81 KB |
| Publish_StrategicNotifications_Synchronous | 200.3 us | 0.33 us | 0.29 us | 15.6250 | 257.81 KB |
| Publish_StrategicNotifications_Parallel    | 243.6 us | 0.83 us | 0.74 us | 32.4707 | 531.25 KB |



