using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NichTest
{
    public class AQ2211Atten: Attennuator
    {
        private string[] slots;
        private double currentOffset;
        private static object syncRoot = SyncRoot_AQ2211.Get_SyncRoot_AQ2211();//used for thread synchronization

        public override bool Initial(Dictionary<string, string> inPara, int syn = 0)
        {
            try
            { 
                this.IOType = inPara["IOTYPE"];
                this.address = inPara["ADDR"];
                this.name = inPara["NAME"];
                this.reset = Convert.ToBoolean(inPara["RESET"]);
                this.role = Convert.ToInt32(inPara["ROLE"]);
                this.slots = inPara["ATTSLOT"].Split(',');
                this.attChannel = inPara["ATTCHANNEL"];
                this.attChannelArray = this.attChannel.Split(',');
                this.attValue = inPara["ATTVALUE"];
                this.totalChannel = inPara["TOTALCHANNEL"];
                this.wavelength = inPara["WAVELENGTH"];                
                this.openDelay = Convert.ToInt32(inPara["OPENDELAY"]);
                this.closeDelay = Convert.ToInt32(inPara["CLOSEDELAY"]);
                this.setattDelay = Convert.ToInt32(inPara["SETATTDELAY"]);

                this.isConnected = false;
                switch (IOType)
                {
                    case "GPIB": 
                        lock (syncRoot)
                        {
                            this.WriteString("*IDN?");
                            string content = this.ReadString();
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
                Log.SaveLogToTxt("Failed to initial AQ2211 attennuator.");
                return false;
            }
        }

        public override bool Configure(int syn = 0)
        {
            lock (syncRoot)
            {
                if (this.isConfigured)//曾经设定过
                {
                    return true;
                }

                if (this.reset == true)
                {
                    this.Reset();
                }
                for (int i = 0; i < this.slots.Length; i++)
                {
                    //ch.Add(SlotList[i]);
                    this.attSlot = this.slots[i].ToString();
                    if (attChannelArray.Length == 1)
                    {
                        attChannel = attChannelArray[0];
                    }
                    else
                    {
                        attChannel = attChannelArray[i];
                    }

                    this.ConfigWavelength((i + 1).ToString(), syn);
                    this.AttnValue(attValue, 0);
                    this.OutPutSwitch(true, syn);
                }
                this.isConfigured = true;
                return this.isConfigured;
            }
        }

        public bool Reset()
        {
            lock (syncRoot)
            {
                if (this.WriteString("*RST"))
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

        private bool ConfigWavelength(string dutcurrentchannel, int syn = 0)
        {
            lock (syncRoot)
            {
                bool flag = false;
                bool flag1 = false;
                int k = 0;
                string readtemp = "";
                double waveinput;
                double waveoutput;
                string[] wavtemp = new string[4];
                wavelength = wavelength.Trim();
                wavtemp = wavelength.Split(new char[] { ',' });
                byte i = Convert.ToByte(Convert.ToInt16(dutcurrentchannel) - 1);
                try
                {
                    if (syn == 0)
                    {
                        Log.SaveLogToTxt("Attennuator slot is " + attSlot + " Channel " + attChannel + " Wavelength is " + wavtemp[i] + "nm");
                        return this.WriteString(":INP" + attSlot + ":Channel" + attChannel + ":WAV " + wavtemp[i] + "nm");
                    }
                    else
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            flag1 = this.WriteString(":INP" + attSlot + ":Channel" + attChannel + ":WAV " + wavtemp[i] + "nm");
                            if (flag1 == true)
                                break;
                        }

                        if (flag1 == true)
                        {
                            for (k = 0; k < 3; k++)
                            {
                                this.WriteString(":INP" + attSlot + ":Channel" + attChannel + ":WAV?");
                                readtemp = this.ReadString();
                                waveinput = Convert.ToDouble(wavtemp[i]);
                                waveoutput = Convert.ToDouble(readtemp) * Math.Pow(10, 9);
                                if (waveinput == waveoutput)
                                {
                                    break;
                                }
                            }

                            if (k <= 3)
                            {
                                flag = true;
                                Log.SaveLogToTxt("Attennuator slot is " + attSlot + " Channel " + attChannel + " Wavelength is " + wavtemp[i] + "nm");
                            }
                            else
                            {
                                Log.SaveLogToTxt("Set attennuator wavelength failed");
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

        public override bool AttnValue(string InputPower, int syn = 1)
        {
            lock (syncRoot)
            {
                bool flag = false;
                bool flag1 = false;
                string readtemp = "";
                try
                {
                    attValue = (currentOffset - Convert.ToDouble(InputPower)).ToString("F2");// 仪器中的Offset将不在添加.在计算中使用.

                    if (Convert.ToDouble(attValue) < 0)
                    {
                        attValue = "0";
                        Log.SaveLogToTxt("Light source is too small");
                    }

                    try
                    {
                        string Str = "";
                        int i = 0;
                        do
                        {
                            this.WriteString(":INP" + attSlot + ":Channel" + attChannel + ":ATT?");
                            Str = this.ReadString();
                            Thread.Sleep(200);
                            i++;
                        } while (Str == null && i < 3);

                        this.WriteString(":INP" + attSlot + ":Channel" + attChannel + ":ATT?");

                        double TempVale = double.Parse(Str);
                        sleepTime = Convert.ToInt16(Math.Abs(double.Parse(attValue) - TempVale) * 120);
                    }
                    catch
                    {
                        sleepTime = 2000;
                    }

                    if (syn == 0)
                    {
                        Log.SaveLogToTxt("Attennuator slot is " + attSlot + " attennuator value is " + attValue);
                        flag = this.WriteString(":INP" + attSlot + ":Channel" + attChannel + ":ATT " + attValue);
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            flag1 = this.WriteString(":INP" + attSlot + ":Channel" + attChannel + ":ATT " + attValue);
                            Thread.Sleep(sleepTime);
                            if (flag1 == true)
                            {
                                break;
                            }
                        }

                        if (flag1 == true)
                        {
                            for (int k = 0; k < 3; k++)
                            {
                                this.WriteString(":INP" + attSlot + ":Channel" + attChannel + ":ATT?");
                                Thread.Sleep(100);
                                readtemp = this.ReadString();
                                if (Convert.ToDouble(readtemp) == Convert.ToDouble(attValue))
                                {
                                    flag = true;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            flag = true;
                        }
                    }

                    if (flag)
                    {
                        Log.SaveLogToTxt("Attennuator slot is " + attSlot + " attennuator value is " + attValue);
                    }
                    else
                    {
                        Log.SaveLogToTxt("set attennuator value failed");
                    }
                    return flag;
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return false;
                }
            }
        }

        public override bool OutPutSwitch(bool Swith, int syn = 0)
        {
            lock (syncRoot)
            {
                string index;
                bool flag = false;
                bool flag1 = false;
                int k = 0;
                string readtemp = "";
                string intswitch = "";
                int delay = 0;
                if (Swith)
                {
                    index = "ON";
                    intswitch = "1";
                    delay = openDelay;
                }
                else
                {
                    index = "OFF";
                    intswitch = "0";
                    delay = closeDelay;
                }
                try
                {
                    if (syn == 0)
                    {//MyIO.WriteString(":INP" + AttSlot+"Channel"+attChannelannel + ":ATT " + AttValue);
                     //":OUTP" + AttSlot + ":STAT " + index  :OUTP7:STAT 0    :OUTP7:Channel1:STAT 0
                        Log.SaveLogToTxt("Attennuator slot is " + attSlot + " state is " + index);
                        flag = this.WriteString(":OUTP" + attSlot + ":Channel" + attChannel + ":STAT " + index);
                        Thread.Sleep(delay);
                        return flag;
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            flag1 = this.WriteString(":OUTP" + attSlot + ":Channel" + attChannel + ":STAT " + index);

                            if (flag1 == true)
                                break;
                        }
                        if (flag1 == true)
                        {
                            for (k = 0; k < 3; k++)
                            {
                                this.WriteString(":OUTP" + attSlot + ":Channel" + attChannel + ":STAT?");
                                readtemp = this.ReadString();
                                if (readtemp == intswitch)
                                    break;
                            }
                            if (k <= 3)
                            {
                                Log.SaveLogToTxt("Attennuator slot is " + attSlot + " state is " + index);
                                flag = true;
                            }
                            else
                            {
                                Log.SaveLogToTxt("Set attennuator state failed");
                            }

                        }
                        Thread.Sleep(delay);
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

        public override bool ConfigOffset(int channel, double offset, int syn = 0)
        {
            lock (syncRoot)
            {
                this.offsetByCh.Add(channel, offset);
                return true;
            }
        }

        public override bool ChangeChannel(int channel, int syn = 0)
        {
            lock (syncRoot)
            {
                string[] wavtemp = new string[4];
                wavelength = wavelength.Trim();
                wavtemp = wavelength.Split(new char[] { ',' });

                bool flag1, flag2;
                double offset = 0;
                //CurrentChannel = channel;
                try
                {
                    if (attChannelArray.Length == 1)
                    {
                        attChannel = attChannelArray[0];
                    }
                    else if (attChannelArray.Length == Convert.ToInt16(totalChannel))
                    {
                        attChannel = attChannelArray[Convert.ToByte(channel) - 1];
                    }
                    else
                    {
                        return false;
                    }

                    attSlot = this.slots[Convert.ToByte(channel) - 1].ToString();
                    flag1 = this.ConfigWavelength(channel.ToString());
                    if (this.offsetByCh.Keys.Contains(channel))
                    {
                        offset = offsetByCh[channel];
                    }
                    flag2 = this.SetOffset(0);//将所有 Offset设置为了 0
                    currentOffset = offset;
                    this.AttnValue("-10", 0);
                    Log.SaveLogToTxt("The current channel of attennuator is " + channel + " Wavelength is " + wavtemp[channel - 1] + " Offset is " + offset);

                    if (flag1 && flag2)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return false;
                }
            }
        }

        private bool SetOffset(double offset, int syn = 0)
        {
            lock (syncRoot)
            {
                bool flag = false;
                bool flag1 = false;
                int k = 0;
                string readtemp = "";
                try
                {
                    //offset = Math.Abs((Convert.ToDouble(AttValue) + Convert.ToDouble(offset))).ToString();
                    if (syn == 0)
                    {
                        Log.SaveLogToTxt("attennuator slot is " + attSlot + " offset is " + offset);
                        return this.WriteString(":INP" + attSlot + ":OFFS " + offset);
                    }
                    else
                    {
                        offset = offset * -1;
                        for (int i = 0; i < 3; i++)
                        {
                            flag1 = this.WriteString(":INP" + attSlot + ":OFFS " + offset);
                            if (flag1 == true)
                                break;
                        }
                        if (flag1 == true)
                        {
                            for (k = 0; k < 3; k++)
                            {
                                this.WriteString(":INP" + attSlot + ":OFFS?");
                                readtemp = this.ReadString();
                                if (Convert.ToDouble(readtemp) == Convert.ToDouble(offset))
                                {
                                    break;
                                }
                            }

                            if (k <= 3)
                            {
                                flag = true;
                                Log.SaveLogToTxt("attennuator slot is " + attSlot + " offset is " + offset);
                            }
                            else
                            {
                                Log.SaveLogToTxt("Set attennuator offset failed");
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

        public override bool SetAllChannnel_RxOverLoad(float RxOverLoad)
        {
            lock (syncRoot)
            {
                for (int i = 0; i < Convert.ToInt32(totalChannel); i++)
                {
                    ChangeChannel((i + 1), 0);
                    AttnValue(RxOverLoad.ToString(), 0);
                }
                ChangeChannel(ConditionParaByTestPlan.Channel, 0);
                AttnValue(RxOverLoad.ToString(), 0);
                return true;
            }
        }

        public override bool SetAttnValue(double attValue, int syn = 1)
        {
            lock (syncRoot)
            {
                bool flag = false;
                bool flag1 = false;
                int k = 0;
                try
                {
                    if (syn == 0)
                    {
                        Log.SaveLogToTxt("Attennuator slot is " + attSlot + "attennuator value is " + attValue);
                        flag = this.WriteString(":INP" + attSlot + ":Channel" + attChannel + ":ATT " + attValue);
                        // Thread.Sleep(SleepTime);
                        //Thread.Sleep(setattdelay);
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            //:INP7:Channel1:Att 8
                            flag1 = this.WriteString(":INP" + attSlot + ":Channel" + attChannel + ":ATT " + attValue);
                            // Thread.Sleep(setattdelay);
                            if (flag1 == true)
                                break;
                        }
                        if (flag1 == true)
                        {
                            for (k = 0; k < 3; k++)
                            {
                                this.WriteString(":INP" + attSlot + ":Channel" + attChannel + ":ATT?");
                                Thread.Sleep(100);
                                string readtemp = this.ReadString();

                                if (Convert.ToDouble(readtemp) == Convert.ToDouble(attValue))
                                {
                                    flag = true;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            flag = true;
                        }
                    }

                    if (flag)
                    {
                        Log.SaveLogToTxt("Attennuator slot is " + attSlot + " attennuator value is " + attValue);
                    }
                    else
                    {
                        Log.SaveLogToTxt("set attennuator value failed");
                    }

                    return flag;
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return false;
                }
            }
        }
    }
}
