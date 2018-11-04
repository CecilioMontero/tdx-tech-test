using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using FileProcessorTDXTechTest.Models;
using FileProcessorTDXTechTest.ValidationModelsDB;
using FileProcessorTDXTechTestData;
using FileProcessorTDXTechTestData.Models;
using FluentValidation.Results;

namespace FileProcessorTDXTechTest.FileHelpers
{
    public class ValidationHandler : IValidationHandler
    {
        private readonly IOrderRepository _orderRepo;
        public ValidationHandler(IOrderRepository orderRepo)
        {
            _orderRepo = orderRepo;
        }

        public string HeaderValidator(List<string> headers)
        {
            var headerFields = new List<string> { "OrderId", "Country", "ItemType", "OrderDate", "UnitsSold", "UnitPrice" };

            var numberHeaderFields = 6;
            var count = 0;
            foreach (var field in headers)
            {
                if (headerFields.Contains(field) && count != numberHeaderFields)
                    headerFields.Remove(field);
            }

            if (!headerFields.Any())
                return string.Empty;

            return "Header is not in the right format";
        }

        public async Task<ValidationResult> FileDataValidator(LoadedFileRows fileLoaded, List<string> operationFormatErrors)
        {
            var validator = new OrderValidator();

            ValidationResult dataValidation = null;

            List<Guid> ids = fileLoaded.FileRows.Select(x => x.OrderId).ToList();
            bool orderAlreadyExists = await _orderRepo.OrderExistsAlready(ids);

            foreach (var row in fileLoaded.FileRows)
            {
                dataValidation = validator.Validate(row);

                if (dataValidation.IsValid && orderAlreadyExists)
                {
                    operationFormatErrors.Add($"The Order {row.OrderId} is already in the system");
                    return dataValidation;
                }
            }

            return dataValidation;
        }

        public bool FileFormatValidator(IEnumerable<Order> convertedFile, LoadedFileRows fileLoaded, List<string> operationFormatErrors)
        {
            var isBadFormat = false;
            try
            {
                if (convertedFile != null)
                {
                    fileLoaded.FileRows = convertedFile.ToList();
                }
            }
            catch (CsvHelperException ex)
            {
                operationFormatErrors.Add($"{ex.Message} and {ex.InnerException}");
                isBadFormat = true;
            }
            return isBadFormat;
        }

        public OperationResult<LoadedFileRows> GetOperationResult(LoadedFileRows fileLoaded, 
                                                                  IList<ValidationFailure> validationErrors, 
                                                                  List<string> operationErrors)
        {
            return new OperationResult<LoadedFileRows>
            {
                OperationFormatError = operationErrors,
                OperationValidationError = validationErrors,
                OperationValue = fileLoaded
            };
        }
    }
}