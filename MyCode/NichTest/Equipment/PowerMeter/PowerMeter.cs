using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using Ivi.Visa.Interop;
using System.Reflection;

namespace NichTest
{
    public class PowerMeter : Equipment 
    {
        protected string slot;
        protected string wavelength;
        protected string channel;
        protected string[] channelArray;
        protected string unitType;//0 "dBm",1 "W"
        
        public virtual bool ConfigWavelength(int syn = 0) { return true; }
        public virtual double ReadPower(int channel) { return 0; }
        public virtual bool SelectUnit(int syn = 0) { return true; }        
    }
}
