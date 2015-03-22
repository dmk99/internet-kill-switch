using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Input;
using InternetKillSwitchApplication.KillSwitch;

namespace InternetKillSwitchApplication.ViewModel
{
    public class KillSwitchViewModel
    {
        private readonly KillSwitchServiceClient _killSwitchServiceClient;

        /// <summary>
        /// Constructor.
        /// </summary>
        public KillSwitchViewModel()
        {
            _killSwitchServiceClient = new KillSwitchServiceClient("InternetKillSwitchService.KillSwitchService");
            
            NetworkAdapters = new ObservableCollection<NetworkAdapterCustom>(_killSwitchServiceClient.GetSimplifiedNetworkAdapters());

            InitializeCommands();
        }

        /// <summary>
        /// Initial the commands.
        /// </summary>
        private void InitializeCommands()
        {
            SetToNoneCommand = new DelegateCommand(() =>
            {
                if (SelectedAdapter != null)
                {
                    SelectedAdapter.Category = NetworkAdapterCategory.None;
                }
            });

            SetToLocalCommand = new DelegateCommand(() =>
            {
                if (SelectedAdapter != null)
                {
                    SelectedAdapter.Category = NetworkAdapterCategory.Local;
                }
            });

            SetToVpnCommand = new DelegateCommand(() =>
            {
                if (SelectedAdapter != null)
                {
                    SelectedAdapter.Category = NetworkAdapterCategory.Vpn;
                }
            });

            SaveCommand = new DelegateCommand(Save);
        }

        /// <summary>
        /// Save all the adapters' categories.
        /// </summary>
        private void Save()
        {
            var local = NetworkAdapters.Where(i => i.Category == NetworkAdapterCategory.Local);
            var vpn = NetworkAdapters.Where(i => i.Category == NetworkAdapterCategory.Vpn);
            var none = NetworkAdapters.Where(i => i.Category == NetworkAdapterCategory.None);

            _killSwitchServiceClient.RemoveLocalAdapters(none.ToArray());
            _killSwitchServiceClient.RemoveVpnAdapters(none.ToArray());

            _killSwitchServiceClient.AddLocalAdapters(local.ToArray());
            _killSwitchServiceClient.AddVpnAdapters(vpn.ToArray());
        }

        /// <summary>
        /// Gets or sets the currently selected adapter.
        /// </summary>
        public NetworkAdapterCustom SelectedAdapter { get; set; }

        /// <summary>
        /// Gets or sets the network adapters.
        /// </summary>
        public ObservableCollection<NetworkAdapterCustom> NetworkAdapters { get; set; }

        /// <summary>
        /// Gets or sets the save command.
        /// </summary>
        public ICommand SaveCommand { get; set; }

        /// <summary>
        /// Gets or sets the command to set to none.
        /// </summary>
        public ICommand SetToNoneCommand { get; set; }

        /// <summary>
        /// Gets or sets the command to set to Local.
        /// </summary>
        public ICommand SetToLocalCommand { get; set; }

        /// <summary>
        /// Gets or sets the command to set to VPN.
        /// </summary>
        public ICommand SetToVpnCommand { get; set; }
    }
}
