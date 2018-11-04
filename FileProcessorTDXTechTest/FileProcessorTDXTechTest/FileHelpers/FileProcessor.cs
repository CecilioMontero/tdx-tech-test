using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using FileProcessorTDXTechTest.Models;
using FileProcessorTDXTechTestData;
using FileProcessorTDXTechTestData.Models;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;

namespace FileProcessorTDXTechTest.FileHelpers
{
    public class FileProcessor
    {       
        private readonly List<string> _operationFormatErrors = new List<string>();
        IList<ValidationFailure> _validationErrors;
        private readonly IOrderRepository _orderRepo;
        private readonly IValidationHandler _validationHandler;

        public FileProcessor(IOrderRepository orderRepo, IValidationHandler validationHandler)
        {
            _orderRepo = orderRepo;
            _validationHandler = validationHandler;
        }

        public OperationResult<LoadedFileRows> FileConversion(IFormFile file)
        {
            IEnumerable<Order> convertedFile;
            var fileLoaded = new LoadedFileRows();

            if (file != null)
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    var csv = CsvHeaderReader(reader);

                    convertedFile = csv.GetRecords<Order>();                   
                    var isBadFormat = _validationHandler.FileFormatValidator(convertedFile, fileLoaded, _operationFormatErrors);

                    if (!isBadFormat)
                    {
                        Task<ValidationResult> validation = _validationHandler.FileDataValidator(fileLoaded, _operationFormatErrors);
                        _validationErrors = validation.Result.Errors;

                        if (!_validationErrors.Any() && !_operationFormatErrors.Any())
                        {
                            _orderRepo.StoreEmployeeDetails(fileLoaded.FileRows);                           
                            return _validationHandler.GetOperationResult(fileLoaded, _validationErrors, _operationFormatErrors);                           
                        }
                    }                    
                }
            }
            else { _operationFormatErrors.Add("No file loaded"); }

            return _validationHandler.GetOperationResult(null,  _validationErrors, _operationFormatErrors);
        }

        private CsvReader CsvHeaderReader(StreamReader reader)
        {
            var csv = new CsvReader(reader);
            csv.Read();
            csv.ReadHeader();

            var headers = csv.Context.HeaderRecord.ToList();
            var headerErrors = _validationHandler.HeaderValidator(headers);

            if (!string.IsNullOrEmpty(headerErrors))
                _operationFormatErrors.Add(headerErrors);

            return csv;
        }
    }
}
