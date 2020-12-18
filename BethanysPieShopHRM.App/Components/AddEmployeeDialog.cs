using BethanysPieShopHRM.App.Services;
using BethanysPieShopHRM.Shared;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BethanysPieShopHRM.App.Components
{
    public partial class AddEmployeeDialog
    {
        public Employee Employee { get; set; } = new Employee { CountryId = 1, JobCategoryId = 1, BirthDate = DateTime.Now, JoinedDate = DateTime.Now };

        [Inject]
        public IEmployeeDataService EmployeeDataService { get; set; }

        public bool ShowDialog { get; set; }

        /// <summary>
        /// This EventCallback makes it possible to communicate with the parent component (EmployeeOverview), and raise an event when something happens,
        /// like closing a dialog. The Parameter attribute makes it possible to subscribe to the event in the parent component.
        /// </summary>
        [Parameter]
        public EventCallback<bool> CloseEventCallback { get; set; }

        /// <summary>
        /// This will be called from the EmployeeOverview UI via the reference property in EmployeeOverview.cs
        /// </summary>
        public void Show()
        {
            ResetDialog();
            ShowDialog = true;
            StateHasChanged();
        }

        public void Close()
        {
            ShowDialog = false;
            StateHasChanged();
        }

        private void ResetDialog()
        {
            Employee = new Employee { CountryId = 1, JobCategoryId = 1, BirthDate = DateTime.Now, JoinedDate = DateTime.Now };

        }

        protected async Task HandleValidSubmit()
        {
            await EmployeeDataService.AddEmployee(Employee);

            ShowDialog = false;

            //Raise the event so that it can be subscribed to in the EmployeeOverview UI
            await CloseEventCallback.InvokeAsync(true);
            StateHasChanged();
        }
    }
}
