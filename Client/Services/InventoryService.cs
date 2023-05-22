
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Web;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using Radzen;

namespace DotNetGalutinis.Client
{
    public partial class InventoryService
    {
        private readonly HttpClient httpClient;
        private readonly Uri baseUri;
        private readonly NavigationManager navigationManager;

        public InventoryService(NavigationManager navigationManager, HttpClient httpClient, IConfiguration configuration)
        {
            this.httpClient = httpClient;

            this.navigationManager = navigationManager;
            this.baseUri = new Uri($"{navigationManager.BaseUri}odata/Inventory/");
        }


        public async System.Threading.Tasks.Task ExportUsersToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/inventory/users/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/inventory/users/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportUsersToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/inventory/users/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/inventory/users/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetUsers(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<DotNetGalutinis.Server.Models.Inventory.User>> GetUsers(Query query)
        {
            return await GetUsers(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<DotNetGalutinis.Server.Models.Inventory.User>> GetUsers(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"Users");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetUsers(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<DotNetGalutinis.Server.Models.Inventory.User>>(response);
        }

        partial void OnCreateUser(HttpRequestMessage requestMessage);

        public async Task<DotNetGalutinis.Server.Models.Inventory.User> CreateUser(DotNetGalutinis.Server.Models.Inventory.User user = default(DotNetGalutinis.Server.Models.Inventory.User))
        {
            var uri = new Uri(baseUri, $"Users");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(user), Encoding.UTF8, "application/json");

            OnCreateUser(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<DotNetGalutinis.Server.Models.Inventory.User>(response);
        }

        partial void OnDeleteUser(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteUser(long id = default(long))
        {
            var uri = new Uri(baseUri, $"Users({id})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteUser(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetUserById(HttpRequestMessage requestMessage);

        public async Task<DotNetGalutinis.Server.Models.Inventory.User> GetUserById(string expand = default(string), long id = default(long))
        {
            var uri = new Uri(baseUri, $"Users({id})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetUserById(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<DotNetGalutinis.Server.Models.Inventory.User>(response);
        }

        partial void OnUpdateUser(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdateUser(long id = default(long), DotNetGalutinis.Server.Models.Inventory.User user = default(DotNetGalutinis.Server.Models.Inventory.User))
        {
            var uri = new Uri(baseUri, $"Users({id})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);


            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(user), Encoding.UTF8, "application/json");

            OnUpdateUser(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        public async System.Threading.Tasks.Task ExportItemsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/inventory/items/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/inventory/items/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportItemsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/inventory/items/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/inventory/items/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetItems(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<DotNetGalutinis.Server.Models.Inventory.Item>> GetItems(Query query)
        {
            return await GetItems(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<DotNetGalutinis.Server.Models.Inventory.Item>> GetItems(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"Items");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetItems(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<DotNetGalutinis.Server.Models.Inventory.Item>>(response);
        }

        partial void OnCreateItem(HttpRequestMessage requestMessage);

        public async Task<DotNetGalutinis.Server.Models.Inventory.Item> CreateItem(DotNetGalutinis.Server.Models.Inventory.Item item = default(DotNetGalutinis.Server.Models.Inventory.Item))
        {
            var uri = new Uri(baseUri, $"Items");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(item), Encoding.UTF8, "application/json");

            OnCreateItem(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<DotNetGalutinis.Server.Models.Inventory.Item>(response);
        }

        partial void OnDeleteItem(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteItem(long id = default(long))
        {
            var uri = new Uri(baseUri, $"Items({id})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteItem(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetItemById(HttpRequestMessage requestMessage);

        public async Task<DotNetGalutinis.Server.Models.Inventory.Item> GetItemById(string expand = default(string), long id = default(long))
        {
            var uri = new Uri(baseUri, $"Items({id})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetItemById(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<DotNetGalutinis.Server.Models.Inventory.Item>(response);
        }

        partial void OnUpdateItem(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdateItem(long id = default(long), DotNetGalutinis.Server.Models.Inventory.Item item = default(DotNetGalutinis.Server.Models.Inventory.Item))
        {
            var uri = new Uri(baseUri, $"Items({id})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);


            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(item), Encoding.UTF8, "application/json");

            OnUpdateItem(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        public async System.Threading.Tasks.Task ExportReservationsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/inventory/reservations/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/inventory/reservations/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportReservationsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/inventory/reservations/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/inventory/reservations/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetReservations(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<DotNetGalutinis.Server.Models.Inventory.Reservation>> GetReservations(Query query)
        {
            return await GetReservations(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<DotNetGalutinis.Server.Models.Inventory.Reservation>> GetReservations(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"Reservations");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetReservations(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<DotNetGalutinis.Server.Models.Inventory.Reservation>>(response);
        }

        partial void OnCreateReservation(HttpRequestMessage requestMessage);

        public async Task<DotNetGalutinis.Server.Models.Inventory.Reservation> CreateReservation(DotNetGalutinis.Server.Models.Inventory.Reservation reservation = default(DotNetGalutinis.Server.Models.Inventory.Reservation))
        {
            var uri = new Uri(baseUri, $"Reservations");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(reservation), Encoding.UTF8, "application/json");

            OnCreateReservation(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<DotNetGalutinis.Server.Models.Inventory.Reservation>(response);
        }

        partial void OnDeleteReservation(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteReservation(long id = default(long))
        {
            var uri = new Uri(baseUri, $"Reservations({id})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteReservation(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetReservationById(HttpRequestMessage requestMessage);

        public async Task<DotNetGalutinis.Server.Models.Inventory.Reservation> GetReservationById(string expand = default(string), long id = default(long))
        {
            var uri = new Uri(baseUri, $"Reservations({id})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetReservationById(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<DotNetGalutinis.Server.Models.Inventory.Reservation>(response);
        }

        partial void OnUpdateReservation(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdateReservation(long id = default(long), DotNetGalutinis.Server.Models.Inventory.Reservation reservation = default(DotNetGalutinis.Server.Models.Inventory.Reservation))
        {
            var uri = new Uri(baseUri, $"Reservations({id})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);


            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(reservation), Encoding.UTF8, "application/json");

            OnUpdateReservation(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }
    }
}