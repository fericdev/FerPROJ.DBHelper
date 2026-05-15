using FerPROJ.DBHelper.DBCrud;
using FerPROJ.DBHelper.Entity.Cache;
using FerPROJ.Design.Class;
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

        public async Task ExecuteBackgroundTracker() {

            await CBackgroundTaskManager.RunTaskInBackgroundAsync(async () => 
            {
                var cacheVersionApiRepository = new CacheVersionApiRepository();

                var latestVersion = await cacheVersionApiRepository.GetAllAsync();

                if (!latestVersion.IsNullOrEmpty()) {

                    var serverVersion = latestVersion.FirstOrDefault();

                    var localVersion = CConfigurationManager.GetValue<int>(nameof(CacheVersion.VersionNo), nameof(CacheVersion));

                    if (serverVersion.VersionNo > localVersion) {
                        await ClearCacheAsync();
                    }
                }

            }, 30);
        }
    }
}
