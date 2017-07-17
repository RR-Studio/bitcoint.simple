using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using NBitcoin;
using NBitcoin.Protocol;
using NBitcoin.Protocol.Behaviors;
using NBitcoin.SPV;

namespace ConsoleApp1
{
    public class SearchNodes
    {
        internal NodesGroup _Group;

        NodeConnectionParameters _ConnectionParameters;

        public EventHandler<NodesCollection> OnChangeNodesCollection;


        public string AddressFile { get => App.AppDir + "adderss.bat"; }

        public string ChainFile { get => App.AppDir + "chain.bat"; }

        public string TrackerFile { get => App.AppDir + "tracker.bat"; }

        private Timer Time { get; set; }

        public void Search()
        {
            var parameters = new NodeConnectionParameters();
            parameters.TemplateBehaviors.Add(new AddressManagerBehavior(GetAddressManager())); //So we find nodes faster
            parameters.TemplateBehaviors.Add(new ChainBehavior(GetChain())); //So we don't have to load the chain each time we start
            parameters.TemplateBehaviors.Add(new TrackerBehavior(GetTracker())); //Tracker knows which scriptPubKey and outpoints to track, it monitors all your wallets at the same
           
            _Group = new NodesGroup(App.Network, parameters, new NodeRequirement()
            {
                RequiredServices = NodeServices.Network //Needed for SPV
            });
            _Group.MaximumNodeConnection = 4;
            _Group.Connect();
            _ConnectionParameters = parameters;

            Time = new Timer
            {
                Interval = 5000,
                Enabled = true,
                AutoReset = true
            };
            Time.Elapsed += TimeOnElapsed;
            Time.Start();
        }

        private void TimeOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            SaveAsync();
            OnChangeNodesCollection?.Invoke(this, _Group.ConnectedNodes);
            
        }

        private void SaveAsync()
        {
        
            GetAddressManager().SavePeerFile(AddressFile, App.Network);
            using (var fs = File.Open(ChainFile, FileMode.Create))
            {
                GetChain().WriteTo(fs);
            }
            using (var fs = File.Open(TrackerFile, FileMode.Create))
            {
                GetTracker().Save(fs);
            }
           
        }

        private AddressManager GetAddressManager()
        {
            if (_ConnectionParameters != null)
            {
                return _ConnectionParameters.TemplateBehaviors.Find<AddressManagerBehavior>().AddressManager;
            }
            try
            {
                lock (App.Saving)
                {
                    return AddressManager.LoadPeerFile(App.AppDir+ "adderss.bat");
                }
            }
            catch
            {
                return new AddressManager();
            }
        }

        private ConcurrentChain GetChain()
        {
            if (_ConnectionParameters != null)
            {
                return _ConnectionParameters.TemplateBehaviors.Find<ChainBehavior>().Chain;
            }
            var chain = new ConcurrentChain(App.Network);
            try
            {
                lock (App.Saving)
                {
                    chain.Load(File.ReadAllBytes(ChainFile));
                }
            }
            catch
            {
            }
            return chain;
        }

        private Tracker GetTracker()
        {
            if (_ConnectionParameters != null)
            {
                return _ConnectionParameters.TemplateBehaviors.Find<TrackerBehavior>().Tracker;
            }
            try
            {
                lock (App.Saving)
                {
                    using (var fs = File.OpenRead(TrackerFile))
                    {
                        return Tracker.Load(fs);
                    }
                }
            }
            catch
            {
            }
            return new Tracker();
        }
    }
}
