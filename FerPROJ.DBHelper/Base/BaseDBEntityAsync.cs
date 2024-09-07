using FerPROJ.DBHelper.Class;
using FerPROJ.DBHelper.CRUD;
using FerPROJ.DBHelper.Query;
using FerPROJ.Design.Class;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static FerPROJ.Design.Class.CEnum;

namespace FerPROJ.DBHelper.Base {
    public abstract class BaseDBEntityAsync<EntityContext, TSource, TType> : IDisposable where EntityContext : DbContext where TSource : CValidator {
        public string _tableName { get; set; }
        public string _tableDetailsName { get; set; }
        public EntityContext _ts;

        protected BaseDBEntityAsync() {
            _ts = NewConnection();
        }
        protected BaseDBEntityAsync(EntityContext ts) {
            _ts = UseConnection(ts);
        }
        private EntityContext NewConnection() {
            return Activator.CreateInstance<EntityContext>();
        }
        private EntityContext UseConnection(EntityContext useConn) {
            return useConn;
        }
        public void Dispose() {
            _ts.Dispose();
        }
        //
        protected async virtual Task SaveDataAsync(TSource myDTO) {
            await Task.CompletedTask;
        }
        public async Task SaveDTOAsync(TSource myDTO, bool EnableValidation = false, bool confirmation = true) {
            if (myDTO == null) {
                throw new ArgumentNullException($"{nameof(myDTO)} is null!");
            }
            //
            try {
                if (EnableValidation) {
                    if (!myDTO.DataValidation()) {
                        throw new ArgumentException(myDTO.Error);
                    }
                }
                if (!myDTO.Success) {
                    throw new ArgumentException(myDTO.Error);
                }
                if (confirmation) {
                    if (CShowMessage.Ask("Are you sure to save this data?", "Confirmation")) {
                        await SaveDataAsync(myDTO);
                        CShowMessage.Info("Saved Successfully!", "Success");
                    }
                }
                else {
                    await SaveDataAsync(myDTO);
                    CShowMessage.Info("Saved Successfully!", "Success");
                }
            }
            catch (Exception ex) {
                throw ex;
            }
            finally {
                _ts.Dispose();
            }
        }
        //
        protected async virtual Task UpdateDataAsync(TSource myDTO) {
            await Task.CompletedTask;
        }
        public async Task UpdateDTOAsync(TSource myDTO, bool EnableValidation = false) {
            if (myDTO == null) {
                throw new ArgumentNullException($"{nameof(myDTO)} is null!");
            }
            //
            try {
                if (EnableValidation) {
                    if (!myDTO.DataValidation()) {
                        throw new ArgumentException("Failed!");
                    }
                }
                if (!myDTO.Success) {
                    throw new ArgumentException(myDTO.Error);
                }
                if (CShowMessage.Ask("Are you sure to update this data?", "Confirmation")) {
                    await UpdateDataAsync(myDTO);
                    CShowMessage.Info("Updated Successfully!", "Success");
                }
            }
            catch (Exception ex) {
                throw ex;
            }
            finally {
                _ts.Dispose();
            }
        }
        //
        protected async virtual Task DeleteDataAsync(TType id) {
            await Task.CompletedTask;
        }
        public async Task DeleteByIdAsync(TType id) {
            if (id == null) { 
                throw new ArgumentException($"{nameof(id)} is null!");
            }
            //
            try {
                if (CShowMessage.Ask("Are you sure to delete this data?", "Confirmation")) {
                    await DeleteDataAsync(id);
                    CShowMessage.Info("Deleted Successfully!", "Success");
                }
            }
            catch (Exception ex) {
                throw ex;
            }
            finally {
                _ts.Dispose();
            }
        }
        //
        protected async virtual Task DeleteMultipleDataAsync(TType id) {
            await Task.CompletedTask;
        }
        public async Task DeleteMultipleDataByIdsAsync(List<TType> ids) {
            if (ids.Count > 0) {
                //
                var sb = new StringBuilder();
                var askMessage = ids.Count > 1 ? "Are you sure to delete these data's?" : "Are you sure to delete this data?";
                var resultMessage = ids.Count > 1 ? "All the data's selected has been deleted successfully!" : "Deleted Successfully!";
                //
                if (CShowMessage.Ask(askMessage, "Confirmation")) {
                    foreach (var id in ids) {
                        try {
                            await DeleteMultipleDataAsync(id);
                        }
                        catch (Exception) {
                            //
                            sb.AppendLine(id.ToString());
                            continue;
                        }
                    }
                    if (sb.Length <= 0) {
                        CShowMessage.Info(resultMessage);
                    }
                    else {
                        CShowMessage.Warning($"The following id's has not been deleted:\n{sb.ToString()}");
                    }
                }
            }
            else {
                throw new ArgumentException($"{nameof(ids)} is null!");
            }

        }
    }
}
