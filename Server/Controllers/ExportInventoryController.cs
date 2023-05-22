using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

using DotNetGalutinis.Server.Data;

namespace DotNetGalutinis.Server.Controllers
{
    public partial class ExportInventoryController : ExportController
    {
        private readonly InventoryContext context;
        private readonly InventoryService service;

        public ExportInventoryController(InventoryContext context, InventoryService service)
        {
            this.service = service;
            this.context = context;
        }

        [HttpGet("/export/Inventory/users/csv")]
        [HttpGet("/export/Inventory/users/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportUsersToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetUsers(), Request.Query), fileName);
        }

        [HttpGet("/export/Inventory/users/excel")]
        [HttpGet("/export/Inventory/users/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportUsersToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetUsers(), Request.Query), fileName);
        }

        [HttpGet("/export/Inventory/items/csv")]
        [HttpGet("/export/Inventory/items/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportItemsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetItems(), Request.Query), fileName);
        }

        [HttpGet("/export/Inventory/items/excel")]
        [HttpGet("/export/Inventory/items/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportItemsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetItems(), Request.Query), fileName);
        }

        [HttpGet("/export/Inventory/reservations/csv")]
        [HttpGet("/export/Inventory/reservations/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportReservationsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetReservations(), Request.Query), fileName);
        }

        [HttpGet("/export/Inventory/reservations/excel")]
        [HttpGet("/export/Inventory/reservations/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportReservationsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetReservations(), Request.Query), fileName);
        }
    }
}
