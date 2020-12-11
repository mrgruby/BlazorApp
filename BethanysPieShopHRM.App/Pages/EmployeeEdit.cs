using BethanysPieShopHRM.App.Services;
using BethanysPieShopHRM.Shared;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BethanysPieShopHRM.App.Pages
{
    public partial class EmployeeEdit
    {
        [Parameter]//This will make Blazor look for the parameter in the querystring
        public string EmployeeId { get; set; }

        public Employee Employee { get; set; } = new Employee();

        [Inject]
        public IEmployeeDataService EmployeeDataService { get; set; }

        [Inject]
        public ICountryDataService CountryDataService { get; set; }

        [Inject]
        public IJobCategoryDataService CategoryDataService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        public List<Country> Countries { get; set; } = new List<Country>();

        public List<JobCategory> JobCategories { get; set; } = new List<JobCategory>();

        //Field to bind the selected value i SelectList
        protected string CountryId = string.Empty;
        protected string JobCategoryId = string.Empty;

        //Used in the view to store state of screen
        protected string StatusClass;
        protected string Message;
        protected bool Saved;

        /// <summary>
        /// Load an Employee based on id, as as source for the component UI
        /// </summary>
        /// <returns></returns>
        protected async override Task OnInitializedAsync()
        {
            //Employee = (await EmployeeDataService.GetEmployeeDetails(int.Parse(EmployeeId)));
            Countries = (await CountryDataService.GetAllCountries()).ToList();
            JobCategories = (await CategoryDataService.GetAllJobCategories()).ToList();

            int.TryParse(EmployeeId, out var employeeId);

            if (employeeId == 0)//New employee is created.
            {
                //Fill in some defaults for the new employee
                Employee = new Employee {CountryId = 1, JobCategoryId = 1, BirthDate = DateTime.Now, JoinedDate = DateTime.Now};
            }
            else
            {
                Employee = (await EmployeeDataService.GetEmployeeDetails(int.Parse(EmployeeId)));
            }
            //To make two way binding work on the Countries SelectList, in order to show the Employees country as the selecte value, 
            //we need to convert the Employee.CountryId to a string, and put it in a helper field (CountryId), that can then be used by the SelectList.
            //Basically, when binding to a ddl etc. using an Id from a model, it needs to be converted to string. 
            CountryId = Employee.CountryId.ToString();
            JobCategoryId = Employee.JobCategoryId.ToString();
        }

        /// <summary>
        /// If the submit succeeds, call EmployeeDataService to add the new employee. Then, set appropriate values in the fields on the view.
        /// </summary>
        protected async Task HandleValidSubmit()
        {
            //This will be used to show or hide the form in the UI, depending on the state of the transaction.
            //True = hide the form and only show message. False = show the form and also the message if there is one.
            Saved = false;
            //These are the selected values from the select lists. They will be parsed to int and added to the Employee.
            Employee.CountryId = int.Parse(CountryId);
            Employee.JobCategoryId = int.Parse(JobCategoryId);

            if (Employee.EmployeeId == 0) //new
            {
                var addedEmployee = await EmployeeDataService.AddEmployee(Employee);
                if (addedEmployee != null)
                {
                    StatusClass = "alert-success";
                    Message = "New employee added successfully.";
                    Saved = true;
                }
                else
                {
                    StatusClass = "alert-danger";
                    Message = "Something went wrong adding the new employee. Please try again.";
                    Saved = false;
                }
            }
            else
            {
                await EmployeeDataService.UpdateEmployee(Employee);
                StatusClass = "alert-success";
                Message = "Employee updated successfully.";
                Saved = true;
            }
        }

        /// <summary>
        /// If the submit fails, set appropriate values in the fields on the view.
        /// </summary>
        protected void HandleInvalidSubmit()
        {
            StatusClass = "alert-danger";
            Message = "There are some validation errors. Please try again.";
        }

        public async Task DeleteEmployee()
        {
            await EmployeeDataService.DeleteEmployee(Employee.EmployeeId);

            StatusClass = "alert-success";
            Message = "Employee deleted successfully.";
            Saved = true;
        }

        protected void NavigateToOverview()
        {
            NavigationManager.NavigateTo("/employeeoverview");
        }
    }
}
