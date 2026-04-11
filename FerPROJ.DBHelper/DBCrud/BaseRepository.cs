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
using System.Data;
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
        where TEntity : BaseEntity {

        #region BaseProperties
        public readonly EntityContext _ts;
        private readonly bool _tsAlone;
        #endregion

        #region ctor
        protected BaseRepository() {
            _ts = Activator.CreateInstance<EntityContext>();
            _tsAlone = true;
            OpenConnection();
        }
        protected BaseRepository(EntityContext ts) {
            _ts = ts;
            _tsAlone = false;
            OpenConnection();
        }
        #endregion

        #region IDisposable
        public void Dispose() {
            if (_tsAlone) {
                _ts?.Dispose();
            }
        }
        public void OpenConnection() {
            if (_ts.Database.Connection.State != ConnectionState.Open) {
                _ts.Database.Connection.Open();
            }
        }
        public void CloseConnection() {
            if (_ts.Database.Connection.State != ConnectionState.Closed) {
                _ts.Database.Connection.Close();
            }
        }
        #endregion

        #region Base GET for Model
        public virtual async Task<TModel> GetPrepareModelAsync(TModel model = null, string prefix = null) {
            if (model == null) {
                model = Activator.CreateInstance<TModel>();
            }
            if (prefix.IsNullOrEmpty()) {
                prefix = $"{typeof(TModel).Name.ToStringUpperLettersOnly()}-";
            }
            model.FormId = await GetGeneratedIDAsync(prefix, false);
            return model;
        }
        public virtual async Task<TModel> GetPrepareModelByEntityAsync(TEntity entity) {
            if (entity.IsNullOrEmpty()) {
                entity = Activator.CreateInstance<TEntity>();
            }

            return entity.ToDestination<TModel>();
        }
        public virtual async Task<TModel> GetPrepareModelByIdAsync(TType id) {
            var entity = await GetByIdAsync(id);
            return await CacheManager.GetOrCreateCacheAsync(CacheManager.ModelPrefix, id, async () => {
                return await GetPrepareModelByEntityAsync(entity);
            });
        }
        public virtual async Task<TModel> GetPrepareModelByPredicateAsync(Expression<Func<TEntity, bool>> predicate) {
            var entity = await GetByPredicateAsync(predicate);
            return await CacheManager.GetOrCreateCacheAsync(CacheManager.ModelPrefix, entity.GetPropertyValue<string>("Id"), async () => {
                return await GetPrepareModelByEntityAsync(entity);
            });
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
        public virtual async Task<IEnumerable<TModel>> GetViewModelWithSearchAsync(string searchText, DateTime? dateFrom, DateTime? dateTo, int dataLimit = int.MaxValue) {

            var query = await GetAllWithSearchAsync(null, dateFrom, dateTo);

            query = query.GetAllActiveOnly();

            query = query.OrderByProperty("DateCreated", false);

            var result = await query.SelectListAsync(async c => {

                return await CacheManager.GetOrCreateCacheAsync(CacheManager.ListModelPrefix, c.GetPropertyValue<string>("Id"), async () => {
                    return await GetPrepareModelByEntityAsync(c);
                });

            }, c => c.SearchForText(searchText), dataLimit);

            return result;
        }
        public virtual async Task<IEnumerable<TModel>> GetViewModelWithSearchAsync(Expression<Func<TEntity, bool>> whereCondition, string searchText, DateTime? dateFrom, DateTime? dateTo, int dataLimit = int.MaxValue) {

            var query = await GetAllAsync(whereCondition);

            query = query.GetAllActiveOnly();

            query = query.OrderByProperty("DateCreated", false);

            var result = await query.SelectListAsync(async c => {

                return await CacheManager.GetOrCreateCacheAsync(CacheManager.ListModelPrefix, c.GetPropertyValue<string>("Id"), async () => {
                    return await GetPrepareModelByEntityAsync(c);
                });

            }, c => c.SearchFor(searchText, dateFrom, dateTo, d => d.DateCreated), dataLimit);

            return result;
        }
        public virtual async Task<(IEnumerable<TModel> ModelItems, int TotalCount)> GetViewModelWithSearchAsync(string searchText, DateTime? dateFrom, DateTime? dateTo, int page, int dataLimit = int.MaxValue) {

            var query = await GetAllWithSearchAsync(null, dateFrom, dateTo, int.MaxValue);

            dataLimit = !searchText.IsNullOrEmpty() ||
                        !dateFrom.IsNullOrEmpty() ||
                        !dateTo.IsNullOrEmpty() ? int.MaxValue : dataLimit;

            query = query.GetAllActiveOnly();

            query = query.OrderByProperty("DateCreated", false);

            var result = await query.SelectListAsync(async c => {

                return await CacheManager.GetOrCreateCacheAsync(CacheManager.ListModelPrefix, c.GetPropertyValue<string>("Id"), async () => {
                    return await GetPrepareModelByEntityAsync(c);
                });

            }, c => c.SearchForText(searchText), page, dataLimit);

            return (result, query.Count());
        }
        public virtual async Task<(IEnumerable<TModel> ModelItems, int TotalCount)> GetViewModelWithSearchAsync(Expression<Func<TEntity, bool>> whereCondition, string searchText, DateTime? dateFrom, DateTime? dateTo, int page, int dataLimit = int.MaxValue) {

            var query = await GetAllAsync(whereCondition);

            query = query.GetAllActiveOnly();

            query = query.OrderByProperty("DateCreated", false);

            dataLimit = !searchText.IsNullOrEmpty() ||
                        !dateFrom.IsNullOrEmpty() ||
                        !dateTo.IsNullOrEmpty() ? int.MaxValue : dataLimit;

            var result = await query.SelectListAsync(async c => {

                return await CacheManager.GetOrCreateCacheAsync(CacheManager.ListModelPrefix, c.GetPropertyValue<string>("Id"), async () => {
                    return await GetPrepareModelByEntityAsync(c);
                });

            }, c => c.SearchFor(searchText, dateFrom, dateTo, d => d.DateCreated), page, dataLimit);

            return (result, query.Count());
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
        public virtual async Task LoadComboBoxAsync(CComboBoxKrypton cmb, string cmbName, string cmbValue, Expression<Func<TEntity, bool>> whereCondition = null) {
            var listItems = whereCondition != null
                ? await _ts.GetAllAsync(whereCondition)
                : await _ts.GetAllAsync<TEntity>();
            cmb.FillComboBox(cmbName, cmbValue, listItems);
        }

        public virtual async Task LoadComboBoxAsync(CComboBoxKrypton cmb, Func<TEntity, string> cmbName, string cmbValue, Expression<Func<TEntity, bool>> whereCondition = null) {
            var listItems = whereCondition != null
                ? await _ts.GetAllAsync(whereCondition)
                : await _ts.GetAllAsync<TEntity>();
            cmb.FillComboBox(cmbName, cmbValue, listItems);
        }

        public virtual async Task LoadComboBoxByEntityAsync<T>(CComboBoxKrypton cmb, string cmbName, string cmbValue, Expression<Func<T, bool>> whereCondition = null) where T : class {
            var listItems = whereCondition != null
                ? await _ts.GetAllAsync(whereCondition)
                : await _ts.GetAllAsync<T>();
            cmb.FillComboBox(cmbName, cmbValue, listItems);
        }

        public virtual async Task LoadComboBoxByEntityAsync<T>(CComboBoxKrypton cmb, Func<T, string> cmbName, string cmbValue, Expression<Func<T, bool>> whereCondition = null) where T : class {
            var listItems = whereCondition != null
                ? await _ts.GetAllAsync(whereCondition)
                : await _ts.GetAllAsync<T>();
            cmb.FillComboBox<T>(cmbName, cmbValue, listItems);
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
            await _ts.SoftRemoveAndCommitAsync(tbl);
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

        #region Execute 
        public async Task ExecuteAsync(
            Func<BaseRepository<EntityContext, TModel, TEntity, TType>, Task> action) {
            try {
                await action(this);
            }
            catch (Exception ex) {
                CDialogManager.Warning($"An error occurred during execution: {ex.Message}", GetType().Name);
            }
            finally {
                Dispose();
            }
        }

        public async Task<TResult> ExecuteAsync<TResult>(
            Func<BaseRepository<EntityContext, TModel, TEntity, TType>, Task<TResult>> action) {
            try {
                return await action(this);
            }
            catch (Exception ex) {
                CDialogManager.Warning($"An error occurred during execution: {ex.Message}", GetType().Name);
                return default;
            }
            finally {
                Dispose();
            }
        }
        public async Task ExecuteAsync<TRepository>(
            Func<TRepository, Task> action) where TRepository : BaseRepository<EntityContext, TModel, TEntity, TType> {
            try {
                // Cast 'this' to the derived repository type
                await action((TRepository)this);
            }
            catch (Exception ex) {
                CDialogManager.Warning($"An error occurred during execution: {ex.Message}", GetType().Name);
            }
            finally {
                Dispose();
            }
        }

        public async Task<TResult> ExecuteAsync<TRepository, TResult>(
            Func<TRepository, Task<TResult>> action) where TRepository : BaseRepository<EntityContext, TModel, TEntity, TType> {
            try {
                // Cast 'this' to the derived repository type
                return await action((TRepository)this);
            }
            catch (Exception ex) {
                CDialogManager.Warning($"An error occurred during execution: {ex.Message}", GetType().Name);
                return default;
            }
            finally {
                Dispose();
            }
        }
        #endregion
    }

    public abstract class BaseItemRepository<EntityContext, TModel, TModelItem, TEntity, TEntityItem> :
        BaseRepository<EntityContext, TModel, TEntity, Guid>
        where EntityContext : DbContext
        where TModel : BaseFormModel<TModelItem>
        where TEntity : BaseEntity
        where TModelItem : BaseModelItem
        where TEntityItem : BaseEntityItem {

        #region ctor
        protected BaseItemRepository() : base() { }
        protected BaseItemRepository(EntityContext ts) : base(ts) { }
        #endregion

        #region Base GET for Model Item
        public override async Task<TModel> GetPrepareModelByEntityAsync(TEntity entity) {
            return await CacheManager.GetOrCreateCacheAsync(CacheManager.ModelPrefix, entity.GetPropertyValue<string>("Id"), async () => {
                var model = await base.GetPrepareModelByEntityAsync(entity);
                model.Items = await GetPrepareModelItemsByParentIdAsync(entity.Id);
                return model;
            });
        }
        public virtual async Task<List<TModelItem>> GetPrepareModelItemsByParentIdAsync(Guid parentId) {

            return await CacheManager.GetOrCreateCacheAsync(CacheManager.ListModelItemPrefix, parentId, async () => {

                var entities = await GetAllItemsByParentIdAsync(parentId);

                var modelItems = new List<TModelItem>();

                foreach (var entity in entities) {

                    var modelItem = await GetPrepareModelItemByEntityAsync(entity);

                    modelItems.Add(modelItem);
                }

                return modelItems;
            });
        }
        public virtual async Task<TModelItem> GetPrepareModelItemByEntityAsync(TEntityItem entity) {
            return entity.ToDestination<TModelItem>();
        }
        #endregion

        #region Base GET for Item
        public virtual async Task<IEnumerable<TEntityItem>> GetAllItemsByParentIdAsync(Guid parentId) {
            return await _ts.GetAllItemsByParentIdAsync<TEntityItem>(parentId);
        }

        public virtual async Task<TEntityItem> GetItemByParentIdAsync(Guid parentId) {
            return await _ts.GetByParentIdAsync<TEntityItem>(parentId);
        }

        public virtual async Task<TEntityItem> GetItemByPredicateAsync(Expression<Func<TEntityItem, bool>> predicate) {
            return await _ts.GetByPredicateAsync(predicate);
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

        #region Base Finalize Data
        protected async virtual Task SaveFinalizeDataAsync(TModel model, List<TModelItem> modelItems) {
            model.FinalizeStatus = FinalizeStatusTypes.Completed.ToString();
            await _ts.UpdateModelAndCommitAsync<TModel, TModelItem, TEntity, TEntityItem>(model, modelItems);
        }

        public async Task<bool> SaveFinalizeModelAsync(TModel model, List<TModelItem> modelItems, bool enabledValidation = false, bool confirmation = true, bool returnResult = true) {
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
                    if (CDialogManager.Ask("You won’t be able to update this data again.\n" +
                                           "Are you sure to finalize this data?", "Confirmation")) {
                        await SaveFinalizeDataAsync(model, modelItems);
                        CDialogManager.Info("Saved Successfully!", "Success");
                        return true;
                    }
                }
                else {
                    await SaveFinalizeDataAsync(model, modelItems);
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

        #region Execute 
        public async Task ExecuteAsync(
            Func<BaseItemRepository<EntityContext, TModel, TModelItem, TEntity, TEntityItem>, Task> action) {
            try {
                await action(this);
            }
            catch (Exception ex) {
                CDialogManager.Warning($"An error occurred during execution: {ex.Message}", GetType().Name);
            }
            finally {
                Dispose();
            }
        }

        public async Task<TResult> ExecuteAsync<TResult>(
            Func<BaseItemRepository<EntityContext, TModel, TModelItem, TEntity, TEntityItem>, Task<TResult>> action) {
            try {
                return await action(this);
            }
            catch (Exception ex) {
                CDialogManager.Warning($"An error occurred during execution: {ex.Message}", GetType().Name);
                return default;
            }
            finally {
                Dispose();
            }
        }
        #endregion
    }
}