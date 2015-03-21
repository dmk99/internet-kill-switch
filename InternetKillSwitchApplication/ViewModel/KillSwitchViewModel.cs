using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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
            _killSwitchServiceClient = new KillSwitchServiceClient();

            NetworkAdapters = new ObservableCollection<NetworkAdapter>(_killSwitchServiceClient.GetNetworkAdapters());
        }

        /// <summary>
        /// Gets or sets the network adapters.
        /// </summary>
        public ObservableCollection<NetworkAdapter> NetworkAdapters { get; set; } 
    }
}
