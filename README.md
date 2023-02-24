# EmBrito.Dataverse.Extensions.Messaging


[![Nuget](https://img.shields.io/nuget/v/EmBrito.Dataverse.Extensions.Messaging)](https://www.nuget.org/packages/EmBrito.Dataverse.Extensions.Messaging)
[![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/emerbrito/dataverse-client-messaging/dotnet-build.yml)](https://github.com/emerbrito/dataverse-client-messaging/actions/workflows/dotnet-build.yml)

# Message Dispatcher

The `MessageDispatcher<TMessage>` dispatches a message to one or more handlers based on a message filter. The main purpose of the dispatcher is to abstract message handling.


``` mermaid
flowchart TB        
    B[Dispatcher] --> C{Dispatch}
    C -->|match handler| D[fa:fa-check Handler]
    C -->|match handler| E[fa:fa-check Handler]
    C -.-|mismatch| F[fa:fa-times Handler]
```

## Using The Message Dispatcher

Start by creating a dispatcher, which could be by instantiating the generic `MessageDispatcher<TMessage>` or extending it. Then create message handlers to handle the messages. 

You can also let your DI container take care of the the dispatcher and handlers initialization (see the Dependency Injection section for details).

### Creating a dispatcher

A generic dispatcher to push string messages down to one or more handlers:

``` csharp
public class HandleServiceBusFunction
{
    MessageDispatcher<string> _dispatcher;

    public HandleServiceBusFunction(MessageDispatcher<string> dispatcher)
    {
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
    }
}
```

This example will provide the exact same functionality as the previous but will result in code that is more readable and easier to understand, by extending the `MessageDispatcher<string>` and creating an specialized class:

``` csharp
public class StringMessageDispatcher : MessageDispatcher<string>
{
    public StringMessageDispatcher(IServiceProvider serviceProvider, ILogger<MessageDispatcher<string>> logger) 
        : base(serviceProvider, logger)
    {
    }
}

public class HandleServiceBusFunction
{
    StringMessageDispatcher _dispatcher;

    public HandleServiceBusFunction(StringMessageDispatcher dispatcher)
    {
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
    }
}

```

### Creating Message Handlers

Implement the `IMessageHandler<TMessage>` to create a message handler. See the Dependency Injection section to understand how handlers are registered with dispatchers.

``` csharp
public class MyContactHandler : IMessageHandler<string>
{
    public bool CanHandle(string message)
    {
        // this filter will determine whether the "Handle"
        // method will be called.
        return message.Contains("@");
    }

    public Task Handle(string message, CancellationToken cancellationToken)
    {
        // do something with the message
    }
}
```

Since handlers will be instantiated by the DI container you can specify dependencies in the constructor.

``` csharp
    public class MyUpdateHandler : IMessageHandler<string>
    {
        HttpClient _httpClient;

        public MyUpdateHandler(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException();
        }

        public bool CanHandle(string message)
        {
            throw new NotImplementedException();
        }

        public Task Handle(string message, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
```

# DataverseMessageDispatcher

The `DataverseMessageDispatcher` is an implementation of the message dispatcher meant to dipatch the dataverse `RemoteExecutionContext` (the message sent by the dataverse through the Azure service bus integration).

You should extend the `DataverseMessageHandler` abstract class to create your own handlers based on your application needs.

# Dependency Injection

Registering the built in `DataverseMessageDispatcher`. This will also register all `DataverseMessageHandler` in the specified assembly.

``` csharp
public override void Configure(IFunctionsHostBuilder builder)
{
    builder.Services.AddOptions();
    builder.Services.AddLogging();
    builder.Services.AddDataverseMessagetDispatcher(typeof(MyAzureFunction).Assembly)
}
```

The `AddMessagetDispatcher` extension method will allow you to register a dispatcher and specify the handlers associated with it.

Example 1: Limiting the numbers of handlers associated with the built in.`DataverseMessageDispatcher`.

``` csharp
public override void Configure(IFunctionsHostBuilder builder)
{
    builder.Services.AddOptions();
    builder.Services.AddLogging();
    builder.Services.AddMessagetDispatcher<DataverseMessageDispatcher>(options => 
    {
        options.AddHandler<ContactUpdateHandler>();
        options.AddHandler<AccountCreateHandler>();
    })
}
```

Example 2: Registering a custom dispatcher and associated message handlers.

``` csharp
public override void Configure(IFunctionsHostBuilder builder)
{
    builder.Services.AddOptions();
    builder.Services.AddLogging();
    .AddMessagetDispatcher<StringMessageDispatcher>(options =>
    {
        options.AddHandler<EmailAddressHandler>();
        options.AddHandler<PhoneNumberHandler>();
    })
}
```

Example 3: Registering a custom dispatcher, scanning assmebly and associating all handlers implemeting `IMessageHandler<string>`.

``` csharp
public override void Configure(IFunctionsHostBuilder builder)
{
    builder.Services.AddOptions();
    builder.Services.AddLogging();
    .AddMessagetDispatcher<StringMessageDispatcher>(options =>
    {
        options.ScanHandlers<IMessageHandler<string>>(new[] 
        { 
            typeof(MessageDispatcherTests).Assembly 
        });
    })
}
```

