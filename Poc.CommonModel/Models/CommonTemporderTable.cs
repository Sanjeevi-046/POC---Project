using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POC.CommonModel.Models;

[Table("temporderTable")]
public partial class CommonTemporderTable
{
    [Key]
    public int OrderId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? OrderDate { get; set; }

    public int? UserId { get; set; }

    public int? ProductId { get; set; }

    [Column(TypeName = "decimal(18, 0)")]
    public decimal? OrderPrice { get; set; }
}