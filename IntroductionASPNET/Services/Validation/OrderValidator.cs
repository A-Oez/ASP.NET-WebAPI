namespace IntroductionASPNET.Services.Validation
{
    public class OrderValidator : AbstractValidator<Order>
    {
        public OrderValidator() 
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Ordernumber)
                .NotEmpty()
                .MinimumLength(6)
                .Matches("[0-9]").WithMessage("Ordernumber must contain nummeric characters");

        }
    }
}
