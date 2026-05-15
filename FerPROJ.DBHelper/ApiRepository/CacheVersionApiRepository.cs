using FerPROJ.DBHelper.DBCrud;
using FerPROJ.DBHelper.Entity.Cache;
using FerPROJ.Design.FormModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerPROJ.DBHelper.ApiRepository {
    public class CacheVersionApiRepository : BaseApiRepository<CacheVersionModel, CacheVersion, Guid> {
        public CacheVersionApiRepository() : base("CacheVersionApiRepository.php") {
        }
    }
}
