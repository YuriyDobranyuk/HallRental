using FluentValidation;
using HallRental.BL.DTOs.Hall;

namespace HallRental.Api.Validators.Hall
{
    public class HallValidator : AbstractValidator<HallRequest>
    {
        public HallValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Hall name is required.")
                .MinimumLength(2).WithMessage("Hall name must minimum 2 characters.")
                .MaximumLength(100).WithMessage("Hall name cannot exceed 100 characters.");

            RuleFor(x => x.Capacity)
                .InclusiveBetween(0, int.MaxValue)
                .WithMessage($"Hall capacity must be between 0 and {int.MaxValue}.");

            RuleFor(x => x.BaseHourlyRate)
                .InclusiveBetween(0, decimal.MaxValue)
                .WithMessage($"Hall base hourly rate must be between 0 and {decimal.MaxValue}.");

            RuleFor(x => x.AvailableServiceIds)
                .NotEmpty().WithMessage("Services ids list is required.")
                .ForEach(id => id.NotEmpty().WithMessage("Service ID cannot be empty."));
        }
    }
}
