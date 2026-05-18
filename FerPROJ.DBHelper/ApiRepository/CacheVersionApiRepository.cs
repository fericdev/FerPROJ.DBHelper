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
    public class CacheVersionApiRepository : BaseApiRepository<CacheVersionModel, CacheVersion> {
        public CacheVersionApiRepository() {
        }

        public async Task ExecuteClearCacheAsync() {

            await CBackgroundTaskManager.RunTaskInBackgroundAsync(async () => 
            {
                var latestVersion = await GetAllAsync();

                if (!latestVersion.IsNullOrEmpty()) {

                    var serverVersion = latestVersion.FirstOrDefault();

                    var localVersion = CConfigurationManager.GetValue<int>(nameof(CacheVersion.VersionNo), nameof(CacheVersion));

                    if (serverVersion.VersionNo > localVersion) {

                        await ClearCacheAsync();

                        CConfigurationManager.CreateOrSetValue(nameof(CacheVersion.VersionNo), serverVersion.VersionNo.ToString(), nameof(CacheVersion));
                    }
                }

            }, 30);
        }

        public async Task ExecuteUpdateCacheAsync() {

            CBackgroundTaskManager.RunTaskAndForget(async () => {

                var latestVersion = await GetAllAsync();

                var serverVersion = new CacheVersion();

                if (latestVersion.IsNullOrEmpty()) {

                    var serverVersionModel = new CacheVersionModel {
                        VersionNo = 1,
                    };

                    serverVersion = serverVersionModel.ToDestination<CacheVersion>();

                    await SaveDataAsync(serverVersion);
                }
                else {
                    serverVersion = latestVersion.FirstOrDefault();

                    serverVersion.VersionNo += 1;

                    serverVersion.DateModified = DateTime.Now;

                    await UpdateDataAsync(serverVersion);

                }

                CConfigurationManager.CreateOrSetValue(nameof(CacheVersion.VersionNo), serverVersion.ToString(), nameof(CacheVersion));
            });
        }
    }
}
