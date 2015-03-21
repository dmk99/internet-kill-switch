using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.ServiceProcess;
using System.Text;
using System.Management;
using System.Reflection;
using System.ServiceModel;
using System.Threading;
using InternetKillSwitchService.Interfaces;
using InternetKillSwitchService.Services;

namespace InternetKillSwitchService
{
    public partial class Service1 : ServiceBase
    {
        private Thread _thread;
        private ManualResetEvent _shutdownEvent = new ManualResetEvent(false);
        private ServiceHost _host;

        public Service1()
        {
            InitializeComponent();
        }

        public void OnDebug()
        {
            OnStart(new string[0]);
        }

        protected override void OnStart(string[] args)
        {
            //_host = new ServiceHost(typeof(KillSwitchService));
            _host = new ServiceHost(new KillSwitchService());
            _host.Open();

            NetworkChange.NetworkAddressChanged += NetworkChangeOnNetworkAddressChanged;

            _thread = new Thread(Start);
            _thread.Name = "Background timer";
            _thread.Start();
        }

        private void NetworkChangeOnNetworkAddressChanged(object sender, EventArgs e)
        {
            var service = _host.SingletonInstance as IKillSwitchService;

            if (service != null)
            {
                service.NetworkChangeOnNetworkAvailabilityChanged();
            }
        }

        private void Start()
        {
            while (!_shutdownEvent.WaitOne(0))
            {
                // Replace the Sleep() call with the work you need to do
                Thread.Sleep(1000);
            }
        }

        protected override void OnStop()
        {
            _shutdownEvent.Set();
            if (!_thread.Join(3000))
            { // give the thread 3 seconds to stop
                _thread.Abort();
            }

            try
            {
                if (_host.State != CommunicationState.Closed)
                {
                    _host.Close();
                }
            }
            catch
            {
                // handle exception somehow...log to event viewer, for example
            }
        }
    }
}
