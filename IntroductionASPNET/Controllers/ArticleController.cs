namespace IntroductionASPNET.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles="Admin")]
    public class ArticleController : ControllerBase
    {
        private readonly IArticleService _articleService;

        public ArticleController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        [HttpGet, AllowAnonymous]
        public async Task<ActionResult<List<Article>>> GetArticles()
        {
            var result = await _articleService.GetArticles();

            if (result is null)
                return BadRequest("No Article");

            return Ok(result);
        }


        [HttpGet("{ArticleNumber}")]
        public async Task<ActionResult<Article>> GetArticleByID(string ArticleNumber)
        {
            var result = await _articleService.GetArticlesByID(ArticleNumber);
            var articleValue = _articleService.GetIDArticleValue();
            return result.ToOk(a => articleValue, "Article","GetID");   
        }

        [HttpPost]
        public async Task<ActionResult<Article>> AddArticle(Article article)
        {
            var result = await _articleService.AddArticle(article);
            return result.ToOk(a => article.Articlenumber+" "+result.IsSuccess, "Article","ADD");
        }

        [HttpPut]
        public async Task<ActionResult<Article>> UpdateArticle(Article article) 
        {
            var result = await _articleService.UpdateArticle(article);
            return result.ToOk(a => article.Articlenumber + " " + result.IsSuccess, "Article", "ADD");
        }

        [HttpDelete("{ArticleNumber}")] 
        public async Task<ActionResult<Article>> DeleteArticleByID(string ArticleNumber)
        {
            var result = await _articleService.DeleteArticle(ArticleNumber);
            return result.ToOk(a => ArticleNumber + " " + result.IsSuccess, "Article", "ADD");
        }

    }
}
