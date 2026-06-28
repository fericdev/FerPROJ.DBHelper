using FerPROJ.DBHelper.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerPROJ.DBHelper.Entity.Companies {
    public class SystemCompany : BaseEntity {
        [StringLength(int.MaxValue)]
        public string CompanyLogoUrl { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyContactNo { get; set; }
        public string CompanyAddress { get; set; }
        public bool CompanyEnabled { get; set; }
        [ColumnType("mediumblob")]
        public byte[] CompanyLogo { get; set; }
    }
}
