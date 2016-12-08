using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using Ivi.Visa.Interop;
using System.Collections;

namespace NichTest
{
    public class AQ2211OpticalSwitch : OpticalSwitch
    {
        public override bool Initial(Dictionary<string, string> inPara, int syn = 0)
        {
            try
            { 
                this.IOType = inPara["IOTYPE"];
                this.address = inPara["ADDR"];
                this.name = inPara["NAME"];
                this.reset = Convert.ToBoolean(inPara["RESET"]);
                this.role = Convert.ToInt32(inPara["ROLE"]);
                this.slots = inPara["OPTICALSWITCHSLOT"];
                this.channel = Convert.ToInt32(inPara["SWITCHCHANNEL"]);
                this.toChannel = Convert.ToInt32(inPara["TOCHANNEL"]);
                this.BidiTx_Channel = inPara["BIDITXCHANNEL"].Split(',');
                this.BidiRx_Channel = inPara["BIDIRXCHANNEL"].Split(',');

                this.isConnected = false;
                switch (IOType)
                {
                    case "GPIB":
                        lock (myIO)
                        {
                            myIO.WriteString(IOPort.Type.GPIB, "GPIB0::" + address, "*IDN?");
                            string content = myIO.ReadString(IOPort.Type.GPIB, "GPIB0::" + address);
                            this.isConnected = content.Contains("AQ22");
                        }
                        break;

                    default:
                        Log.SaveLogToTxt("GPIB port error.");
                        break;
                }
                return this.isConnected;
            }
            catch
            {
                Log.SaveLogToTxt("Failed to initial AQ2211 optical switch.");
                return false;
            }
        }

        public override bool Configure(int syn = 0)
        {
            lock (myIO)
            {
                try
                {
                    if (this.isConfigured)//曾经设定过
                    {
                        return true;
                    }
                    else//未曾经设定过
                    {
                        if (this.reset == true)
                        {
                            this.Reset();
                        }
                        this.SwitchChannel(syn);
                        this.isConfigured = true;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.Message);
                    return false;
                }
            }
        }

        public bool Reset()
        {
            lock (myIO)
            {
                if (myIO.WriteString(IOPort.Type.GPIB, "GPIB0::" + address, "*RST"))
                {
                    Thread.Sleep(3000);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public override bool ChangeChannel(int channel, int syn = 0)
        {
            lock (myIO)
            {
                bool flag = false;
                bool flag1 = false;
                int k = 0;
                string readtemp = "";
                try
                {
                    if (syn == 0)
                    {
                        Log.SaveLogToTxt("Optical switch change channel to " + channel);
                        return myIO.WriteString(IOPort.Type.GPIB, "GPIB0::" + address, ":route" + this.slots + ":chan" + this.channel + " A," + channel);
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            flag1 = myIO.WriteString(IOPort.Type.GPIB, "GPIB0::" + address, ":route" + this.slots + ":chan" + this.channel + " A," + channel);
                            if (flag1 == true)
                                break;
                        }
                        if (flag1 == true)
                        {
                            for (k = 0; k < 3; k++)
                            {

                                myIO.WriteString(IOPort.Type.GPIB, "GPIB0::" + address, ":route" + this.slots + "?");
                                readtemp = myIO.ReadString(IOPort.Type.GPIB, "GPIB0::" + address);
                                if (readtemp == " A," + channel)
                                {
                                    break;
                                }
                            }

                            if (k <= 3)
                            {
                                Log.SaveLogToTxt("Optical switch change channel to " + channel);
                                flag = true;
                            }
                            else
                            {
                                Log.SaveLogToTxt("Optical switch change channel failed.");
                            }

                        }
                        return flag;
                    }
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return false;
                }
            }
        }

        public override bool SwitchChannel(int syn = 0)
        {
            lock (myIO)
            {
                bool flag = false;
                bool flag1 = false;
                int k = 0;
                string readtemp = "";
                try
                {
                    if (syn == 0)
                    {
                        Log.SaveLogToTxt("Optical switch change channel to " + this.toChannel);
                        return myIO.WriteString(IOPort.Type.GPIB, "GPIB0::" + address, ":route" + this.slots + ":chan" + this.channel + " A," + this.toChannel);
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            flag1 = myIO.WriteString(IOPort.Type.GPIB, "GPIB0::" + address, ":route" + this.slots + ":chan" + this.channel + " A," + this.toChannel);
                            if (flag1 == true)
                            {
                                break;
                            }
                        }
                        if (flag1 == true)
                        {
                            for (k = 0; k < 3; k++)
                            {

                                myIO.WriteString(IOPort.Type.GPIB, "GPIB0::" + address, ":route" + this.slots + "?");
                                readtemp = myIO.ReadString(IOPort.Type.GPIB, "GPIB0::" + address);
                                if (readtemp == "A," + this.toChannel)
                                {
                                    break;
                                }
                            }

                            if (k <= 3)
                            {
                                Log.SaveLogToTxt("Optical switch change channel to " + this.toChannel);
                                flag = true;
                            }
                            else
                            {
                                Log.SaveLogToTxt("Optical switch change channel failed");
                            }
                        }
                        return flag;
                    }
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return false;
                }
            }
        }

        public override bool CheckEquipmentRole(byte TestModelType, byte Channel)
        {//// 0=NA,1=TX,2=RX
            lock (myIO)
            {
                int actualChannel = 1;

                if (this.role == 0)//TX Rx 公用
                {
                    if (TestModelType == 1)//Tx
                    {
                        actualChannel = Convert.ToInt32(BidiTx_Channel[Channel - 1]);
                    }
                    if (TestModelType == 2)//Rx
                    {
                        actualChannel = Convert.ToInt32(BidiRx_Channel[Channel - 1]);
                    }
                    this.ChangeChannel(actualChannel, 1);
                }
                return true;
            }  
        }
    }
}
