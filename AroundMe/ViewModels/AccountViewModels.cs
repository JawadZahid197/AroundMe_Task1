using System.ComponentModel.DataAnnotations;

namespace AroundMe.ViewModels;
public class RegisterViewModel
{
    [Required] public string FullName
    {
        get;
        set;
    }
    ="";
    [Required] public string Username
    {
        get;
        set;
    }
    ="";
    [Required, EmailAddress] public string Email
    {
        get;
        set;
    }
    ="";
    [Required, MinLength(6), DataType(DataType.Password)] public string Password
    {
        get;
        set;
    }
    ="";
    [Compare(nameof(Password)), DataType(DataType.Password)] public string ConfirmPassword
    {
        get;
        set;
    }
    ="";
}

public class LoginViewModel
{
    [Required, EmailAddress] public string Email
    {
        get;
        set;
    }
    ="";
    [Required, DataType(DataType.Password)] public string Password
    {
        get;
        set;
    }
    ="";
    public string? ReturnUrl
    {
        get;
        set;
    }
}
