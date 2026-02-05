using Polly;
using Polly.Retry;
using System.Net.Mail;

namespace Microservice.ProductWebAPI;

public static class MyExentions
{
    public static void AddPollyPipeline(this IServiceCollection services)
    {
        services.AddResiliencePipeline("http", configure =>
         {
             configure.AddRetry(new RetryStrategyOptions()
             {
                 MaxRetryAttempts = 3,
                 Delay = TimeSpan.FromSeconds(15),
                 ShouldHandle = new PredicateBuilder().Handle<HttpRequestException>()
             });
         });

        services.AddResiliencePipeline("smtp", configure =>
         {
             configure.AddRetry(new RetryStrategyOptions()
             {
                 MaxRetryAttempts = 3,
                 Delay = TimeSpan.FromSeconds(60),
                 ShouldHandle = new PredicateBuilder().Handle<SmtpException>()
             });
         });
    }

    public static ResiliencePipeline<HttpResponseMessage> HttpPipeline = new ResiliencePipelineBuilder<HttpResponseMessage>()
   .AddRetry(new RetryStrategyOptions<HttpResponseMessage>
   {
       MaxRetryAttempts = 3, //deneme süresi
       Delay = TimeSpan.FromSeconds(5), //her denemede bu kadar bekle
       ShouldHandle = new PredicateBuilder<HttpResponseMessage>() //sonucu nasıl işleyeceği
           .Handle<HttpRequestException>() //exception fırltatırsa tekrar dene
           .HandleResult(r => !r.IsSuccessStatusCode) //exception fırlatmasa bile dönen sonuç buysa başarısız say
   })
   .AddTimeout(TimeSpan.FromSeconds(20)) //Timeout, bir deneme 20 saniyeyi aşarsa iptal eder.
   .Build();
}
