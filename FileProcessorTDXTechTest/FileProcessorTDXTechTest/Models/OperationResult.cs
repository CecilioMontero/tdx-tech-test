using System.Collections.Generic;
using FluentValidation.Results;

namespace FileProcessorTDXTechTest.Models
{
    public class OperationResult<T>
    {
        public T OperationValue { get; set; }
        public List<string> OperationFormatError { get; set; }
        public IList<ValidationFailure> OperationValidationError { get; set; }
    }
}
