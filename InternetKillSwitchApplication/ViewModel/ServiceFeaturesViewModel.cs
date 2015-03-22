using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using InternetKillSwitchApplication.KillSwitch;

namespace InternetKillSwitchApplication.ViewModel
{
    public class ServiceFeaturesViewModel
    {        
        private readonly KillSwitchServiceClient _killSwitchServiceClient;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ServiceFeaturesViewModel()
        {
            _killSwitchServiceClient = new KillSwitchServiceClient("InternetKillSwitchService.KillSwitchService");
            
            InitializeCommands();
        }

        /// <summary>
        /// Initialize all the commands
        /// </summary>
        void InitializeCommands()
        {
            ShowSettingsCommand = new DelegateCommand(() =>
            {
                Application.Current.MainWindow = new Options();
                Application.Current.MainWindow.ShowDialog();
            },
            () => Application.Current.MainWindow == null);

            EnableAllLocalCommand = new DelegateCommand(() =>
            {
                _killSwitchServiceClient.EnableAllLocal();
            });

            DisableAllLocalCommand = new DelegateCommand(() =>
            {
                _killSwitchServiceClient.DisableAllLocal();
            });

            PauseServiceCommand = new DelegateCommand(() =>
            {
                _killSwitchServiceClient.SetPaused();
            },
            () => _killSwitchServiceClient.IsPaused() == false);

            UnpauseServiceCommand = new DelegateCommand(() =>
            {
                _killSwitchServiceClient.SetUnpaused();
            },
            () => _killSwitchServiceClient.IsPaused());
        }

        /// <summary>
        /// The show settings command.
        /// </summary>
        public ICommand ShowSettingsCommand { get; set; }

        /// <summary>
        /// The enable all local command.
        /// </summary>
        public ICommand EnableAllLocalCommand { get; set; }

        /// <summary>
        /// The disable all local command.
        /// </summary>
        public ICommand DisableAllLocalCommand { get; set; }

        /// <summary>
        /// The pause service command.
        /// </summary>
        public ICommand PauseServiceCommand { get; set; }

        /// <summary>
        /// The unpause service command.
        /// </summary>
        public ICommand UnpauseServiceCommand { get; set; }
    }
}
