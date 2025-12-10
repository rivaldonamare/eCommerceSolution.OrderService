namespace BusinessLogicLayer.DTO;

public record UserDTO(Guid UserId, string Email, string Name, string Gender, string Token, bool IsSuccess)
{
    public UserDTO() : this(default, string.Empty, string.Empty, string.Empty, string.Empty, false) { }
}
