using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doorfail.Core.Data
{
    public abstract class Entity<TId>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public TId ID { get; set; }

        [Column(TypeName = "Datetime2")]
        public DateTime CreatedDate { get; set; }
        [Column(TypeName = "Datetime2")]
        public DateTime UpdatedDate { get; set; }
        public TId CreatedBy { get; set; }
        public TId UpdatedBy { get; set; }
        public bool Active { get; set; }
    }
}