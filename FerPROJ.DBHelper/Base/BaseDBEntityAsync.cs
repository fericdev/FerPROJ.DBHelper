using FerPROJ.DBHelper.Class;
using FerPROJ.DBHelper.CRUD;
using FerPROJ.Design.BaseDTO;
using FerPROJ.DBHelper.DBCache;
using FerPROJ.DBHelper.DBExtensions;
using FerPROJ.DBHelper.Query;
using FerPROJ.Design.Class;
using FerPROJ.Design.Controls;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static FerPROJ.Design.Class.CEnum;

namespace FerPROJ.DBHelper.Base {
    public abstract class BaseDBEntityAsync<EntityContext, TModel, TEntity, TType> : IDisposable where EntityContext : DbContext where TModel : BaseDTO where TEntity : class {
       
        #region BaseProperties
        public string _tableName { get; set; }
        public string _tableDetailsName { get; set; }
        public bool AllowDuplicate { get; set; }
        public EntityContext _ts;
        #endregion

        #region ctor
        protected BaseDBEntityAsync() {
            _ts = NewConnection();
        }
        protected BaseDBEntityAsync(EntityContext ts) {
            _ts = UseConnection(ts);
        }
        #endregion

        #region CreateConnection
        private EntityContext NewConnection() {
            return Activator.CreateInstance<EntityContext>();
        }
        private EntityContext UseConnection(EntityContext useConn) {
            return useConn;
        }
        public void Dispose() {
            _ts.Dispose();
            DBTransactionExtensions.AllowDuplicate = true;
            DBTransactionExtensions.PropertyToCheck = null;
        }
        #endregion

        #region Base GetDBEntity Method
        //
        public async Task<string> GetGeneratedIDAsync(string prefix, bool withSlash = true) {
            return await _ts.GetGeneratedIDAsync<TEntity>(prefix, withSlash);
        }
        public async Task<string> GetGeneratedIDAsync(string prefix, bool withSlash, Expression<Func<TEntity, bool>> whereCondition) {
            return await _ts.GetGeneratedIDAsync(prefix, withSlash, whereCondition);
        }
        protected virtual async Task<IEnumerable<TEntity>> GetAllAsync() {
            return await _ts.GetAllAsync<TEntity>();
        }
        protected virtual async Task<IEnumerable<TEntity>> GetAllWithSearchAsync(string searchText, DateTime? dateFrom, DateTime? dateTo) {
            return await _ts.GetAllWithSearchAsync<TEntity>(searchText, dateFrom, dateTo);
        }
        protected virtual async Task<IEnumerable<TModel>> GetAllDTOWithSearchAsync(string searchText, DateTime? dateFrom, DateTime? dateTo) {
            var query = await _ts.GetAllWithSearchAsync<TEntity>(searchText, dateFrom, dateTo);
            return query.ToDestination<TModel>();
        }
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> whereCondition) {
            return await _ts.GetAllAsync(whereCondition);
        }
        public virtual async Task<TEntity> GetByIdAsync(TType id) {
            return await _ts.GetByIdAsync<TEntity, TType>(id);
        }
        public virtual async Task<TEntity> GetByIdAsync(TType id, string propertyName) {
            return await _ts.GetByIdAsync<TEntity, TType>(id, propertyName);
        }
        public virtual async Task<TEntity> GetByPropertyAsync<TValueType>(TValueType propertyValue, string propertyName) {
            return await _ts.GetByIdAsync<TEntity, TValueType>(propertyValue, propertyName);
        }
        public virtual async Task<TEntity> GetByPredicateAsync(Expression<Func<TEntity, bool>> predicate) {
            return await _ts.GetByPredicateAsync(predicate);
        }
        #endregion

