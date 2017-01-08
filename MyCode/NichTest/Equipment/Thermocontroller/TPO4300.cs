using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NichTest
{
    public class TPO4300 : Thermocontroller
    {
        private string sensor;
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
                this.airflow = inPara["FLSE"];
                this.upperLimit = inPara["ULIM"];
                this.lowerLimit = inPara["LLIM"];
                this.sensor = inPara["SENSOR"];

                this.isConnected = false;
                switch (IOType)
                {
                    case "GPIB":
                        lock (syncRoot)
                        {
                            this.WriteString("*IDN?");
                            string content = this.ReadString();
                            this.isConnected = content.Contains("TEMPTRONIC");
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
                Log.SaveLogToTxt("Failed to initial TPO4300 Thermocontroller.");
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

                this.WriteString("LGIN X-Stream");
                Thread.Sleep(500);
                this.WriteString("DTYP 1");
                this.WriteString("DUTC 70");
                AirFlowSetting();
                AirTempUpperlimit(syn);
                AirTemplowerlimit(syn);
                SensorType(syn);//no sensor 时，DUTControlModeOnOFF打不开，返回为0
                this.WriteString("WNDW 0.5");
                this.WriteString("TRKL 1");
                DUTControlModeOnOFF(1, syn);//nosensor 返回为0
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

        public override string ReadCurrentTemp()
        {
            lock (syncRoot)
            {
                try
                {
                    this.WriteString("TEMP?");
                    return this.ReadString(32);
                }
                catch
                {
                    return Algorithm.MyNaN.ToString();
                }
            }
        }

        public override bool SetPointTemp(double Temp, int syn = 0)
        {
            lock (syncRoot)
            {
                bool flag = false;
                bool flag1 = false;
                int k = 0;
                string readtemp = "";

                try
                {
                    string Str = string.Format("{0:N1}", Temp);
                    SetPositionUPDown("1");
                    if (Temp < 20)// Low
                    {
                        SetPoint("2");

                    }
                    else if (Temp > 30)//Hig
                    {
                        SetPoint("0");
                    }
                    else//AMB
                    {
                        SetPoint("1");
                    }


                    if (syn == 0)
                    {
                        Log.SaveLogToTxt("Temp is " + Temp);
                        return this.WriteString("SETP " + Str);
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            flag1 = this.WriteString("SETP " + Str);

                            if (flag1 == true)
                                break;
                        }
                        if (flag1 == true)
                        {
                            for (k = 0; k < 3; k++)
                            {
                                this.WriteString("SETP?");
                                readtemp = this.ReadString();
                                if (readtemp == Str + "\r\n")
                                    break;
                            }
                            if (k <= 3)
                            {
                                Log.SaveLogToTxt("Temp is " + Temp);
                                flag = true;
                            }
                            else
                            {
                                Log.SaveLogToTxt("set Temp wrong");
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

        private bool AirFlowSetting()
        {
            lock (syncRoot)
            {
                try
                {
                    bool flag = this.WriteString("FLSE " + this.airflow);
                    Log.SaveLogToTxt("AirFlowSet is " + this.airflow);
                    return flag;
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return false;
                }
            }
        }

        private bool AirTempUpperlimit(int syn = 0)
        {
            lock (syncRoot)
            {
                bool flag = false;
                bool flag1 = false;
                int k = 0;
                string readtemp = "";
                try
                {
                    if (syn == 0)
                    {
                        Log.SaveLogToTxt("TempsUpperlimit is" + upperLimit);
                        return this.WriteString("ULIM " + upperLimit);
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            flag1 = this.WriteString("ULIM " + upperLimit);
                            if (flag1 == true)
                                break;
                        }
                        if (flag1 == true)
                        {
                            for (k = 0; k < 3; k++)
                            {
                                this.WriteString("ULIM?");
                                readtemp = this.ReadString();
                                if (Convert.ToDouble(readtemp) == Convert.ToDouble(upperLimit))
                                    break;
                            }
                            if (k <= 3)
                            {
                                Log.SaveLogToTxt("TempsUpperlimit is" + upperLimit);

                                flag = true;
                            }
                            else
                            {
                                Log.SaveLogToTxt("set TempsUpperlimit wrong");
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

        private bool AirTemplowerlimit(int syn = 0)
        {
            lock (syncRoot)
            {
                bool flag = false;
                bool flag1 = false;
                int k = 0;
                string readtemp = "";
                try
                {
                    if (syn == 0)
                    {
                        Log.SaveLogToTxt("Tempslowerlimit is" + lowerLimit);
                        return this.WriteString("LLIM " + lowerLimit);
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            flag1 = this.WriteString("LLIM " + lowerLimit);
                            if (flag1 == true)
                                break;
                        }
                        if (flag1 == true)
                        {
                            for (k = 0; k < 3; k++)
                            {
                                this.WriteString("LLIM?");
                                readtemp = this.ReadString();
                                if (Convert.ToDouble(readtemp) == Convert.ToDouble(lowerLimit))
                                    break;
                            }
                            if (k <= 3)
                            {
                                Log.SaveLogToTxt("Tempslowerlimit is" + lowerLimit);
                                flag = true;
                            }
                            else
                            {
                                Log.SaveLogToTxt("set Tempslowerlimit wrong");
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

        private bool SensorType(int syn = 0)//0 No Sensor,1 T,2 k,3 rtd,4 diode 
        {
            lock (syncRoot)
            {
                bool flag = false;
                bool flag1 = false;
                int k = 0;
                string readtemp = "";
                try
                {
                    if (syn == 0)
                    {
                        Log.SaveLogToTxt("Sensor is" + sensor);
                        return this.WriteString("DSNS " + sensor);
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            flag1 = this.WriteString("DSNS " + sensor);
                            if (flag1 == true)
                                break;
                        }
                        if (flag1 == true)
                        {
                            for (k = 0; k < 3; k++)
                            {
                                this.WriteString("DSNS?");
                                readtemp = this.ReadString();
                                if (readtemp == sensor + "\r\n")
                                    break;
                            }
                            if (k <= 3)
                            {
                                Log.SaveLogToTxt("Sensor is" + sensor);
                                flag = true;
                            }
                            else
                            {
                                Log.SaveLogToTxt("set Sensor wrong");
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

        private bool DUTControlModeOnOFF(byte Switch, int syn = 0)//1 ON,0 OFF
        {
            lock (syncRoot)
            {
                bool flag = false;
                bool flag1 = false;
                int k = 0;
                string readtemp = "";

                try
                {
                    if (syn == 0)
                    {
                        Log.SaveLogToTxt("DUTControlModeOnOFF is" + Switch.ToString());
                        return this.WriteString("DUTM " + Switch.ToString());
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            flag1 = this.WriteString("DUTM " + Switch.ToString());
                            if (flag1 == true)
                                break;
                        }
                        if (flag1 == true)
                        {
                            for (k = 0; k < 3; k++)
                            {

                                this.WriteString("DUTM?");
                                readtemp = this.ReadString();
                                if (readtemp == Switch.ToString() + "\r\n")
                                    break;
                            }
                            if (k <= 3)
                            {
                                Log.SaveLogToTxt("DUTControlModeOnOFF is" + Switch.ToString());
                                flag = true;
                            }
                            else
                            {
                                Log.SaveLogToTxt("set DUTControlModeOnOFF wrong");
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

        private bool SetPositionUPDown(string position, int syn = 0)//0 UP,1 DOWN
        {
            lock (syncRoot)
            {
                bool flag = false;
                bool flag1 = false;
                int k = 0;
                string readtemp = "";
                try
                {
                    if (syn == 0)
                    {
                        Log.SaveLogToTxt("position is" + position);
                        return this.WriteString("HEAD " + position);
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            flag1 = this.WriteString("HEAD " + position);
                            if (flag1 == true)
                                break;
                        }
                        if (flag1 == true)
                        {
                            for (k = 0; k < 3; k++)
                            {
                                this.WriteString("HEAD?");
                                readtemp = this.ReadString();
                                if (readtemp == position + "\r\n")
                                    break;
                            }
                            if (k <= 3)
                            {
                                Log.SaveLogToTxt("position is" + position);
                                flag = true;
                            }
                            else
                            {
                                Log.SaveLogToTxt("set position wrong");
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

        private bool SetPoint(string Point, int syn = 0)//0 HOT,1 AMB,2 code
        {
            lock (syncRoot)
            {
                bool flag = false;
                bool flag1 = false;
                int k = 0;
                string readtemp = "";
                try
                {
                    if (syn == 0)
                    {
                        Log.SaveLogToTxt("point is " + Point);
                        return this.WriteString("SETN " + Point);
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            flag1 = this.WriteString("SETN " + Point);
                            this.WriteString("SOAK 15");
                            if (flag1 == true)
                                break;
                        }
                        if (flag1 == true)
                        {
                            for (k = 0; k < 3; k++)
                            {
                                this.WriteString("SETN?");
                                readtemp = this.ReadString();
                                if (readtemp == Point + "\r\n")
                                    break;
                            }
                            if (k <= 3)
                            {
                                Log.SaveLogToTxt("point is " + Point);
                                flag = true;
                            }
                            else
                            {
                                Log.SaveLogToTxt("set point wrong");
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
}
