using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NichTest
{
    public class Equipment: IEquipment
    {
        protected IOPort myIO = IOPort.GetIOPort();
        protected bool isConnected;
        protected bool isConfigured;
        protected string IOType;
        protected string address;
        protected string name;
        protected bool reset;
        protected int role;
        protected Dictionary<int, double> offsetByCh = new Dictionary<int, double>();

        public virtual bool Initial(Dictionary<string, string> inPara, int syn = 0) { return true; }
        public virtual bool Configure(int syn = 0) { return true; }
        public virtual bool OutPutSwitch(bool isON, int syn = 0) { return true; }
        public virtual bool ConfigOffset(int channel, double offset, int syn = 0) { return true; }
        public virtual bool ConfigOffset(int channel, double offset_VCC, double offset_ICC, int syn = 0) { return true; }
        public virtual bool ChangeChannel(int channel, int syn = 0) { return true; }

        protected virtual bool WriteString(string str_Write)
        {
            return myIO.WriteString(IOPort.Type.GPIB, "GPIB0::" + address, str_Write);
        }
        protected virtual string ReadString(int count = 0)
        {
            return myIO.ReadString(IOPort.Type.GPIB, "GPIB0::" + address, count);
        }
    }
}
