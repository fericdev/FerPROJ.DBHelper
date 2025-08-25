using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerPROJ.DBHelper.Base {
    public partial class BaseEntity {
        [Key]
        public Guid Id { get; set; }
        public string FormId { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; } = null;
        public DateTime? DateDeleted { get; set; } = null;
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; } = string.Empty;
        public string Status { get; set; }
    }
}
