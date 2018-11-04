using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FileProcessorTDXTechTest.FileHelpers;
using FileProcessorTDXTechTest.Models;
using FileProcessorTDXTechTestData;
using FileProcessorTDXTechTestData.Models;
using FluentAssertions;
using FluentValidation.Results;
using NSubstitute;
using NUnit.Framework;

namespace FileProcessorTDXTechTestTests
{
    public class ValidationHandlerTests
    {
        private IOrderRepository _orderRepo;
        private ValidationHandler _validationHandler;
        private Fixture _fixture;
        private static Guid _validOrderId;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _validOrderId = _fixture.Create<Guid>();
            _orderRepo = Substitute.For<IOrderRepository>();
            _orderRepo.OrderExistsAlready(Arg.Any<List<Guid>>()).Returns(false);
            _validationHandler = new ValidationHandler(_orderRepo);
        }

        [TearDown]
        public void Teardown()
        {
            _validationHandler = null;
            _orderRepo = null;
            _fixture = null;
        }

        [Test]
        public void HeaderValidator_ShouldReturnEmptyString_WhenHeaderIsValid()
        {
            //Arrange            
            var headerValues = new List<string> {"OrderId", "Country", "ItemType", "OrderDate", "UnitsSold", "UnitPrice"};

            //Act
            var sut = new ValidationHandler(_orderRepo).HeaderValidator(headerValues);

            //Assert
            sut.Should().Be(string.Empty);
        }

        [Test]
        public void HeaderValidator_ShouldReturnErrorMessage_WhenHeaderIsInvalid()
        {      
            var headerValues = new List<string> { "O", "Country", "ItemType", "OrderDate", "UnitsSold", "UnitPrice" };
            const string message = "Header is not in the right format";

            var sut = _validationHandler.HeaderValidator(headerValues);

            sut.Should().Be(message);
        }

        [Test]
        public void FileDataValidator_ShouldReturnValidResultAndZeroErrors_WhenInValidData()
        {
            var operationFormatError = new List<string>();
            
            var sut = _validationHandler.FileDataValidator(GetValidFileLoadedData1(), operationFormatError);

            sut.Should().NotBeNull();
            sut.Result.IsValid.Should().BeTrue();
            sut.Result.Errors.Count.Should().Be(0);
        }

        [Test]
        public void FileDataValidator_ShouldReturnError_WhenInValidData()
        {
            var invalidOrderid = new Guid();
            const string invalidCountry = "123456789123456789120";
            var operationFormatError = new List<string>();

            var sut = _validationHandler.FileDataValidator(GetInValidFileLoadedData(invalidOrderid, invalidCountry), 
                                                            operationFormatError);

            sut.Should().NotBeNull();
            sut.Result.IsValid.Should().BeFalse();
            sut.Result.Errors.Count.Should().Be(2);
        }

        [Test]
        public void FileDataValidator_ShouldReturnErrorMessage_WhenIsValidDataAndOrderIdAlreadyExistsInDb()
        {
            var operationFormatError = new List<string>();
            _orderRepo.OrderExistsAlready(Arg.Any<List<Guid>>()).Returns(true);

            var loadedFileData = GetValidFileLoadedData1();
            _validationHandler.FileDataValidator(loadedFileData, operationFormatError);

            var message = $"The Order {loadedFileData.FileRows.FirstOrDefault().OrderId} is already in the system";
            operationFormatError.FirstOrDefault().Should().Be(message);
        }

        [Test]
        public void FileFormatValidator_ShouldNotBeBadFormat_WhenAnyDataInTheFileIsRightFormat()
        {
            var fileLoaded = new LoadedFileRows();
            var operationFormatErrors = new List<string>();

            var sut = _validationHandler.FileFormatValidator(ValidFileLoadedData2, fileLoaded, operationFormatErrors);

            sut.Should().BeFalse();
        }

        [Test]
        public void GetOperationResult_ShouldReturnObject()
        {
            IList<ValidationFailure> validationErrors = new List<ValidationFailure>();
            var fileLoaded = new LoadedFileRows();
            var operationErrors = new List<string>();

            var sut = _validationHandler.GetOperationResult(fileLoaded, validationErrors, operationErrors);

            sut.Should().BeOfType<OperationResult<LoadedFileRows>>();
        }

        private static LoadedFileRows GetValidFileLoadedData1()
        {
            return new LoadedFileRows
            {
                FileRows = new List<Order>
                {
                    new Order
                    {
                        OrderId = _validOrderId,
                        Country = "England",
                        ItemType = "Cosmetics",
                        OrderDate = new DateTime(2018, 09, 18),
                        UnitsSold = 8446,
                        UnitPrice = 437.2
                    }
                }
            };
        }

        private static IEnumerable<Order> ValidFileLoadedData2 => new List<Order>
        {
            new Order
            {
                OrderId = _validOrderId,
                Country = "England",
                ItemType = "Cosmetics",
                OrderDate = new DateTime(2018, 09, 18),
                UnitsSold = 8446,
                UnitPrice = 437.2
            }
        };

        private static LoadedFileRows GetInValidFileLoadedData(Guid orderId, string country)
        {
            return new LoadedFileRows
            {
                FileRows = new List<Order>
                {
                    new Order
                    {
                        OrderId = orderId,
                        Country = country,
                        ItemType = "Cosmetics",
                        OrderDate = new DateTime(2018, 09, 18),
                        UnitsSold = 8446,
                        UnitPrice = 437.2
                    }
                }
            };
        }
    }
}
