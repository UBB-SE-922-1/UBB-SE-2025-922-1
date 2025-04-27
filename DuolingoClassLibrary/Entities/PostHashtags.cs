using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuolingoClassLibrary.Entities
{
    public class PostHashtags
    {
        [Key]
        [Column(Order = 0)]
        public int PostId { get; set; }
        
        [Key]
        [Column(Order = 1)]
        public int HashtagId { get; set; }
    }
}
