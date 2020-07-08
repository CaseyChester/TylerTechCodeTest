using PrsnlMgmt.Data;
using PrsnlMgmt.Mvvm.Model;
using PrsnlMgmt.Services.DataService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PrsnlMgmt.Mvvm.Employees
{
    public class EmployeeListViewModel : BindableBase
    {
        private static readonly LookupItem UnmanagedItem = new LookupItem
        {
            DisplayMember = "[Unmanaged]",
            Id = -1
        };

        private ObservableCollection<Employee> _employees;
        private ObservableCollection<LookupItem> _managers;
        private PrsnlMgmtDataService _pmdsvc = new PrsnlMgmtDataService(() => new PrsnlMgmtDbContext());
        private LookupItem _selectedManager;

        public EmployeeListViewModel()
        {
            AddEmployeeCommand = new RelayCommand(() => AddEmployeeRequested?.Invoke());
        }

        #region Bindable data properties

        public ObservableCollection<Employee> Employees
        {
            get => _employees;
            set => SetProperty(ref _employees, value);
        }

        public ObservableCollection<LookupItem> Managers
        {
            get => _managers;
            set => SetProperty(ref _managers, value);
        }

        public LookupItem SelectedManager
        {
            get => _selectedManager;
            set
            {
                // when the SelectedManager changes, let's load employees
                if (SetProperty(ref _selectedManager, value))
                    LoadEmployees();
            }
        }

        #endregion

        #region Commands

        public event Action AddEmployeeRequested;

        public RelayCommand AddEmployeeCommand { get; private set; }

        #endregion

        #region Inter-ViewModel Messaging

        public async void LoadManagers()
        {
            try
            {
                // if there was a previous selected manager, store that id
                int? prevSelMgrId = null;
                if (SelectedManager != null)
                    prevSelMgrId = SelectedManager.Id;

                // start task to fetch all managers
                var fetchManagersTask = _pmdsvc.GetAllManagersAsync();

                // meanwhile, build a new collection and add the 'unmanaged' entry
                var oc = new ObservableCollection<LookupItem>();
                oc.Add(UnmanagedItem);

                // build a lookup entry for each returned manager and add it to the new coll
                foreach (var mgr in await fetchManagersTask)
                {
                    var mgrItem = new LookupItem { DisplayMember = mgr.FullName, Id = mgr.Id };
                    oc.Add(mgrItem);
                }

                // assign the new collection and selected manager if applicable
                Managers = oc;
                SelectedManager = oc.SingleOrDefault(i => i.Id == prevSelMgrId);
            }
            catch (Exception ex)
            {
                RaiseError(ex, "An error occurred while loading managers.");
            }
        }

        private async void LoadEmployees()
        {
            try
            {
                List<Employee> emps = new List<Employee>();
                if (SelectedManager != null)
                {
                    if (SelectedManager == UnmanagedItem)
                    {
                        // load unmanaged employees
                        emps.AddRange(await _pmdsvc.GetUnmanagedEmployeesAsync());
                    }
                    else
                    {
                        // load employees managed by SelectedManager
                        emps.AddRange(await _pmdsvc.GetEmployeesByManagerIdAsync(SelectedManager.Id));
                    }
                }
                Employees = new ObservableCollection<Employee>(emps);
            }
            catch (Exception ex)
            {
                RaiseError(ex, "An error occurred while loading the employee list.");
            }
        }

        #endregion
    }
}