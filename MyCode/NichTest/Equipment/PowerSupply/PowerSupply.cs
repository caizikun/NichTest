using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NichTest
{
    public class PowerSupply : Equipment
    {
        protected int closeDelay;
        protected int openDelay;
        protected double currentOffset;
        protected double voltageOffset;
        protected int channel_DUT;
        protected double current_DUT;
        protected double voltage_DUT;
        protected int channel_Source;
        protected double current_Source;
        protected double voltage_Source;
        
        public virtual double GetCurrent()
        {
            return Algorithm.MyNaN;
        }

        public virtual double GetVoltage()
        {
            return Algorithm.MyNaN;
        }

        public virtual bool ConfigVoltageCurrent(double voltage, int syn = 0) { return true; }
    }
}
