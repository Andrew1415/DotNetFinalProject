using System;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Radzen;

using DotNetGalutinis.Server.Data;

namespace DotNetGalutinis.Server
{
    public partial class InventoryService
    {
        InventoryContext Context
        {
           get
           {
             return this.context;
           }
        }

        private readonly InventoryContext context;
        private readonly NavigationManager navigationManager;

        public InventoryService(InventoryContext context, NavigationManager navigationManager)
        {
            this.context = context;
            this.navigationManager = navigationManager;
        }

        public void Reset() => Context.ChangeTracker.Entries().Where(e => e.Entity != null).ToList().ForEach(e => e.State = EntityState.Detached);

        public void ApplyQuery<T>(ref IQueryable<T> items, Query query = null)
        {
            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }
        }


        public async Task ExportUsersToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/inventory/users/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/inventory/users/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportUsersToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/inventory/users/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/inventory/users/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnUsersRead(ref IQueryable<DotNetGalutinis.Server.Models.Inventory.User> items);

        public async Task<IQueryable<DotNetGalutinis.Server.Models.Inventory.User>> GetUsers(Query query = null)
        {
            var items = Context.Users.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnUsersRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnUserGet(DotNetGalutinis.Server.Models.Inventory.User item);
        partial void OnGetUserById(ref IQueryable<DotNetGalutinis.Server.Models.Inventory.User> items);


        public async Task<DotNetGalutinis.Server.Models.Inventory.User> GetUserById(long id)
        {
            var items = Context.Users
                              .AsNoTracking()
                              .Where(i => i.ID == id);

 
            OnGetUserById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnUserGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnUserCreated(DotNetGalutinis.Server.Models.Inventory.User item);
        partial void OnAfterUserCreated(DotNetGalutinis.Server.Models.Inventory.User item);

        public async Task<DotNetGalutinis.Server.Models.Inventory.User> CreateUser(DotNetGalutinis.Server.Models.Inventory.User user)
        {
            OnUserCreated(user);

            var existingItem = Context.Users
                              .Where(i => i.ID == user.ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Users.Add(user);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(user).State = EntityState.Detached;
                throw;
            }

            OnAfterUserCreated(user);

            return user;
        }

        public async Task<DotNetGalutinis.Server.Models.Inventory.User> CancelUserChanges(DotNetGalutinis.Server.Models.Inventory.User item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnUserUpdated(DotNetGalutinis.Server.Models.Inventory.User item);
        partial void OnAfterUserUpdated(DotNetGalutinis.Server.Models.Inventory.User item);

        public async Task<DotNetGalutinis.Server.Models.Inventory.User> UpdateUser(long id, DotNetGalutinis.Server.Models.Inventory.User user)
        {
            OnUserUpdated(user);

            var itemToUpdate = Context.Users
                              .Where(i => i.ID == user.ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(user);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterUserUpdated(user);

            return user;
        }

        partial void OnUserDeleted(DotNetGalutinis.Server.Models.Inventory.User item);
        partial void OnAfterUserDeleted(DotNetGalutinis.Server.Models.Inventory.User item);

        public async Task<DotNetGalutinis.Server.Models.Inventory.User> DeleteUser(long id)
        {
            var itemToDelete = Context.Users
                              .Where(i => i.ID == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnUserDeleted(itemToDelete);


            Context.Users.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterUserDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportItemsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/inventory/items/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/inventory/items/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportItemsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/inventory/items/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/inventory/items/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnItemsRead(ref IQueryable<DotNetGalutinis.Server.Models.Inventory.Item> items);

        public async Task<IQueryable<DotNetGalutinis.Server.Models.Inventory.Item>> GetItems(Query query = null)
        {
            var items = Context.Items.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnItemsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnItemGet(DotNetGalutinis.Server.Models.Inventory.Item item);
        partial void OnGetItemById(ref IQueryable<DotNetGalutinis.Server.Models.Inventory.Item> items);


        public async Task<DotNetGalutinis.Server.Models.Inventory.Item> GetItemById(long id)
        {
            var items = Context.Items
                              .AsNoTracking()
                              .Where(i => i.ID == id);

 
            OnGetItemById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnItemGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnItemCreated(DotNetGalutinis.Server.Models.Inventory.Item item);
        partial void OnAfterItemCreated(DotNetGalutinis.Server.Models.Inventory.Item item);

        public async Task<DotNetGalutinis.Server.Models.Inventory.Item> CreateItem(DotNetGalutinis.Server.Models.Inventory.Item item)
        {
            OnItemCreated(item);

            var existingItem = Context.Items
                              .Where(i => i.ID == item.ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Items.Add(item);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(item).State = EntityState.Detached;
                throw;
            }

            OnAfterItemCreated(item);

            return item;
        }

        public async Task<DotNetGalutinis.Server.Models.Inventory.Item> CancelItemChanges(DotNetGalutinis.Server.Models.Inventory.Item item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnItemUpdated(DotNetGalutinis.Server.Models.Inventory.Item item);
        partial void OnAfterItemUpdated(DotNetGalutinis.Server.Models.Inventory.Item item);

        public async Task<DotNetGalutinis.Server.Models.Inventory.Item> UpdateItem(long id, DotNetGalutinis.Server.Models.Inventory.Item item)
        {
            OnItemUpdated(item);

            var itemToUpdate = Context.Items
                              .Where(i => i.ID == item.ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(item);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterItemUpdated(item);

            return item;
        }

        partial void OnItemDeleted(DotNetGalutinis.Server.Models.Inventory.Item item);
        partial void OnAfterItemDeleted(DotNetGalutinis.Server.Models.Inventory.Item item);

        public async Task<DotNetGalutinis.Server.Models.Inventory.Item> DeleteItem(long id)
        {
            var itemToDelete = Context.Items
                              .Where(i => i.ID == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnItemDeleted(itemToDelete);


            Context.Items.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterItemDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportReservationsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/inventory/reservations/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/inventory/reservations/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportReservationsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/inventory/reservations/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/inventory/reservations/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnReservationsRead(ref IQueryable<DotNetGalutinis.Server.Models.Inventory.Reservation> items);

        public async Task<IQueryable<DotNetGalutinis.Server.Models.Inventory.Reservation>> GetReservations(Query query = null)
        {
            var items = Context.Reservations.AsQueryable();

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnReservationsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnReservationGet(DotNetGalutinis.Server.Models.Inventory.Reservation item);
        partial void OnGetReservationById(ref IQueryable<DotNetGalutinis.Server.Models.Inventory.Reservation> items);


        public async Task<DotNetGalutinis.Server.Models.Inventory.Reservation> GetReservationById(long id)
        {
            var items = Context.Reservations
                              .AsNoTracking()
                              .Where(i => i.ID == id);

 
            OnGetReservationById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnReservationGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnReservationCreated(DotNetGalutinis.Server.Models.Inventory.Reservation item);
        partial void OnAfterReservationCreated(DotNetGalutinis.Server.Models.Inventory.Reservation item);

        public async Task<DotNetGalutinis.Server.Models.Inventory.Reservation> CreateReservation(DotNetGalutinis.Server.Models.Inventory.Reservation reservation)
        {
            OnReservationCreated(reservation);

            var existingItem = Context.Reservations
                              .Where(i => i.ID == reservation.ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Reservations.Add(reservation);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(reservation).State = EntityState.Detached;
                throw;
            }

            OnAfterReservationCreated(reservation);

            return reservation;
        }

        public async Task<DotNetGalutinis.Server.Models.Inventory.Reservation> CancelReservationChanges(DotNetGalutinis.Server.Models.Inventory.Reservation item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnReservationUpdated(DotNetGalutinis.Server.Models.Inventory.Reservation item);
        partial void OnAfterReservationUpdated(DotNetGalutinis.Server.Models.Inventory.Reservation item);

        public async Task<DotNetGalutinis.Server.Models.Inventory.Reservation> UpdateReservation(long id, DotNetGalutinis.Server.Models.Inventory.Reservation reservation)
        {
            OnReservationUpdated(reservation);

            var itemToUpdate = Context.Reservations
                              .Where(i => i.ID == reservation.ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(reservation);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterReservationUpdated(reservation);

            return reservation;
        }

        partial void OnReservationDeleted(DotNetGalutinis.Server.Models.Inventory.Reservation item);
        partial void OnAfterReservationDeleted(DotNetGalutinis.Server.Models.Inventory.Reservation item);

        public async Task<DotNetGalutinis.Server.Models.Inventory.Reservation> DeleteReservation(long id)
        {
            var itemToDelete = Context.Reservations
                              .Where(i => i.ID == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnReservationDeleted(itemToDelete);


            Context.Reservations.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterReservationDeleted(itemToDelete);

            return itemToDelete;
        }
        }
}