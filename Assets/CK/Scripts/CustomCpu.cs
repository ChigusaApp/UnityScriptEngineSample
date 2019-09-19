//  (C)2019 Chigusa
using UnityEngine;
using VirtualMachine;

namespace ScriptEngine
{
    /// <summary>
    /// カスタムCPUのサンプル
    /// （フリー関数とフリー変数）
    /// </summary>
    public partial class CustomCpu : Cpu
    {
        //[Newtonsoft.Json.JsonIgnore]
        public bool RoutinBreakRequest { get; set; }


        /// <summary>
        /// コンストラクター
        /// </summary>
        public CustomCpu() : base()
        {
        }

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="data"></param>
        public CustomCpu(Data data) : base(data)
        {
        }

        /// <summary>
        /// デュプリケート
        /// </summary>
        /// <returns>結果</returns>
        public override Cpu Duplicate()
        {
            return new CustomCpu(DataItem);
        }

        //public void SetData(CustomCpu target)
        //{
        //    DataItem = target.DataItem;
        //}
        //public override void SetData(Data data)
        //{
        //    DataItem = data;
        //}


        /// <summary>
        /// コルーチン用
        /// </summary>
        /// <param name="preAction">実行前やyield復帰後に呼び出すアクション</param>
        /// <param name="yieldAction">中断したときに呼び出すアクション</param>
        /// <param name="finishAction">終了したときに呼び出すアクション</param>
        /// <returns>IEnumerator</returns>
        public System.Collections.IEnumerator Routin(System.Action<CustomCpu> preAction = null, System.Action<CustomCpu> yieldAction = null, System.Action<CustomCpu> finishAction = null)
        {
            do
            {
                YieldFlag = false;
                RoutinBreakRequest = false;
                preAction?.Invoke(this);
                while (CommandIndex < DataItem.Commands.Count)
                {
#if DEBUG
                    try
                    {
                        OneCodeRun();
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError("例外発生：" + e.ToString());
                        yield break;
                    }
#else
                    OneCodeRun();
#endif
                    if (!ActiveFlag || YieldFlag)
                        break;
                }
                if (YieldFlag)
                    yieldAction?.Invoke(this);
                if (ActiveFlag && YieldFlag)
                    yield return 0;
                if (RoutinBreakRequest)
                    yield break;
            } while (ActiveFlag && YieldFlag);
            finishAction?.Invoke(this);
        }



        /// <summary>
        /// 戻り値の取得
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="number">配列の要素数（登録されていない型はサイズを指定）</param>
        /// <returns>結果の値</returns>
        public override T GetResult<T>(int number = 0)
        {
            return new CustomArgVariable(GetResult(CustomArgVariable.GetArgSize<T>(number)), CustomArgVariable.GetArgSize<T>(1)).Get<T>();
        }



