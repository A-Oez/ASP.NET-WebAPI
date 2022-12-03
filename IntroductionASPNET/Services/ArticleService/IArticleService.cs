namespace IntroductionASPNET.Services.ArticleServices
{
    public interface IArticleService
    {
        Task<List<Article>> GetArticles();
        Task<Result<Article>> GetArticlesByID(string ArticleNumber);
        string GetIDArticleValue();
        Task<Result<Article>> AddArticle(Article article);
        Task<Result<Article>> UpdateArticle(Article article);
        Task<Result<Article>> DeleteArticle(string ArticleNumber);

    }
}
