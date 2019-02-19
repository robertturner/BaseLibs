using BaseLibs.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppTests
{
    class Program
    {
        class AClass
        {

            public string AClassProperty { get; set; }
            public int AValProperty { get; set; }
            //public StructVal AStructVal { get; set; }

            public string AMethod(int num, string str)
            {
                return str + " " + num;
            }
        }

        static void Main(string[] args)
        {

            var pi = typeof(AClass).GetProperty(nameof(AClass.AValProperty));
            var setter = pi.DelegateForSetProperty();

            var methodofSetter = setter.Method;

            var aMethDel = typeof(AClass).GetMethod(nameof(AClass.AMethod)).DelegateForMethod();
            var methodOfAMethDel = aMethDel.Method;

            var inst = new AClass();

            
            //var t = new MethodCallExpression()
            

        }

        
    }
}
