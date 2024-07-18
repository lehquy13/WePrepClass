namespace WePrepClass.Application.Interfaces;

public interface IValidator
{
    string GenerateValidationCode();
    string HashPassword(string input);
}