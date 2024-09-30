using System.ComponentModel.DataAnnotations.Schema;

namespace PB303Fashion.DataAccessLayer.Entities;

public class TopTrending:Entity
{
    public string? ImgUrl { get; set; }
    [NotMapped]
    public IFormFile? ImageFile { get; set; }
    public string Description  { get; set; }
    public string Content { get; set; }
    public string SubText { get; set; }
}
