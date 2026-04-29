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
    public abstract class BaseApiRepository<TModel, TEntity, TType> : IDisposable where TModel : BaseModel where TEntity : BaseEntity
    {
        protected readonly HttpClient _httpClient;
        public string Endpoint { get; set; }
        protected BaseApiRepository()
        {
            Endpoint = Endpoint?.Trim('/') ?? throw new ArgumentNullException(nameof(Endpoint));
            _httpClient = new HttpClient { BaseAddress = new Uri(CAppConstants.API_BASE_URL.TrimEnd('/') + "/") };
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        protected string GetUrl(params object[] segments)
        {
            var url = new StringBuilder(Endpoint);
            foreach (var seg in segments)
            {
                url.Append($"/{seg}");
            }
            return url.ToString();
        }

        // GET: List
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync(GetUrl());
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<TEntity>>(json);
        }

        // GET: By Id
        public virtual async Task<TEntity> GetByIdAsync(TType id)
        {
            var response = await _httpClient.GetAsync(GetUrl(id));
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TEntity>(json);
        }

        // GET: With search/query params
        public virtual async Task<TEntity> GetByPredicateAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var response = await _httpClient.GetAsync(GetUrl() + "?" + predicate.ToQuery());
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TEntity>(json);
        }

        // POST: Create
        public virtual async Task<bool> SaveDTOAsync(TModel model, bool enabledValidation = false)
        {
            if (model == null)
                throw new ArgumentNullException($"{nameof(model)} is null!");

            if (enabledValidation && !model.DataValidation())
            {
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

            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(GetUrl(), content);
            response.EnsureSuccessStatusCode();
            return true;
        }

        // PUT: Update
        public virtual async Task<bool> UpdateDTOAsync(TType id, TModel model, bool enabledValidation = false)
        {
            if (model == null)
                throw new ArgumentNullException($"{nameof(model)} is null!");

            if (enabledValidation && !model.DataValidation())
            {
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

            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(GetUrl(id), content);
            response.EnsureSuccessStatusCode();
            return true;
        }

        // DELETE
        public virtual async Task<bool> DeleteByIdAsync(TType id)
        {
            if (id == null)
                throw new ArgumentNullException($"{nameof(id)} is null!");
            var response = await _httpClient.DeleteAsync(GetUrl(id));
            response.EnsureSuccessStatusCode();
            return true;
        }
    }
}
