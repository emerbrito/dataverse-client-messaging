using EmBrito.Dataverse.Extensions.Messaging;
using EmBrito.Dataverse.Extensions.Tests.Messaging.MessageHandlers;
using EmBrito.Dataverse.Extensions.Tests.Messaging.Dispatchers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xrm.Sdk;

namespace EmBrito.Dataverse.Extensions.Tests.Messaging
{
    public class MessageDispatcherTests
    {
        [Fact]
        public async Task Can_Register_Dataverse_Dispatcher_Plus_All_Handlers()
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddDataverseMessagetDispatcher(typeof(MessageDispatcherTests).Assembly)
                .BuildServiceProvider();

            var orgname = "myorg1";
            var entityname = "account";
            var service = serviceProvider.GetService<DataverseMessageDispatcher>();
            var message = new RemoteExecutionContext
            {
                PrimaryEntityName = entityname,
                OrganizationName = orgname
            };

            // configure handler filters to ensure at least 3 handlers will be used.
            DataverseUpdateEntityNameHandler.EntityNameFilter = entityname;
            DataverseFilterByEntityNameHandler.EntityNameFilter = entityname;

            var handlerCount = await service.Dispatch(message, default);

            Assert.True(handlerCount >= 3);
            Assert.Equal(DataverseUpdateOrgNameHandler.FormatOrgName(orgname), DataverseUpdateOrgNameHandler.LastUpdatedOrgName);
            Assert.Equal(DataverseUpdateEntityNameHandler.FormatEntityName(entityname), DataverseUpdateEntityNameHandler.LastUpdatedEntityName);
        }

        [Fact]
        public async Task Can_Register_Dataverse_Dispatcher_With_Selected_Handlers()
        {

            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddMessagetDispatcher<DataverseMessageDispatcher>(options =>
                {
                    options.AddHandler<DataverseUpdateEntityNameHandler>();
                    options.AddHandler<DataverseFilterByEntityNameHandler>();
                })
                .BuildServiceProvider();

            var orgname = "myorg2";
            var entityname = "contact";
            var service = serviceProvider.GetService<DataverseMessageDispatcher>();
            var message = new RemoteExecutionContext
            {
                PrimaryEntityName = entityname,
                OrganizationName = orgname
            };

            DataverseUpdateEntityNameHandler.EntityNameFilter = entityname;
            DataverseFilterByEntityNameHandler.EntityNameFilter = entityname;

            var handlerCount = await service.Dispatch(message, default);

            Assert.Equal(2, handlerCount);
            Assert.Equal(DataverseUpdateEntityNameHandler.FormatEntityName(entityname), DataverseUpdateEntityNameHandler.LastUpdatedEntityName);

        }

        [Fact]
        public async Task Can_Register_Custom_Dispatcher_With_Selected_Handlers()
        {

            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddMessagetDispatcher<StringMessageDispatcher>(options =>
                {
                    options.AddHandler<StringEmptyMessageHandler>();
                    options.AddHandler<StringFilteredHandler>();
                    options.AddHandler<StringUpdateStringHandler>();
                })
                .BuildServiceProvider();

            var message = "message";
            var service = serviceProvider.GetService<StringMessageDispatcher>();

            StringFilteredHandler.StringToMatch = "donotmatch";
            StringUpdateStringHandler.StringToMatch = message;

            var handlerCount = await service.Dispatch(message, default);

            Assert.Equal(2, handlerCount);
            Assert.Equal(StringUpdateStringHandler.FormatString(message), StringUpdateStringHandler.LastUpdatedString);

        }

        [Fact]
        public async Task Can_Register_Custom_Dispatcher_With_Assembly_Scan()
        {

            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddMessagetDispatcher<StringMessageDispatcher>(options =>
                {
                    options.ScanHandlers<IMessageHandler<string>>(new[] { typeof(MessageDispatcherTests).Assembly });
                })
                .BuildServiceProvider();

            var message = "message";
            var service = serviceProvider.GetService<StringMessageDispatcher>();

            StringFilteredHandler.StringToMatch = "donotmatch";
            StringUpdateStringHandler.StringToMatch = message;

            var handlerCount = await service.Dispatch(message, default);

            Assert.Equal(2, handlerCount);
            Assert.Equal(StringUpdateStringHandler.FormatString(message), StringUpdateStringHandler.LastUpdatedString);
        }

        [Fact]
        public async Task Can_Register_Custom_Dispatcher_Without_Extend_Class()
        {

            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddMessagetDispatcher<MessageDispatcher<string>>(options =>
                {
                    options.AddHandler<StringEmptyMessageHandler>();
                    options.AddHandler<StringFilteredHandler>();
                    options.AddHandler<StringUpdateStringHandler>();
                })
                .BuildServiceProvider();

            var message = "message2";
            var service = serviceProvider.GetService<MessageDispatcher<string>>();

            StringFilteredHandler.StringToMatch = "donotmatch";
            StringUpdateStringHandler.StringToMatch = message;

            var handlerCount = await service.Dispatch(message, default);

            Assert.Equal(2, handlerCount);
            Assert.Equal(StringUpdateStringHandler.FormatString(message), StringUpdateStringHandler.LastUpdatedString);

        }

        [Fact]
        public async Task Can_Filter_Handler()
        {

            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddMessagetDispatcher<DataverseMessageDispatcher>(options =>
                {
                    options.AddHandler<DataverseUpdateEntityNameHandler>();
                    options.AddHandler<DataverseFilterByEntityNameHandler>();
                })
                .BuildServiceProvider();

            var orgname = "myorg3";
            var entityname = "opportunity";
            var service = serviceProvider.GetService<DataverseMessageDispatcher>();
            var message = new RemoteExecutionContext
            {
                PrimaryEntityName = entityname,
                OrganizationName = orgname
            };

            DataverseUpdateEntityNameHandler.LastUpdatedEntityName = null;
            DataverseUpdateEntityNameHandler.EntityNameFilter = "account";
            DataverseFilterByEntityNameHandler.EntityNameFilter = entityname;

            var handlerCount = await service.Dispatch(message, default);

            Assert.Equal(1, handlerCount);
            Assert.Null(DataverseUpdateEntityNameHandler.LastUpdatedEntityName);

        }

        [Fact]
        public void Can_Prevent_Handler_Mismatch()
        {

            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddMessagetDispatcher<StringMessageDispatcher>(options =>
                {
                    options.AddHandler<StringEmptyMessageHandler>();
                    options.AddHandler<DataverseFilterByEntityNameHandler>();
                })
                .BuildServiceProvider();

            Assert.Throws<ArgumentException>(() =>
            {
                var service = serviceProvider.GetService<StringMessageDispatcher>();
            });

        }

        [Fact]
        public async Task Can_Prevent_Duplicate__Assembly_Scan_Registration()
        {

            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddMessagetDispatcher<StringMessageDispatcher>(options =>
                {
                    options.ScanHandlers<IMessageHandler<string>>(new[] { typeof(MessageDispatcherTests).Assembly });
                    options.ScanHandlers<IMessageHandler<string>>(new[] { typeof(MessageDispatcherTests).Assembly });
                })
                .BuildServiceProvider();

            var message = "message";
            var service = serviceProvider.GetService<StringMessageDispatcher>();

            StringFilteredHandler.StringToMatch = "donotmatch";
            StringUpdateStringHandler.StringToMatch = message;

            var handlerCount = await service.Dispatch(message, default);

            Assert.Equal(2, handlerCount);
            Assert.Equal(StringUpdateStringHandler.FormatString(message), StringUpdateStringHandler.LastUpdatedString);
        }

    }
}