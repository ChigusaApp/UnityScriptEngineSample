//  (C)2019 Chigusa
using System.Collections.Generic;
using UnityEngine;
using VirtualMachine;

namespace ScriptEngine
{
    /// <summary>
    /// 引数用変数データ
    /// </summary>
    public class CustomArgVariable : ArgVariable
    {

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="args">引数</param>
        public CustomArgVariable() : base()
        {
        }

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="args">引数</param>
        public CustomArgVariable(List<Value> values, int typeSize = 0) : base(values, typeSize)
        {
        }

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="args">引数</param>
        public CustomArgVariable(int typeSize, params Value[] args) : base(typeSize, args)
        {
        }

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="args">引数</param>
        public CustomArgVariable(bool popFlag, List<Value> values) : base(popFlag, values)
        {
        }

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="args">引数</param>
        public CustomArgVariable(bool popFlag, params Value[] args) : base(popFlag, args)
        {
        }

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="args">引数</param>
        public CustomArgVariable(params ArgVariable[] args) : base(args)
        {
        }



        /// <summary>
        /// 作成
        /// </summary>
        /// <param name="args">引数</param>
        public new static CustomArgVariable Create(params object[] args)
        {
            var result = new CustomArgVariable();
            //foreach (var arg in args)
            for (int index = args.Length - 1; index >= 0; index--)
            {
                var arg = args[index];
                result.Offsets.Add(result.Values.Count);
                if (arg is ArgVariable tempArgVariable)
                {
                    result.Sizes.Add(tempArgVariable.Values.Count);
                    result.Numbers.Add(tempArgVariable.Values.Count);
                    result.Values.AddRange(tempArgVariable.Values);
                }
                else if (arg is Vector2[] vector2s)
                {
                    result.Sizes.Add(vector2s.Length * 3);
                    result.Numbers.Add(vector2s.Length);
                    foreach (var vector2 in vector2s)
                    {
                        result.Values.Add(Value.Create(vector2.x));
                        result.Values.Add(Value.Create(vector2.y));
                    }
                }
                else if (arg is Vector2 vector2)
                {
                    result.Sizes.Add(2);
                    result.Numbers.Add(1);
                    result.Values.Add(Value.Create(vector2.x));
                    result.Values.Add(Value.Create(vector2.y));
                }
                else if (arg is Vector3[] vector3s)
                {
                    result.Sizes.Add(vector3s.Length * 3);
                    result.Numbers.Add(vector3s.Length);
                    foreach (var vector3 in vector3s)
                    {
                        result.Values.Add(Value.Create(vector3.x));
                        result.Values.Add(Value.Create(vector3.y));
                        result.Values.Add(Value.Create(vector3.z));
                    }
                }
                else if (arg is Vector3 vector3)
                {
                    result.Sizes.Add(3);
                    result.Numbers.Add(1);
                    result.Values.Add(Value.Create(vector3.x));
                    result.Values.Add(Value.Create(vector3.y));
                    result.Values.Add(Value.Create(vector3.z));
                }
                else if (arg is Vector4[] vector4s)
                {
                    result.Sizes.Add(vector4s.Length * 4);
                    result.Numbers.Add(vector4s.Length);
                    foreach (var vector4 in vector4s)
                    {
                        result.Values.Add(Value.Create(vector4.x));
                        result.Values.Add(Value.Create(vector4.y));
                        result.Values.Add(Value.Create(vector4.z));
                        result.Values.Add(Value.Create(vector4.w));
                    }
                }
                else if (arg is Vector4 vector4)
                {
                    result.Sizes.Add(4);
                    result.Numbers.Add(1);
                    result.Values.Add(Value.Create(vector4.x));
                    result.Values.Add(Value.Create(vector4.y));
                    result.Values.Add(Value.Create(vector4.z));
                    result.Values.Add(Value.Create(vector4.w));
                }
                else if (arg is Quaternion[] quaternions)
                {
                    result.Sizes.Add(quaternions.Length * 4);
                    result.Numbers.Add(quaternions.Length);
                    foreach (var quaternion in quaternions)
                    {
                        result.Values.Add(Value.Create(quaternion.x));
                        result.Values.Add(Value.Create(quaternion.y));
                        result.Values.Add(Value.Create(quaternion.z));
                        result.Values.Add(Value.Create(quaternion.w));
                    }
                }
                else if (arg is Quaternion quaternion)
                {
                    result.Sizes.Add(4);
                    result.Numbers.Add(1);
                    result.Values.Add(Value.Create(quaternion.x));
                    result.Values.Add(Value.Create(quaternion.y));
                    result.Values.Add(Value.Create(quaternion.z));
                    result.Values.Add(Value.Create(quaternion.w));
                }
                else if (arg is Color[] colors)
                {
                    result.Sizes.Add(colors.Length * 4);
                    result.Numbers.Add(colors.Length);
                    foreach (var color in colors)
                    {
                        result.Values.Add(Value.Create(color.r));
                        result.Values.Add(Value.Create(color.g));
                        result.Values.Add(Value.Create(color.b));
                        result.Values.Add(Value.Create(color.a));
                    }
                }
                else if (arg is Color color)
                {
                    result.Sizes.Add(4);
                    result.Numbers.Add(1);
                    result.Values.Add(Value.Create(color.r));
                    result.Values.Add(Value.Create(color.g));
                    result.Values.Add(Value.Create(color.b));
                    result.Values.Add(Value.Create(color.a));
                }
                else
                {
                    result.Sizes.Add(1);
                    result.Numbers.Add(1);
                    result.Values.Add(Value.Create(arg));
                }
            }
            return result;
        }


