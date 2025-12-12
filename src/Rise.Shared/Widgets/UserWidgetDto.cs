namespace Rise.Shared.Widgets;

/// <summary>
/// Data transfer objects for user widget configurations.
/// </summary>
public static class UserWidgetDto
{
    /// <summary>
    /// Represents a user's widget instance for display.
    /// Contains widget reference and layout properties (position, size).
    /// </summary>
    public class Index
    {
        public required Guid Id { get; set; }
        public string? UserId { get; set; }
        public required WidgetDto.Index Widget { get; set; }
        public required int X { get; set; }
        public required int Y { get; set; }
        public required int Width { get; set; }
        public required int Height { get; set; }
        public required int MinWidth { get; set; }
    }

    /// <summary>
    /// Represents a user widget for update operations.
    /// Contains widget identifier and updated layout properties.
    /// </summary>
    public class Update
    {
        public required Guid Id { get; set; }
        public required string WidgetName { get; set; }
        public required int X { get; set; }
        public required int Y { get; set; }
        public required int Width { get; set; }
        public required int Height { get; set; }
        public required int MinWidth { get; set; }

    }
    public class Validator : AbstractValidator<Update>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.WidgetName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.X).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Y).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Width).GreaterThan(0);
            RuleFor(x => x.Height).GreaterThan(0);
            RuleFor(x => x.MinWidth).GreaterThan(0);
        }
    }

}