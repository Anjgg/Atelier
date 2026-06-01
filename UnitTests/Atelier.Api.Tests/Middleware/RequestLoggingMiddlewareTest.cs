using Atelier.Api._Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace Atelier.Api.Tests.Middleware
{
    [TestClass]
    public class RequestLoggingMiddlewareTest
    {
        private Mock<ILogger<RequestLoggingMiddleware>> _loggerMock = null!;

        [TestInitialize]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<RequestLoggingMiddleware>>();
        }

        #region Middleware Logging Tests

        [TestMethod]
        public async Task InvokeAsync_Always_LogsInformationExactlyTwice()
        {
            var context = BuildContext();
            var middleware = BuildMiddleware(_ => Task.CompletedTask);

            await middleware.InvokeAsync(context);

            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception?>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Exactly(2)
            );
        }

        [TestMethod]
        public async Task InvokeAsync_InboundLog_ContainsMethod()
        {
            var messages = CaptureLogMessages();
            var context = BuildContext(
                method: "POST",
                path: "/api/players",
                remoteIp: IPAddress.Parse("192.168.1.1"),
                user: AuthenticatedUser("bob"));
            context.Response.StatusCode = 201;
            var middleware = BuildMiddleware(_ => Task.CompletedTask);
            

            await middleware.InvokeAsync(context);

            StringAssert.Contains(messages[0], "POST");
            StringAssert.Contains(messages[0], "/api/players");
            StringAssert.Contains(messages[0], "192.168.1.1");
            StringAssert.Contains(messages[1], "201");
            StringAssert.Contains(messages[1], "POST");
            StringAssert.Contains(messages[1], "/api/players");
            StringAssert.Contains(messages[1], "bob");

        }

        #endregion

        #region Tests Helper

        private static DefaultHttpContext BuildContext(
            string method = "GET",
            string path = "/api/test",
            string? queryString = null,
            IPAddress? remoteIp = null,
            ClaimsPrincipal? user = null)
        {
            var context = new DefaultHttpContext();
            context.Request.Method = method;
            context.Request.Path = path;
            context.Request.QueryString = queryString != null
                ? new QueryString(queryString)
                : QueryString.Empty;

            context.Connection.RemoteIpAddress = remoteIp ?? IPAddress.Parse("127.0.0.1");

            if (user != null)
                context.User = user;

            return context;
        }

        private static ClaimsPrincipal AuthenticatedUser(string name = "alice")
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.NameIdentifier, "1")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            return new ClaimsPrincipal(identity);
        }
        
        private List<string> CaptureLogMessages()
        {
            var messages = new List<string>();

            _loggerMock
                .Setup(l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception?>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
                .Callback<LogLevel, EventId, object, Exception?, Delegate>(
                    (_, _, state, _, formatter) =>
                        messages.Add(formatter.DynamicInvoke(state, null) as string ?? string.Empty)
                );

            return messages;
        }

        private RequestLoggingMiddleware BuildMiddleware(RequestDelegate next)
            => new RequestLoggingMiddleware(next, _loggerMock.Object);

        #endregion
    }
}
