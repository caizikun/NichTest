using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NichTest
{
    class E3631 : PowerSupply
    {
        private static object syncRoot = new object();//used for thread synchronization

        public override bool Initial(Dictionary<string, string> inPara, int syn = 0)
        {
            try
            {                
                this.IOType = inPara["IOTYPE"];
                this.address = inPara["ADDR"];
                this.name = inPara["NAME"];
                this.reset = Convert.ToBoolean(inPara["RESET"]);
                this.role = Convert.ToInt32(inPara["ROLE"]);
                this.channel_DUT = Convert.ToInt32(inPara["DUTCHANNEL"]);
                this.voltage_DUT = Convert.ToDouble(inPara["DUTVOLTAGE"]);
                this.current_DUT = Convert.ToDouble(inPara["DUTCURRENT"]);
                this.channel_Source = Convert.ToInt32(inPara["OPTSOURCECHANNEL"]);
                this.voltage_Source = Convert.ToDouble(inPara["OPTVOLTAGE"]);
                this.current_Source = Convert.ToDouble(inPara["OPTCURRENT"]);
                this.openDelay = Convert.ToInt32(inPara["OPENDELAY"]);
                this.closeDelay = Convert.ToInt32(inPara["CLOSEDELAY"]);

                this.isConnected = false;
                switch (IOType)
                {
                    case "GPIB":
                        lock (syncRoot)
                        {
                            this.WriteString("*IDN?");
                            string content = this.ReadString();
                            this.isConnected = content.Contains("E3631");
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
                Log.SaveLogToTxt("Failed to initial E3631.");
                return false;
            }
        }

        public override bool Configure(int syn = 0)
        {
            lock (syncRoot)
            {
                try
                {
                    if (this.isConfigured)//曾经设定过
                    {
                        return true;
                    }

                    if (this.reset == true)
                    {
                        this.isConfigured = this.Reset();
                    }

                    if ((channel_DUT == 1) || (channel_DUT == 2))
                    {
                        ConfigVoltageCurrent(channel_DUT, voltage_DUT, current_DUT);
                    }

                    if ((channel_Source == 1) || (channel_Source == 2))
                    {
                        ConfigVoltageCurrent(channel_Source, voltage_Source, current_Source);
                    }

                    this.isConfigured = OutPutSwitch(true, syn) && this.isConfigured;
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

        public override bool OutPutSwitch(bool isON, int syn = 0)
        {
            lock (syncRoot)
            {
                // "ON "" OFF"            
                string command;
                int delay = 0;
                string intswitch;

                if (isON)
                {
                    command = "ON";
                    intswitch = "1";
                    delay = this.openDelay;
                }
                else
                {
                    command = "OFF";
                    intswitch = "0";
                    delay = this.closeDelay;
                }

                try
                {
                    bool result = false;
                    if (syn == 0)
                    {
                        result = this.WriteString("OUTP " + command);
                        Thread.Sleep(delay);
                        return result;
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            result = this.WriteString("OUTP " + command);
                            if (result == true)
                            {
                                break;
                            }
                        }

                        if (result == true)
                        {
                            int k;
                            for (k = 0; k < 3; k++)
                            {
                                this.WriteString("OUTP?");
                                string readtemp = this.ReadString();
                                if (readtemp == intswitch + "\n")
                                {
                                    break;
                                }
                            }

                            if (k <= 3)
                            {
                                result = true;
                            }
                            else
                            {
                                result = false;
                            }

                            Thread.Sleep(delay);
                        }
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return false;
                }
            }
        }

        public override bool ConfigOffset(int channel, double offset_VCC, double offset_ICC, int syn = 0)
        {
            lock (syncRoot)
            {
                this.voltageOffset = offset_VCC;
                this.currentOffset = offset_ICC;
                return true;
            }
        }

        protected bool ConfigVoltageCurrent(int channel, double voltage, double current)
        {
            lock (syncRoot)
            {
                string command = "";//= "APPL P6V," + str_V + "," + Str_I;
                string command_channel = "";
                voltage += voltageOffset;

                if (channel == 1)
                {
                    command = "APPL P6V," + voltage + "," + current;
                    command_channel = "APPL P6V";
                }
                else if (channel == 2)
                {
                    command = "APPL P25V," + voltage + "," + current;
                    command_channel = "APPL P25V";
                }

                try
                {
                    Log.SaveLogToTxt("E3631 channel is " + command_channel + " voltage is " + voltage.ToString("f3") + " current is" + current);
                    return this.WriteString(command);
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return false;
                }
            }
        }

        public override bool ConfigVoltageCurrent(double voltage, int syn = 0)
        {
            lock (syncRoot)
            {
                double volandoffset = voltage + voltageOffset;
                bool flag = false;
                string command = "";//= "APPL P6V," + str_V + "," + Str_I;
                string command_channel = "";
                if (Convert.ToInt16(channel_DUT) == 1)
                {
                    command = "APPL P6V," + volandoffset + "," + current_DUT;
                    command_channel = "APPL P6V";
                }
                else if (Convert.ToInt16(channel_DUT) == 2)
                {
                    command = "APPL P25V," + volandoffset + "," + current_DUT;
                    command_channel = "APPL P25V";
                }
                syn = 0;
                try
                {
                    if (syn == 0)
                    {
                        this.WriteString(command);
                        this.WriteString("*opc?");
                        string StrTemp = this.ReadString().Replace("\n", "");
                        if (StrTemp == "1")
                        {
                            flag = true;
                        }
                        else
                        {
                            flag = false;
                        }

                        // string Str = GetVoltage().ToString();
                        Log.SaveLogToTxt("E3631 channel is " + command_channel + " voltage is " + volandoffset.ToString("f3") + " current is " + current_DUT);
                        return flag;
                    }
                    else// 因为读取当前的电压值 会导致仪器脱离程控, 不适用同步模式
                    {
                        flag = false;

                        for (int i = 0; i < 3; i++)
                        {

                            flag = this.WriteString(command);
                            Thread.Sleep(500);
                            // MyIO.WriteString(str_channel + "?");
                            string StrVoltage = GetVoltage().ToString();
                            // double SetVoltage = vol + offset;
                            double MeasurtVoltage = Convert.ToDouble(StrVoltage);
                            if (MeasurtVoltage <= volandoffset * 1.005 && MeasurtVoltage >= volandoffset * 0.995)
                            {
                                flag = true;
                                Log.SaveLogToTxt("E3631 channel is " + command_channel + " voltage is " + volandoffset.ToString("f3") + " current is " + current_DUT);
                                break;
                            }
                        }
                        if (!flag)
                        {
                            Log.SaveLogToTxt("E3631 channel is " + command_channel + " voltage is " + volandoffset.ToString("f3") + " current is " + current_DUT + "error");
                        }
                        return flag;
                    }
                }
                catch
                {
                    Log.SaveLogToTxt("E3631 channel is " + command_channel + " voltage is " + volandoffset.ToString("f3") + " current is " + current_DUT + "error");
                    return false;
                }
            }
        }

        public override double GetCurrent()
        {
            lock (syncRoot)
            {
                try
                {
                    this.WriteString("MEAS:CURR? P6V");
                    double current = (Convert.ToDouble((this.ReadString(25)))) * 1000;
                    // 因为读取当前的电压值(电流)，会导致仪器脱离程控(一直在执行指令), 所以加延时等仪器恢复。                    
                    Thread.Sleep(10000);
                    return current - currentOffset;
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return Algorithm.MyNaN;
                }
            }
        }

        public override double GetVoltage()
        {
            lock (syncRoot)
            {
                try
                {  
                    this.WriteString("MEAS:VOLT? P6V");
                    double voltage = Convert.ToDouble((this.ReadString(10)));
                    // 因为读取当前的电压值(电流)，会导致仪器脱离程控(一直在执行指令), 所以加延时等仪器恢复。                    
                    Thread.Sleep(10000);
                    return voltage;
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return Algorithm.MyNaN;
                }
            }
        }
    }
}
