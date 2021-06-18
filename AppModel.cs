using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appmeiser
{
    class AppModel : INotifyPropertyChanged
    {
        private ObservableCollection<ProcessModel> _processes = new ObservableCollection<ProcessModel>();
        public ObservableCollection<ProcessModel> Processes => _processes;

        public event PropertyChangedEventHandler PropertyChanged;

        // ------------------------------------------------------------------------------------------
        // ctor
        // ------------------------------------------------------------------------------------------
        public AppModel()
        {
            var lookup = GetProcessInstanceIdLookup();
            foreach (var process in Process.GetProcesses())
            {
                var instanceName = process.ProcessName;
                if(lookup.ContainsKey(process.Id))
                {
                    instanceName = lookup[process.Id];
                }
                Processes.Add(new ProcessModel(process, lookup));

            }
        }

        // ------------------------------------------------------------------------------------------
        // ctor
        // ------------------------------------------------------------------------------------------
        Dictionary<int, string> GetProcessInstanceIdLookup()
        {
            var output = new Dictionary<int, string>();
            var cat = new PerformanceCounterCategory("Process");

            string[] instances = cat.GetInstanceNames();
            foreach (string instanceName in instances)
            {

                try
                {
                    using (PerformanceCounter cnt = new PerformanceCounter("Process",  "ID Process", instanceName, true))
                    {
                        int val = (int)cnt.RawValue;
                        output[val] = instanceName;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error while checking instance name: " + e.Message);
                }
            }
            return output;
        }
        
    }
}
