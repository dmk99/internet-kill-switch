using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.ServiceModel;
using System.Text;
using InternetKillSwitchService.Interfaces;
using ROOT.CIMV2.Win32;

namespace InternetKillSwitchService.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class KillSwitchService : IKillSwitchService
    {
        private IDictionary<string, NetworkAdapter> _allAdapters;
        private IList<string> _allVpnAdaptersToWatch;
        private IList<string> _allLocalAdaptersToWatch;

        public KillSwitchService()
        {
            var adapters = GetNetworkAdapters();

            this.Log().Info("Constructor....");

            _allAdapters = adapters.ToDictionary(i => i.NetConnectionID, i => i);
            _allVpnAdaptersToWatch = new List<string>();
            _allLocalAdaptersToWatch = new List<string>();
        }

        /// <summary>
        /// Called when the network connection status changes.
        /// </summary>
        public void NetworkChangeOnNetworkAvailabilityChanged()
        {
            this.Log().Info("A change to the network occurred.");
            var disconnected = new List<string>();
            var allNetworkAdapters = GetNetworkAdapters().ToDictionary(i => i.NetConnectionID, i => i);

            foreach (var networkAdapter in allNetworkAdapters)
            {
                if (networkAdapter.Value.NetConnectionStatus != 2)
                {
                    disconnected.Add(networkAdapter.Value.NetConnectionID);
                }
            }

            var disableLocal = false;

            foreach (var disconnect in disconnected)
            {
                if (_allVpnAdaptersToWatch.Contains(disconnect))
                {
                    disableLocal = true;
                    break;
                }
            }

            if (disableLocal)
            {
                DisableAllLocal();
            }
        }

        /// <summary>
        /// Get all the network adapters that currently exist.
        /// </summary>
        /// <returns>The list of network adapters.</returns>
        public IEnumerable<NetworkAdapter> GetNetworkAdapters()
        {
            var wmiQuery = new SelectQuery("SELECT * FROM Win32_NetworkAdapter WHERE NetConnectionId != NULL");
            var searchProcedure = new ManagementObjectSearcher(wmiQuery);

            return from ManagementObject item in searchProcedure.Get() select new NetworkAdapter(item);
        }

        /// <summary>
        /// Disable the selected network adapter.
        /// </summary>
        /// <param name="o">The object to disable.</param>
        /// <returns>True if the invocation succeeds.</returns>
        public bool DisableNetworkAdapter(NetworkAdapter o)
        {
            return o.Disable() != 0;
        }

        /// <summary>
        /// Enable the selected network adapter.
        /// </summary>
        /// <param name="o">The object to disable.</param>
        /// <returns>True if the invocation succeeds.</returns>
        public bool EnableNetworkAdapter(NetworkAdapter o)
        {
            return o.Enable() != 0;
        }

        /// <summary>
        /// Try and get the name of the management object.
        /// </summary>
        /// <param name="o">The object to get the name of.</param>
        /// <param name="name">The name of the network adapter if it exists.</param>
        /// <returns>The name if it exists.</returns>
        public bool TryGetNetworkAdapterName(NetworkAdapter o, out string name)
        {
            name = o.NetConnectionID;

            if (name != null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get the current IP address.
        /// </summary>
        /// <returns>The IP address.</returns>
        public string GetIpAddress()
        {
            return string.Empty;
        }

        /// <summary>
        /// Enable all the local network adapters.
        /// </summary>
        public void EnableAllLocal()
        {
            foreach (var local in _allLocalAdaptersToWatch)
            {
                _allAdapters[local].Enable();
            }
        }

        /// <summary>
        /// Enable all the vpn network adapters.
        /// </summary>
        public void EnableAllVpn()
        {
            foreach (var local in _allVpnAdaptersToWatch)
            {
                _allAdapters[local].Enable();
            }
        }

        /// <summary>
        /// Disable all the local network adapters.
        /// </summary>
        public void DisableAllLocal()
        {
            foreach (var local in _allLocalAdaptersToWatch)
            {
                _allAdapters[local].Disable();
            }
        }

        /// <summary>
        /// Disable all the vpn network adapters.
        /// </summary>
        public void DisableAllVpn()
        {
            foreach (var local in _allVpnAdaptersToWatch)
            {
                _allAdapters[local].Disable();
            }
        }

        /// <summary>
        /// Add VPN adapters to keep track of them.
        /// </summary>
        /// <param name="adapters">The adapters to add.</param>
        public void AddVpnAdapters(IEnumerable<NetworkAdapter> adapters)
        {
            foreach (var networkAdapter in adapters)
            {
                if (_allVpnAdaptersToWatch.Contains(networkAdapter.NetConnectionID) == false)
                {
                    _allVpnAdaptersToWatch.Add(networkAdapter.NetConnectionID);
                }
            }
        }

        /// <summary>
        /// Add local adapters, when the VPN is not in use.
        /// </summary>
        /// <param name="adapters">The adapters to add.</param>
        public void AddLocalAdapters(IEnumerable<NetworkAdapter> adapters)
        {
            foreach (var networkAdapter in adapters)
            {
                if (_allLocalAdaptersToWatch.Contains(networkAdapter.NetConnectionID) == false)
                {
                    _allLocalAdaptersToWatch.Add(networkAdapter.NetConnectionID);
                }
            }
        }

        /// <summary>
        /// Remove the VPN adapters.
        /// </summary>
        /// <param name="adapters">The adapters to remove.</param>
        public void RemoveVpnAdapters(IEnumerable<NetworkAdapter> adapters)
        {
            foreach (var networkAdapter in adapters)
            {
                _allVpnAdaptersToWatch.Remove(networkAdapter.NetConnectionID);
            }
        }

        /// <summary>
        /// Remove the Local adapters.
        /// </summary>
        /// <param name="adapters">The adapters to remove.</param>
        public void RemoveLocalAdapters(IEnumerable<NetworkAdapter> adapters)
        {
            foreach (var networkAdapter in adapters)
            {
                _allLocalAdaptersToWatch.Remove(networkAdapter.NetConnectionID);
            }
        }
    }
}
