namespace BusinessLogicLayer.HttpClient;

public class ProductServiceClient
{
    private readonly System.Net.Http.HttpClient _httpClient;
    private readonly IDistributedCache _distributedCache;

    public ProductServiceClient(System.Net.Http.HttpClient httpClient, IDistributedCache distributedCache)
    {
        _httpClient = httpClient;
        _distributedCache = distributedCache;
    }

    public async Task<ProductDTO?> GetProductByProductID(Guid productID)
    {
        var cachedKey = $"product:{productID}";
        var cachedProduct = await _distributedCache.GetStringAsync(cachedKey);

        if (cachedProduct != null)
        {
            var productFromCache = JsonSerializer.Deserialize<ProductDTO>(cachedProduct);
            return productFromCache;
        }

        HttpResponseMessage response = await _httpClient.GetAsync($"/gateway/products/search/product-id/{productID}");

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

        ProductDTO? product = await response.Content.ReadFromJsonAsync<ProductDTO>();

        if (product is null)
        {
            throw new ArgumentException("Invalid Product ID");
        }

        var serialized = JsonSerializer.Serialize(product);
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        };

        await _distributedCache.SetStringAsync(cachedKey, serialized, options);

        return product;
    }
}
