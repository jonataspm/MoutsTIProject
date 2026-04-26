namespace Ambev.DeveloperEvaluation.Application.Dtos;

public class AddressDto
{
    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public int Number { get; set; }
    public string Zipcode { get; set; } = string.Empty;
    public GeolocationDto Geolocation { get; set; } = new();
}