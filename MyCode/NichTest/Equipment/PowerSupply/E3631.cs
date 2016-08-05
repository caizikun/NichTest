using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NichTest
{
    class E3631 : PowerSupply
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
                        myIO = new IOPort(IOType, "GPIB0::" + address);
                        myIO.IOConnect();
                        myIO.WriteString("*IDN?");
                        string content = myIO.ReadString();
                        this.isConnected = content.Contains("E3631");
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

        public bool Reset()
        {
            if (myIO.WriteString("*RST"))
            {
                Thread.Sleep(3000);
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool OutPutSwitch(bool isON, int syn = 0)
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
                    result = myIO.WriteString("OUTP " + command);
                    Thread.Sleep(delay);
                    return result;
                }
                else
                {
                    for (int i = 0; i < 3; i++)
                    {
                        result = myIO.WriteString("OUTP " + command);
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
                            myIO.WriteString("OUTP?");
                            string readtemp = myIO.ReadString();
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

        public override bool ConfigOffset(int channel, double offset, int syn = 0)
        {
            this.voltageOffset = offset;
            return true;
        }

        protected bool ConfigVoltageCurrent(int channel, double voltage, double current)
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
                return myIO.WriteString(command);                
            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.ToString());
                return false;
            }
        }

        public override bool ConfigVoltageCurrent(double voltage, int syn = 0)
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
                    myIO.WriteString(command);
                    myIO.WriteString("*opc?");
                    string StrTemp = myIO.ReadString().Replace("\n", "");
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

                        flag = myIO.WriteString(command);
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

        public override double GetCurrent()
        {
            double current = 0;
            try
            {
                myIO.WriteString("MEAS:CURR? P6V");
                current = (Convert.ToDouble((myIO.ReadString(25)))) * 1000;
                return current;
            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.ToString());
                return current;
            }
        }

        public override double GetVoltage()
        {
            double voltage = 0;
            try
            {
                myIO.WriteString("MEAS:VOLT? P6V");
                voltage = Convert.ToDouble((myIO.ReadString(10)));
                return voltage;
            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.ToString());
                return voltage;
            }
        }
    }
}
