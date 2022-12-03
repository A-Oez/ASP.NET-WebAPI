namespace WebAPITest.Validation
{
    public class TestArticleValidate : AbstractValidator<Article>
    {
        public TestArticleValidate()
        {
            RuleFor(x => x.Articlenumber)
               .NotEmpty()
               .MinimumLength(10);
        }

    }
}
