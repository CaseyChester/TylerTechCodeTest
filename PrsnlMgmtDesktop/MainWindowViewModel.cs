using PrsnlMgmt.Mvvm;
using PrsnlMgmt.Mvvm.Employees;
using PrsnlMgmt.Mvvm.Shared;
using System;

namespace PrsnlMgmtDesktop
{
    public class MainWindowViewModel : BindableBase
    {
        private BindableBase _errorSourceViewModel;
        private BindableBase _currentViewModel;
        private ErrorViewModel _errorViewModel;
        private EmployeeDetailViewModel _empDetailViewModel;
        private EmployeeListViewModel _empListViewModel;

        public MainWindowViewModel()
        {
            _empListViewModel = new EmployeeListViewModel();
            _empListViewModel.Error += NavigateToErrorView;
            _empListViewModel.AddEmployeeRequested += NavigateToAddNewEmployeeDetailView;
        }

        public BindableBase CurrentViewModel
        {
            get { return _currentViewModel; }
            set { SetProperty(ref _currentViewModel, value); }
        }

        public void NavigateToEmployeeListView()
        {
            CurrentViewModel = _empListViewModel;
        }

        private void NavigateToAddNewEmployeeDetailView()
        {
            if (_empDetailViewModel != null)
            {
                _empDetailViewModel.Done -= NavigateToEmployeeListView;
                _empDetailViewModel.Error -= NavigateToErrorView;
            }

            _empDetailViewModel = new EmployeeDetailViewModel();
            _empDetailViewModel.Error += NavigateToErrorView;
            _empDetailViewModel.Done += NavigateToEmployeeListView;

            CurrentViewModel = _empDetailViewModel;
        }

        private void NavigateToErrorView(ErrorViewModel model)
        {
            _errorSourceViewModel = CurrentViewModel;
            _errorViewModel = model;
            _errorViewModel.Done += ContinueAfterError;
            CurrentViewModel = _errorViewModel;
        }

        private void ContinueAfterError()
        {
            if(_errorViewModel != null)
            {
                _errorViewModel.Done -= ContinueAfterError;
            }
            CurrentViewModel = _errorSourceViewModel;
            _errorViewModel = null;
        }
    }
}