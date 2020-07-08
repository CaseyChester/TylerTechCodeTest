namespace PrsnlMgmt.Mvvm.Employees
{
    public class EmployeeDetail : BindableBase
    {
        private string _employeeId;
        private string _firstName;
        private string _lastName;
        private int _managerId;

        public string EmployeeId
        {
            get { return _employeeId; }
            set { SetProperty(ref _employeeId, value); }
        }

        public string FirstName
        {
            get { return _firstName; }
            set { SetProperty(ref _firstName, value); }
        }

        public string LastName
        {
            get { return _lastName; }
            set { SetProperty(ref _lastName, value); }
        }

        public int ManagerId
        {
            get { return _managerId; }
            set { SetProperty(ref _managerId, value); }
        }
    }
}