using PrsnlMgmt.Data;
using PrsnlMgmt.Mvvm.Model;
using PrsnlMgmt.Services.DataService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PrsnlMgmt.Mvvm.Employees
{
    public class EmployeeDetailViewModel : ValidatableBindableBase
    {
        private static readonly LookupItem UnmanagedItem = new LookupItem
        {
            DisplayMember = "[Unmanaged]",
            Id = -1
        };

        private string _employeeId;
        private string _firstName;
        private string _lastName;
        private ObservableCollection<LookupItem> _managers;
        private PrsnlMgmtDataService _pmdsvc = new PrsnlMgmtDataService(() => new PrsnlMgmtDbContext());
        private ObservableCollection<SelectableItem<string>> _roles;
        private LookupItem _selectedManager;

        public EmployeeDetailViewModel()
        {
            InitCommands();
        }

        #region Bindable Data Properties

        [Required]
        public string EmployeeId
        {
            get => _employeeId;
            set
            {
                if (SetProperty(ref _employeeId, value))
                    ValidateEmployeeIdUniqueness(value);
            }
        }

        [Required]
        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        [Required]
        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        public ObservableCollection<LookupItem> Managers
        {
            get => _managers;
            set => SetProperty(ref _managers, value);
        }

        public ObservableCollection<SelectableItem<string>> Roles
        {
            get => _roles;
            set => SetProperty(ref _roles, value);
        }

        [Required]
        public LookupItem SelectedManager
        {
            get => _selectedManager;
            set => SetProperty(ref _selectedManager, value);
        }

        private void ValidateEmployeeIdUniqueness(string empId)
        {
            if (!string.IsNullOrEmpty(empId))
            {
                try
                {
                    Employee existing = _pmdsvc.GetEmployeeByEmployeeId(empId);
                    if (existing != null)
                        SetErrors(nameof(EmployeeId), $"The Employee ID '{empId}' is already assigned to {existing.FullName}.");
                }
                catch (Exception ex)
                {
                    RaiseError(ex, "An error occurred while attempting to validate the uniqueness of the supplied Employee ID for a new employee.");
                }
            }
        }
        #endregion Bindable Data Properties

        #region Commands

        public event Action Done;

        public RelayCommand CancelCommand { get; private set; }

        public RelayCommand SaveCommand { get; private set; }

        // a helper method to extract the EmployeeRoles that
        // have been selected in the UI
        private IEnumerable<EmployeeRole> SelectedRoles => Roles
                .Where(item => item.IsSelected)
                .Select(item => item.SourceObject)
                .Cast<EmployeeRole>();

        private bool CanSave() => !HasErrors;

        private void InitCommands()
        {
            SaveCommand = new RelayCommand(OnSave, CanSave);
            CancelCommand = new RelayCommand(OnCancel);
        }
        private void OnCancel()
        {
            Done?.Invoke();
        }

        private async void OnSave()
        {
            try
            {
                var newEmp = new Employee
                {
                    Manager = (Employee)SelectedManager.Tag,
                    EmployeeId = this.EmployeeId,
                    FirstName = this.FirstName,
                    LastName = this.LastName,
                    Roles = SelectedRoles.ToList()
                };

                await _pmdsvc.InsertEmployeeAsync(newEmp);
                Done?.Invoke();
            }
            catch (Exception ex)
            {
                RaiseError(ex, "An error occurred while saving new employee.");
            }
        }
        #endregion Commands

        #region Inter-ViewModel Messaging

        public void OnLoad()
        {
            try
            {
                LoadManagers();
                LoadRoles();

                // note: easiest way to have validation work with empty data
                // entry controls is just to have them initially validated. we
                // end up with those data entry fields 'red boxed' initially
                // which isn't astehetic, but functional
                ValidateAnnotatedProperties();
            }
            catch (Exception ex)
            {
                RaiseError(ex, "An error occurred while loading the Employee Detail View.");
            }
        }

        protected override void OnErrorsChanged(DataErrorsChangedEventArgs args)
        {
            base.OnErrorsChanged(args);
            // when the ErrorsChanged event is raised ask SaveCommand
            // to requery validation
            SaveCommand.RaiseCanExecuteChanged();
        }

        private async void LoadManagers()
        {
            // for a new employee, any existing employee could be their manager
            // so start task to fetch all employees
            var fetchManagersTask = _pmdsvc.GetAllEmployeesAsync();

            // meanwhile, create new collection and add the 'Unmanaged' entry
            // (note: an employee might not have a manager)
            var oc = new ObservableCollection<LookupItem>();
            oc.Add(UnmanagedItem);

            // when the db call completes, build a lookup item for each and
            // add to collection
            foreach (var emp in (await fetchManagersTask).OrderBy(emp => emp.FullName))
                oc.Add(new LookupItem
                {
                    DisplayMember = emp.FullName,
                    Id = emp.Id,
                    Tag = emp
                });

            // assign the new managers collection
            Managers = oc;
        }

        private async void LoadRoles()
        {
            var roles = await _pmdsvc.GetAllEmployeeRolesAsync();
            Roles = new ObservableCollection<SelectableItem<string>>(roles.ToSelectableItems(r => r.Name));
        }

        #endregion Inter-ViewModel Messaging
    }
}