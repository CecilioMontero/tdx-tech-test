using System.Collections.Generic;
using System.Threading.Tasks;
using FileProcessorTDXTechTest.Models;
using FileProcessorTDXTechTestData.Models;
using FluentValidation.Results;

namespace FileProcessorTDXTechTest.FileHelpers
{
    public interface IValidationHandler
    {
        Task<ValidationResult> FileDataValidator(LoadedFileRows fileLoaded, List<string> operationFormatErrors);
        OperationResult<LoadedFileRows> GetOperationResult(LoadedFileRows fileLoaded, IList<ValidationFailure> validationErros, List<string> operationErrors);
        bool FileFormatValidator(IEnumerable<Order> convertedFile, LoadedFileRows fileLoaded, List<string> operationFormatErrors);
        string HeaderValidator(List<string> headers);
    }
}
