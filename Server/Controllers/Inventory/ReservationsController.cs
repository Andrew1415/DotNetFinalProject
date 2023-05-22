using System;
using System.Net;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using DotNetGalutinis.Server.Models.Inventory;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DotNetGalutinis.Server.Controllers.Inventory
{
    [Route("odata/Inventory/Reservations")]
    public partial class ReservationsController : ODataController
    {
        private DotNetGalutinis.Server.Data.InventoryContext context;

        public ReservationsController(DotNetGalutinis.Server.Data.InventoryContext context)
        {
            this.context = context;
        }

        
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<DotNetGalutinis.Server.Models.Inventory.Reservation> GetReservations()
        {
            var items = context.Reservations
                .Join(context.Items, reservation => reservation.Item_ID, item => item.ID, (reservation, item) => new { Reservation = reservation, Item = item })
                .Join(context.Users, temp => temp.Reservation.User_ID, user => user.ID, (temp, user) => new { temp.Reservation, temp.Item, User = user })
                .Select(result => new DotNetGalutinis.Server.Models.Inventory.Reservation
                {
                    ID = result.Reservation.ID,
                    Item_ID = result.Reservation.Item_ID,
                    User_ID = result.Reservation.User_ID,
                    From = result.Reservation.From,
                    To = result.Reservation.To,
                    // Include other Reservation properties as needed
                    ItemInfo = result.Item.Name,
                    UserInfo = /*result.User.Name*/ $"{result.User.Name} {result.User.Surname}"
                })
                .AsQueryable();

            return items;
        } 

        partial void OnReservationsRead(ref IQueryable<DotNetGalutinis.Server.Models.Inventory.Reservation> items);

        partial void OnReservationGet(ref SingleResult<DotNetGalutinis.Server.Models.Inventory.Reservation> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/Inventory/Reservations(ID={ID})")]
        public SingleResult<DotNetGalutinis.Server.Models.Inventory.Reservation> GetReservation(long key)
        {
            var items = this.context.Reservations.Where(i => i.ID == key);
            var result = SingleResult.Create(items);

            OnReservationGet(ref result);

            return result;
        }
        partial void OnReservationDeleted(DotNetGalutinis.Server.Models.Inventory.Reservation item);
        partial void OnAfterReservationDeleted(DotNetGalutinis.Server.Models.Inventory.Reservation item);

        [HttpDelete("/odata/Inventory/Reservations(ID={ID})")]
        public IActionResult DeleteReservation(long key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var item = this.context.Reservations
                    .Where(i => i.ID == key)
                    .FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                this.OnReservationDeleted(item);
                this.context.Reservations.Remove(item);
                this.context.SaveChanges();
                this.OnAfterReservationDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnReservationUpdated(DotNetGalutinis.Server.Models.Inventory.Reservation item);
        partial void OnAfterReservationUpdated(DotNetGalutinis.Server.Models.Inventory.Reservation item);

        [HttpPut("/odata/Inventory/Reservations(ID={ID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutReservation(long key, [FromBody]DotNetGalutinis.Server.Models.Inventory.Reservation item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (item == null || (item.ID != key))
                {
                    return BadRequest();
                }
                this.OnReservationUpdated(item);
                this.context.Reservations.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Reservations.Where(i => i.ID == key);
                ;
                this.OnAfterReservationUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/Inventory/Reservations(ID={ID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchReservation(long key, [FromBody]Delta<DotNetGalutinis.Server.Models.Inventory.Reservation> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var item = this.context.Reservations.Where(i => i.ID == key).FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                patch.Patch(item);

                this.OnReservationUpdated(item);
                this.context.Reservations.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Reservations.Where(i => i.ID == key);
                ;
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnReservationCreated(DotNetGalutinis.Server.Models.Inventory.Reservation item);
        partial void OnAfterReservationCreated(DotNetGalutinis.Server.Models.Inventory.Reservation item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] DotNetGalutinis.Server.Models.Inventory.Reservation item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (item == null)
                {
                    return BadRequest();
                }

                this.OnReservationCreated(item);
                this.context.Reservations.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Reservations.Where(i => i.ID == item.ID);

                ;

                this.OnAfterReservationCreated(item);

                return new ObjectResult(SingleResult.Create(itemToReturn))
                {
                    StatusCode = 201
                };
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }
    }
}
