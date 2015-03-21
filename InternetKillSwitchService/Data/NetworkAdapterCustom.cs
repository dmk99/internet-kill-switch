using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ROOT.CIMV2.Win32;

namespace InternetKillSwitchService.Data
{
    [DataContract]
    public class NetworkAdapterCustom
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="connectionName">The unique name.</param>
        /// <param name="connectionStatus">The connection status.</param>
        public NetworkAdapterCustom(string connectionName, uint connectionStatus)
        {
            ConnectionName = connectionName;
            ConnectionStatus = connectionStatus;
        }

        /// <summary>
        /// Construct the object based on a network adapter.
        /// </summary>
        /// <param name="adapter">The adapter.</param>
        public NetworkAdapterCustom(NetworkAdapter adapter) 
            : this(adapter.NetConnectionID, adapter.NetConnectionStatus)
        {
            
        }

        [DataMember]
        public string ConnectionName { get; set; }

        [DataMember]
        public uint ConnectionStatus { get; set; }
    }
}
