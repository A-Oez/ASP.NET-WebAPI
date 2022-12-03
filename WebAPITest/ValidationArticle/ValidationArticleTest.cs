namespace WebAPITest.Validation
{
    public class ValidationArticleTest
    {

        private readonly IValidator<Article> _validator;

        public ValidationArticleTest()
        {
            _validator = new TestArticleValidate();
        }

        [Fact]
        public async void ArticleNumberLessThen10Chars()
        {
            // Arrange, Act, Assert Pattern -> Unit Test
            Article article = new Article();
            article.Articlenumber = "123456";

            var validationResult = await _validator.ValidateAsync(article);

            if (validationResult.IsValid.Equals(false))
                Assert.Fail(validationResult.ToString());


            Assert.True(validationResult.IsValid.Equals(true));
        }

        [Fact]
        public async void ArticleNumberEmpty()
        {
            var validationResult = await _validator.ValidateAsync(new Article());

            if (validationResult.IsValid.Equals(false))
                Assert.Fail(validationResult.ToString());


            Assert.True(validationResult.IsValid.Equals(true));
        }

        [Fact]
        public async void ArticleNumberCorrect()
        {
            Article article = new Article();
            article.Articlenumber = "1234567899";

            var validationResult = await _validator.ValidateAsync(article);

            if (validationResult.IsValid.Equals(false))
                Assert.Fail(validationResult.ToString());


            Assert.True(validationResult.IsValid.Equals(true));
        }

    }
}
