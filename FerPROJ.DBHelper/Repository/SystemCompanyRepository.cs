using FerPROJ.DBHelper.DBCrud;
using FerPROJ.DBHelper.Entity.Companies;
using FerPROJ.Design.FormModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerPROJ.DBHelper.Repository {
    public class SystemCompanyRepository : BaseRepository<BaseDbContext, SystemCompanyModel, SystemCompany, Guid> {
        public SystemCompanyRepository() {
        }

        public SystemCompanyRepository(BaseDbContext ts) : base(ts) {
        }
        public async Task<SystemCompanyModel> GetActiveSystemCompanyAsync() {
            return await GetPrepareModelByPredicateAsync(c => c.CompanyEnabled);
        }
    }
}
