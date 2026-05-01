using FerPROJ.DBHelper.DBCache;
using FerPROJ.DBHelper.DBExtensions;
using FerPROJ.DBHelper.Entity;
using FerPROJ.Design.BaseModels;
using FerPROJ.Design.Class;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FerPROJ.DBHelper.DBCrud {
    public abstract class BaseApiRepository<TModel, TEntity, TType>
        where TModel : BaseModel
        where TEntity : BaseEntity {
        protected readonly string _endpoint;

        #region CTOR
        protected BaseApiRepository(string endpoint) {
            _endpoint = endpoint;
        }
        #endregion

        #region Get Model
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

            query = query.Where(c => c.ApplicationId == CAppConstants.APPLLICATION_ID);

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
        #endregion

        #region Get Entity

        // ✅ GET ALL
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync() {
            return await CApiManager.GetAsync<List<TEntity>>(GetUrl(ActionTypes.Get));
        }
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(string url) {
            return await CApiManager.GetAsync<List<TEntity>>(url);
        }
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate) {
            var url = GetUrl(ActionTypes.Get) + predicate.ToQuery();
            return await GetAllAsync(url);
        }

        // ✅ GET BY ID
        public virtual async Task<TEntity> GetByIdAsync(TType id) {
            var result = await GetAllAsync(GetUrl(ActionTypes.Get, id));
            return result.FirstOrDefault();
        }

        // ✅ SEARCH
        public virtual async Task<TEntity> GetByPredicateAsync(Expression<Func<TEntity, bool>> predicate) {
            var url = GetUrl(ActionTypes.Get) + predicate.ToQuery();
            var result = await GetAllAsync(url);
            return result.FirstOrDefault();
        }
        #endregion

        #region CRUD
        // ✅ CREATE
        public virtual async Task<bool> SaveDTOAsync(TModel model, bool validate = false) {
            ValidateModel(model, validate);

            var entity = model.ToDestination<TEntity>();

            await SaveDataAsync(entity);

            return true;
        }
        public virtual async Task SaveDataAsync(TEntity entity) {
            await CApiManager.PostAsync<TEntity, object>(GetUrl(ActionTypes.Save), entity);
        }

        // ✅ UPDATE
        public virtual async Task<bool> UpdateDTOAsync(TType id, TModel model, bool validate = false) {
            ValidateModel(model, validate);

            var entity = model.ToDestination<TEntity>();

            return await UpdateDataAsync(id, entity);
        }
        public virtual async Task<bool> UpdateDataAsync(TType id, TEntity entity) {
            return await CApiManager.PostAsync(GetUrl(ActionTypes.Update, id), entity);
        }

        // ✅ DELETE
        public virtual async Task<bool> DeleteByIdAsync(TType id) {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            return await CApiManager.DeleteAsync(GetUrl(ActionTypes.Delete, id));
        }
        #endregion

        #region Validation
        // 🔹 SHARED VALIDATION
        private void ValidateModel(TModel model, bool validate) {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (validate && !model.DataValidation())
                throw new ArgumentException(model.ErrorMessage);

            if (!model.Success)
                throw new ArgumentException(model.Error);
        }
        #endregion

        #region Utilities
        protected string GetUrl(ActionTypes actionType, params object[] segments) {
            var sb = new StringBuilder(_endpoint);

            sb.Append($"?action={actionType.ToString().ToLower()}");

            foreach (var seg in segments) {
                if (seg == null) continue;

                sb.Append($"&value={Uri.EscapeDataString(seg.ToString())}");
            }

            return sb.ToString();
        }
        #endregion
    }
    public enum ActionTypes {
        Get,
        Save,
        Update,
        Delete,
    }
}
