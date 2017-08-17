namespace ForeignExchange2.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using GalaSoft.MvvmLight.Command;
    using Models;

    public class MainViewModel
    {
        #region Properties
        public string Amount
        {
            get;
            set;
        }

        public ObservableCollection<Rate> Rates
        {
            get;
            set;
        }

        public Rate SourceRate
        {
            get;
            set;
        }

        public Rate TargetRate
        {
            get;
            set;
        }

        public bool IsRunning
        {
            get;
            set;
        }

        public bool IsEnabled
        {
            get;
            set;
        }

        public string Result
        {
            get;
            set;
        }
        #endregion

        public MainViewModel()
        {
        }

        #region Commands
        public ICommand ConvertCommmand
        {
            get
            {
                return new RelayCommand(Convert);
            }
        }

        void Convert()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
