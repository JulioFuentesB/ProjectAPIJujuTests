using System.ComponentModel.DataAnnotations;

namespace Business.Common.DTOs.Customer
{
    public class CustomerUpdateDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(250, ErrorMessage = "El nombre no puede exceder los 250 caracteres")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios")]
        public string Name { get; set; }
    }
}
