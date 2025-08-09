using Microsoft.Extensions.DependencyInjection;
using PlainBytes.Mediation.Mediator;
using PlainBytes.Mediation.Mediator.Contracts;
using PlainBytes.Mediation.Mediator.Results;
using SampleApp.Commands;
using SampleApp.Notifications;
using SampleApp.Queries;

// 1. Setup Dependency Injection and Mediator
var services = new ServiceCollection();
services.AddMediator();

// 2. Register Handlers
services.AddTransient<IRequestHandler<CreateUserCommand>, CreateUserCommandHandler>();
services.AddTransient<IRequestHandler<CreateUserWithIdCommand, int>, CreateUserWithIdCommandHandler>();

services.AddTransient<IRequestHandler<GetUserNameQuery, string>, GetUserNameQueryHandler>();
services.AddTransient<IRequestHandler<FailingQuery, string>, FailingQueryHandler>();

services.AddTransient<INotificationHandler<UserCreatedNotification>, SendToastNotificationHandler>();
services.AddTransient<INotificationHandler<UserCreatedNotification>, SendWelcomeEmailNotificationHandler>();

// 3. Build Service Provider and Get Mediator
var provider = services.BuildServiceProvider();
var mediator = provider.GetRequiredService<IMediator>();

// 4. Example Usages
// Send a command (no return value)
await mediator.Send(new CreateUserCommand { UserName = "Alice" });

// Send a command (with return value)
var userId = await mediator.Send(new CreateUserWithIdCommand { UserName = "Bob" });
Console.WriteLine($"Returned user id: {userId}");

// Execute a query
var userName = await mediator.Get(new GetUserNameQuery { UserId = userId });
Console.WriteLine($"Fetched user name: {userName}");

// Publish a notification
await mediator.Publish(new UserCreatedNotification { UserName = userName });

// 5. RequestResult Examples

var nameResult = await mediator.TrySend(new CreateUserCommand { UserName = "Charlie" });
Console.WriteLine($"TrySend result: {nameResult}");

var idResult = await mediator.TrySend(new CreateUserWithIdCommand { UserName = "Diana" });
Console.WriteLine($"TrySend result: {idResult}");

var userNameResult = await mediator.TryGet(new GetUserNameQuery { UserId = 42 });
Console.WriteLine($"TryGet result: {userNameResult}");

var failedResult = await mediator.TryGet(new FailingQuery());
Console.WriteLine($"TryGet failed result: {failedResult}");
