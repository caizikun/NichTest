using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Text.RegularExpressions;

namespace NichTest
{
    public partial class CalculatorForm : Form
    {
        private double inputData;
        private bool flag = false;
        private string opperator;
        Stack<object> myStack = new Stack<object>();
        ArrayList bArray = new ArrayList();
        StringBuilder temSting = new StringBuilder();
        
        public CalculatorForm()
        {
            InitializeComponent();
        }

        public void AddNum(int inputData)
        {
            if (flag)
            {
                textBox1.Text = "";
                flag = false;
            }

            this.textBox2.Text += inputData.ToString();
            this.textBox1.Text += inputData.ToString();

        }

        public void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                if (flag == false)
                {
                    int lens1 = this.textBox2.Text.Length;
                    int lens2 = this.textBox1.Text.Length;
                    string nwText = this.textBox2.Text.Substring(0, lens1 - lens2);
                    this.textBox2.Text = nwText;
                    this.textBox1.Text = "0";
                    flag = true;
                }
            }
            catch
            {
                return;
            }

        }
        
        private void btnAllClear_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = "";
            this.textBox2.Text = "";
        }

        private void btnSeventh_Click(object sender, EventArgs e)/* 对应数字键 7 */
        {
            this.AddNum(7);
        }

        private void btnFirst_Click(object sender, EventArgs e)/* 对应数字键 1 */
        {
            this.AddNum(1);
        }

        private void btnSecond_Click(object sender, EventArgs e)/* 对应数字键 2 */
        {
            this.AddNum(2);

        }

        private void btnThird_Click(object sender, EventArgs e)/* 对应数字键 3 */
        {
            this.AddNum(3);
        }

        private void btnFourth_Click(object sender, EventArgs e)/* 对应数字键 4 */
        {
            this.AddNum(4);
        }

        private void btnFifth_Click(object sender, EventArgs e)/* 对应数字键 5 */
        {
            this.AddNum(5);
        }

        private void btnSixth_Click(object sender, EventArgs e)/* 对应数字键 6 */
        {
            this.AddNum(6);
        }

        private void btnEigth_Click(object sender, EventArgs e)/* 对应数字键 8 */
        {
            this.AddNum(8);
        }

        private void btnNinth_Click(object sender, EventArgs e)/* 对应数字键 9 */
        {
            this.AddNum(9);
        }

        private void btnZero_Click(object sender, EventArgs e)
        {
            this.AddNum(0);
        }

        public void InputOpperator()
        {
            flag = true;
            MyStack<double> oppNums = new MyStack<double>();
            oppNums.Push(double.Parse(textBox1.Text));
        }

        public void opperClear()
        {
            int lens = this.textBox2.Text.Length;
            string nwText = this.textBox2.Text.Substring(0, lens - 1);
            this.textBox2.Text = nwText;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                InputOpperator();
                int lens = this.textBox2.Text.Length;
                string lastInput = this.textBox2.Text.Substring(lens-1,1);
                //opperator = "+";
                //opprators.Push("+");
                if (lastInput == "+" || lastInput == "-" || lastInput == "*" || lastInput == "/")
                {
                    opperClear();
                }
                this.textBox2.Text += "+";
            }
            catch 
            {
                return;
            }
        }

        private void btnSubtraction_Click(object sender, EventArgs e)
        {
            try
            {
                InputOpperator();
                int lens = this.textBox2.Text.Length;
                string lastInput = this.textBox2.Text.Substring(lens - 1, 1);
                if (lastInput == "+" || lastInput == "-" || lastInput == "*" || lastInput == "/")
                {
                    opperClear();
                }
                this.textBox2.Text += "-";
            }
            catch 
            {

                return;
            }
        }

        private void btnMultiplication_Click(object sender, EventArgs e)
        {
            try
            {
                InputOpperator();
                int lens = this.textBox2.Text.Length;
                string lastInput = this.textBox2.Text.Substring(lens - 1, 1);
                if (lastInput == "+" || lastInput == "-" || lastInput == "*" || lastInput == "/")
                {
                    opperClear();
                }
                this.textBox2.Text += "*";
            }
            catch 
            {

                return;
            }

        }

        private void btnDivide_Click(object sender, EventArgs e)
        {
            try
            {
                InputOpperator();
                int lens = this.textBox2.Text.Length;
                string lastInput = this.textBox2.Text.Substring(lens - 1, 1);
                if (lastInput == "+" || lastInput == "-" || lastInput == "*" || lastInput == "/")
                {
                    opperClear();
                }
                this.textBox2.Text += "/";
            }
            catch 
            {

                return;
            }

        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            try
            {             
                if (flag == false && this.textBox2.Text.Length ==0)
                {
                    opperator = "(";
                    this.textBox2.Text += opperator;
                    flag = true;
                }
                else if (flag == true)
                {
                    opperator = "(";
                    this.textBox2.Text += opperator;
                    flag = true;
                }
            }
            catch
            {
                return;
            }
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            try
            {
                InputOpperator();
                opperator = ")";
                this.textBox2.Text += opperator;
                flag = true;
                //if (flag == false)
                //{
                //    oppNums.Push(double.Parse(textBox1.Text));
                //}
                //int counts = opprators.Size();
                //for (int n = 0; n < counts; n++)
                //{
                //    opperator = opprators.Pop();
                //    num2 = oppNums.Pop();
                //    num1 = oppNums.Pop();
                //    IOpperationFactory fac = new MyFactory();
                //    IMathOpperation oper = fac.CreationFactory(num1, num2, opperator);
                //    results = oper.GetResult();
                //    this.textBox1.Text = results + "";
                //    oppNums.Push(double.Parse(textBox1.Text));
                //}
               
            }
            catch 
            {
                return;
            }
        }

        private void btnEqual_Click(object sender, EventArgs e)
        {
            try
            { 
                string expression = this.textBox2.Text.Trim();
                if (expression.Contains("."))
                {
                    ArrayList temArrlist = Parse(expression);
                    double temResult = Calculate(temArrlist);
                    this.textBox1.Text = temResult.ToString();
                    this.textBox2.Text = this.textBox1.Text;
                    return;
                }

                Regex regex = new Regex(@"
                            ^                               (?#匹配开头)
                            [-+]?                           (?#开头可以出现正负号)
                            ([0-9]+($|[-+*/]))*             (?#可选数-符号-数-符号-……-数-符号或结尾)
                            (
                                (
                                    (?<o>\()                (?#左括号，保存到o名字下)
                                    [-+]?                   (?#可选正负号)
                                    ([0-9]+[-+*/])*         (?#可选数-符-数-符……)
                                )+                          (?#可以重复出现左括号)
                                [0-9]+                      (?#左右括号之间最起码需要一个操作数)
                                (
                                    (?<-o>\))               (?#右括号，匹配的同时去掉一个左括号)
                                    ([-+*/][0-9]+)*         (?#可选符-数-符-数……)
                                )+                          (?#可以重复出现右括号，仅当还有左括号剩余)
                                ($|[-+*/])                  (?#要么结尾，要么在下一个左括号出现之前出现一个运算符)
                            )*                              (?#重复出现左括号)
                            (?(o)(?!))                      (?#如果还有左括号剩余就不匹配任何东西)
                            (?<=[0-9)])                     (?#检查结尾前是否数字或右括号)
                            $                               (?#匹配结尾)
                            ", RegexOptions.IgnorePatternWhitespace);

                bool result = regex.IsMatch(expression);
                if (result == true)
                {
                    ArrayList temArrlist = Parse(expression);
                    double temResult = Calculate(temArrlist);
                    this.textBox1.Text = temResult.ToString();
                    this.textBox2.Text = this.textBox1.Text;
                }
                else
                {
                    this.textBox2.Text = "";
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch 
            {
                return;
            }
        }

        public double Calculate(ArrayList bArray)//86+9*4-8 ,{86,9,4,*,+,8}{-}
        {           
                double no1, no2, ret;
                Stack myStack = new Stack();
                myStack.Clear();
                string ch;
                for (int i = 0; i < bArray.Count; i++)
                {
                    if (bArray[i] == null || Convert.ToString(bArray[i]) == "")
                    {
                        bArray.RemoveAt(i);
                    }
                }
                string[] aStr = new string[bArray.Count];
                for (int i = 0; i < bArray.Count; i++)
                {
                    aStr[i] = Convert.ToString(bArray[i]);
                    aStr[i] = aStr[i].Replace(" ", "");
                    aStr[i] = aStr[i].ToUpper();
                }
                myStack.Clear();

                for (int i = 0; i < aStr.Length; i++)//86+9*4-8 ,{86,9,4,*,+,8}{-}
                {
                    ch = aStr[i];
                    if (IsOperandStr(ch))//如果是操作数，直接压入栈 122,8
                    {
                        myStack.Push(ch);
                    }
                    else //如果是操作符，就弹出两个数字来进行运算
                    {
                        string str1 = Convert.ToString(myStack.Pop());
                        no1 = Convert.ToDouble(str1);
                        string str2 = Convert.ToString(myStack.Pop());
                        no2 = Convert.ToDouble(str2);

                        IOpperationFactory fac = new MyFactory();
                        IMathOpperation oper = fac.CreationFactory(no2, no1, ch);
                        ret = oper.GetResult();
                        myStack.Push(ret);//将结果压入栈
                    }
                }
           
            return Convert.ToDouble(myStack.Pop());//弹出最后的运算结果
        }

        public ArrayList Parse(string expression)
        {
            char ch, ch1;
            MyStack<object> myStack = new MyStack<object>();
            ArrayList bArray = new ArrayList();
            bArray.Clear();
            StringBuilder tempSring = new StringBuilder();
            tempSring.Clear();
            expression = expression.Trim();
            expression = expression.Replace(" ", "");
            char[] A = expression.ToCharArray(); //将字符串转成字符数组，要注意的是，不能有大于10的数存在

            UInt32 OperandCount = 0;
            for (int i = 0; i < A.Length; i++)
            {
                ch = A[i];
                if (i == A.Length - 1 && IsOperand(ch))//非符号
                {
                    OperandCount++;
                    tempSring.Append(A, (int)(i - OperandCount + 1), (int)OperandCount);
                    IsOperand(ch);
                    string temstr = Convert.ToString(tempSring);
                    bArray.Add(temstr);
                    tempSring.Clear();
                    OperandCount = 0;
                    continue;
                }

                if (i == 0 && ch == '-')
                {
                    bArray.Add(0);                                                  
                }
                if (IsOperand(ch) && i <= A.Length - 1) //如果是操作数，直接放入B中
                {
                    OperandCount++;
                    continue;
                }
                else
                {
                    if (ch == '(') //如果是'('，将它放入堆栈中
                        myStack.Push(ch);
                    else if (ch == ')') //如果是')'
                    {
                        tempSring.Append(A, (int)(i - OperandCount), (int)OperandCount);
                        string temstr = Convert.ToString(tempSring);
                        bArray.Add(temstr);
                        tempSring.Clear();
                        OperandCount = 0;
                        int counts = myStack.Size();
                        while (counts != 0) //不停地弹出堆栈中的内容，直到遇到'('
                        {
                            ch = (char)myStack.Pop();
                            if (ch == '(')
                                break;
                            else
                                bArray.Add(ch);
                        }
                    }
                    else //既不是'('，也不是')'，是其它操作符，比如+, -, *, /之类的 86+9*4-8 ,{86,9,4,*,+,8,-}
                    {
                        tempSring.Append(A, (int)(i - OperandCount), (int)OperandCount);
                        string temstr = Convert.ToString(tempSring);
                        bArray.Add(temstr);
                        tempSring.Clear();
                        OperandCount = 0;
                        if (!myStack.IsEmpty())//如果非空
                        {
                            do
                            {
                                ch1 = (char)myStack.Pop();//弹出栈顶元素
                                if (Priority(ch) > Priority(ch1)) //如果栈顶元素的优 先级小于读取到的操作符
                                {
                                    myStack.Push(ch1);//将栈顶元素放回堆栈
                                    myStack.Push(ch);//将读取到的操作符放回堆栈
                                    break;
                                }
                                else//如果栈顶元素的优先级比较高或者两者相等时
                                {
                                    bArray.Add(ch1);
                                    if (myStack.IsEmpty()) //如果堆栈为空
                                    {
                                        myStack.Push(ch); //将读取到的操作符压入堆栈中
                                        break;
                                    }
                                }
                            } while (!myStack.IsEmpty());//堆栈为非空
                        }
                        else //如果堆栈为空，就把操作符放入堆栈中
                        {
                            myStack.Push(ch);
                        }
                    }
                }
            }

            while (!myStack.IsEmpty())//堆栈非空
                bArray.Add(myStack.Pop());
            //for (int i = 0; i < bArray.Count; i++)
            //{
            //    Console.WriteLine(bArray[i].ToString());
            //}
            return bArray;
        }        
         //判断是否是操作数
        public bool IsOperand(char ch)
        {
            char[] operators = { '+', '-', '*', '/', '(', ')' };
            for (int i = 0; i < operators.Length; i++)
                if (ch == operators[i])
                    return false;
            return true;
        }

        public bool IsOperandStr(string ch)
        {
            string[] operators = { "+", "-", "*", "/", "(", ")" };
            for (int i = 0; i < operators.Length; i++)
                if (ch == operators[i])
                    return false;
            return true;
        }
        //返回运算符的优先级
        public int Priority(char ch)
        {
            int priority;

            switch (ch)
            {
                case '+':
                    priority = 1;
                    break;
                case '-':
                    priority = 1;
                    break;
                case '*':
                    priority = 2;
                    break;
                case '/':
                    priority = 2;
                    break;
                default:
                    priority = 0;
                    break;
            }

            return priority;
        }
       
        private void btnPoint_Click(object sender, EventArgs e)
        {
            try
            {
                if (flag == false)
                {
                    inputData = double.Parse(textBox1.Text);
                    int n = textBox1.Text.IndexOf(".");
                    if (n == -1)
                    {
                        this.textBox1.Text += ".";
                    }
                    this.textBox2.Text += ".";
                    flag = false;
                }

            }
            catch 
            {
                return;
            }

        }
    }
}

