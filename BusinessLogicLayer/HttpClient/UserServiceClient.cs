namespace BusinessLogicLayer.HttpClient;

public class UserServiceClient
{
    private readonly System.Net.Http.HttpClient _httpClient;
    private readonly IDistributedCache _distributedCache;

    public UserServiceClient(System.Net.Http.HttpClient httpClient, IDistributedCache distributedCache)
    {
        _httpClient = httpClient;
        _distributedCache = distributedCache;
    }

    public async Task<UserDTO?> GetUserByUserID(Guid userID)
    {
        var cachedKey = $"user:{userID}";
        var cachedUser = await _distributedCache.GetStringAsync(cachedKey);

        if (cachedUser != null)
        {
            var userFromCached = JsonSerializer.Deserialize<UserDTO>(cachedUser);
            return userFromCached;
        }

        HttpResponseMessage response = await _httpClient.GetAsync($"gateway/users/userId?userId={userID}");

        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                throw new HttpRequestException("Bad request", null, System.Net.HttpStatusCode.BadRequest);
            }
            else
            {
                throw new HttpRequestException($"Http request failed with status code {response.StatusCode}");
            }
        }

        UserDTO? user = await response.Content.ReadFromJsonAsync<UserDTO>();

        if (user == null)
        {
            throw new ArgumentException("Invalid User ID");
        }

        var serialized = JsonSerializer.Serialize(user);
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        };

        await _distributedCache.SetStringAsync(cachedKey, serialized, options);

        return user;
    }
}
