#region namespace
using FerPROJ.DBHelper.DBCache;
using FerPROJ.DBHelper.DBExtensions;
using FerPROJ.DBHelper.Entity;
using FerPROJ.Design.BaseModels;
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
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static FerPROJ.Design.Class.CBaseEnums;
#endregion

namespace FerPROJ.DBHelper.DBCrud {

    public abstract class BaseRepository<EntityContext, TModel, TEntity, TType> : IDisposable
        where EntityContext : DbContext
        where TModel : BaseModel
        where TEntity : class {

        #region BaseProperties
        public bool AllowDuplicate { get; set; }

        public EntityContext _ts;
        #endregion

        #region ctor
        protected BaseRepository() {
            _ts = Activator.CreateInstance<EntityContext>();
        }
        protected BaseRepository(EntityContext ts) {
            _ts = ts;
        }
        #endregion

        #region IDisposable
        public void Dispose() {
            _ts.Dispose();
            DbContextExtensions.AllowDuplicate = true;
            DbContextExtensions.PropertiesToCheck = new List<string>();
        }
        #endregion

        #region Base GET for Model
        public virtual async Task<TModel> GetPrepareModelAsync(TModel model = null, string prefix = "FRM#") {
            if (model == null) {
                model = Activator.CreateInstance<TModel>();
            }
            model.FormId = await GetGeneratedIDAsync(prefix, false);
            return model;
        }
        public virtual async Task<TModel> GetPrepareModelByEntityAsync(TEntity entity) {
            return entity.ToDestination<TModel>();
        }
        public virtual async Task<TModel> GetPrepareModelByIdAsync(TType id) {
            var entity = await GetByIdAsync(id);
            return entity.ToDestination<TModel>();
        }
        public virtual async Task<TModel> GetPrepareModelByPredicateAsync(Expression<Func<TEntity, bool>> predicate) {
            var entity = await GetByPredicateAsync(predicate);
            return entity.ToDestination<TModel>();
        }
        #endregion

        #region Base GetDBEntity Method
        public async Task<string> GetGeneratedIDAsync(string prefix, bool withSlash = true) {
            return await _ts.GetGeneratedIDAsync<TEntity>(prefix, withSlash);
        }

        public async Task<string> GetGeneratedIDAsync(string prefix, bool withSlash, Expression<Func<TEntity, bool>> whereCondition) {
            return await _ts.GetGeneratedIDAsync(prefix, withSlash, whereCondition);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync() {
            return await _ts.GetAllAsync<TEntity>();
        }

        protected virtual async Task<IEnumerable<TEntity>> GetAllWithSearchAsync(string searchText, DateTime? dateFrom, DateTime? dateTo, int dataLimit = int.MaxValue) {
            return await _ts.GetAllWithSearchAsync<TEntity>(searchText, dateFrom, dateTo, dataLimit);
        }
        public virtual async Task<IEnumerable<TModel>> GetViewModelWithSearchAsync(string searchText, DateTime? dateFrom, DateTime? dateTo, int dateLimit = int.MaxValue) {

            var query = await GetAllWithSearchAsync(null, dateFrom, dateTo, dateLimit);

            return await query.SelectListAsync(async c => {

                return await GetPrepareModelByEntityAsync(c);

            }, c => c.SearchForText(searchText), dateLimit);
        }
        public virtual async Task<IEnumerable<TModel>> GetViewModelWithSearchAsync(Expression<Func<TEntity, bool>> whereCondition, string searchText, DateTime? dateFrom, DateTime? dateTo, int dateLimit = int.MaxValue) {

            var query = await GetAllAsync(whereCondition);

            return await query.SelectListAsync(async c => {

                return await GetPrepareModelByEntityAsync(c);

            }, c => c.SearchFor(searchText, dateFrom, dateTo, d => d.DateCreated), dateLimit);
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
        public virtual async Task LoadComboBoxAsync(CComboBoxKrypton cmb, string cmbName, string cmbValue, Expression<Func<TEntity, bool>> whereCondition = null, bool assignSelectedValue = false) {
            var listItems = whereCondition != null
                ? await _ts.GetAllAsync(whereCondition)
                : await _ts.GetAllAsync<TEntity>();
            cmb.FillComboBox(cmbName, cmbValue, listItems, assignSelectedValue);
        }

        public virtual async Task LoadComboBoxAsync(CComboBoxKrypton cmb, Func<TEntity, string> cmbName, string cmbValue, Expression<Func<TEntity, bool>> whereCondition = null, bool assignSelectedValue = false) {
            var listItems = whereCondition != null
                ? await _ts.GetAllAsync(whereCondition)
                : await _ts.GetAllAsync<TEntity>();
            cmb.FillComboBox(cmbName, cmbValue, listItems, assignSelectedValue);
        }

        public virtual async Task LoadComboBoxByEntityAsync<T>(CComboBoxKrypton cmb, string cmbName, string cmbValue, Expression<Func<T, bool>> whereCondition = null, bool assignSelectedValue = false) where T : class {
            var listItems = whereCondition != null
                ? await _ts.GetAllAsync(whereCondition)
                : await _ts.GetAllAsync<T>();
            cmb.FillComboBox(cmbName, cmbValue, listItems, assignSelectedValue);
        }

        public virtual async Task LoadComboBoxByEntityAsync<T>(CComboBoxKrypton cmb, Func<T, string> cmbName, string cmbValue, Expression<Func<T, bool>> whereCondition = null, bool assignSelectedValue = false) where T : class {
            var listItems = whereCondition != null
                ? await _ts.GetAllAsync(whereCondition)
                : await _ts.GetAllAsync<T>();
            cmb.FillComboBox<T>(cmbName, cmbValue, listItems, assignSelectedValue);
        }
        #endregion

        #region Base DTO CRUD
        protected async virtual Task SaveDataAsync(TModel myDTO) {
            myDTO.Id = Guid.NewGuid();
            await _ts.SaveDTOAndCommitAsync<TModel, TEntity>(myDTO);
        }

        public async Task<bool> SaveDTOAsync(TModel myDTO, bool enabledValidation = false, bool confirmation = true, bool returnResult = true) {
            if (myDTO == null)
                throw new ArgumentNullException($"{nameof(myDTO)} is null!");

            if (enabledValidation && !myDTO.DataValidation()) {
                var sb = new StringBuilder();
                if (!string.IsNullOrEmpty(myDTO.Error))
                    sb.AppendLine("Error 1: " + myDTO.Error);
                if (!string.IsNullOrEmpty(myDTO.ErrorMessage))
                    sb.AppendLine("Error 2: " + myDTO.ErrorMessage);
                if (myDTO.ErrorMessages.Length > 0)
                    sb.AppendLine("Error 3: " + myDTO.ErrorMessages.ToString());
                throw new ArgumentException(sb.ToString());
            }

            if (!myDTO.Success)
                throw new ArgumentException(myDTO.Error);

            try {
                if (confirmation) {
                    if (CDialogManager.Ask("Are you sure to save this data?", "Confirmation")) {
                        await SaveDataAsync(myDTO);
                        CDialogManager.Info("Saved Successfully!", "Success");
                        return true;
                    }
                }
                else {
                    await SaveDataAsync(myDTO);
                    if (returnResult)
                        CDialogManager.Info("Saved Successfully!", "Success");
                    return true;
                }
            }
            catch (DbEntityValidationException ex) {
                var sb = new StringBuilder();
                var validationResult = ex.EntityValidationErrors.FirstOrDefault();
                if (validationResult != null && validationResult.ValidationErrors.Count > 0) {
                    foreach (var validationError in validationResult.ValidationErrors)
                        sb.AppendLine($"{validationError.ErrorMessage}");
                }
                throw new ArgumentException(sb.ToString());
            }
            catch (DbUpdateException ex) {
                var sb = new StringBuilder();
                var innerEx = ex.InnerException;
                int innerLevel = 1;
                while (innerEx != null) {
                    sb.AppendLine($"Inner Exception Level {innerLevel}: {innerEx.Message}\n");
                    if (innerEx is MySqlException mySqlEx)
                        sb.AppendLine($"SQL Error Code: {mySqlEx.Number}\n");
                    else if (innerEx is System.Data.SqlClient.SqlException sqlEx)
                        sb.AppendLine($"SQL Error Code: {sqlEx.Number}\n");
                    innerEx = innerEx.InnerException;
                    innerLevel++;
                }
                if (ex.Entries != null && ex.Entries.Any()) {
                    sb.AppendLine("\nEntities involved in the exception:");
                    foreach (var entry in ex.Entries)
                        sb.AppendLine($"TableName: {entry.Entity.GetType().Name}, Operation: {entry.State}");
                }
                throw new ArgumentException(sb.ToString());
            }
            return false;
        }

        protected async virtual Task UpdateDataAsync(TModel myDTO) {
            await _ts.UpdateDTOAndCommitAsync<TModel, TEntity>(myDTO);
        }

        public async Task<bool> UpdateDTOAsync(TModel myDTO, bool enabledValidation = false, bool confirmation = true, bool returnResult = true) {
            if (myDTO == null)
                throw new ArgumentNullException($"{nameof(myDTO)} is null!");

            if (enabledValidation && !myDTO.DataValidation()) {
                var sb = new StringBuilder();
                if (!string.IsNullOrEmpty(myDTO.Error))
                    sb.AppendLine("Error 1: " + myDTO.Error);
                if (!string.IsNullOrEmpty(myDTO.ErrorMessage))
                    sb.AppendLine("Error 2: " + myDTO.ErrorMessage);
                if (myDTO.ErrorMessages.Length > 0)
                    sb.AppendLine("Error 3: " + myDTO.ErrorMessages.ToString());
                throw new ArgumentException(sb.ToString());
            }

            if (!myDTO.Success)
                throw new ArgumentException(myDTO.Error);

            try {
                if (confirmation) {
                    if (CDialogManager.Ask("Are you sure to update this data?", "Confirmation")) {
                        await UpdateDataAsync(myDTO);
                        CDialogManager.Info("Updated Successfully!", "Success");
                        return true;
                    }
                }
                else {
                    await UpdateDataAsync(myDTO);
                    if (returnResult)
                        CDialogManager.Info("Updated Successfully!", "Success");
                    return true;
                }
            }
            catch (DbEntityValidationException ex) {
                var sb = new StringBuilder();
                var validationResult = ex.EntityValidationErrors.FirstOrDefault();
                if (validationResult != null && validationResult.ValidationErrors.Count > 0) {
                    foreach (var validationError in validationResult.ValidationErrors)
                        sb.AppendLine($"{validationError.ErrorMessage}");
                }
                throw new ArgumentException(sb.ToString());
            }
            catch (DbUpdateException ex) {
                var sb = new StringBuilder();
                var innerEx = ex.InnerException;
                int innerLevel = 1;
                while (innerEx != null) {
                    sb.AppendLine($"Error Level {innerLevel}: {innerEx.Message}\n");
                    if (innerEx is MySqlException mySqlEx)
                        sb.AppendLine($"SQL Error Code: {mySqlEx.Number}\n");
                    else if (innerEx is System.Data.SqlClient.SqlException sqlEx)
                        sb.AppendLine($"SQL Error Code: {sqlEx.Number}\n");
                    innerEx = innerEx.InnerException;
                    innerLevel++;
                }
                if (ex.Entries != null && ex.Entries.Any()) {
                    sb.AppendLine("Tables involved in the exception:");
                    foreach (var entry in ex.Entries)
                        sb.AppendLine($"TableName: {entry.Entity.GetType().Name}\nOperation: {entry.State}");
                }
                throw new ArgumentException(sb.ToString());
            }
            return false;
        }

        protected async virtual Task DeleteDataAsync(TType id) {
            var tbl = await _ts.GetByIdAsync<TEntity, TType>(id);
            if (tbl == null)
                return;
            await _ts.RemoveAndCommitAsync(tbl);
        }

        public async Task<bool> DeleteByIdAsync(TType id) {
            if (id == null) {
                CDialogManager.Warning($"{nameof(id)} is null!");
                return false;
            }
            try {
                if (CDialogManager.Ask("Are you sure to delete this data?", "Confirmation")) {
                    await DeleteDataAsync(id);
                    CDialogManager.Info("Deleted Successfully!", "Success");
                    return true;
                }
            }
            catch (Exception ex) {
                throw ex;
            }
            return false;
        }

        public async Task DeleteMultipleDataByIdsAsync(List<TType> ids) {
            if (ids == null || ids.Count <= 0)
                throw new ArgumentException($"{nameof(ids)} is null!");

            using (var trans = _ts.Database.BeginTransaction()) {
                try {
                    var sb = new StringBuilder();
                    var askMessage = ids.Count > 1 ? "Are you sure to delete these data's?" : "Are you sure to delete this data?";
                    var resultMessage = ids.Count > 1 ? "All the data's selected has been deleted successfully!" : "Deleted Successfully!";

                    if (CDialogManager.Ask(askMessage, "Confirmation")) {
                        foreach (var id in ids) {
                            try {
                                await DeleteDataAsync(id);
                            }
                            catch (Exception) {
                                sb.AppendLine(id.ToString());
                                continue;
                            }
                        }
                        trans.Commit();
                        if (sb.Length <= 0)
                            CDialogManager.Info(resultMessage);
                        else
                            CDialogManager.Warning($"The following id's has not been deleted:\n{sb}");
                    }
                }
                catch (Exception ex) {
                    trans.Rollback();
                    throw ex;
                }
            }
        }
        #endregion

        #region Utilities
        public virtual async Task<bool> HasDataAsync(Expression<Func<TEntity, bool>> predicate = null) {
            return await _ts.HasDataAsync(predicate);
        }
        #endregion

        #region Base Cache Methods
        public virtual async Task LoadCachedAsync() {
            var entities = await _ts.GetAllUnCachedAsync<TEntity>();
            await _ts.SaveAllToCacheAsync(entities);
        }
        #endregion
    }

    public abstract class BaseItemRepository<EntityContext, TModel, TModelItem, TEntity, TEntityItem> :
        BaseRepository<EntityContext, TModel, TEntity, Guid>
        where EntityContext : DbContext
        where TModel : BaseModel
        where TEntity : BaseEntity
        where TModelItem : BaseModelItem
        where TEntityItem : BaseEntityItem {

        #region ctor
        protected BaseItemRepository() : base() { }
        protected BaseItemRepository(EntityContext ts) : base(ts) { }
        #endregion

        #region Base GET for Model Item
        public virtual async Task<List<TModelItem>> GetPrepareModelItemsByParentIdAsync(Guid parentId) {
            var entities = await GetAllItemsByParentIdAsync(parentId);
            return entities.ToDestination<TModelItem>();
        }
        #endregion

        #region Base GET for Item
        public virtual async Task<IEnumerable<TEntityItem>> GetAllItemsByParentIdAsync(Guid parentId) {
            return await _ts.GetAllItemsByParentIdAsync<TEntityItem>(parentId);
        }

        public virtual async Task<TEntityItem> GetItemByParentIdAsync(Guid parentId) {
            return await _ts.GetByParentIdAsync<TEntityItem>(parentId);
        }
        #endregion

        #region Base SAVE for Item
        protected async virtual Task SaveDataAsync(TModel model, List<TModelItem> modelItems) {
            model.Id = Guid.NewGuid();
            await _ts.SaveModelAndCommitAsync<TModel, TModelItem, TEntity, TEntityItem>(model, modelItems);
        }

        public async Task<bool> SaveModelAsync(TModel model, List<TModelItem> modelItems, bool enabledValidation = false, bool confirmation = true, bool returnResult = true) {
            if (model == null)
                throw new ArgumentNullException($"{nameof(model)} is null!");

            if (enabledValidation && !model.DataValidation()) {
                var sb = new StringBuilder();
                if (!string.IsNullOrEmpty(model.Error))
                    sb.AppendLine("Error 1: " + model.Error);
                if (!string.IsNullOrEmpty(model.ErrorMessage))
                    sb.AppendLine("Error 2: " + model.ErrorMessage);
                if (model.ErrorMessages.Length > 0)
                    sb.AppendLine("Error 3: " + model.ErrorMessages.ToString());
                throw new ArgumentException(sb.ToString());
            }

            if (!model.Success)
                throw new ArgumentException(model.Error);

            try {
                if (confirmation) {
                    if (CDialogManager.Ask("Are you sure to save this data?", "Confirmation")) {
                        await SaveDataAsync(model, modelItems);
                        CDialogManager.Info("Saved Successfully!", "Success");
                        return true;
                    }
                }
                else {
                    await SaveDataAsync(model, modelItems);
                    if (returnResult)
                        CDialogManager.Info("Saved Successfully!", "Success");
                    return true;
                }
            }
            catch (DbEntityValidationException ex) {
                var sb = new StringBuilder();
                var validationResult = ex.EntityValidationErrors.FirstOrDefault();
                if (validationResult != null && validationResult.ValidationErrors.Count > 0) {
                    foreach (var validationError in validationResult.ValidationErrors)
                        sb.AppendLine($"{validationError.ErrorMessage}");
                }
                throw new ArgumentException(sb.ToString());
            }
            catch (DbUpdateException ex) {
                var sb = new StringBuilder();
                var innerEx = ex.InnerException;
                int innerLevel = 1;
                while (innerEx != null) {
                    sb.AppendLine($"Inner Exception Level {innerLevel}: {innerEx.Message}\n");
                    if (innerEx is MySqlException mySqlEx)
                        sb.AppendLine($"SQL Error Code: {mySqlEx.Number}\n");
                    else if (innerEx is System.Data.SqlClient.SqlException sqlEx)
                        sb.AppendLine($"SQL Error Code: {sqlEx.Number}\n");
                    innerEx = innerEx.InnerException;
                    innerLevel++;
                }
                if (ex.Entries != null && ex.Entries.Any()) {
                    sb.AppendLine("\nEntities involved in the exception:");
                    foreach (var entry in ex.Entries)
                        sb.AppendLine($"TableName: {entry.Entity.GetType().Name}, Operation: {entry.State}");
                }
                throw new ArgumentException(sb.ToString());
            }
            return false;
        }
        #endregion

        #region Base UPDATE for Item
        protected async virtual Task UpdateDataAsync(TModel model, List<TModelItem> modelItems) {
            await _ts.UpdateModelAndCommitAsync<TModel, TModelItem, TEntity, TEntityItem>(model, modelItems);
        }

        public async Task<bool> UpdateModelAsync(TModel model, List<TModelItem> modelItems, bool enabledValidation = false, bool confirmation = true, bool returnResult = true) {
            if (model == null)
                throw new ArgumentNullException($"{nameof(model)} is null!");

            if (enabledValidation && !model.DataValidation()) {
                var sb = new StringBuilder();
                if (!string.IsNullOrEmpty(model.Error))
                    sb.AppendLine("Error 1: " + model.Error);
                if (!string.IsNullOrEmpty(model.ErrorMessage))
                    sb.AppendLine("Error 2: " + model.ErrorMessage);
                if (model.ErrorMessages.Length > 0)
                    sb.AppendLine("Error 3: " + model.ErrorMessages.ToString());
                throw new ArgumentException(sb.ToString());
            }

            if (!model.Success)
                throw new ArgumentException(model.Error);

            try {
                if (confirmation) {
                    if (CDialogManager.Ask("Are you sure to update this data?", "Confirmation")) {
                        await UpdateDataAsync(model, modelItems);
                        CDialogManager.Info("Updated Successfully!", "Success");
                        return true;
                    }
                }
                else {
                    await UpdateDataAsync(model, modelItems);
                    if (returnResult)
                        CDialogManager.Info("Updated Successfully!", "Success");
                    return true;
                }
            }
            catch (DbEntityValidationException ex) {
                var sb = new StringBuilder();
                var validationResult = ex.EntityValidationErrors.FirstOrDefault();
                if (validationResult != null && validationResult.ValidationErrors.Count > 0) {
                    foreach (var validationError in validationResult.ValidationErrors)
                        sb.AppendLine($"{validationError.ErrorMessage}");
                }
                throw new ArgumentException(sb.ToString());
            }
            catch (DbUpdateException ex) {
                var sb = new StringBuilder();
                var innerEx = ex.InnerException;
                int innerLevel = 1;
                while (innerEx != null) {
                    sb.AppendLine($"Inner Exception Level {innerLevel}: {innerEx.Message}\n");
                    if (innerEx is MySqlException mySqlEx)
                        sb.AppendLine($"SQL Error Code: {mySqlEx.Number}\n");
                    else if (innerEx is System.Data.SqlClient.SqlException sqlEx)
                        sb.AppendLine($"SQL Error Code: {sqlEx.Number}\n");
                    innerEx = innerEx.InnerException;
                    innerLevel++;
                }
                if (ex.Entries != null && ex.Entries.Any()) {
                    sb.AppendLine("\nEntities involved in the exception:");
                    foreach (var entry in ex.Entries)
                        sb.AppendLine($"TableName: {entry.Entity.GetType().Name}, Operation: {entry.State}");
                }
                throw new ArgumentException(sb.ToString());
            }
            return false;
        }
        #endregion

        #region Base DELETE for Item
        protected override async Task DeleteDataAsync(Guid id) {
            var tbl = await _ts.GetByIdAsync<TEntity, Guid>(id);
            if (tbl == null)
                return;
            var items = await _ts.GetAllItemsByParentIdAsync<TEntityItem>(id);
            await _ts.RemoveRangeAndCommitAsync(items);
            await _ts.RemoveAndCommitAsync(tbl);
        }
        #endregion
    }
}