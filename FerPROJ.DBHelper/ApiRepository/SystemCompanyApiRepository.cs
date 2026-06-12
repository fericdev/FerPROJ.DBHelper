using FerPROJ.DBHelper.DBCrud;
using FerPROJ.DBHelper.Entity.Companies;
using FerPROJ.Design.FormModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerPROJ.DBHelper.ApiRepository {
    public class SystemCompanyApiRepository : BaseApiRepository<SystemCompanyModel, SystemCompany> {
        public SystemCompanyApiRepository() {
            
        }
        public async Task<SystemCompanyModel> GetActiveSystemCompanyAsync() {
            return await GetPrepareModelByPredicateAsync(c => c.CompanyEnabled);
        }
    }
}
