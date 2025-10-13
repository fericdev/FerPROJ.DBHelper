using FerPROJ.DBHelper.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerPROJ.DBHelper.Entity.Users {
    public partial class User : BaseEntity {
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string Password { get; set; }
        public string UserRole { get; set; }
    }
}
