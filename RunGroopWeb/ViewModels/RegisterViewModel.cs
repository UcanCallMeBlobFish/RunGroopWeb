namespace RunGroopWeb.ViewModels;

public class RegisterViewModel
{
    [Display(Name = "EmailAddress")]
    [Required(ErrorMessage = "emaill is reuqired")]
    public string Email { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Password Do Not match")]
    public string ConfirmPassword { get; set; }


}