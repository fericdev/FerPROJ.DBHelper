using FerPROJ.DBHelper.DBCrud;
using FerPROJ.DBHelper.Helper;
using FerPROJ.Design.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerPROJ.DBHelper.Entity.Companies {
    public class SystemCompanyMigration : IDbContextMigration<BaseDbContext> {
        public void Dispose() {
            throw new NotImplementedException();
        }

        public async Task RunMigrationAsync(BaseDbContext dbContext) {
            await DbContextHelper.CreateOrUpdateTableOfEntityAsync<SystemCompany>(dbContext);
        }
    }
}
