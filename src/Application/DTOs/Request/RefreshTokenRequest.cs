using System.ComponentModel.DataAnnotations;

public class RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;

    [Required]
    public string ExpiredToken { get; set; } = string.Empty;
}

