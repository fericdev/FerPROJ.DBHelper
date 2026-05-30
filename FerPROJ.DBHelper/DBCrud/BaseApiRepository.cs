using FerPROJ.DBHelper.ApiRepository;
using FerPROJ.DBHelper.DBCache;
using FerPROJ.DBHelper.DBExtensions;
using FerPROJ.DBHelper.Entity;
using FerPROJ.DBHelper.Entity.Cache;
using FerPROJ.Design.BaseModels;
using FerPROJ.Design.Class;
using FerPROJ.Design.Controls;
using FerPROJ.Design.FormModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace FerPROJ.DBHelper.DBCrud {
    public abstract class BaseApiRepository<TModel, TEntity>
        where TModel : BaseModel
        where TEntity : BaseEntity {
        protected readonly string _endpoint;

        #region CTOR
        protected BaseApiRepository(string endpoint) {
            _endpoint = endpoint;
        }
        protected BaseApiRepository() {
            _endpoint = "BaseApiRepository.php";
        }
        #endregion

        #region Get Model
        public virtual async Task<IEnumerable<TModel>> GetAllPrepareModelAsync() {

            var entities = await GetAllAsync();

            var modelList = new List<TModel>();

            foreach (var entity in entities) {

                var model = await GetPrepareModelByEntityAsync(entity);

                modelList.Add(model);
            }

            return modelList;
        }
        public virtual async Task<TModel> GetPrepareModelByEntityAsync(TEntity entity) {

            if (entity.IsNullOrEmpty()) {
                entity = Activator.CreateInstance<TEntity>();
            }

            return entity.ToDestination<TModel>();
        }
        public virtual async Task<TModel> GetPrepareModelByIdAsync(Guid id) {

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

        #region Get View Model
        public virtual async Task<(IEnumerable<TModel> ModelItems, int TotalCount)> GetViewModelWithSearchAsync(string searchText, DateTime? dateFrom, DateTime? dateTo, int page, int dataLimit = int.MaxValue) {


            if (dateFrom.IsCurrentDate() && dateTo.IsCurrentDate() && !searchText.IsNullOrEmpty()) {
                dateFrom = null;
                dateTo = null;
            }

            var query = await GetAllAsync();

            dataLimit = !searchText.IsNullOrEmpty() ||
                        !dateFrom.IsNullOrEmpty() ||
                        !dateTo.IsNullOrEmpty() ? int.MaxValue : dataLimit;

            query = query.GetAllActiveOnly();

            query = query.OrderByProperty("DateMarked", false);

            var result = await query.SelectListAsync(async c => {

                return await CacheManager.GetOrCreateCacheAsync(CacheManager.ListModelPrefix, c.GetPropertyValue<string>("Id"), async () => {
                    return await GetPrepareModelByEntityAsync(c);
                });

            }, c => c.SearchForText(searchText), page, dataLimit);

            return (result, query.Count());
        }
        public virtual async Task<(IEnumerable<TModel> ModelItems, int TotalCount)> GetViewModelWithSearchAsync(Expression<Func<TEntity, bool>> whereCondition, string searchText, DateTime? dateFrom, DateTime? dateTo, int page, int dataLimit = int.MaxValue) {

            if (dateFrom.IsCurrentDate() && dateTo.IsCurrentDate() && !searchText.IsNullOrEmpty()) {
                dateFrom = null;
                dateTo = null;
            }

            var query = await GetAllAsync(whereCondition);

            query = query.GetAllActiveOnly();

            query = query.OrderByProperty("DateMarked", false);

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
        public virtual async Task<IEnumerable<TModel>> GetViewModelWithSearchAsync(string searchText, DateTime? dateFrom, DateTime? dateTo, int dataLimit = int.MaxValue) {

            if (dateFrom.IsCurrentDate() && dateTo.IsCurrentDate() && !searchText.IsNullOrEmpty()) {
                dateFrom = null;
                dateTo = null;
            }

            var query = await GetAllAsync();

            query = query.GetAllActiveOnly();

            query = query.OrderByProperty("DateMarked", false);

            dataLimit = !searchText.IsNullOrEmpty() ||
                        !dateFrom.IsNullOrEmpty() ||
                        !dateTo.IsNullOrEmpty() ? int.MaxValue : dataLimit;

            var result = await query.SelectListAsync(async c => {

                return await CacheManager.GetOrCreateCacheAsync(CacheManager.ListModelPrefix, c.GetPropertyValue<string>("Id"), async () => {
                    return await GetPrepareModelByEntityAsync(c);
                });

            }, c => c.SearchFor(searchText, dateFrom, dateTo, d => d.DateCreated), dataLimit);

            return result;
        }
        public virtual async Task<IEnumerable<TModel>> GetViewModelWithSearchAsync(Expression<Func<TEntity, bool>> whereCondition, string searchText, DateTime? dateFrom, DateTime? dateTo, int dataLimit = int.MaxValue) {

            if (dateFrom.IsCurrentDate() && dateTo.IsCurrentDate() && !searchText.IsNullOrEmpty()) {
                dateFrom = null;
                dateTo = null;
            }

            var query = await GetAllAsync(whereCondition);

            query = query.GetAllActiveOnly();

            query = query.OrderByProperty("DateMarked", false);

            dataLimit = !searchText.IsNullOrEmpty() ||
                        !dateFrom.IsNullOrEmpty() ||
                        !dateTo.IsNullOrEmpty() ? int.MaxValue : dataLimit;

            var result = await query.SelectListAsync(async c => {

                return await CacheManager.GetOrCreateCacheAsync(CacheManager.ListModelPrefix, c.GetPropertyValue<string>("Id"), async () => {
                    return await GetPrepareModelByEntityAsync(c);
                });

            }, c => c.SearchFor(searchText, dateFrom, dateTo, d => d.DateCreated), dataLimit);

            return result;
        }
        #endregion

        #region Get Entity
        public virtual async Task<IEnumerable<T>> GetAllAsync<T>(
                Expression<Func<T, bool>> predicate = null,
                Expression<Func<T, object>> orderBy = null,
                bool descending = false,
                int? take = null
            ) where T : BaseEntity {

            var query = new List<string>();

            // predicate
            if (predicate != null) {
                query.Add(predicate.ToQuery());
            }

            // order by
            if (orderBy != null) {
                var memberExpression = orderBy.Body as MemberExpression;

                // handle boxing for value types
                if (memberExpression == null &&
                    orderBy.Body is UnaryExpression unary) {
                    memberExpression =
                        unary.Operand as MemberExpression;
                }

                if (memberExpression != null) {
                    query.Add(
                        $"orderBy={memberExpression.Member.Name}"
                    );

                    query.Add(
                        $"descending={descending}"
                    );
                }
            }

            // take
            if (take.HasValue) {
                query.Add($"take={take.Value}");
            }

            var url = GetUrl(ActionTypes.Get);

            if (query.Any()) {
                url += "?" + string.Join("&", query);
            }

            return await GetAllAsync<T>(url);
        }
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(
                Expression<Func<TEntity, bool>> predicate = null,
                Expression<Func<TEntity, object>> orderBy = null,
                bool descending = false,
                int? take = null
            ) {

            var query = new List<string>();

            // predicate
            if (predicate != null) {
                query.Add(predicate.ToQuery());
            }

            // order by
            if (orderBy != null) {
                var memberExpression = orderBy.Body as MemberExpression;

                // handle boxing for value types
                if (memberExpression == null &&
                    orderBy.Body is UnaryExpression unary) {
                    memberExpression =
                        unary.Operand as MemberExpression;
                }

                if (memberExpression != null) {
                    query.Add(
                        $"orderBy={memberExpression.Member.Name}"
                    );

                    query.Add(
                        $"descending={descending}"
                    );
                }
            }

            // take
            if (take.HasValue) {
                query.Add($"take={take.Value}");
            }

            var url = GetUrl(ActionTypes.Get);

            if (query.Any()) {
                url += "?" + string.Join("&", query);
            }

            return await GetAllAsync<TEntity>(url);
        }

        // ✅ GET ALL
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync() {
            return await CacheManager.GetOrCreateCacheAsync(CacheManager.ListEntityPrefix, typeof(TEntity).Name, async () => {
                return await CApiManager.GetAsync<List<TEntity>>(GetUrl(ActionTypes.Get));
            });
        }
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(string url) {
            return await CacheManager.GetOrCreateCacheAsync(CacheManager.ListEntityPrefix, typeof(TEntity).Name + url, async () => {
                return await CApiManager.GetAsync<List<TEntity>>(url);
            });
        }
        public virtual async Task<IEnumerable<T>> GetAllAsync<T>(string url) where T : BaseEntity {
            return await CacheManager.GetOrCreateCacheAsync(CacheManager.ListEntityPrefix, typeof(T).Name + url, async () => {
                return await CApiManager.GetAsync<List<T>>(url);
            });
        }
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate) {
            var url = GetUrl(ActionTypes.Get) + predicate.ToQuery();
            return await GetAllAsync(url);
        }

        // ✅ GET BY ID
        public virtual async Task<TEntity> GetByIdAsync(Guid id) {
            var result = await GetAllAsync(GetUrl(ActionTypes.Get, ("Id", id)));
            return result.FirstOrDefault();
        }

        // ✅ SEARCH
        public virtual async Task<TEntity> GetByPredicateAsync(Expression<Func<TEntity, bool>> predicate) {
            var url = GetUrl(ActionTypes.Get) + predicate.ToQuery();
            var result = await GetAllAsync(url);
            return result.FirstOrDefault();
        }
        public virtual async Task<T> GetByPredicateAsync<T>(Expression<Func<T, bool>> predicate) where T : BaseEntity {
            var url = GetUrl(ActionTypes.Get) + predicate.ToQuery();
            var result = await GetAllAsync<T>(url);
            return result.FirstOrDefault();
        }
        public virtual async Task<TReturn> GetRawQueryAsync<TReturn>(string rawQuery, string property) {
            var url = GetUrl(ActionTypes.RawQuery, ("query", rawQuery));
            var result = await CApiManager.GetAsync<string>(url);
            using (var doc = JsonDocument.Parse(result)) {
                var root = doc.RootElement;
                var rawValue = root
                    .GetProperty("data")
                    .EnumerateArray()
                    .FirstOrDefault()
                    .GetProperty(property)
                    .GetRawText();

                return rawValue.To<TReturn>();
            }

        }
        #endregion

        #region Load ComboBox
        public virtual async Task LoadComboBoxAsync(CComboBoxKrypton cmb, Expression<Func<TEntity, bool>> whereCondition = null) {
            var listItems = whereCondition != null
                ? await GetAllAsync(whereCondition)
                : await GetAllAsync();
            cmb.FillComboBox("Name", "Id", listItems);
        }
        public virtual async Task LoadComboBoxAsync(CComboBoxKrypton cmb, string cmbName, string cmbValue, Expression<Func<TEntity, bool>> whereCondition = null) {
            var listItems = whereCondition != null
                ? await GetAllAsync(whereCondition)
                : await GetAllAsync();
            cmb.FillComboBox(cmbName, cmbValue, listItems);
        }
        #endregion

        #region CRUD
        // ✅ CREATE
        public virtual async Task<bool> SaveModelAsync(TModel model, bool validate = false) {
            if (!IsResultSuccess(model, validate)) {
                return false;
            }
            if (CDialogManager.Ask("Are you sure to save this data?", "Confirmation")) {
                var entity = model.ToDestination<TEntity>();
                await SaveDataAsync(entity);
                await ClearCacheAsync();
                await new CacheVersionApiRepository().ExecuteUpdateCacheAsync();
                CDialogManager.Info("Data saved successfully.");
                return true;
            }
            return false;
        }
        public virtual async Task SaveDataAsync(TEntity entity) {
            entity.Id = Guid.NewGuid();
            await CApiManager.PostAsync<TEntity, object>(GetUrl(ActionTypes.Save), entity);
        }
        public virtual async Task SaveDataAsync<T>(T entity) where T : BaseEntity {
            entity.Id = Guid.NewGuid();
            await CApiManager.PostAsync<T, object>(GetUrl(ActionTypes.Save), entity);
        }

        // ✅ UPDATE
        public virtual async Task<bool> UpdateModelAsync(TModel model, bool validate = false) {
            if (!IsResultSuccess(model, validate)) {
                return false;
            }
            if (CDialogManager.Ask("Are you sure to update this data?", "Confirmation")) {
                var existingEntity = await GetByIdAsync(model.Id);
                var entity = model.ToDestination(existingEntity);
                await UpdateDataAsync(entity);
                await ClearCacheAsync();
                await new CacheVersionApiRepository().ExecuteUpdateCacheAsync();
                CDialogManager.Info("Data updated successfully.");
                return true;
            }
            return false;
        }
        public virtual async Task<bool> UpdateDataAsync(TEntity entity) {
            return await CApiManager.PostAsync(GetUrl(ActionTypes.Update), entity);
        }
        public virtual async Task<bool> UpdateDataAsync<T>(T entity) where T : BaseEntity {
            return await CApiManager.PostAsync(GetUrl(ActionTypes.Update), entity);
        }

        // ✅ DELETE
        public virtual async Task<bool> DeleteByIdAsync(Guid id) {
            if (id == null) {
                throw new ArgumentNullException(nameof(id));
            }

            if (CDialogManager.Ask("Are you sure to delete this data?", "Confirmation")) {
                await ClearCacheAsync();
                await new CacheVersionApiRepository().ExecuteUpdateCacheAsync();
                await DeleteDataAsync(id);
                CDialogManager.Info("Data deleted successfully.");
                return true;
            }
            return false;
        }
        public virtual async Task<bool> DeleteDataAsync(Guid id) {
            var entity = await GetByIdAsync(id);
            entity.Status = CAppConstants.IN_ACTIVE_STATUS;
            return await UpdateDataAsync(entity);
        }
        #endregion

        #region Validation
        // 🔹 SHARED VALIDATION
        protected bool IsResultSuccess(TModel model, bool validate) {
            return validate ? model.DataValidationResult() : true;
        }
        #endregion

        #region Utilities
        protected virtual string GetUrl(ActionTypes actionType, params (string Key, object Value)[] segments) {
            var sb = new StringBuilder(_endpoint);

            sb.Append($"?action={actionType.ToString().ToLower()}");
            sb.Append($"&table={Uri.EscapeDataString(typeof(TEntity).Name)}");

            foreach (var seg in segments) {
                if (seg.Value == null) continue;

                sb.Append($"&{seg.Key}={Uri.EscapeDataString(seg.Value.ToString())}");
            }

            return sb.ToString();
        }
        protected async Task ClearCacheAsync() {
            CacheManager.ClearCacheByPrefix(CacheManager.ModelPrefix);
            CacheManager.ClearCacheByPrefix(CacheManager.ListModelPrefix);
            CacheManager.ClearCacheByPrefix(CacheManager.ModelItemPrefix);
            CacheManager.ClearCacheByPrefix(CacheManager.ListModelItemPrefix);
            CacheManager.ClearCacheByPrefix(CacheManager.EntityItemPrefix);
            CacheManager.ClearCacheByPrefix(CacheManager.EntityPrefix);
            CacheManager.ClearCacheByPrefix(CacheManager.ListEntityPrefix);
            CacheManager.ClearCacheByPrefix(CacheManager.ListEntityItemPrefix);
        }
        #endregion
    }
    public abstract class BaseItemApiRepository<TModel, TModelItem, TEntity, TEntityItem> : BaseApiRepository<TModel, TEntity>
        where TModel : BaseFormModel<TModelItem>
        where TModelItem : BaseModelItem
        where TEntityItem : BaseEntityItem
        where TEntity : BaseEntity {

        protected readonly string _endpointItem;

        #region CTOR
        protected BaseItemApiRepository(string endpointItem) {
            _endpointItem = endpointItem;
        }
        protected BaseItemApiRepository() {
            _endpointItem = "BaseApiRepository.php";
        }
        #endregion

        #region Get View Model Item
        public virtual async Task<(IEnumerable<TModelItem> ModelItems, int TotalCount)> GetViewModelItemWithSearchAsync(string searchText, int page, int dataLimit = int.MaxValue) {

            var query = await GetAllItemsAsync();

            dataLimit = !searchText.IsNullOrEmpty() ? int.MaxValue : dataLimit;

            var result = await query.SelectListAsync(async c => {

                return await CacheManager.GetOrCreateCacheAsync(CacheManager.ListModelItemPrefix, c.GetPropertyValue<string>("Id"), async () => {
                    return await GetPrepareModelItemByEntityAsync(c);
                });

            }, c => c.SearchForText(searchText), page, dataLimit);

            return (result, query.Count());
        }
        public virtual async Task<(IEnumerable<TModelItem> ModelItems, int TotalCount)> GetViewModelItemWithSearchAsync(Expression<Func<TEntityItem, bool>> whereCondition, string searchText, int page, int dataLimit = int.MaxValue) {

            var query = await GetAllItemsAsync(whereCondition);

            dataLimit = !searchText.IsNullOrEmpty() ? int.MaxValue : dataLimit;

            var result = await query.SelectListAsync(async c => {

                return await CacheManager.GetOrCreateCacheAsync(CacheManager.ListModelItemPrefix, c.GetPropertyValue<string>("Id"), async () => {
                    return await GetPrepareModelItemByEntityAsync(c);
                });

            }, c => c.SearchForText(searchText), page, dataLimit);

            return (result, query.Count());
        }
        public virtual async Task<IEnumerable<TModelItem>> GetViewModelItemWithSearchAsync(string searchText, int dataLimit = int.MaxValue) {

            var query = await GetAllItemsAsync();

            var result = await query.SelectListAsync(async c => {

                return await CacheManager.GetOrCreateCacheAsync(CacheManager.ListModelItemPrefix, c.GetPropertyValue<string>("Id"), async () => {
                    return await GetPrepareModelItemByEntityAsync(c);
                });

            }, c => c.SearchForText(searchText), dataLimit);

            return result;
        }
        public virtual async Task<IEnumerable<TModelItem>> GetViewModelItemWithSearchAsync(Expression<Func<TEntityItem, bool>> whereCondition, string searchText, int dataLimit = int.MaxValue) {

            var query = await GetAllItemsAsync(whereCondition);

            var result = await query.SelectListAsync(async c => {

                return await CacheManager.GetOrCreateCacheAsync(CacheManager.ListModelItemPrefix, c.GetPropertyValue<string>("Id"), async () => {
                    return await GetPrepareModelItemByEntityAsync(c);
                });

            }, c => c.SearchForText(searchText), dataLimit);

            return result;
        }
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

                if (entities.IsNullOrEmpty()) {
                    return modelItems;
                }

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
        public virtual async Task<TModelItem> GetPrepareModelItemByIdAsync(Guid id) {
            var entity = await GetItemByIdAsync(id);
            return await CacheManager.GetOrCreateCacheAsync(CacheManager.ModelItemPrefix, id, async () => {
                return await GetPrepareModelItemByEntityAsync(entity);
            });
        }
        #endregion

        #region Base GET for Item
        public virtual async Task<IEnumerable<TEntityItem>> GetAllItemsAsync() {
            return await CacheManager.GetOrCreateCacheAsync(CacheManager.ListEntityItemPrefix, typeof(TEntityItem).Name, async () => {
                return await CApiManager.GetAsync<List<TEntityItem>>(GetItemUrl(ActionTypes.Get));
            });
        }
        public virtual async Task<IEnumerable<TEntityItem>> GetAllItemsAsync(string url) {
            return await CacheManager.GetOrCreateCacheAsync(CacheManager.ListEntityItemPrefix, typeof(TEntity).Name + url, async () => {
                return await CApiManager.GetAsync<List<TEntityItem>>(url);
            });
        }
        public virtual async Task<IEnumerable<TEntityItem>> GetAllItemsAsync(Expression<Func<TEntityItem, bool>> predicate) {
            var url = GetItemUrl(ActionTypes.Get) + predicate.ToQuery();
            return await GetAllItemsAsync(url);
        }
        public virtual async Task<IEnumerable<TEntityItem>> GetAllItemsByParentIdAsync(Guid parentId) {
            return await GetAllItemsAsync(c => c.ParentId.Equals(parentId));
        }
        public virtual async Task<TEntityItem> GetItemByIdAsync(Guid id) {
            var result = await GetAllItemsAsync(c => c.Id == id);
            return result.FirstOrDefault();
        }
        #endregion

        #region Base CRUD for Item
        public virtual async Task SaveItemDataAsync(TEntityItem entity) {
            await CApiManager.PostAsync<TEntityItem, object>(GetItemUrl(ActionTypes.Save), entity);
        }
        public virtual async Task<bool> UpdateItemDataAsync(TEntityItem entity) {
            return await CApiManager.PostAsync(GetItemUrl(ActionTypes.Update), entity);
        }
        public virtual async Task<bool> DeleteItemByIdAsync(Guid id) {
            return await CApiManager.DeleteAsync(GetItemUrl(ActionTypes.Delete, ("Id", id)));
        }
        public override async Task<bool> SaveModelAsync(TModel model, bool validate = false) {
            if (!IsResultSuccess(model, validate)) {
                return false;
            }
            if (CDialogManager.Ask("Are you sure to save this data?", "Confirmation")) {

                var entity = model.ToDestination<TEntity>();

                await SaveDataAsync(entity);

                await ClearCacheAsync();

                await new CacheVersionApiRepository().ExecuteUpdateCacheAsync();

                foreach (var item in model.Items) {

                    var itemEntity = item.ToDestination<TEntityItem>();

                    itemEntity.Id = Guid.NewGuid();

                    itemEntity.ParentId = entity.Id;

                    await SaveItemDataAsync(itemEntity);
                }
                CDialogManager.Info("Data saved successfully.");

                return true;

            }
            return false;
        }
        public override async Task<bool> UpdateModelAsync(TModel model, bool validate = false) {
            if (!IsResultSuccess(model, validate)) {
                return false;
            }

            if (CDialogManager.Ask("Are you sure to update this data?", "Confirmation")) {

                var existingEntity = await GetByIdAsync(model.Id);

                var entity = model.ToDestination(existingEntity);

                await UpdateDataAsync(entity);

                await ClearCacheAsync();

                await new CacheVersionApiRepository().ExecuteUpdateCacheAsync();

                foreach (var item in model.Items) {

                    var existingItemEntity = await GetItemByIdAsync(item.Id);

                    if (existingItemEntity.IsNullOrEmpty()) {

                        var newItemEntity = item.ToDestination<TEntityItem>();

                        newItemEntity.Id = Guid.NewGuid();

                        newItemEntity.ParentId = model.Id;

                        item.Id = newItemEntity.Id;

                        item.ParentId = newItemEntity.ParentId;

                        await SaveItemDataAsync(newItemEntity);
                    }
                    else {

                        var newItemEntity = model.ToDestination(existingItemEntity);

                        await UpdateItemDataAsync(newItemEntity);
                    }
                }
                // Handle deletions
                var entityItems = new CMappingExtensionList<TModelItem, TEntityItem>().GetMappingResultList(model.Items);
                var existingItems = await GetAllItemsByParentIdAsync(model.Id);

                // Check for incoming ids
                var incomingIds = entityItems
                    .Select(x => x.Id)
                    .Where(id => !id.IsNullOrEmpty())
                    .ToHashSet();

                // Items for removal
                var entityItemsForDeletion = existingItems
                    .Where(item => !incomingIds.Contains(item.Id))
                    .ToList();

                // Delete items that are not in the incoming list
                foreach (var item in entityItemsForDeletion) {
                    await DeleteItemByIdAsync(item.Id);
                }

                CDialogManager.Info("Data updated successfully.");

                return true;
            }
            return false;
        }
        #endregion

        #region Utilities
        protected string GetItemUrl(ActionTypes actionType, params (string Key, object Value)[] segments) {
            var sb = new StringBuilder(_endpointItem);

            sb.Append($"?action={actionType.ToString().ToLower()}");
            sb.Append($"&table={Uri.EscapeDataString(typeof(TEntityItem).Name)}");

            foreach (var seg in segments) {
                if (seg.Value == null) continue;

                sb.Append($"&{seg.Key}={Uri.EscapeDataString(seg.Value.ToString())}");
            }

            return sb.ToString();
        }
        #endregion

    }
    public abstract class BaseFormApiRepository<TModel, TEntity> : BaseApiRepository<TModel, TEntity>
        where TModel : BaseFormModel
        where TEntity : BaseFormEntity {

        #region CTOR
        protected BaseFormApiRepository() {
        }
        #endregion

        #region Generate Form Id
        public async Task<string> GetGeneratedFormIdAsync(string prefix) {
            if (prefix.IsNullOrEmpty()) {
                prefix = string.Concat(typeof(TEntity).Name.Where(char.IsUpper).Take(2));
            }
            var currentCount = await GetRawQueryAsync<int>($"SELECT COUNT(*) AS Total FROM {typeof(TEntity).Name}", "Total");
            return $"{prefix}-{currentCount:D4}";
        }
        #endregion

        #region Get Form Model
        public async Task<TModel> GetPrepareModelAsync(TModel model = null, string prefix = null) {
            if (model.IsNullOrEmpty()) {
                model = Activator.CreateInstance<TModel>();
                model.FormId = "DRAFT";
            }
            else {
                model.FormId = await GetGeneratedFormIdAsync(prefix);
            }
            model.Id = Guid.NewGuid();
            return model;
        }
        #endregion

        #region CRUD
        public override async Task<bool> UpdateModelAsync(TModel model, bool validate = false) {
            if (!IsResultSuccess(model, validate)) {
                return false;
            }
            return await base.UpdateModelAsync(model, validate);
        }
        public override async Task<bool> SaveModelAsync(TModel model, bool validate = false) {
            if (!IsResultSuccess(model, validate)) {
                return false;
            }
            model = await GetPrepareModelAsync(model);
            return await base.SaveModelAsync(model, validate);
        }
        #endregion
    }
    public abstract class BaseFormItemApiRepository<TModel, TModelItem, TEntity, TEntityItem> : BaseItemApiRepository<TModel, TModelItem, TEntity, TEntityItem>
        where TModel : BaseFormModel<TModelItem>
        where TModelItem : BaseModelItem
        where TEntityItem : BaseEntityItem
        where TEntity : BaseFormEntity {

        #region CTOR
        protected BaseFormItemApiRepository() {
        }
        #endregion

        #region Generate Form Id
        public async Task<string> GetGeneratedFormIdAsync(string prefix) {
            if (prefix.IsNullOrEmpty()) {
                prefix = string.Concat(typeof(TEntity).Name.Where(char.IsUpper).Take(2));
            }
            var currentCount = await GetRawQueryAsync<int>($"SELECT COUNT(*) AS Total FROM {typeof(TEntity).Name}", "Total");
            return $"{prefix}-{currentCount:D4}";
        }
        #endregion

        #region Get Form Model
        public async Task<TModel> GetPrepareModelAsync(TModel model = null, string prefix = null) {
            if (model.IsNullOrEmpty()) {
                model = Activator.CreateInstance<TModel>();
                model.FormId = "DRAFT";
            }
            else {
                model.FormId = await GetGeneratedFormIdAsync(prefix);
            }
            model.Id = Guid.NewGuid();
            return model;
        }
        #endregion

        #region CRUD
        public override async Task<bool> UpdateModelAsync(TModel model, bool validate = false) {
            if (!IsResultSuccess(model, validate)) {
                return false;
            }
            return await base.UpdateModelAsync(model, validate);
        }
        public override async Task<bool> SaveModelAsync(TModel model, bool validate = false) {
            if (!IsResultSuccess(model, validate)) {
                return false;
            }
            model = await GetPrepareModelAsync(model);
            return await base.SaveModelAsync(model, validate);
        }
        #endregion
    }


    public enum ActionTypes {
        Get,
        RawQuery,
        Save,
        Update,
        Delete,
    }
}
