using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoFixture;
using FileProcessorTDXTechTest.FileHelpers;
using FileProcessorTDXTechTest.Models;
using FileProcessorTDXTechTestData;
using FileProcessorTDXTechTestData.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NUnit.Framework;
using FluentValidation.Results;

namespace FileProcessorTDXTechTestTests
{
    public class FileProcessorTests
    {
        private IOrderRepository _orderRepo;
        private Fixture _fixture;
        private static Guid _validOrderId;
        private IValidationHandler _validationHandler;
        private FileProcessor _fileProcessor;
        private IFormFile _file;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _validOrderId = _fixture.Create<Guid>();
            _orderRepo = Substitute.For<IOrderRepository>();
            _validationHandler = Substitute.For<IValidationHandler>();

            _file = Substitute.For<IFormFile>();
            var content = "File Content";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            _file.OpenReadStream().Returns(ms);
            _file.FileName.Returns("fileName");
            _file.Headers.Returns(new HeaderDictionary());
            _file.Length.Returns(ms.Length);

            _validationHandler.FileDataValidator(
                    Arg.Any<LoadedFileRows>(),
                    Arg.Any<List<string>>())
                    .Returns(new ValidationResult());

            _fileProcessor = new FileProcessor(_orderRepo, _validationHandler);

            _validationHandler.GetOperationResult(Arg.Any<LoadedFileRows>(),
                    Arg.Any<IList<ValidationFailure>>(),
                    Arg.Any<List<string>>())
                .Returns(ValidFileDataForOperationResult());
        }

        [TearDown]
        public void Teardown()
        {
            _validationHandler = null;
            _fixture = null;
            _validationHandler = null;
        }

        [Test]
        public void FileConversion_ShouldOperationResultGetFileData_WhenFileHasValidFormatAndNoErrors()
        {          
            var sut =_fileProcessor.FileConversion(_file);
;
            sut.OperationValue.FileRows.FirstOrDefault()?.OrderId.Should().Be(_validOrderId);
            sut.OperationValue.FileRows.FirstOrDefault()?.Country.Should().Be("England");
            sut.OperationValue.FileRows.FirstOrDefault()?.ItemType.Should().Be("Cosmetics");
            sut.OperationValue.FileRows.FirstOrDefault()?.OrderDate.Should().Be(new DateTime(2018, 09, 18));
            sut.OperationValue.FileRows.FirstOrDefault()?.UnitsSold.Should().Be(8446);
            sut.OperationValue.FileRows.FirstOrDefault()?.UnitPrice.Should().Be(437.2);

            sut.OperationFormatError.Should().BeEmpty();
            sut.OperationValidationError.Should().BeEmpty();
        }

        private OperationResult<LoadedFileRows> ValidFileDataForOperationResult()
        {
            return new OperationResult<LoadedFileRows>
            {
                OperationFormatError = new List<string>(),
                OperationValidationError = new List<ValidationFailure>(),
                OperationValue = new LoadedFileRows
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
                }
            };
        }
    }
}
