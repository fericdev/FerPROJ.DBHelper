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

namespace FerPROJ.DBHelper.DBCrud
{
    public abstract class BaseApiRepository<TModel, TEntity, TType>
        where TModel : BaseModel
        where TEntity : BaseEntity {
        public string Endpoint { get; set; }

        protected string GetUrl(params object[] segments) {
            var sb = new StringBuilder(Endpoint);

            foreach (var seg in segments)
                sb.Append($"/{seg}");

            return sb.ToString();
        }

        // ✅ GET ALL
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync() {
            return await CApiManager.GetAsync<List<TEntity>>(GetUrl());
        }

        // ✅ GET BY ID
        public virtual async Task<TEntity> GetByIdAsync(TType id) {
            return await CApiManager.GetAsync<TEntity>(GetUrl(id));
        }

        // ✅ SEARCH
        public virtual async Task<TEntity> GetByPredicateAsync(Expression<Func<TEntity, bool>> predicate) {
            var url = GetUrl() + "?action=get&" + predicate.ToQuery();
            return await CApiManager.GetAsync<TEntity>(url);
        }

        // ✅ CREATE
        public virtual async Task<bool> SaveDTOAsync(TModel model, bool validate = false) {
            ValidateModel(model, validate);

            await CApiManager.PostAsync<TModel, object>(
                GetUrl() + "?action=save", model);

            return true;
        }

        // ✅ UPDATE
        public virtual async Task<bool> UpdateDTOAsync(TType id, TModel model, bool validate = false) {
            ValidateModel(model, validate);

            return await CApiManager.PutAsync(GetUrl(id), model);
        }

        // ✅ DELETE
        public virtual async Task<bool> DeleteByIdAsync(TType id) {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            return await CApiManager.DeleteAsync(GetUrl(id));
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
