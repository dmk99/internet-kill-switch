using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using InternetKillSwitchService.Data;
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
        private bool _isPaused;

        public KillSwitchService()
        {
            var adapters = GetNetworkAdapters();
            _allAdapters = adapters.ToDictionary(i => i.NetConnectionID, i => i);
            _allVpnAdaptersToWatch = new List<string>();
            _allLocalAdaptersToWatch = new List<string>();
            _isPaused = false;

            ReloadAdapters();
        }

        /// <summary>
        /// Called when the network connection status changes.
        /// </summary>
        public void NetworkChangeOnNetworkAvailabilityChanged()
        {
            if (_isPaused)
            {
                this.Log().Info("The service is currently paused...");
                return;
            }

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
            var localDisconnected = false;

            foreach (var disconnect in disconnected)
            {
                if (_allVpnAdaptersToWatch.Contains(disconnect))
                {
                    disableLocal = true;
                    break;
                }

                if (_allLocalAdaptersToWatch.Contains(disconnect))
                {
                    localDisconnected = true;
                }
            }

            if (disableLocal && localDisconnected == false)
            {
                DisableAllLocal();
            }
        }

        /// <summary>
        /// Get all the network adapters that currently exist.
        /// </summary>
        /// <returns>The list of network adapters.</returns>
        public IEnumerable<NetworkAdapterCustom> GetSimplifiedNetworkAdapters()
        {
            this.Log().Info("GetNetworkAdapters");
            var wmiQuery = new SelectQuery("SELECT * FROM Win32_NetworkAdapter WHERE NetConnectionId != NULL");
            var searchProcedure = new ManagementObjectSearcher(wmiQuery);

            var adapters = (from ManagementObject item in searchProcedure.Get()
                select new NetworkAdapterCustom(new NetworkAdapter(item))).ToList();

            foreach (var adapter in adapters)
            {
                adapter.Category = TryMatchAdapterCategory(adapter);
            }

            return adapters;
        }

        /// <summary>
        /// Attempts to match the adapter name to existing adapters to find the category.
        /// </summary>
        private NetworkAdapterCategory TryMatchAdapterCategory(NetworkAdapterCustom adapter)
        {
            if (_allLocalAdaptersToWatch.Contains(adapter.ConnectionName))
            {
                return NetworkAdapterCategory.Local;
            }

            if (_allVpnAdaptersToWatch.Contains(adapter.ConnectionName))
            {
                return NetworkAdapterCategory.Vpn;
            }

            return NetworkAdapterCategory.None;
        }

        /// <summary>
        /// Disable the selected network adapter.
        /// </summary>
        /// <param name="o">The object to disable.</param>
        /// <returns>True if the invocation succeeds.</returns>
        public bool DisableNetworkAdapter(NetworkAdapterCustom o)
        {
            NetworkAdapter adapter;

            if (TryGetNetworkAdapterFromCustom(o, out adapter))
            {
                return adapter.Disable() != 0;
            }

            return false;
        }

        /// <summary>
        /// Enable the selected network adapter.
        /// </summary>
        /// <param name="o">The object to disable.</param>
        /// <returns>True if the invocation succeeds.</returns>
        public bool EnableNetworkAdapter(NetworkAdapterCustom o)
        {
            NetworkAdapter adapter;

            if (TryGetNetworkAdapterFromCustom(o, out adapter))
            {
                return adapter.Enable() != 0;   
            }

            return false;
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
                this.Log().Info("Enabling {0}", local);
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
                this.Log().Info("Enabling {0}", local);
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
                this.Log().Info("Disabling {0}", local);
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
                this.Log().Info("Disabling {0}", local);
                _allAdapters[local].Disable();
            }
        }

        /// <summary>
        /// Add VPN adapters to keep track of them.
        /// </summary>
        /// <param name="adapters">The adapters to add.</param>
        public void AddVpnAdapters(IEnumerable<NetworkAdapterCustom> adapters)
        {
            foreach (var networkAdapter in adapters)
            {
                if (_allVpnAdaptersToWatch.Contains(networkAdapter.ConnectionName) == false)
                {
                    this.Log().Info("Adding {0} to VPN List", networkAdapter.ConnectionName);
                    _allVpnAdaptersToWatch.Add(networkAdapter.ConnectionName);
                }
            }

            SaveVpnAdapters();
        }

        /// <summary>
        /// Add local adapters, when the VPN is not in use.
        /// </summary>
        /// <param name="adapters">The adapters to add.</param>
        public void AddLocalAdapters(IEnumerable<NetworkAdapterCustom> adapters)
        {
            foreach (var networkAdapter in adapters)
            {
                if (_allLocalAdaptersToWatch.Contains(networkAdapter.ConnectionName) == false)
                {
                    this.Log().Info("Adding {0} to Local List", networkAdapter.ConnectionName);
                    _allLocalAdaptersToWatch.Add(networkAdapter.ConnectionName);
                }
            }

            SaveLocalAdapters();
        }

        /// <summary>
        /// Remove the VPN adapters.
        /// </summary>
        /// <param name="adapters">The adapters to remove.</param>
        public void RemoveVpnAdapters(IEnumerable<NetworkAdapterCustom> adapters)
        {
            foreach (var networkAdapter in adapters)
            {
                this.Log().Info("Removing {0} from VPN List", networkAdapter.ConnectionName);
                _allVpnAdaptersToWatch.Remove(networkAdapter.ConnectionName);
            }

            SaveVpnAdapters();
        }

        /// <summary>
        /// Remove the Local adapters.
        /// </summary>
        /// <param name="adapters">The adapters to remove.</param>
        public void RemoveLocalAdapters(IEnumerable<NetworkAdapterCustom> adapters)
        {
            foreach (var networkAdapter in adapters)
            {
                this.Log().Info("Removing {0} from Local List", networkAdapter.ConnectionName);
                _allLocalAdaptersToWatch.Remove(networkAdapter.ConnectionName);
            }

            SaveLocalAdapters();
        }

        /// <summary>
        /// Return true if the service is paused.
        /// </summary>
        /// <returns>True if the service is paused.</returns>
        public bool IsPaused()
        {
            return _isPaused;
        }

        /// <summary>
        /// Set the service to be paused.
        /// </summary>
        public void SetPaused()
        {
            _isPaused = true;
        }

        /// <summary>
        /// Set the service to be unpaused.
        /// </summary>
        public void SetUnpaused()
        {
            _isPaused = false;
        }

        /// <summary>
        /// Get all the network adapters that currently exist.
        /// </summary>
        /// <returns>The list of network adapters.</returns>
        private IEnumerable<NetworkAdapter> GetNetworkAdapters()
        {
            this.Log().Info("GetNetworkAdapters");
            var wmiQuery = new SelectQuery("SELECT * FROM Win32_NetworkAdapter WHERE NetConnectionId != NULL");
            var searchProcedure = new ManagementObjectSearcher(wmiQuery);

            return from ManagementObject item in searchProcedure.Get() select new NetworkAdapter(item);
        }

        /// <summary>
        /// Return the adapter and true if it exists. False otherwise.
        /// </summary>
        /// <param name="o">The object to search for.</param>
        /// <param name="networkAdapter">The output if found.</param>
        /// <returns>True if successful.</returns>
        private bool TryGetNetworkAdapterFromCustom(NetworkAdapterCustom o, out NetworkAdapter networkAdapter)
        {
            networkAdapter = null;

            if (_allAdapters.ContainsKey(o.ConnectionName) == false)
            {
                return false;
            }

            networkAdapter = _allAdapters[o.ConnectionName];
            return true;
        }

        /// <summary>
        /// Save the adapters to file for later use.
        /// </summary>
        private void SaveLocalAdapters()
        {
            App.Default.LocalNetworks = "";

            foreach (var local in _allLocalAdaptersToWatch)
            {
                App.Default.LocalNetworks = App.Default.LocalNetworks + local + ",";
            }

            App.Default.Save();
        }

        /// <summary>
        /// Save the adapters to file for later use.
        /// </summary>
        private void SaveVpnAdapters()
        {
            App.Default.VpnNetworks = "";

            foreach (var vpn in _allVpnAdaptersToWatch)
            {
                App.Default.VpnNetworks = App.Default.VpnNetworks + vpn + ",";
            }

            App.Default.Save();
        }

        /// <summary>
        /// Reload the adapters if they already exist.
        /// </summary>
        private void ReloadAdapters()
        {
            var localNetworks = App.Default.LocalNetworks;
            var vpnNetworks = App.Default.VpnNetworks;

            if (string.IsNullOrEmpty(localNetworks) == false)
            {
                var networks = localNetworks.Split(',');

                foreach (var network in networks)
                {
                    if (_allAdapters.ContainsKey(network))
                    {
                        _allLocalAdaptersToWatch.Add(network);
                    }
                }
            }

            if (string.IsNullOrEmpty(vpnNetworks) == false)
            {
                var networks = vpnNetworks.Split(',');

                foreach (var network in networks)
                {
                    if (_allAdapters.ContainsKey(network))
                    {
                        _allVpnAdaptersToWatch.Add(network);
                    }
                }
            }
        }
    }
}
