using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Up_To_Date__UTD_.Controllers;
using Up_To_Date__UTD_.Models;

namespace Up_To_Date__UTD_.Tests.UnitTests.Controllers
{
    public class HomeControllerTests
    {
        private readonly Mock<ILogger<HomeController>> _loggerMock; // Mock for the logger
        private readonly HomeController _controller; // Instance of the controller being tested

        // Constructor to initialize the mock and the controller
        public HomeControllerTests()
        {
            _loggerMock = new Mock<ILogger<HomeController>>();
            _controller = new HomeController(_loggerMock.Object);
        }

        // Test for the Index action
        [Fact]
        public void Index_ReturnsViewResult()
        {
            var result = _controller.Index(); 
            var viewResult = Assert.IsType<ViewResult>(result); 
            Assert.Null(viewResult.ViewName); 
        }

        // Test for the Privacy action
        [Fact]
        public void Privacy_ReturnsViewResult()
        {
            var result = _controller.Privacy();
            var viewResult = Assert.IsType<ViewResult>(result); 
            Assert.Null(viewResult.ViewName); 
        }
    }
}