        /// <summary>
        /// サイズの取得
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="number"></param>
        /// <returns></returns>
        public new static int GetArgSize<T>(int number = 1)
        {
            if (typeof(T) == typeof(Vector2))
                return 2;
            else if (typeof(T) == typeof(Vector2[]))
                return 2 * number;
            else if (typeof(T) == typeof(Vector3))
                return 3;
            else if (typeof(T) == typeof(Vector3[]))
                return 3 * number;
            else if (typeof(T) == typeof(Vector4))
                return 4;
            else if (typeof(T) == typeof(Vector4[]))
                return 4 * number;
            else if (typeof(T) == typeof(Quaternion))
                return 4;
            else if (typeof(T) == typeof(Quaternion[]))
                return 4 * number;
            else if (typeof(T) == typeof(Color))
                return 4;
            else if (typeof(T) == typeof(Color[]))
                return 4 * number;
            return ArgVariable.GetArgSize<T>(number);
        }


        /// <summary>
        /// 取得
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="argIndex"></param>
        /// <param name="offsetIndex"></param>
        /// <returns>結果</returns>
        public override object ToCasted<T>(int argIndex = 0, int offsetIndex = 0)
        {
            if (Values == null || Offsets.Count <= argIndex)
                return default(T);

            var typeSize = GetArgSize<T>();
            if (typeof(T) == typeof(CustomArgVariable))
            {
                var result = new CustomArgVariable();
                var valueIndex = Offsets[argIndex];
                result.Offsets.Add(0);
                result.Sizes.Add(Sizes[argIndex]);
                result.Numbers.Add(Numbers[argIndex]);
                for (int index = 0; index < Numbers[argIndex]; index++)
                {
                    result.Values.Add(Values[valueIndex + index]);
                }
                return result;
            }
            else if (typeof(T) == typeof(Vector2[]))
            {
                var valueIndex = Offsets[argIndex];
                var result = new Vector2[Numbers[argIndex]];
                for (int index = 0; index < Numbers[argIndex]; index++)
                {
                    result[index] = new Vector2
                    {
                        x = Values[valueIndex + index * typeSize + 0].GetFloat(),
                        y = Values[valueIndex + index * typeSize + 1].GetFloat(),
                    };
                }
                return result;
            }
            else if (typeof(T) == typeof(Vector2))
            {
                var valueIndex = Offsets[argIndex] + offsetIndex * typeSize;
                return new Vector2
                {
                    x = Values[valueIndex + 0].GetFloat(),
                    y = Values[valueIndex + 1].GetFloat(),
                };
            }
            else if (typeof(T) == typeof(Vector3[]))
            {
                var valueIndex = Offsets[argIndex];
                var result = new Vector3[Numbers[argIndex]];
                for (int index = 0; index < Numbers[argIndex]; index++)
                {
                    result[index] = new Vector3
                    {
                        x = Values[valueIndex + index * typeSize + 0].GetFloat(),
                        y = Values[valueIndex + index * typeSize + 1].GetFloat(),
                        z = Values[valueIndex + index * typeSize + 2].GetFloat(),
                    };
                }
                return result;
            }
            else if (typeof(T) == typeof(Vector3))
            {
                var valueIndex = Offsets[argIndex] + offsetIndex * typeSize;
                return new Vector3
                {
                    x = Values[valueIndex + 0].GetFloat(),
                    y = Values[valueIndex + 1].GetFloat(),
                    z = Values[valueIndex + 2].GetFloat(),
                };
            }
            else if (typeof(T) == typeof(Vector4[]))
            {
                var valueIndex = Offsets[argIndex];
                var result = new Vector4[Numbers[argIndex]];
                for (int index = 0; index < Numbers[argIndex]; index++)
                {
                    result[index] = new Vector4
                    {
                        x = Values[valueIndex + index * typeSize + 0].GetFloat(),
                        y = Values[valueIndex + index * typeSize + 1].GetFloat(),
                        z = Values[valueIndex + index * typeSize + 2].GetFloat(),
                        w = Values[valueIndex + index * typeSize + 3].GetFloat(),
                    };
                }
                return result;
            }
            else if (typeof(T) == typeof(Vector4))
            {
                var valueIndex = Offsets[argIndex] + offsetIndex * typeSize;
                return new Vector4
                {
                    x = Values[valueIndex + 0].GetFloat(),
                    y = Values[valueIndex + 1].GetFloat(),
                    z = Values[valueIndex + 2].GetFloat(),
                    w = Values[valueIndex + 3].GetFloat(),
                };
            }
            else if (typeof(T) == typeof(Quaternion[]))
            {
                var valueIndex = Offsets[argIndex];
                var result = new Quaternion[Numbers[argIndex]];
                for (int index = 0; index < Numbers[argIndex]; index++)
                {
                    result[index] = new Quaternion
                    {
                        x = Values[valueIndex + index * typeSize + 0].GetFloat(),
                        y = Values[valueIndex + index * typeSize + 1].GetFloat(),
                        z = Values[valueIndex + index * typeSize + 2].GetFloat(),
                        w = Values[valueIndex + index * typeSize + 3].GetFloat(),
                    };
                }
                return result;
            }
            else if (typeof(T) == typeof(Quaternion))
            {
                var valueIndex = Offsets[argIndex] + offsetIndex * typeSize;
                return new Quaternion
                {
                    x = Values[valueIndex + 0].GetFloat(),
                    y = Values[valueIndex + 1].GetFloat(),
                    z = Values[valueIndex + 2].GetFloat(),
                    w = Values[valueIndex + 3].GetFloat(),
                };
            }
            else if (typeof(T) == typeof(Color[]))
            {
                var valueIndex = Offsets[argIndex];
                var result = new Color[Numbers[argIndex]];
                for (int index = 0; index < Numbers[argIndex]; index++)
                {
                    result[index] = new Color
                    {
                        r = Values[valueIndex + index * typeSize + 0].GetFloat(),
                        g = Values[valueIndex + index * typeSize + 1].GetFloat(),
                        b = Values[valueIndex + index * typeSize + 2].GetFloat(),
                        a = Values[valueIndex + index * typeSize + 3].GetFloat(),
                    };
                }
                return result;
            }
            else if (typeof(T) == typeof(Color))
            {
                var valueIndex = Offsets[argIndex] + offsetIndex * typeSize;
                return new Color
                {
                    r = Values[valueIndex + 0].GetFloat(),
                    g = Values[valueIndex + 1].GetFloat(),
                    b = Values[valueIndex + 2].GetFloat(),
                    a = Values[valueIndex + 3].GetFloat(),
                };
            }
            return base.ToCasted<T>(argIndex, offsetIndex);
        }


        /// <summary>
        /// 結果専用の取得
        /// （argIndexとoffsetIndexが0）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <returns>値</returns>
        public new static T Get<T>(List<Value> args)
        {
            return (T)(new CustomArgVariable(args).ToCasted<T>());
        }



        /// <summary>
        /// 値をすべて特定の型に変換する
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        public override void ConvertAll<T>()
        {
            var tempSize = GetArgSize<T>();
            var tempNumber = Values.Count / tempSize;
            Values = Values.GetRange(0, tempNumber * tempSize);
            Offsets.Clear();
            Numbers.Clear();
            Sizes.Clear();
            Offsets.Add(0);
            Numbers.Add(tempNumber);
            Sizes.Add(Values.Count);
        }

    }
}
