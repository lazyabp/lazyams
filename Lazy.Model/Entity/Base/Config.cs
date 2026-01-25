using System.ComponentModel.DataAnnotations;

namespace Lazy.Model.Entity;

public class Config : BaseEntity
{
    [Required(ErrorMessage = "The key field is required.")]
    [StringLength(128, ErrorMessage = "The key cannot exceed 128 characters.")]
    public string Key { get; set; }

    public string Value { get; set; }
}
