using FileProcessorTDXTechTestData.Models;
using FluentValidation;

namespace FileProcessorTDXTechTest.ValidationModelsDB
{
    public class OrderValidator : AbstractValidator<Order>
    {
        public OrderValidator()
        {
            RuleFor(reg => reg.OrderId)
                            .NotEmpty()
                            .WithMessage("OrderId is required");
            RuleFor(reg => reg.Country)
                            .NotEmpty()
                            .Length(1, 20)
                            .WithMessage("Country is longer than 20 characters");
            RuleFor(reg => reg.ItemType)
                            .NotEmpty()
                            .WithMessage("ItemType is required");
            RuleFor(reg => reg.OrderDate)
                            .NotEmpty()
                            .WithMessage("OrderDate is required");
            RuleFor(reg => reg.UnitsSold)
                            .NotEmpty()
                            .WithMessage("UnitsSold is required");
            RuleFor(reg => reg.UnitPrice)
                            .NotEmpty()
                            .WithMessage("UnitPrice is required");
        }
    }
}
