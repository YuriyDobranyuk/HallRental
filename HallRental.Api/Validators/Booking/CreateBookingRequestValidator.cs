using FluentValidation;
using HallRental.BL.DTOs.Booking;

namespace HallRental.Api.Validators.Booking
{
    public sealed class CreateBookingRequestValidator : AbstractValidator<CreateBookingRequest>
    {
        public CreateBookingRequestValidator()
        {
            RuleFor(x => x.HallId).NotEmpty().WithMessage("Hall Id is required.");

            RuleFor(x => x.StartUtc).NotEmpty().WithMessage("StartUtc is required.");

            RuleFor(x => x.DurationMinutes).GreaterThan(0).LessThanOrEqualTo(60 * 24 * 30)
                .WithMessage("DurationMinutes must have correct value.");

            When(x => x.SelectedServiceIds is not null && x.SelectedServiceIds.Count > 0, () =>
            {
                RuleForEach(x => x.SelectedServiceIds)
                    .NotEmpty().WithMessage("Service ID cannot be empty.");
            });
        }
    }
}
