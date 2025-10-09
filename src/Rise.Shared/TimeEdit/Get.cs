namespace Rise.Shared.TimeEdit;

public static partial class TimeEditRequest
{
    public class Get
    {
        /// <summary>
        /// The unique identifier of the project.
        /// </summary>
        public required int TimeEditId { get; set; }
        
        /// <summary>
        /// The name of the project.
        /// </summary>
        public required string Name { get; set; }
        
        public class Validator : AbstractValidator<Get>
        {
            public Validator()
            {
                RuleFor(x => x.Name).NotEmpty().MaximumLength(250); 
            }
        }
    }
}