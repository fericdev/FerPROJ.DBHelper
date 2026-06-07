using FerPROJ.DBHelper.DBCrud;
using FerPROJ.DBHelper.Entity.Companies;
using FerPROJ.Design.FormModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerPROJ.DBHelper.Repository {
    public class CompanyRepository : BaseRepository<BaseDbContext, SystemCompanyModel, SystemCompany, Guid> {
    }
}