        /// <summary>
        /// コードの実行
        /// </summary>
        public override void OneCodeRun()
        {
            Code op = (Code)DataItem.Commands[CommandIndex++];
            //Value rhs = new Value();
            //int argCount = 0;
            //int tempNumber = 0;
            int variableIndex;
            string tempName;
            //string tempText;
            //string tempText;
            //bool findFlag = false;
            //Value tempValue;

            switch (op)
            {
                default:
                    CommandIndex--;
                    base.OneCodeRun();
                    return;


                //case Code.FreeCall:
                //    variableIndex = GetIntValue();
                //    tempName = DataItem.FreeFunctionsName[variableIndex];       //  フリー関数名
                //    argCount = Pop().GetInteger();                              //  argCount
                //
                //
                //    switch (tempName)
                //    {
                //        case "void FreeFunc(string)":
                //            findFlag = true;
                //            Console.WriteLine("FreeFunc : {0}", Pop().SVal);
                //            break;
                //        case "int FreeIntToStr5()":
                //            findFlag = true;
                //            Console.WriteLine("FreeIntToStr : {0}", 322);
                //            Push(322);
                //            break;
                //        case "int FreeIntToStr3(string)":
                //            findFlag = true;
                //            tempText = Pop().SVal;
                //            Console.WriteLine("FreeIntToStr : {0}", tempText);
                //            Push(int.Parse(tempText));
                //            break;
                //        case "int FreeIntToStr3(string,string)":
                //            findFlag = true;
                //            tempText = Pop().SVal;
                //            tempText = Pop().SVal;
                //            Console.WriteLine("FreeIntToStr : {0}", tempText);
                //            Push(int.Parse(tempText));
                //            break;
                //        case "TipVector3 FreeIntToStr(string)":
                //            findFlag = true;
                //            tempText = Pop().SVal;
                //            Console.WriteLine("FreeIntToStr : {0}", tempText);
                //            Push(float.Parse(tempText));
                //            Push(float.Parse(tempText));
                //            Push(float.Parse(tempText));
                //            break;
                //        case "TipVector3 FreeIntToStr7(TipVector3)":
                //            findFlag = true;
                //            Pop();      //  .z
                //            Pop();      //  .y
                //            Console.WriteLine("FreeIntToStr : {0}", Pop().GetFloat());      //  .x
                //            Push(1.1f);
                //            Push(1.2f);
                //            Push(1.3f);
                //            break;
                //        default:
                //            Stacks.Remove(argCount);
                //            break;
                //    }
                //
                //    if (!findFlag)
                //    {
                //        //  ここでフリー関数
                //        //  クラスを継承してそこで実装のこと
                //        Console.WriteLine("フリー関数が実装されていません。");
                //
                //        if (tempName.Substring(0, 5) != "void ")
                //            Console.WriteLine("戻り値のあるフリー関数は、必ず実装してください。");
                //    }
                //
                //    break;


                case Code.PushFreeValue:
                    //  数値型変数をフリーへプッシュ
                    variableIndex = GetIntValue();
                    tempName = DataItem.VariablesName[variableIndex];
                    switch (tempName)
                    {
                        case "DeltaTime":
                            Push(Time.deltaTime);
                            break;
                        default:
                            CommandIndex--;
                            CommandIndex--;
                            base.OneCodeRun();
                            return;
                    }
                    break;

                    //case Code.PopFreeValue:
                    //    //  数値型変数をフリーからポップ
                    //    tempValue = Pop();
                    //    variableIndex = GetIntValue();
                    //    tempName = DataItem.VariablesName[variableIndex];
                    //    if (tempName == "freeInt")
                    //        freeInt = tempValue.GetInteger();
                    //    break;
                    //
                    //case Code.PushFreeArray:
                    //    //  数値型配列変数をフリーへプッシュ
                    //    rhs = Pop();
                    //    variableIndex = GetIntValue();
                    //    tempName = DataItem.VariablesName[variableIndex];
                    //    if (tempName == "freeFloatArray")
                    //        Push(freeFloatArray[rhs.Elements.i]);
                    //    else if (tempName == "freeVector3.z")
                    //        Push(freeVector3.Z);
                    //    else if (tempName == "freeVector3Array.x")
                    //    {
                    //        if (rhs.Elements.i % 3 == 2)
                    //            Push(freeVector3Array[rhs.Elements.i / 3].Z);
                    //    }
                    //    else
                    //        Push(0);
                    //    break;
                    //
                    //case Code.PopFreeArray:
                    //    //  数値型配列変数をフリーからポップ
                    //    rhs = Pop();
                    //    variableIndex = GetIntValue();
                    //    tempName = DataItem.VariablesName[variableIndex];
                    //    if (tempName == "freeFloatArray")
                    //        freeFloatArray[rhs.Elements.i] = Pop().GetFloat();
                    //    else if (tempName == "freeVector3.z")
                    //        freeVector3.Z = Pop().GetFloat();
                    //    else if (tempName == "freeVector3Array.x")
                    //    {
                    //        if (rhs.Elements.i % 3 == 2)
                    //            freeVector3Array[rhs.Elements.i / 3].Z = Pop().GetFloat();
                    //    }
                    //    else
                    //        Pop();
                    //    break;
                    //
                    //case Code.PushFreeValueFreeSize:
                    //    //  構造体変数をフリーへプッシュ
                    //    tempNumber = Pop().GetInteger();              //  サイズ
                    //    variableIndex = GetIntValue();
                    //    tempName = DataItem.VariablesName[variableIndex];
                    //    for (int index = 0; index < tempNumber; index++)
                    //    {
                    //        tempName = DataItem.VariablesName[variableIndex + index];
                    //        if (tempName == "freeVector3.x")
                    //            Push(freeVector3.X);
                    //        else if (tempName == "freeVector3.y")
                    //            Push(freeVector3.Y);
                    //        else if (tempName == "freeVector3.z")
                    //            Push(freeVector3.Z);
                    //        else
                    //            Push(0);
                    //    }
                    //    break;
                    //
                    //case Code.PopFreeValueFreeSize:
                    //    //  構造体変数をフリーからポップ
                    //    tempNumber = Pop().GetInteger();              //  サイズ
                    //    variableIndex = GetIntValue();
                    //    for (int index = tempNumber - 1; index >= 0; index--)
                    //    {
                    //        tempName = DataItem.VariablesName[variableIndex + index];
                    //        if (tempName == "freeVector3.x")
                    //            freeVector3.X = Pop().GetFloat();
                    //        else if (tempName == "freeVector3.y")
                    //            freeVector3.Y = Pop().GetFloat();
                    //        else if (tempName == "freeVector3.z")
                    //            freeVector3.Z = Pop().GetFloat();
                    //        else
                    //            Pop();
                    //    }
                    //    break;
                    //
                    //case Code.PushFreeArrayFreeSize:
                    //    //  構造体配列変数をフリーへプッシュ
                    //    tempNumber = Pop().GetInteger();              //  サイズ
                    //    rhs = Pop();
                    //    variableIndex = GetIntValue();
                    //    tempName = DataItem.VariablesName[variableIndex];
                    //    for (int index = 0; index < tempNumber; index++)
                    //    {
                    //        if (tempName == "freeVector3Array.x")
                    //        {
                    //            Push(freeVector3Array[index + rhs.Elements.i / 3].X);
                    //            Push(freeVector3Array[index + rhs.Elements.i / 3].Y);
                    //            Push(freeVector3Array[index + rhs.Elements.i / 3].Z);
                    //            index += 3 - 1;
                    //        }
                    //        else
                    //        {
                    //            Push(0);
                    //        }
                    //    }
                    //    break;
                    //
                    //case Code.PopFreeArrayFreeSize:
                    //    //  構造体配列変数をフリーからポップ
                    //    tempNumber = Pop().GetInteger();              //  サイズ
                    //    rhs = Pop();
                    //    variableIndex = GetIntValue();
                    //    tempName = DataItem.VariablesName[variableIndex];
                    //    for (int index = 0; index < tempNumber; index++)
                    //    {
                    //        if (tempName == "freeVector3Array.x")
                    //        {
                    //            freeVector3Array[index + rhs.Elements.i / 3].X = Pop().GetFloat();
                    //            freeVector3Array[index + rhs.Elements.i / 3].Y = Pop().GetFloat();
                    //            freeVector3Array[index + rhs.Elements.i / 3].Z = Pop().GetFloat();
                    //            index += 3 - 1;
                    //        }
                    //        else
                    //        {
                    //            Pop();
                    //        }
                    //    }
                    //    break;
                    //
                    //case Code.IncrementFreeValue:
                    //    //  数値型変数（フリー）をインクリメント
                    //    variableIndex = GetIntValue();
                    //    tempName = DataItem.VariablesName[variableIndex];
                    //    if (tempName == "freeInt")
                    //        freeInt++;
                    //    break;
                    //
                    //case Code.DecrementFreeValue:
                    //    //  数値型変数（フリー）をデクリメント
                    //    variableIndex = GetIntValue();
                    //    tempName = DataItem.VariablesName[variableIndex];
                    //    if (tempName == "freeInt")
                    //        freeInt--;
                    //    break;
                    //
                    //case Code.IncrementFreeArray:
                    //    //  数値型配列変数（フリー）をインクリメント
                    //    rhs = Pop();
                    //    variableIndex = GetIntValue();
                    //    tempName = DataItem.VariablesName[variableIndex];
                    //    if (tempName == "freeFloatArray")
                    //        freeFloatArray[rhs.Elements.i]++;
                    //    else if (tempName == "freeVector3.x")
                    //    {
                    //        tempName = DataItem.VariablesName[variableIndex + rhs.Elements.i];
                    //        if (tempName == "freeVector3.z")
                    //            freeVector3.Z++;
                    //    }
                    //    else if (tempName == "freeVector3Array.x")
                    //    {
                    //        if (rhs.Elements.i % 3 == 2)
                    //            freeVector3Array[rhs.Elements.i / 3].Z++;
                    //    }
                    //    break;
                    //
                    //case Code.DecrementFreeArray:
                    //    //  数値型配列変数（フリー）をデクリメント
                    //    rhs = Pop();
                    //    variableIndex = GetIntValue();
                    //    tempName = DataItem.VariablesName[variableIndex];
                    //    if (tempName == "freeFloatArray")
                    //        freeFloatArray[rhs.Elements.i]--;
                    //    else if (tempName == "freeVector3.x")
                    //    {
                    //        tempName = DataItem.VariablesName[variableIndex + rhs.Elements.i];
                    //        if (tempName == "freeVector3.z")
                    //            freeVector3.Z--;
                    //    }
                    //    else if (tempName == "freeVector3Array.x")
                    //    {
                    //        if (rhs.Elements.i % 3 == 2)
                    //            freeVector3Array[rhs.Elements.i / 3].Z++;
                    //    }
                    //    break;
            }

        }


    }

}
