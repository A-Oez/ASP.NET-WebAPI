namespace IntroductionASPNET.Services.Validation
{
    public class CustomerValidator : AbstractValidator<Customer>
    {
        public CustomerValidator() 
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.UserId)
                .NotEmpty()
                .MinimumLength(5);
        }
    }
}
