namespace BusinessLogicLayer.HttpClient;

public class ProductServiceClient
{
    private readonly System.Net.Http.HttpClient _httpClient;

    public ProductServiceClient(System.Net.Http.HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ProductDTO?> GetProductByProductID(Guid productID)
    {
        HttpResponseMessage response = await _httpClient.GetAsync($"/api/products/search/product-id/{productID}");

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

        return product;
    }
}
