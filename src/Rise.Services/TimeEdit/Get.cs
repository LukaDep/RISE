using Destructurama.Attributed;
using FluentValidation;

namespace Rise.Shared.TimeEdit;

/// <summary>
/// DTO's voor TimeEdit-aanvragen.
/// </summary>
public static partial class TimeEditRequest
{
    /// <summary>
    /// Requestobject voor het ophalen van TimeEdit-reservaties.
    /// </summary>
    public class Get
    {
        /// <summary>
        /// De unieke ID van het TimeEdit-project (bijv. cursus of agenda).
        /// </summary>
        [LogMasked]
        public required string TimeEditId { get; set; }

        /// <summary>
        /// De naam van het project, enkel voor logging/label.
        /// </summary>
        [LogMasked]
        public string? Name { get; set; }

        /// <summary>
        /// Optioneel: begin van de periode die je wilt ophalen.
        /// </summary>
        public DateTime? From { get; set; }

        /// <summary>
        /// Optioneel: einde van de periode die je wilt ophalen.
        /// </summary>
        public DateTime? To { get; set; }

        /// <summary>
        /// Optioneel: resource of klascode.
        /// </summary>
        [LogMasked]
        public string? ResourceCode { get; set; }
    }

    /// <summary>
    /// Validator voor de Get-aanvraag.
    /// </summary>
    // public class Validator : AbstractValidator<Get>
    // {
    //     public Validator()
    //     {
    //         RuleFor(x => x.TimeEditId)
    //             .NotEmpty()
    //             .WithMessage("TimeEditId is verplicht.");

    //         RuleFor(x => x.Name)
    //             .NotEmpty()
    //             .MaximumLength(250);

    //         When(x => x.From.HasValue && x.To.HasValue, () =>
    //         {
    //             RuleFor(x => x.To)
    //                 .GreaterThanOrEqualTo(x => x.From)
    //                 .WithMessage("De einddatum moet na de begindatum liggen.");
    //         });
    //     }
    // }
}
