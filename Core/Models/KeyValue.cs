using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doorfail.Core.Models
{
    public abstract class KeyValue<TId, TKey, TValue>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public TId ID { get; set; }

        public TKey Key { get; set; }
        public TKey Value { get; set; }
    }
}