        #region Base Load Methods
        // Load
        public virtual async Task LoadComboBoxAsync(CComboBoxKrypton cmb, string cmbName, string cmbValue, Expression<Func<TEntity, bool>> whereCondition = null) {
            var listItems = whereCondition != null ? await _ts.GetAllAsync(whereCondition) : await _ts.GetAllAsync<TEntity>();
            cmb.FillComboBox(cmbName, cmbValue, listItems);
        }
        public virtual async Task LoadComboBoxAsync(CComboBoxKrypton cmb, Func<TEntity, string> cmbName, string cmbValue, Expression<Func<TEntity, bool>> whereCondition = null) {
            var listItems = whereCondition != null ? await _ts.GetAllAsync(whereCondition) : await _ts.GetAllAsync<TEntity>();
            cmb.FillComboBox(cmbName, cmbValue, listItems);
        }
        public virtual async Task LoadComboBoxByEntityAsync<T>(CComboBoxKrypton cmb, string cmbName, string cmbValue, Expression<Func<T, bool>> whereCondition = null) where T : class {
            var listItems = whereCondition != null ? await _ts.GetAllAsync(whereCondition) : await _ts.GetAllAsync<T>();
            cmb.FillComboBox(cmbName, cmbValue, listItems);
        }
        public virtual async Task LoadComboBoxByEntityAsync<T>(CComboBoxKrypton cmb, Func<T, string> cmbName, string cmbValue, Expression<Func<T, bool>> whereCondition = null) where T : class {
            var listItems = whereCondition != null ? await _ts.GetAllAsync(whereCondition) : await _ts.GetAllAsync<T>();
            cmb.FillComboBox<T>(cmbName, cmbValue, listItems);
        }

        #endregion

