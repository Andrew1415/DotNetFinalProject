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
    [Route("odata/Inventory/Users")]
    public partial class UsersController : ODataController
    {
        private DotNetGalutinis.Server.Data.InventoryContext context;

        public UsersController(DotNetGalutinis.Server.Data.InventoryContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<DotNetGalutinis.Server.Models.Inventory.User> GetUsers()
        {
            var items = this.context.Users.AsQueryable<DotNetGalutinis.Server.Models.Inventory.User>();
            this.OnUsersRead(ref items);

            return items;
        }

        partial void OnUsersRead(ref IQueryable<DotNetGalutinis.Server.Models.Inventory.User> items);

        partial void OnUserGet(ref SingleResult<DotNetGalutinis.Server.Models.Inventory.User> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/Inventory/Users(ID={ID})")]
        public SingleResult<DotNetGalutinis.Server.Models.Inventory.User> GetUser(long key)
        {
            var items = this.context.Users.Where(i => i.ID == key);
            var result = SingleResult.Create(items);

            OnUserGet(ref result);

            return result;
        }
        partial void OnUserDeleted(DotNetGalutinis.Server.Models.Inventory.User item);
        partial void OnAfterUserDeleted(DotNetGalutinis.Server.Models.Inventory.User item);

        [HttpDelete("/odata/Inventory/Users(ID={ID})")]
        public IActionResult DeleteUser(long key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var item = this.context.Users
                    .Where(i => i.ID == key)
                    .FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                this.OnUserDeleted(item);
                this.context.Users.Remove(item);
                this.context.SaveChanges();
                this.OnAfterUserDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnUserUpdated(DotNetGalutinis.Server.Models.Inventory.User item);
        partial void OnAfterUserUpdated(DotNetGalutinis.Server.Models.Inventory.User item);

        [HttpPut("/odata/Inventory/Users(ID={ID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutUser(long key, [FromBody]DotNetGalutinis.Server.Models.Inventory.User item)
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
                this.OnUserUpdated(item);
                this.context.Users.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Users.Where(i => i.ID == key);
                ;
                this.OnAfterUserUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/Inventory/Users(ID={ID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchUser(long key, [FromBody]Delta<DotNetGalutinis.Server.Models.Inventory.User> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var item = this.context.Users.Where(i => i.ID == key).FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                patch.Patch(item);

                this.OnUserUpdated(item);
                this.context.Users.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Users.Where(i => i.ID == key);
                ;
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnUserCreated(DotNetGalutinis.Server.Models.Inventory.User item);
        partial void OnAfterUserCreated(DotNetGalutinis.Server.Models.Inventory.User item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] DotNetGalutinis.Server.Models.Inventory.User item)
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

                this.OnUserCreated(item);
                this.context.Users.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Users.Where(i => i.ID == item.ID);

                ;

                this.OnAfterUserCreated(item);

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
