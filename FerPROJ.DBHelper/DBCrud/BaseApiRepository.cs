using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FerPROJ.Design.BaseModels;
using FerPROJ.Design.Class;
using System.Linq.Expressions;
using FerPROJ.DBHelper.Entity;
using FerPROJ.DBHelper.DBExtensions;
using System.Linq;

namespace FerPROJ.DBHelper.DBCrud
{
    public abstract class BaseApiRepository<TModel, TEntity, TType>
        where TModel : BaseModel
        where TEntity : BaseEntity {
        public string Endpoint { get; set; }

        protected string GetUrl(params object[] segments) {
            var sb = new StringBuilder(Endpoint);

            foreach (var seg in segments) {
                sb.Append($"/{seg}");
            }

            return sb.ToString();
        }

        // ✅ GET ALL
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync() {
            return await CApiManager.GetAsync<List<TEntity>>(GetUrl());
        }
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(string url) {
            return await CApiManager.GetAsync<List<TEntity>>(url);
        }

        // ✅ GET BY ID
        public virtual async Task<TEntity> GetByIdAsync(TType id) {
            var result = await GetAllAsync(GetUrl(id));
            return result.FirstOrDefault();
        }

        // ✅ SEARCH
        public virtual async Task<TEntity> GetByPredicateAsync(Expression<Func<TEntity, bool>> predicate) {
            var url = GetUrl() + "?action=get&" + predicate.ToQuery();
            var result = await GetAllAsync(url);
            return result.FirstOrDefault();
        }

        // ✅ CREATE
        public virtual async Task<bool> SaveDTOAsync(TModel model, bool validate = false) {
            ValidateModel(model, validate);

            var entity = model.ToDestination<TEntity>();

            await SaveDataAsync(entity);

            return true;
        }
        public virtual async Task SaveDataAsync(TEntity entity) {
            await CApiManager.PostAsync<TEntity, object>(GetUrl() + "?action=save", entity);
        }

        // ✅ UPDATE
        public virtual async Task<bool> UpdateDTOAsync(TType id, TModel model, bool validate = false) {
            ValidateModel(model, validate);

            var entity = model.ToDestination<TEntity>();

            return await UpdateDataAsync(id, entity);
        }
        public virtual async Task<bool> UpdateDataAsync(TType id, TEntity entity) {
            return await CApiManager.PostAsync(GetUrl(id) + "?action=update", entity);
        }

        // ✅ DELETE
        public virtual async Task<bool> DeleteByIdAsync(TType id) {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            return await CApiManager.DeleteAsync(GetUrl(id) + "?action=delete");
        }

        // 🔹 SHARED VALIDATION
        private void ValidateModel(TModel model, bool validate) {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (validate && !model.DataValidation())
                throw new ArgumentException(model.ErrorMessage);

            if (!model.Success)
                throw new ArgumentException(model.Error);
        }
    }
}
