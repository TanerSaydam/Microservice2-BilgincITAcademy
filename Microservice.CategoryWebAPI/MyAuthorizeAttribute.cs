using Microsoft.AspNetCore.Mvc.Filters;

namespace Microservice.CategoryWebAPI;

public class MyAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    private string _role { get; set; }
    public MyAuthorizeAttribute(string role)
    {
        _role = role;
    }
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        //contextden kullanıcı ID'yi alıyorsunuz
        //role db de var mı kontrol ediliyorsunuz

        //UnauthorizedResult(context);
    }
}
