using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ComType = System.Runtime.InteropServices.ComTypes;

namespace appmeiser
{
    class ProcessModel : INotifyPropertyChanged
    {
        public string Name => _process.ProcessName;
        public int Id => _process.Id;
        public string InstanceName
        {
            get
            {
                if (_instanceNameLookup.TryGetValue(_process.Id, out var name)) return name;
                else return "*killed*";
            }
        }
        public string CommandLine => _process.StartInfo.FileName + " " + _process.StartInfo.Arguments?.ToString();

        public double Cpu { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;
        private Timer _processCheck;

        private Process _process;
        private Dictionary<int, string> _instanceNameLookup;
        PerformanceCounter _cpu;

        // ------------------------------------------------------------------------------------------
        // ctor
        // ------------------------------------------------------------------------------------------
        public ProcessModel(Process process, Dictionary<int, string> instanceNameLookup)
        {
            this._process = process;
            this._instanceNameLookup = instanceNameLookup;

            var instanceName = InstanceName;
            if(instanceName != "*killed*")
            {
                _cpu = new PerformanceCounter("Process", "% Processor Time", instanceName, true);
                this._processCheck = new Timer(timerTick, null, 0, 1000);
            }
        }


        // ------------------------------------------------------------------------------------------
        // timerTick
        // ------------------------------------------------------------------------------------------
        void timerTick(object state)
        {
            try
            {
                Cpu += ((int)(_cpu.NextValue() * 1000))/1000.0;
            }
            catch(Exception e)
            {
                Debug.WriteLine("Measurement error: " + e.Message);
            }
            NotifyPropertyChanged(nameof(Cpu));

        }

        // ------------------------------------------------------------------------------------------
        // NotifyPropertyChanged
        // ------------------------------------------------------------------------------------------
        void NotifyPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
