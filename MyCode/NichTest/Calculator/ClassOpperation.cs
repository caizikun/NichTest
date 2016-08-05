using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NichTest
{
    interface IMathOpperation
    {
         double NumA { set; get; }

         double NumB { set; get; }

         double GetResult();
     }

    class AddOpperation :IMathOpperation
    {
        private double numA, numB;

        public double NumA
        {
            set 
            {
                this.numA = value;
            }
            get
            {
                return this.numA;
            }
        }

        public double NumB
        {
            set
            {
                this.numB = value;
            }
            get
            {
                return this.numB;
            }
        }

        public double GetResult()
        {
            return this.numA + this.numB;
        }
     }

        
    class SubtractionOpperation : IMathOpperation
    {
        private double numA, numB;

        public double NumA
        {
            set
            {
                this.numA = value;
            }
            get
            {
                return this.numA;
            }
        }

        public double NumB
        {
            set
            {
                this.numB = value;
            }
            get
            {
                return this.numB;
            }
        }

        public double GetResult()
        {
            return numA - numB;
        }
    
     }

     class MultiplyOpperation : IMathOpperation
     {
         private double numA, numB;

         public double NumA
         {
             set
             {
                 this.numA = value;
             }
             get
             {
                 return this.numA;
             }
         }

         public double NumB
         {
             set
             {
                 this.numB = value;
             }
             get
             {
                 return this.numB;
             }
         }

         public double GetResult()
         {
             return numA * numB;
         }
      }

     class DivideOpperation : IMathOpperation
     {
         private double numA, numB;

         public double NumA
         {
             set
             {
                 this.numA = value;
             }
             get
             {
                 return this.numA;
             }
         }

         public double NumB
         {
             set
             {
                 this.numB = value;                
             }
             get
             {
                 return this.numB;
             }
         }   

         public double GetResult()
         {
             if (numB == 0)
                 throw new IndexOutOfRangeException("除数不能为零");
             else
             return numA / numB;
         }
    
     }
     class AddandSubOpperation : IMathOpperation
     {
         private double numA, numB;

         public double NumA
         {
             set
             {
                 this.numA = 0;
             }
             get
             {
                 return this.numA;
             }
         }

         public double NumB
         {
             set
             {
                 this.numB = value;
             }
             get
             {
                 return this.numB;
             }
         }

         public double GetResult()
         {            
             return numA - numB;
         }

     }

     class LogOpperation : IMathOpperation
     {
         private double numA, numB;

         public double NumA
         {
             set
             {
                 this.numA = value;
             }
             get
             {
                 return this.numA;
             }
         }

         public double NumB
         {
             set
             {
                 this.numB = value;
             }
             get
             {
                 return this.numB;
             }
         }

         public double GetResult()
         {
             if (numA <= 0)
                 throw new IndexOutOfRangeException("底数不能为非正数");
             else if (numB <= 0)
                 throw new IndexOutOfRangeException("幂数不能为非正数");
             else
                 return Math.Log(numB, numA);
         }

     }

     class LnOpperation : IMathOpperation
     {
         private double numA, numB;

         public double NumA
         {
             set
             {
                 this.numA = value;
             }

             get
             {
                 return this.numA;
             }
         }

         public double NumB
         {
             set
             {
                 this.numB = value;
             }
             get
             {
                 return this.numB;
             }

         }
         public double GetResult()
         {
             if (numA <= 0)
                 throw new IndexOutOfRangeException("幂数不能为非正数");
             else
                 return Math.Log(numA, Math.E);
         }
     }

     class SqrtOpperation : IMathOpperation
     {
         private double numA;

         public double NumA
         {
             set
             {
                 this.numA = value;
             }
             get
             {
                 return this.numA;
             }
         }

         public double NumB
         {
             get;
             set;
         }
         public double GetResult()
         {
             if (numA < 0)
                 throw new IndexOutOfRangeException("底数不能为负");
             else
                 return Math.Sqrt(NumA);
         }
     }

     class PowerOpperation : IMathOpperation
     {
         private double numA, numB;

         public double NumA
         {
             set
             {
                 this.numA = value;
             }
             get
             {
                 return this.numA;
             }
         }

         public double NumB
         {
             set
             {
                 this.numB = value;
             }
             get
             {
                 return this.numB;
             }
         }

         public double GetResult()
         {
             return Math.Pow(numA, numB);
         }
     }

     interface IOpperationFactory
     {
        IMathOpperation CreationFactory(double numA, double numB,string operation);
     }

     class MyFactory : IOpperationFactory
     {
         public IMathOpperation CreationFactory(double numA, double numB, string operation)
         {
             IMathOpperation Coco = null;
             switch(operation)
             {
                  case "+":
                        Coco = new AddOpperation();
                        break;
                  case "-":
                        Coco = new SubtractionOpperation();
                        break;
                  case "*":
                        Coco = new MultiplyOpperation();                      
                        break;
                  case "/":
                        Coco = new DivideOpperation();
                        break;
                 //case "±":
                 //       Coco = new AddandSubOpperation();
                 //       break;
                  //case "^x":
                  //      Coco = new PowerOpperation();
                  //      break;
                  //case "sqrt":
                  //      Coco = new SqrtOpperation();
                  //      break;          
                  //case "log":
                  //      Coco = new LogOpperation();
                  //      break;                       
                  //case "ln":
                  //      Coco = new LnOpperation();
                  //      break;                        
             }
             Coco.NumA = numA;
             Coco.NumB = numB;
             return Coco;
         }
     }    

    }

   

    
 
