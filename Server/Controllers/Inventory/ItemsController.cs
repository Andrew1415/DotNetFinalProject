using System;
using System.Net;
using System.Data;
using System.Linq;
using System.Collections.Generic;
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
    [Route("odata/Inventory/Items")]
    public partial class ItemsController : ODataController
    {
        private DotNetGalutinis.Server.Data.InventoryContext context;

        public ItemsController(DotNetGalutinis.Server.Data.InventoryContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<DotNetGalutinis.Server.Models.Inventory.Item> GetItems()
        {
            var items = this.context.Items.AsQueryable<DotNetGalutinis.Server.Models.Inventory.Item>();
            this.OnItemsRead(ref items);

            return items;
        }

        partial void OnItemsRead(ref IQueryable<DotNetGalutinis.Server.Models.Inventory.Item> items);

        partial void OnItemGet(ref SingleResult<DotNetGalutinis.Server.Models.Inventory.Item> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/Inventory/Items(ID={ID})")]
        public SingleResult<DotNetGalutinis.Server.Models.Inventory.Item> GetItem(long key)
        {
            var items = this.context.Items.Where(i => i.ID == key);
            var result = SingleResult.Create(items);

            OnItemGet(ref result);

            return result;
        }
        partial void OnItemDeleted(DotNetGalutinis.Server.Models.Inventory.Item item);
        partial void OnAfterItemDeleted(DotNetGalutinis.Server.Models.Inventory.Item item);

        [HttpDelete("/odata/Inventory/Items(ID={ID})")]
        public IActionResult DeleteItem(long key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var item = this.context.Items
                    .Where(i => i.ID == key)
                    .FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                this.OnItemDeleted(item);
                this.context.Items.Remove(item);
                this.context.SaveChanges();
                this.OnAfterItemDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnItemUpdated(DotNetGalutinis.Server.Models.Inventory.Item item);
        partial void OnAfterItemUpdated(DotNetGalutinis.Server.Models.Inventory.Item item);

        [HttpPut("/odata/Inventory/Items(ID={ID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutItem(long key, [FromBody]DotNetGalutinis.Server.Models.Inventory.Item item)
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
                this.OnItemUpdated(item);
                this.context.Items.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Items.Where(i => i.ID == key);
                ;
                this.OnAfterItemUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/Inventory/Items(ID={ID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchItem(long key, [FromBody]Delta<DotNetGalutinis.Server.Models.Inventory.Item> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var item = this.context.Items.Where(i => i.ID == key).FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                patch.Patch(item);

                this.OnItemUpdated(item);
                this.context.Items.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Items.Where(i => i.ID == key);
                ;
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnItemCreated(DotNetGalutinis.Server.Models.Inventory.Item item);
        partial void OnAfterItemCreated(DotNetGalutinis.Server.Models.Inventory.Item item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] DotNetGalutinis.Server.Models.Inventory.Item item)
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

                this.OnItemCreated(item);
                this.context.Items.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Items.Where(i => i.ID == item.ID);

                ;

                this.OnAfterItemCreated(item);

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
