namespace IntroductionASPNET.Services
{
    public static class ExtensionService
    {
        public static ActionResult ToOk<TResult, TContract>(this Result<TResult> result, Func<TResult, TContract> mapper, string Controller, string Status)
        {
            return result.Match<ActionResult>(b =>
            {
                var response = mapper(b);
                Log.Information("{0} Data from Controller {2}", Status, response.ToString(), Controller);
                return new OkObjectResult(response);
            }, exception =>
            {
                if (exception is ValidationException validationException)
                {
                    foreach (var error in validationException.Errors)
                    {
                        Log.Error("Controller: {0} Status: {1} Error: {2}", Controller, Status ,error.ErrorMessage);
                        return new BadRequestObjectResult(error.ErrorMessage);
                    }
                }

                if (exception is NullReferenceException nullReferenceException)
                {
                    Log.Error("Controller: {0} Status: {1} Error: {2}", Controller, Status, exception);
                    return new BadRequestObjectResult(exception.Message);
                }

                return new StatusCodeResult(500);
            });
        }

    }
}
