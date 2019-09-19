//  (C)2019 Chigusa
using UnityEngine;
using VirtualMachine;

namespace ScriptEngine
{
    public partial class CustomCpu : Cpu
    {

        #region Base Override
        /// <summary>
        /// システムコール
        /// プリント
        /// </summary>
        protected override void Sys_PrintString()
        {
            Debug.Log("out : " + Pop().SVal);
            Push(0);
        }

        /// <summary>
        /// システムコール
        /// プリント
        /// </summary>
        protected override void Sys_PrintInt()
        {
            Debug.Log("out : " + Pop().GetInteger());
            Push(0);
        }

        /// <summary>
        /// システムコール
        /// プリント
        /// </summary>
        protected override void Sys_PrintFloat()
        {
            Debug.Log("out : " + Pop().GetFloat());
            Push(0);
        }
        #endregion



        /// <summary>
        /// システムコール
        /// </summary>
        /// <param name="systemCall">システムコール</param>
        protected override void Sys_Calls(SystemCall systemCall)
        {
            //string tempText;
            switch ((CustomCompiler.CustomSystemCall)systemCall)
            {
                default:
                    //  Base
                    base.Sys_Calls(systemCall);
                    break;

                case CustomCompiler.CustomSystemCall.RandomValue:
                    //  ランダム値の取得
                    //  Random.value
                    Push(Random.value);
                    Push(1);
                    break;

                case CustomCompiler.CustomSystemCall.RandomRangeFloat:
                    //  ランダム値の範囲取得
                    //   Random.Range(float, float)
                    Push(Random.Range(Pop().GetFloat(), Pop().GetFloat()));
                    Push(1);
                    break;

                case CustomCompiler.CustomSystemCall.RandomRangeInt:
                    //  ランダム値の範囲取得
                    //   Random.Range(int, int)
                    Push(Random.Range(Pop().GetInteger(), Pop().GetInteger()));
                    Push(1);
                    break;
            }
        }


    }

}
