using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;

namespace NichTest
{
    public class Algorithm
    {
        /// <summary>
        /// using regular expressions to check format of serial number
        /// </summary>
        /// <param name="serialNumber">in serial number</param>
        /// <returns>true or false</returns>
        public static bool CheckSerialNumberFormat(string serialNumber)
        {
            Regex regex = new Regex(@"^(?![0-9]+$)(?![A-Z]+$)[0-9A-Z]{12}$", RegexOptions.IgnorePatternWhitespace);
            return regex.IsMatch(serialNumber);
            //分开来注释一下：
            //^ 匹配一行的开头位置
            //(?![0 - 9] +$) 预测该位置后面不全是数字
            //(?![A - Z] +$) 预测该位置后面不全是大写字母
            //[0 - 9A - Z] {12}
            //由12位数字或大写字母组成
            //$ 匹配行结尾位置

            //注：(? !xxxx) 是正则表达式的负向零宽断言一种形式，标识预该位置后不是xxxx字符。
        }

        public static byte[] ObjectToByteArray(object inputData, byte length, bool isLittleendian)
        {
            ArrayList array = new ArrayList();
            double value = Convert.ToDouble(inputData);
            switch (length)
            {
                case 0:
                    break;

                case 1:
                    byte temp_8bits = (byte)value;
                    array = ArrayList.Adapter(new byte[1] { temp_8bits });
                    break;

                case 2:
                    UInt16 temp_16bits = (UInt16)value;
                    array = ArrayList.Adapter(BitConverter.GetBytes(temp_16bits));                    
                    break;

                case 4:
                    UInt32 temp_32bits = (UInt32)value;
                    array = ArrayList.Adapter(BitConverter.GetBytes(temp_32bits));                    
                    break;

                default:
                    break;
            }
            
            if (isLittleendian == false)
            {
                array.Reverse();
            }
            return (byte[])array.ToArray(typeof(byte));
        }

        /// <summary>
        /// DAC是否需要进行位处理
        /// </summary>
        /// <param name="length">字节长度</param>
        /// <param name="StartBit">起始位</param>
        /// <param name="EndBit">结束位</param>
        /// <returns>True=处理;falas=不处理</returns>
        public static bool BitNeedManage(int length, int StartBit, int EndBit)
        {
            int Bitlength = EndBit - StartBit + 1;

            if (length * 8 > Bitlength)// 位数不够,拼凑而成
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// 对于要写入DAC的值进行位计算
        /// </summary>
        /// <param name="writeData">要写的数值</param>
        /// <param name="readData">已经从寄存器读出的数值</param>
        /// <param name="length">字节长度</param>
        /// <param name="startBit">起始位</param>
        /// <param name="endBit">结束位</param>
        /// <param name="type_MCU">Mcu类型,1:8bit,2:16bit</param>
        /// <returns></returns>
        public static int WriteJointBitValue(int writeData, int readData, int length, int startBit, int endBit, int type_MCU = 1)
        {
            int A = 0;
            for (int i = 0; i < length * 8 * type_MCU; i++)
            {
                if (i < startBit || i > endBit)//如果是在它的位置之外,那么全部写0
                {
                    A += Convert.ToInt32(Math.Pow(2, i));
                }
            }

            int b = readData & A;//吧要写的bit写0
            int c = b + writeData * Convert.ToInt32(Math.Pow(2, startBit));
            return c;
        }

        /// <summary>
        /// 对于读出DAC的值进行位计算
        /// </summary>
        /// <param name="readData">已经从寄存器读出的数值</param>
        /// <param name="length">字节长度</param>
        /// <param name="startBit">起始位</param>
        /// <param name="endBit">结束位</param>
        /// <param name="type_MCU">MCU类型,1:8bit,2:16bit</param>
        /// <returns></returns>
        public static int ReadJointBitValue(int readData, int length, int startBit, int endBit, int type_MCU = 1)
        {
            int A = 0;
            for (int i = 0; i < length * 8 * type_MCU; i++)
            {
                if (i >= startBit && i <= endBit)//如果是在它的位置之外,那么全部写0
                {
                    A += Convert.ToInt32(Math.Pow(2, i));
                }
            }
            int b = readData & A;//吧要写的bit写0
            int c = b / Convert.ToInt32(Math.Pow(2, startBit));
            return c;
        }
    }
}
