using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.ServiceModel;
using System.Text;
using System.Threading;
using InternetKillSwitchService.Data;
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
        IEnumerable<NetworkAdapterCustom> GetSimplifiedNetworkAdapters();

        /// <summary>
        /// Disable the selected network adapter.
        /// </summary>
        /// <param name="o">The object to disable.</param>
        /// <returns>True if the invocation succeeds.</returns>
        [OperationContract]
        bool DisableNetworkAdapter(NetworkAdapterCustom o);

        /// <summary>
        /// Enable the selected network adapter.
        /// </summary>
        /// <param name="o">The object to disable.</param>
        /// <returns>True if the invocation succeeds.</returns>
        [OperationContract]
        bool EnableNetworkAdapter(NetworkAdapterCustom o);

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
        void AddVpnAdapters(IEnumerable<NetworkAdapterCustom> adapters);

        /// <summary>
        /// Add local adapters, when the VPN is not in use.
        /// </summary>
        /// <param name="adapters">The adapters to add.</param>
        [OperationContract]
        void AddLocalAdapters(IEnumerable<NetworkAdapterCustom> adapters);

        /// <summary>
        /// Remove the VPN adapters.
        /// </summary>
        /// <param name="adapters">The adapters to remove.</param>
        [OperationContract]
        void RemoveVpnAdapters(IEnumerable<NetworkAdapterCustom> adapters);

        /// <summary>
        /// Remove the Local adapters.
        /// </summary>
        /// <param name="adapters">The adapters to remove.</param>
        [OperationContract]
        void RemoveLocalAdapters(IEnumerable<NetworkAdapterCustom> adapters);

        /// <summary>
        /// Return true if the service is paused.
        /// </summary>
        /// <returns>True if the service is paused.</returns>
        [OperationContract]
        bool IsPaused();

        /// <summary>
        /// Set the service to be paused.
        /// </summary>
        [OperationContract]
        void SetPaused();

        /// <summary>
        /// Set the service to be unpaused.
        /// </summary>
        [OperationContract]
        void SetUnpaused();
    }
}
