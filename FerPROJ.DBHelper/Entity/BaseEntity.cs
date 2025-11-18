using System;
using System.ComponentModel.DataAnnotations;

namespace FerPROJ.DBHelper.Entity {
    public partial class BaseEntity {
        [Key]
        public Guid Id { get; set; }
        public string FormId { get; set; }
        public string Name { get; set; }
        public DateTime? DateCreated { get; set; } = null;
        public DateTime? DateModified { get; set; } = null;
        public DateTime? DateDeleted { get; set; } = null;
        public string CreatedBy { get; set; }
        public Guid CreatedById { get; set; }
        public string ModifiedBy { get; set; } = null;
        public Guid? ModifiedById { get; set; } = null;
        public string Status { get; set; }
    }
    public partial class BaseEntityItem {
        [Key]
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public string Description { get; set; }
    }
}
