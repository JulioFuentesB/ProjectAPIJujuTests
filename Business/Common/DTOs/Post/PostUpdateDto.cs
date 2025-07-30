using System.ComponentModel.DataAnnotations;

namespace Business.Common.DTOs.Post
{
    public class PostUpdateDto : IPostDto
    {

        [Required(ErrorMessage = "El título es requerido")]
        [StringLength(200, ErrorMessage = "El título no puede exceder los 200 caracteres")]
        public string Title { get; set; }

        [Required(ErrorMessage = "El contenido del post es requerido")]
        [StringLength(500, ErrorMessage = "El contenido no puede exceder los 500 caracteres")]
        public string Body { get; set; }

        [Required(ErrorMessage = "El tipo de post es requerido")]
        [Range(1, 10, ErrorMessage = "El tipo debe estar entre 1 y 10")]
        public int Type { get; set; }

        [StringLength(50, ErrorMessage = "La categoría no puede exceder los 50 caracteres")]
        public string Category { get; set; }
    }
}
