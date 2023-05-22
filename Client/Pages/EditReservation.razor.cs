using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace DotNetGalutinis.Client.Pages
{
    public partial class EditReservation
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }
        [Inject]
        public InventoryService InventoryService { get; set; }

        [Parameter]
        public long ID { get; set; }

        protected override async Task OnInitializedAsync()
        {
            reservation = await InventoryService.GetReservationById(id:ID);
        }
        protected bool errorVisible;
        protected DotNetGalutinis.Server.Models.Inventory.Reservation reservation;

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task FormSubmit()
        {
            try
            {
                await InventoryService.UpdateReservation(id:ID, reservation);
                DialogService.Close(reservation);
            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}