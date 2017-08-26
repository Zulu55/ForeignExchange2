namespace ForeignExchange2.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using ForeignExchange2.Helpers;
    using GalaSoft.MvvmLight.Command;
    using Models;
    using Services;
    using Xamarin.Forms;

    public class MainViewModel : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
		#endregion

		#region Services
		ApiService apiService;
		DialogService dialogService;
		DataService dataService;
		#endregion

		#region Attributes
		bool _isRunning;
        bool _isEnabled;
        string _result;
        ObservableCollection<Rate> _rates;
        Rate _sourceRate;
        Rate _targetRate;
        List<Rate> rates;
        string _status;
        #endregion

        #region Properties
        public string Status
        {
			get
			{
				return _status;
			}
			set
			{
				if (_status != value)
				{
					_status = value;
					PropertyChanged?.Invoke(
						this,
						new PropertyChangedEventArgs(nameof(Status)));
				}
			}
		}

        public string Amount
        {
            get;
            set;
        }

        public ObservableCollection<Rate> Rates
        {
			get
			{
                return _rates;
			}
			set
			{
				if (_rates != value)
				{
					_rates = value;
					PropertyChanged?.Invoke(
						this,
						new PropertyChangedEventArgs(nameof(Rates)));
				}
			}
		}

        public Rate SourceRate
        {
			get
			{
				return _sourceRate;
			}
			set
			{
				if (_sourceRate != value)
				{
					_sourceRate = value;
					PropertyChanged?.Invoke(
						this,
						new PropertyChangedEventArgs(nameof(SourceRate)));
				}
			}
		}

        public Rate TargetRate
        {
			get
			{
				return _targetRate;
			}
			set
			{
				if (_targetRate != value)
				{
					_targetRate = value;
					PropertyChanged?.Invoke(
						this,
						new PropertyChangedEventArgs(nameof(TargetRate)));
				}
			}
		}

        public bool IsRunning
        {
            get
            {
                return _isRunning;
            }
            set
            {
                if (_isRunning != value)
                {
                    _isRunning = value;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(IsRunning)));
                }
            }
        }

        public bool IsEnabled
        {
			get
			{
				return _isEnabled;
			}
			set
			{
				if (_isEnabled != value)
				{
					_isEnabled = value;
					PropertyChanged?.Invoke(
						this,
						new PropertyChangedEventArgs(nameof(IsEnabled)));
				}
			}
		}

        public string Result
        {
			get
			{
				return _result;
			}
			set
			{
				if (_result != value)
				{
					_result = value;
					PropertyChanged?.Invoke(
						this,
						new PropertyChangedEventArgs(nameof(Result)));
				}
			}
		}
        #endregion

        #region Constructors
        public MainViewModel()
        {
            apiService = new ApiService();
            dataService = new DataService();
            dialogService = new DialogService();

            LoadRates();
        }
        #endregion

        #region Methods
        async void LoadRates()
        {
            IsRunning = true;
            Result = "Loading rates...";

            var connection = await apiService.CheckConnection();

            if (!connection.IsSuccess)
            {
                LoadLocalData();
			}
            else
            {
                await LoadDataFromAPI();
            }

            if (rates.Count == 0)
            {
				IsRunning = false;
                IsEnabled = false;
				Result = "There are not internet connection and not load " +
                    "previously rates. Please try again with internet " +
                    "connection.";
                Status = "No rates loaded.";
                return;
			}

            Rates = new ObservableCollection<Rate>(rates);

            IsRunning = false;
            IsEnabled = true;
			Result = "Ready to convert!";
		}

        void LoadLocalData()
        {
            rates = dataService.Get<Rate>(false);
			Status = "Rates loaded from local data.";
		}

        async Task LoadDataFromAPI()
        {
			var url = "http://apiexchangerates.azurewebsites.net"; //Application.Current.Resources["URLAPI"].ToString();

			var response = await apiService.GetList<Rate>(
				url,
				"/api/Rates");

			if (!response.IsSuccess)
			{
                LoadLocalData();
                return;
            } 

			// Storage data local
			rates = (List<Rate>)response.Result;
			dataService.DeleteAll<Rate>();
			dataService.Save(rates);

            Status = "Rates loaded from Internet.";
		}
        #endregion

        #region Commands
        public ICommand ChangeCommand
        {
			get
			{
				return new RelayCommand(Change);
			}
		}

        void Change()
        {
            var aux = SourceRate;
            SourceRate = TargetRate;
            TargetRate = aux;
            Convert();
        }

        public ICommand ConvertCommmand
        {
            get
            {
                return new RelayCommand(Convert);
            }
        }

        async void Convert()
        {
            if (string.IsNullOrEmpty(Amount))
            {
                await dialogService.ShowMessage(
                    Lenguages.Error, 
                    Lenguages.AmountValidation);
                return;
            }

            decimal amount = 0;
            if (!decimal.TryParse(Amount, out amount))
            {
                await dialogService.ShowMessage(
					"Error",
					"You must enter a numeric value in amount.");
				return;
			}

			if (SourceRate == null)
			{
				await dialogService.ShowMessage(
					"Error",
					"You must select a source rate.");
				return;
			}

			if (TargetRate == null)
			{
				await dialogService.ShowMessage(
					"Error",
					"You must select a target rate.");
				return;
			}

            var amountConverted = amount / 
                                  (decimal)SourceRate.TaxRate * 
                                  (decimal)TargetRate.TaxRate;

            Result = string.Format(
                "{0} ${1:N2} = {2} ${3:N2}", 
                SourceRate.Code, 
                amount, 
                TargetRate.Code, 
                amountConverted);
		}
        #endregion
    }
}
