using System.ComponentModel.DataAnnotations;
#pragma warning disable CS8618

namespace LagerApp.Views.Product
{
    public class SignupVM
    {
        [Display(Name = "Username")]
        [Required(ErrorMessage = "Användarnamn är obligatoriskt")]
        public string Username { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Lösenord är obligatoriskt")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Confirm password")]
        [Required(ErrorMessage = "Bekräfta lösenord är obligatoriskt")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
