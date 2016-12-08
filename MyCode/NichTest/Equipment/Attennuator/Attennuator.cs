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
    public class Attennuator : Equipment 
    {
        protected string attSlot;
        protected string attValue;
        protected string attChannel;
        protected string[] attChannelArray;
        protected string totalChannel;
        protected string wavelength;
        protected int openDelay;
        protected int closeDelay;
        protected int setattDelay;
        protected int sleepTime;
        protected double powerVariation;
        protected double lastAttValue = -5;
        protected byte lastattChannel = 0;

        public virtual bool ConfigWavelength( int syn = 0) { return true; }
        public virtual bool AttnValue(string InputPow, int syn = 1) { return true; }
        public virtual bool AddCalFactor(string CalFactor) { return true; }
        public virtual double GetAtten() { return 0; }
        public virtual bool SetAllChannnel_RxOverLoad(float RxOverLoad) { return true; }
        public virtual bool SetAttnValue(double AttValue, int syn = 1) { return true; }
        public virtual bool AdjustAttnValue(double AttValue, int syn = 1) { return true; }// 在当前衰减器的衰减值上调整
    }
}
