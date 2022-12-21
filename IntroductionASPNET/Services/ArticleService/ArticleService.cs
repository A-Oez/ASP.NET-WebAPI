namespace IntroductionASPNET.Services.ArticleServices
{
    public class ArticleService : IArticleService
    {
        private readonly TestDBContext _context;
        private readonly ICacheService _cacheService;
        private readonly IValidator<Article> _validator;
        private Article _article;

        public ArticleService(TestDBContext context, ICacheService cacheService, IValidator<Article> validator)
        {
            _context = context;
            _cacheService = cacheService;
            _validator = validator;
        }

        public async Task<List<Article>> GetArticles()
        {
            //Distributed Cache
            //check cache data
            var cacheData = _cacheService.GetData<IEnumerable<Article>>("Article");

            if (cacheData != null && cacheData.Count() > 0)
                return (List<Article>)cacheData;

            //get value from DB
            var value = await _context.Articles.ToListAsync();

            //set expiry time 
            var expiryTime = DateTimeOffset.Now.AddSeconds(20);
            _cacheService.SetData<IEnumerable<Article>>("Article", value, expiryTime);

            return value;
        }

        public async Task<Result<Article>> GetArticlesByID(string ArticleNumber)
        {
            //Distributed Cache 
            var cacheData = _cacheService.GetData<IEnumerable<Article>>("Article");

            //create Article object for validation
            var createdArticle = new Article();
            createdArticle.Articlenumber = ArticleNumber;

            //check validation output
            var validation = await returnValidationError(createdArticle); 
            if(validation.IsSuccess.Equals(false)) return validation;
            
            //if data is not in Cache
            if (cacheData is null)
            {
                var dbArticle = await _context.Articles.FindAsync(ArticleNumber);
                var checkDBNull = returnNullError(dbArticle);

                if(checkDBNull.IsSuccess.Equals(false)) return checkDBNull;

                _article = dbArticle;
                return dbArticle;
            }
            else
            {
                //get ID from cache
                foreach (var article in cacheData)
                {
                    if (article.Articlenumber == ArticleNumber)
                    {
                        _article = article;
                        return article;
                    }
                }
            }
            return null;
        }

        public string GetIDArticleValue()
        {
            if (_article is null)
                return null;

            return "Articlenumber: " + _article.Articlenumber   + ";" +
                   "Description: "   + _article.Description     + ";" +
                   "Price: "         + _article.Price           + ";";
        }           


        public async Task<Result<Article>> AddArticle(Article article)
        {
            //check validation 
            var validation = await returnValidationError(article);
            if (validation.IsSuccess.Equals(false)) return validation;

            _context.Articles.Add(article);
            await _context.SaveChangesAsync();
            return article;
        }

        public async Task<Result<Article>> UpdateArticle(Article article)
        {
            var dbArticle = await _context.Articles.FindAsync(article.Articlenumber);
            
            //check validation 
            var validation = await returnValidationError(dbArticle);
            if (validation.IsSuccess.Equals(false)) return validation;

            //check Null
            var checkDBNull = returnNullError(dbArticle);
            if(checkDBNull.IsSuccess.Equals(false)) return checkDBNull;

            dbArticle.Articlenumber = article.Articlenumber;
            dbArticle.Description = article.Description;
            dbArticle.Price = article.Price;
            await _context.SaveChangesAsync();

            return article;
        }

        public async Task<Result<Article>> DeleteArticle(string ArticleNumber)
        {
            var dbArticle = await _context.Articles.FindAsync(ArticleNumber);

            var createdArticle = new Article();
            createdArticle.Articlenumber = ArticleNumber;

            //check validation
            var validation = await returnValidationError(createdArticle);
            if (validation.IsSuccess.Equals(false)) return validation;

            //check dbNull 
            var checkDBNull = returnNullError(dbArticle);
            if(checkDBNull.IsSuccess.Equals(false)) return checkDBNull;

            //Remove from DB
            _context.Articles.Remove(dbArticle);
            //Remove from Cache
            var cachceData = _cacheService.GetData<IEnumerable<Article>>("Article");
            if (cachceData != null)
            {
                foreach (var article in cachceData)
                {
                    if (article.Articlenumber == ArticleNumber)
                        _cacheService.RemoveData("Article");
                }
            }

            await _context.SaveChangesAsync();
            return new Article();
        }


        //Get Exception Results 
        private async Task<Result<Article>> returnValidationError(Article validation)
        {
            var validationResult = await _validator.ValidateAsync(validation);
            if (validationResult.IsValid)
                return null;

            var validationException = new ValidationException(validationResult.Errors);
            return new Result<Article>(validationException);
        }

        private Result<Article> returnNullError(Article checkDB)
        {
            if (checkDB is null)
            {
                var exception = new NullReferenceException("the given ID does not exists");
                return new Result<Article>(exception);
            }

            return null;
        }


    }
}
