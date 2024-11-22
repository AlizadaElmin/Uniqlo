using System.ComponentModel.DataAnnotations;

namespace Uniqlo.ViewModels.Sliders;

public class SliderCreateVM
{
    [MaxLength(32,ErrorMessage = "Başlıq 32 simvoldan çox ola bilməz."),Required]
    public string Title { get; set; }
    [Required]
    public string Subtitle { get; set; }
    public string Link { get; set; }
    [Required(ErrorMessage = "Fayl seçilməyib.")]
    public IFormFile File { get; set; }
}