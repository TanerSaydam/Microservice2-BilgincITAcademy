# Keycloak

```csharp
public class KeycloakService(
    IOptions<KeycloakConfiguration> options,
    HttpClient httpClient)
{
    public async Task<string> GetAccessToken(CancellationToken cancellationToken = default)
    {
        string endpoint = $"{options.Value.HostName}/realms/{options.Value.Realm}/protocol/openid-connect/token";

        List<KeyValuePair<string, string>> data = new();
        KeyValuePair<string, string> grantType = new("grant_type", "client_credentials");
        KeyValuePair<string, string> clientId = new("client_id", options.Value.ClientId);
        KeyValuePair<string, string> clientSecret = new("client_secret", options.Value.ClientSecret);

        data.Add(grantType);
        data.Add(clientId);
        data.Add(clientSecret);


        var result = await PostUrlEncodedFormAsync<GetAccessTokenResponseDto>(endpoint, data, false, cancellationToken);

        return result.AccessToken;
    }

    public async Task<object> CreateToken(string userName, string userPassword, CancellationToken cancellationToken)
    {
        string endpoint = $"{options.Value.HostName}/realms/{options.Value.Realm}/protocol/openid-connect/token";

        List<KeyValuePair<string, string>> data = new();
        KeyValuePair<string, string> grantType = new("grant_type", "password");
        KeyValuePair<string, string> clientId = new("client_id", options.Value.ClientId);
        KeyValuePair<string, string> clientSecret = new("client_secret", options.Value.ClientSecret);
        KeyValuePair<string, string> username = new("username", userName);
        KeyValuePair<string, string> password = new("password", userPassword);

        data.Add(grantType);
        data.Add(clientId);
        data.Add(clientSecret);
        data.Add(username);
        data.Add(password);

        var response = await PostUrlEncodedFormAsync<object>(endpoint, data, false, cancellationToken);

        return response;
    }

    public async Task<T> PostUrlEncodedFormAsync<T>(string endpoint, List<KeyValuePair<string, string>> data, bool reqToken = false, CancellationToken cancellationToken = default)
    {
        if (reqToken)
        {
            string token = await GetAccessToken();

            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        var message = await httpClient.PostAsync(endpoint, new FormUrlEncodedContent(data), cancellationToken);

        var response = await message.Content.ReadAsStringAsync();

        if (!message.IsSuccessStatusCode)
        {
            throw new ArgumentException("Something went wrong");
        }

        var obj = JsonSerializer.Deserialize<T>(response);

        return obj!;
    }
}

public sealed class GetAccessTokenResponseDto
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = default!;
}

public sealed class KeycloakConfiguration
{
    public string HostName { get; set; } = default!;
    public string ClientId { get; set; } = default!;
    public string Realm { get; set; } = default!;
    public string ClientSecret { get; set; } = default!;
}
```

---

```json
"KeycloakConfiguration": {
    "HostName": "http://localhost:8080",
    "ClientId": "eticaret",
    "Realm": "myrealm",
    "ClientSecret": "pRTpuOIicKlfH6jfIwozOomAgx0vtWo5"
  },
  "Keycloak": {
    "realm": "myrealm",
    "auth-server-url": "http://localhost:8080/",
    "ssl-required": "none",
    "resource": "eticaret",
    "verify-token-audience": false,
    "credentials": {
      "secret": "pRTpuOIicKlfH6jfIwozOomAgx0vtWo5"
    },
    "confidential-port": 0
  }
```

---

```bash
Keycloak.AuthServices.Authentication
Keycloak.AuthServices.Authorization
```

---

```csharp
builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration);
builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("test", policy =>
    {
        policy.RequireResourceRoles("test");
    });
}).AddKeycloakAuthorization(builder.Configuration);
```

```csharp
//For gateway
builder.Services
    .AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.Authority = "http://localhost:8080/realms/myrealm";
        options.TokenValidationParameters.ValidateAudience = false;
        options.RequireHttpsMetadata = false;
    });
builder.Services.AddAuthorization();
```
