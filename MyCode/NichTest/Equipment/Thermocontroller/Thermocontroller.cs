using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NichTest
{
    public class Thermocontroller : Equipment
    {
        protected string airflow;
        protected string upperLimit;
        protected string lowerLimit;

        public virtual bool SetPointTemp(double Temp, int syn = 0) { return true; }
        public virtual string ReadCurrentTemp() { return "0"; }
    }
}
