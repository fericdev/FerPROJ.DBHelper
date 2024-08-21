using FerPROJ.DBHelper.Class;
using FerPROJ.DBHelper.CRUD;
using FerPROJ.DBHelper.Query;
using FerPROJ.Design.Class;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            if (EnableValidation) {
                if (!myDTO.DataValidation()) {
                    throw new ArgumentException("Failed!");
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
        //
        protected async virtual Task UpdateDataAsync(TSource myDTO) {
            await Task.CompletedTask;
        }
        public async void UpdateDTOAsync(TSource myDTO, bool EnableValidation = false) {
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
        //
        protected async virtual Task DeleteDataAsync(TType id) {
            await Task.CompletedTask;
        }
        public async void DeleteByIdAsync(TType id) {
            if (CShowMessage.Ask("Are you sure to delete this data?", "Confirmation")) {
                await DeleteDataAsync(id);
                CShowMessage.Info("Deleted Successfully!", "Success");
            }
        }
        //
    }
}
