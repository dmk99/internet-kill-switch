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
            _killSwitchServiceClient = new KillSwitchServiceClient("InternetKillSwitchService.KillSwitchService");

            NetworkAdapters = new ObservableCollection<NetworkAdapterCustom>(_killSwitchServiceClient.GetSimplifiedNetworkAdapters());
        }

        /// <summary>
        /// Gets or sets the network adapters.
        /// </summary>
        public ObservableCollection<NetworkAdapterCustom> NetworkAdapters { get; set; } 
    }
}
