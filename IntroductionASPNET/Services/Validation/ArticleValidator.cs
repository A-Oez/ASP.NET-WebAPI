namespace IntroductionASPNET.Validation
{
    public class ArticleValidator : AbstractValidator<Article>
    {
        public ArticleValidator() 
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Articlenumber)
                .NotEmpty()
                .MinimumLength(10);
        }
    }
}
