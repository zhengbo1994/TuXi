using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSScriptLibrary;
using Mono.CSharp;

namespace InfoEarthFrame.Common
{
    public class CSScriptHelper
    {
        /// <summary>
        /// 运算表达式结果
        /// </summary>
        /// <param name="express">已赋值给参数的表达式</param>
        /// <returns></returns>
        public object Calutrue(string express)
        {
            try
            {
                string expression = @"using System;                                     public class Script                                     {                                         public object Calutrue()                                         {                                             return (" + express + ");                                         }                                     }";
                dynamic script = CSScript.Evaluator.LoadCode(expression);

                return script.Calutrue();
            }
            catch (Exception ex)
            {
                return "#";
            }
        }
    }
}
