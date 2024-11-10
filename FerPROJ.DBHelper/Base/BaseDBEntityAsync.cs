using FerPROJ.DBHelper.Class;
using FerPROJ.DBHelper.CRUD;
using FerPROJ.DBHelper.DBExtensions;
using FerPROJ.DBHelper.Query;
using FerPROJ.Design.Class;
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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static FerPROJ.Design.Class.CEnum;

namespace FerPROJ.DBHelper.Base {
    public abstract class BaseDBEntityAsync<EntityContext, TSource, TEntity, TType> : IDisposable where EntityContext : DbContext where TSource : CValidator where TEntity : class {
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
        public async Task<string> GetGeneratedIDAsync(string prefix, bool withSlash = true) {
            // Use the first 3 letters of the class name as default prefix if none is provided
            if (string.IsNullOrEmpty(prefix)) {
                prefix = typeof(TEntity).Name.Substring(2, 3).ToUpper();
            }

            // Get the current count and increment by 1
            var count = await _ts.Set<TEntity>().CountAsync() + 1;

            // Extract the numeric portion from the prefix if it’s meant to be a number
            if (long.TryParse(prefix, out long baseNumber)) {

                // Increment the base number by the count
                var newIDNumber = baseNumber + count;

                // Return the new ID with or without the slash as specified
                return withSlash ? $"{newIDNumber.ToString().Insert(4, "-")}" : $"{newIDNumber}";
            }

            // Return the new ID with the format "<prefix>-00<count>"
            return withSlash ? $"{prefix}-00{count}" : $"{prefix}{count}";
        }
        public async Task<string> GetGeneratedIDAsync(string prefix, bool withSlash, Expression<Func<TEntity, bool>> whereCondition) {
            // Use the first 3 letters of the class name as default prefix if none is provided
            if (string.IsNullOrEmpty(prefix)) {
                prefix = typeof(TEntity).Name.Substring(2, 3).ToUpper();
            }

            // Apply the where condition if provided
            var query = _ts.Set<TEntity>().AsQueryable();

            if (whereCondition != null) {
                query = query.Where(whereCondition);
            }

            // Get the count with the where condition, then increment by 1
            var count = await query.CountAsync() + 1;

            // Extract the numeric portion from the prefix if it’s meant to be a number
            if (long.TryParse(prefix, out long baseNumber)) {

                // Increment the base number by the count
                var newIDNumber = baseNumber + count;

                // Return the new ID with or without the slash as specified
                return withSlash ? $"{newIDNumber.ToString().Insert(4, "-")}" : $"{newIDNumber}";
            }

            // Return the new ID with the format "<prefix>-00<count>"
            return withSlash ? $"{prefix}-00{count}" : $"{prefix}{count}";
        }
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync() {
            return await _ts.GetAllAsync<TEntity>();
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
        public virtual async Task<TEntity> GetByAsync<TIDType>(TIDType id, string propertyName) {
            return await _ts.GetByIdAsync<TEntity, TIDType>(id, propertyName);
        }
        //
        protected async virtual Task SaveDataAsync(TSource myDTO) {
            await _ts.SaveDTOAndCommitAsync<TSource, TEntity>(myDTO);
        }
        public async Task<bool> SaveDTOAsync(TSource myDTO, bool EnableValidation = false, bool confirmation = true, bool returnResult = true) {
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
        protected async virtual Task UpdateDataAsync(TSource myDTO) {
            await _ts.UpdateDTOAndCommitAsync<TSource, TEntity>(myDTO);
        }
        public async Task<bool> UpdateDTOAsync(TSource myDTO, bool EnableValidation = false, bool confirmation = true, bool returnResult = true) {
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
        public async Task DeleteByIdAsync(TType id) {
            if (id == null) {
                throw new ArgumentException($"{nameof(id)} is null!");
            }
            //
            using (var trans = _ts.Database.BeginTransaction()) {
                try {
                    if (CShowMessage.Ask("Are you sure to delete this data?", "Confirmation")) {
                        await DeleteDataAsync(id);
                        trans.Commit();
                        CShowMessage.Info("Deleted Successfully!", "Success");
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
    }
}