        #region Base DTO CRUD
        //
        protected async virtual Task SaveDataAsync(TModel myDTO) {
            await _ts.SaveDTOAndCommitAsync<TModel, TEntity>(myDTO);
        }
        public async Task<bool> SaveDTOAsync(TModel myDTO, bool EnableValidation = false, bool confirmation = true, bool returnResult = true) {
            if (myDTO == null) {
                throw new ArgumentNullException($"{nameof(myDTO)} is null!");
            }
            if (EnableValidation) {
                if (!myDTO.DataValidation()) {

                    var sb = new StringBuilder();
                    if (!string.IsNullOrEmpty(myDTO.Error)) {
                        sb.AppendLine("Error 1: " + myDTO.Error);
                    }
                    if (!string.IsNullOrEmpty(myDTO.ErrorMessage)) {
                        sb.AppendLine("Error 2: " + myDTO.ErrorMessage);
                    }
                    if (myDTO.ErrorMessages.Length > 0) {
                        sb.AppendLine("Error 3: " + myDTO.ErrorMessages.ToString());
                    }

                    throw new ArgumentException(sb.ToString());
                }
            }
            if (!myDTO.Success) {
                throw new ArgumentException(myDTO.Error);
            }
            //
            try {
                try {

                    try {
                        if (confirmation) {
                            if (CShowMessage.Ask("Are you sure to save this data?", "Confirmation")) {
                                await SaveDataAsync(myDTO);
                                CShowMessage.Info("Saved Successfully!", "Success");
                                return true;
                            }
                        }
                        else {
                            await SaveDataAsync(myDTO);
                            if (returnResult) {
                                CShowMessage.Info("Saved Successfully!", "Success");
                            }
                            return true;
                        }
                    }
                    catch (DbEntityValidationException ex) {
                        var sb = new StringBuilder();

                        if (ex.EntityValidationErrors.Count() == 1) {
                            var validationResult = ex.EntityValidationErrors.FirstOrDefault();

                            if (validationResult != null && validationResult.ValidationErrors.Count > 0) {
                                // Loop through the ValidationErrors and build the error message
                                foreach (var validationError in validationResult.ValidationErrors) {
                                    //sb.AppendLine($"Field: {validationError.PropertyName}, Error: {validationError.ErrorMessage}\n");
                                    sb.AppendLine($"{validationError.ErrorMessage}");
                                }
                            }
                        }
                        throw new ArgumentException(sb.ToString());
                    }
                }
                catch (DbUpdateException ex) {
                    var sb = new StringBuilder();
                    // Check for inner exceptions (usually this is where the real database error lies)
                    var innerEx = ex.InnerException;
                    int innerLevel = 1;

                    while (innerEx != null) {
                        sb.AppendLine($"Inner Exception Level {innerLevel}: {innerEx.Message}\n");

                        // If it's a SqlException (or MySqlException), get more detailed information
                        if (innerEx is MySqlException mySqlEx) {
                            sb.AppendLine($"SQL Error Code: {mySqlEx.Number}\n");
                        }
                        else if (innerEx is System.Data.SqlClient.SqlException sqlEx) {
                            sb.AppendLine($"SQL Error Code: {sqlEx.Number}\n");
                        }

                        // Move to the next inner exception
                        innerEx = innerEx.InnerException;
                        innerLevel++;
                    }

                    // Optionally include information about the entities involved in the update exception
                    if (ex.Entries != null && ex.Entries.Count() > 0) {
                        sb.AppendLine("\nEntities involved in the exception:");
                        foreach (var entry in ex.Entries) {
                            sb.AppendLine($"TableName: {entry.Entity.GetType().Name}, Operation: {entry.State}");
                        }
                    }

                    throw new ArgumentException(sb.ToString());
                }
            }
            catch (Exception ex) {
                throw ex;
            }
            return false;
        }
        //
        protected async virtual Task UpdateDataAsync(TModel myDTO) {
            await _ts.UpdateDTOAndCommitAsync<TModel, TEntity>(myDTO);
        }
        public async Task<bool> UpdateDTOAsync(TModel myDTO, bool EnableValidation = false, bool confirmation = true, bool returnResult = true) {
            if (myDTO == null) {
                throw new ArgumentNullException($"{nameof(myDTO)} is null!");
            }
            if (EnableValidation) {
                if (!myDTO.DataValidation()) {

                    var sb = new StringBuilder();
                    if (!string.IsNullOrEmpty(myDTO.Error)) {
                        sb.AppendLine("Error 1: " + myDTO.Error);
                    }
                    if (!string.IsNullOrEmpty(myDTO.ErrorMessage)) {
                        sb.AppendLine("Error 2: " + myDTO.ErrorMessage);
                    }
                    if (myDTO.ErrorMessages.Length > 0) {
                        sb.AppendLine("Error 3: " + myDTO.ErrorMessages.ToString());
                    }

                    throw new ArgumentException(sb.ToString());
                }
            }
            if (!myDTO.Success) {
                throw new ArgumentException(myDTO.Error);
            }
            //
            try {
                try {
                    try {
                        if (confirmation) {
                            if (CShowMessage.Ask("Are you sure to update this data?", "Confirmation")) {
                                await UpdateDataAsync(myDTO);
                                CShowMessage.Info("Updated Successfully!", "Success");
                                return true;
                            }
                        }
                        else {
                            await UpdateDataAsync(myDTO);
                            if (returnResult) {
                                CShowMessage.Info("Updated Successfully!", "Success");
                            }
                            return true;
                        }
                    }
                    catch (DbEntityValidationException ex) {
                        //
                        var sb = new StringBuilder();

                        if (ex.EntityValidationErrors.Count() == 1) {
                            var validationResult = ex.EntityValidationErrors.FirstOrDefault();

                            if (validationResult != null && validationResult.ValidationErrors.Count > 0) {
                                // Loop through the ValidationErrors and build the error message
                                foreach (var validationError in validationResult.ValidationErrors) {
                                    //sb.AppendLine($"Field: {validationError.PropertyName}, Error: {validationError.ErrorMessage}\n");
                                    sb.AppendLine($"{validationError.ErrorMessage}");
                                }
                            }
                        }

                        throw new ArgumentException(sb.ToString());
                    }
                }
                catch (DbUpdateException ex) {
                    var sb = new StringBuilder();
                    // Check for inner exceptions (usually this is where the real database error lies)
                    var innerEx = ex.InnerException;
                    int innerLevel = 1;

                    while (innerEx != null) {
                        sb.AppendLine($"Error Level {innerLevel}: {innerEx.Message}\n");

                        // If it's a SqlException (or MySqlException), get more detailed information
                        if (innerEx is MySqlException mySqlEx) {
                            sb.AppendLine($"SQL Error Code: {mySqlEx.Number}\n");
                        }
                        else if (innerEx is System.Data.SqlClient.SqlException sqlEx) {
                            sb.AppendLine($"SQL Error Code: {sqlEx.Number}\n");
                        }

                        // Move to the next inner exception
                        innerEx = innerEx.InnerException;
                        innerLevel++;
                    }

                    // Optionally include information about the entities involved in the update exception
                    if (ex.Entries != null && ex.Entries.Count() > 0) {
                        sb.AppendLine("Tables involved in the exception:");
                        foreach (var entry in ex.Entries) {
                            sb.AppendLine($"TableName: {entry.Entity.GetType().Name}\nOperation: {entry.State}");
                        }
                    }

                    throw new ArgumentException(sb.ToString());
                }
            }
            catch (Exception ex) {
                throw ex;
            }
            return false;
        }
        //
        protected async virtual Task DeleteDataAsync(TType id) {
            var tbl = await _ts.GetByIdAsync<TEntity, TType>(id);
            //
            if (tbl == null) {
                return;
            }
            await _ts.RemoveAndCommitAsync(tbl);
        }
        public async Task<bool> DeleteByIdAsync(TType id) {
            if (id == null) {
                CShowMessage.Warning($"{nameof(id)} is null!");
                return false;
            }
            //
            try {
                if (CShowMessage.Ask("Are you sure to delete this data?", "Confirmation")) {
                    await DeleteDataAsync(id);
                    CShowMessage.Info("Deleted Successfully!", "Success");
                    return true;
                }
            }
            catch (Exception ex) {
                throw ex;
            }
            finally {
                _ts.Dispose();
            }
            return false;
        }
        public async Task DeleteMultipleDataByIdsAsync(List<TType> ids) {
            if (ids.Count <= 0) {
                throw new ArgumentException($"{nameof(ids)} is null!");
            }
            //
            using (var trans = _ts.Database.BeginTransaction()) {
                try {
                    //
                    var sb = new StringBuilder();
                    var askMessage = ids.Count > 1 ? "Are you sure to delete these data's?" : "Are you sure to delete this data?";
                    var resultMessage = ids.Count > 1 ? "All the data's selected has been deleted successfully!" : "Deleted Successfully!";
                    //
                    if (CShowMessage.Ask(askMessage, "Confirmation")) {
                        foreach (var id in ids) {
                            try {
                                await DeleteDataAsync(id);
                            }
                            catch (Exception) {
                                //
                                sb.AppendLine(id.ToString());
                                continue;
                            }
                        }
                        trans.Commit();
                        if (sb.Length <= 0) {
                            CShowMessage.Info(resultMessage);
                        }
                        else {
                            CShowMessage.Warning($"The following id's has not been deleted:\n{sb.ToString()}");
                        }
                    }
                }
                catch (Exception ex) {
                    trans.Rollback();
                    throw ex;
                }
                finally {
                    _ts.Dispose();
                }
            }
        }
        #endregion

        #region Utilities
        public virtual async Task<bool> HasData(Expression<Func<TEntity, bool>> predicate = null) {
            return await _ts.HasData(predicate);
        }
        #endregion

        #region Base Cache Methods
        public virtual async Task LoadCachedAsync() {
            var entities = await _ts.GetAllUnCachedAsync<TEntity>();
            await _ts.SaveAllToCacheAsync(entities);
        }
        #endregion

    }
}
                            