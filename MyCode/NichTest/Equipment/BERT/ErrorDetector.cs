using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NichTest
{
    public class ErrorDetector : Equipment
    {
        protected string rate;
        protected byte totalChannels;
        protected byte prbsLength;//PRBS码型长度:31,7
        protected byte patternType;//ED码型选择（0=PRBS,1=Zero Subsitution,2=Data,3=Alternate,4=Mixed Data,5=Sequense）
        protected int gatingTime;

        public virtual double GetErrorRate(int syn = 0) { return 1; }
        public virtual double RapidErrorRate(int syn = 0) { return 1; }
        public virtual double[] RapidErrorRate_AllCH(int syn = 0) { return null; }
        public virtual double[] RapidErrorCount_AllCH(int syn = 0, bool IsClear = false) { return null; }
        public virtual bool AutoAlign(bool becenter) { return true; }
        public virtual double QureyEdErrorRatio() { return 1; }
        public virtual bool EdGatingStart() { return true; }
        public virtual bool ConfigureERxPolarity(bool polarity) { return true; }
    }
}
