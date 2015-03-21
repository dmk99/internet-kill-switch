using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.ServiceModel;
using System.Text;
using System.Threading;
using ROOT.CIMV2.Win32;

namespace InternetKillSwitchService.Interfaces
{
    [ServiceContract]
    public interface IKillSwitchService
    {
        /// <summary>
        /// When the network status changes, update.
        /// </summary>
        [OperationContract]
        void NetworkChangeOnNetworkAvailabilityChanged();

        /// <summary>
        /// Get all the network adapters that currently exist.
        /// </summary>
        /// <returns>The list of network adapters.</returns>
        [OperationContract]
        IEnumerable<NetworkAdapter> GetNetworkAdapters();

        /// <summary>
        /// Disable the selected network adapter.
        /// </summary>
        /// <param name="o">The object to disable.</param>
        /// <returns>True if the invocation succeeds.</returns>
        [OperationContract]
        bool DisableNetworkAdapter(NetworkAdapter o);

        /// <summary>
        /// Enable the selected network adapter.
        /// </summary>
        /// <param name="o">The object to disable.</param>
        /// <returns>True if the invocation succeeds.</returns>
        [OperationContract]
        bool EnableNetworkAdapter(NetworkAdapter o);

        /// <summary>
        /// Try and get the name of the management object.
        /// </summary>
        /// <param name="o">The object to get the name of.</param>
        /// <param name="name">The name of the network adapter if it exists.</param>
        /// <returns>The name if it exists.</returns>
        [OperationContract]
        bool TryGetNetworkAdapterName(NetworkAdapter o, out string name);

        /// <summary>
        /// Get the current IP address.
        /// </summary>
        /// <returns>The IP address.</returns>
        [OperationContract]
        string GetIpAddress();

        /// <summary>
        /// Enable all the local network adapters.
        /// </summary>
        [OperationContract]
        void EnableAllLocal();

        /// <summary>
        /// Enable all the vpn network adapters.
        /// </summary>
        [OperationContract]
        void EnableAllVpn();

        /// <summary>
        /// Disable all the local network adapters.
        /// </summary>
        [OperationContract]
        void DisableAllLocal();

        /// <summary>
        /// Disable all the vpn network adapters.
        /// </summary>
        [OperationContract]
        void DisableAllVpn();

        /// <summary>
        /// Add VPN adapters to keep track of them.
        /// </summary>
        /// <param name="adapters">The adapters to add.</param>
        [OperationContract]
        void AddVpnAdapters(IEnumerable<NetworkAdapter> adapters);

        /// <summary>
        /// Add local adapters, when the VPN is not in use.
        /// </summary>
        /// <param name="adapters">The adapters to add.</param>
        [OperationContract]
        void AddLocalAdapters(IEnumerable<NetworkAdapter> adapters);

        /// <summary>
        /// Remove the VPN adapters.
        /// </summary>
        /// <param name="adapters">The adapters to remove.</param>
        [OperationContract]
        void RemoveVpnAdapters(IEnumerable<NetworkAdapter> adapters);

        /// <summary>
        /// Remove the Local adapters.
        /// </summary>
        /// <param name="adapters">The adapters to remove.</param>
        [OperationContract]
        void RemoveLocalAdapters(IEnumerable<NetworkAdapter> adapters);
    }
}
