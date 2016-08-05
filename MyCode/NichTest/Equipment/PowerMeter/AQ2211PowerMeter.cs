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
    public class AQ2211PowerMeter : PowerMeter
    {
        private string[] slots;

        public override bool Initial(Dictionary<string, string> inPara, int syn = 0)
        {
            try
            {
                this.IOType = inPara["IOTYPE"];
                this.address = inPara["ADDR"];
                this.name = inPara["NAME"];
                this.reset = Convert.ToBoolean(inPara["RESET"]);
                this.role = Convert.ToInt32(inPara["ROLE"]);
                this.slots = inPara["SLOT"].Split(','); 
                this.channel = inPara["CHANNEL"];
                this.channelArray = this.channel.Split(',');
                this.unitType = inPara["UNITTYPE"];
                this.wavelength = inPara["WAVELENGTH"];

                this.isConnected = false;
                switch (IOType)
                {
                    case "GPIB":
                        myIO = new IOPort(IOType, "GPIB0::" + address);
                        myIO.IOConnect();
                        myIO.WriteString("*IDN?");
                        string content = myIO.ReadString();
                        this.isConnected = content.Contains("AQ22");
                        break;

                    default:
                        Log.SaveLogToTxt("GPIB port error.");
                        break;
                }
                return this.isConnected;
            }
            catch
            {
                Log.SaveLogToTxt("Failed to initial AQ2211 power meter.");
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

        public override bool Configure(int syn = 0)
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

                    for (int i = 0; i < this.slots.Length; i++)
                    {
                        //ch.Add(SlotList[i]);
                        this.slot = this.slots[i].ToString();
                        if (this.channelArray.Length == 1)
                        {
                            channel= channelArray[0];
                        }
                        else
                        {
                            channel = channelArray[i];
                        }

                        this.ConfigWavelength((i + 1).ToString(), syn);
                        this.SelectUnit(syn);
                    }
                    this.isConfigured = true;
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.ToString());
                return false;
            }
        }

        public override bool ConfigOffset(int channel, double offset, int syn = 0)
        {
            return true;
        }

        private bool ConfigWavelength(string dutcurrentchannel, int syn = 0)
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
                    Log.SaveLogToTxt("Power meter slot is " + this.slot + " wavelength is " + wavtemp[i] + "nm");
                    return myIO.WriteString(":SENSE" + this.slot + ":Channel" + this.channel + ":POWer" + ":WAVelength " + wavtemp[i] + "nm");
                }
                else
                {
                    for (int j = 0; j < 3; j++)
                    {
                        flag1 = myIO.WriteString(":SENSE" + this.slot + ":Channel" + this.channel + ":POWer" + ":WAVelength " + wavtemp[i] + "nm");
                        if (flag1 == true)
                            break;
                    }

                    if (flag1 == true)
                    {
                        for (k = 0; k < 3; k++)
                        {
                            myIO.WriteString(":SENSE" + this.slot + ":Channel" + this.channel + ":POWer" + ":WAVelength?");
                            readtemp = myIO.ReadString();
                            waveinput = Convert.ToDouble(wavtemp[i]);
                            waveoutput = Convert.ToDouble(readtemp) * Math.Pow(10, 9);
                            if (waveinput == waveoutput)
                            {
                                break;
                            }
                        }

                        if (k <= 3)
                        {
                            Log.SaveLogToTxt("Power meter slot is " + this.slot + " Channel " + this.channel + " wavelength " + wavtemp[i] + "nm");
                            flag = true;
                        }
                        else
                        {
                            Log.SaveLogToTxt("Power meter config wavelength wrong.");
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

        public override double ReadPower(int channel)
        {
            double power = 99;
            try
            {
                myIO.WriteString(":SENSE" + this.slots[channel - 1] + ":Channel" + this.channelArray[channel - 1] + ":POWer:REFerence:Display");
                myIO.WriteString(":SENSE" + this.slots[channel - 1] + ":Channel" + this.channelArray[channel - 1] + ":POWer:REFerence? TORef");
                string readtemp = myIO.ReadString();
                power = Convert.ToDouble(readtemp);
            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.ToString());
                return power;
            }
            return power;
        }

        public override bool SelectUnit(int syn = 0)//0 "dBm",1 "W"
        {
            bool flag = false;
            bool flag1 = false;
            int k = 0;
            string readtemp = "";
            try
            {
                myIO.WriteString(":SENSE" + this.slot + ":Channel" + this.channel + ":POWer:REFerence:state 0");
                if (syn == 0)
                {
                    Log.SaveLogToTxt("Power meter slot is " + this.slot + " Channel is " + this.channel + " Unit type is " + this.unitType);
                    return myIO.WriteString(":SENSE" + this.slot + ":Channel" + this.channel + ":POWer:UNIT " + this.unitType);
                }
                else
                {
                    for (int j = 0; j < 3; j++)
                    {
                        flag1 = myIO.WriteString(":SENSE" + this.slot + ":Channel" + this.channel + ":POWer:UNIT " + this.unitType);
                        if (flag1 == true)
                            break;
                    }

                    if (flag1 == true)
                    {
                        for (k = 0; k < 3; k++)
                        {
                            myIO.WriteString(":SENSE" + this.slot + ":Channel" + this.channel + ":POWer:UNIT?");
                            readtemp = myIO.ReadString();
                            if (readtemp == ("+" + this.unitType))
                            {
                                break;
                            }
                        }
                        if (k <= 3)
                        {
                            Log.SaveLogToTxt("Power meter slot is " + this.slot + " Channel is " + this.channel + " Unit type is " + this.unitType);
                            flag = true;
                        }
                        else
                        {
                            Log.SaveLogToTxt("Power meter select unit type failed");
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
}
