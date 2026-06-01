using Atelier.Api._Exception;
using Atelier.Api._Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Atelier.Api.Tests.Middleware
{
    [TestClass]
    public class ExceptionHandlingMiddlewareTest
    {
        private Mock<ILogger<ExceptionHandlingMiddleware>> _loggerMock = null!;

        [TestInitialize]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        }

        #region Test Without Exception

        [TestMethod]
        public async Task InvokeAsync_NoException_LoggerIsNotCalled()
        {
            // Arrange
            var context = BuildHttpContext();
            var middleware = MiddlewareOk();

            // Act
            await middleware.InvokeAsync(context);

            // ASsert
            _loggerMock.Verify(
                l => l.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Never
            );
        }

        #endregion

        #region StatsCalculatorException Test

        [TestMethod]
        public async Task InvokeAsync_StatsCalculatorException_Returns500()
        {
            // Arrange
            var context = BuildHttpContext();
            var middleware = MiddlewareThrowing(new StatsCalculatorException("calc error"));

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.AreEqual(500, context.Response.StatusCode);
        }

        #endregion

        #region  NoDataException Test

        [TestMethod]
        public async Task InvokeAsync_NoDataException_Returns404()
        {
            // Arrange
            var context = BuildHttpContext();
            var middleware = MiddlewareThrowing(new NoDataException("no data"));

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.AreEqual(404, context.Response.StatusCode);
        }

        #endregion

        #region ArgumentException Test

        [TestMethod]
        public async Task InvokeAsync_ArgumentException_Returns400()
        {
            // Arrange
            var context = BuildHttpContext();
            var middleware = MiddlewareThrowing(new ArgumentException("bad arg"));

            // Act
            await middleware.InvokeAsync(context);

            // Asssert
            Assert.AreEqual(400, context.Response.StatusCode);
        }

        #endregion

        #region KeyNotFoundException Test

        [TestMethod]
        public async Task InvokeAsync_KeyNotFoundException_Returns404()
        {
            // Arrange
            var context = BuildHttpContext();
            var middleware = MiddlewareThrowing(new KeyNotFoundException("not found"));

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.AreEqual(404, context.Response.StatusCode);
        }

        #endregion

        #region InvalidOperationException Test

        [TestMethod]
        public async Task InvokeAsync_UnhandledException_Returns500()
        {
            // Arrange
            var context = BuildHttpContext();
            var middleware = MiddlewareThrowing(new InvalidOperationException("unexpected"));

            // Act
            await middleware.InvokeAsync(context);

            // Act
            Assert.AreEqual(500, context.Response.StatusCode);
        }

        #endregion

        #region Tests Helper

        private static HttpContext BuildHttpContext()
        {
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            return context;
        }

        private ExceptionHandlingMiddleware MiddlewareThrowing(Exception ex)
        {
            RequestDelegate next = _ => throw ex;
            return new ExceptionHandlingMiddleware(next, _loggerMock.Object);
        }

        private ExceptionHandlingMiddleware MiddlewareOk()
        {
            RequestDelegate next = _ => Task.CompletedTask;
            return new ExceptionHandlingMiddleware(next, _loggerMock.Object);
        }

        #endregion
    }
}
