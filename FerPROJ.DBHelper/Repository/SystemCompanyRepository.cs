using FerPROJ.DBHelper.DBCrud;
using FerPROJ.DBHelper.DBExtensions;
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
        public async Task EnabledSystemCompanyAsync(Guid id) {
            var companies = await GetAllAsync();
            foreach (var company in companies) {
                if (company.Id == id) {
                    company.CompanyEnabled = true;
                }
                else {
                    company.CompanyEnabled = false;
                }
                await _ts.UpdateAsync(company);
            }
            await _ts.SaveChangesAsync();
        }
    }
}
