using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace NichTest
{
    public class ExceptionDictionary : Dictionary<int, string>
    {
        public enum Code : int
        {
            _0x02000 = 0x02000,
            _0x02001 = 0x02001,

            _0x02100 = 0x02100,
            _0x02101 = 0x02101,
            _0x02102 = 0x02102,
            _0x02103 = 0x02103,
            _0x02104 = 0x02104,
            _0x02105 = 0x02105,
            _0x02106 = 0x02106,
            _0x02107 = 0x02107,

            _0x02500 = 0x02500,
            _0x02501 = 0x02501,
            _0x02502 = 0x02502,
            _0x02503 = 0x02503,
            _0x02F00 = 0x02F00,

            _0x02F03 = 0x02F03,
            _0x02F04 = 0x02F04,
            _0x02F05 = 0x02F05,

            _0x02FFF = 0x02FFF,

            _0xFFFFF = 0xFFFFF,
        }

        public static string GetMessage(Code code)
        {
            Dictionary<Code, string> dic = new Dictionary<Code, string>();
            dic.Add(Code._0x02000, "nonstandard spec");
            dic.Add(Code._0x02001, "nonstandard input parameters");

            dic.Add(Code._0x02100, "Coeff not write in");
            dic.Add(Code._0x02101, "Failed to ACCouple");
            dic.Add(Code._0x02102, "Failed to collect curving parameters");
            dic.Add(Code._0x02103, "Failed to DCCouple");
            dic.Add(Code._0x02104, "Calculate step error");
            dic.Add(Code._0x02105, "Calculate regist error");
            dic.Add(Code._0x02106, "Adjust ERbyIbias error");
            dic.Add(Code._0x02107, "Adjust APbyMod error");

            dic.Add(Code._0x02500, "low optical power");
            dic.Add(Code._0x02501, "high optical power");
            dic.Add(Code._0x02502, "low ER");
            dic.Add(Code._0x02503, "hight ER");

            dic.Add(Code._0x02F00, "lack equipment");

            dic.Add(Code._0x02F03, "equipment is normal, but it is disfunction.");
            dic.Add(Code._0x02F04, "nonstandard output data");
            dic.Add(Code._0x02F05, "nonstandard process data");

            dic.Add(Code._0x02FFF, "can't save log");

            dic.Add(Code._0xFFFFF, "unknown");
            return dic[code];
        }
    }

    public class MyException : ApplicationException, ISerializable
    {
        public const ushort NaN = 9999;

        private ExceptionDictionary.Code id;
        private string trace;

        public ExceptionDictionary.Code ID
        {
            get
            {
                return this.id;
            }
        }

        public override string Message
        {
            get
            {
                return ExceptionDictionary.GetMessage(id);
            }
        }


        public override string StackTrace
        {
            get
            {
                return this.trace;
            }
        }

        public MyException(ExceptionDictionary.Code code)
        {
            id = code;
        }

        public MyException(ExceptionDictionary.Code code, string stackTrace)
        {
            id = code;
            trace = stackTrace;
        }
    }
}
