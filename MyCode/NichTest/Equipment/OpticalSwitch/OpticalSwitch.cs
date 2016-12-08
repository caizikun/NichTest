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
    public class OpticalSwitch : Equipment
    {        
        protected string slots;
        protected int channel;
        protected int toChannel;
        protected string[] BidiTx_Channel;
        protected string[] BidiRx_Channel;

        public virtual bool SwitchChannel(int syn = 0) { return true; }
        /// <summary>
        /// 切换模式 (0=长光纤;1=短光纤)
        /// </summary>
        /// <param name="Type">类别 0=长光纤;1=短光纤</param>
        /// <param name="syn">同步 1=同步;0=异步</param>
        /// <returns></returns>
        public virtual bool SelectMode(byte Type, int syn = 0) { return true; }
        public override bool ConfigOffset(int channel, double offset, int syn = 0){ return true;}       
        public virtual bool CheckEquipmentRole(byte TestModelType, byte Channel) { return true; }
    }
}

