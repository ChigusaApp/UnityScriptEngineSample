//  (C)2019 Chigusa
namespace ScriptEngine
{
    public class CustomStack<Type> : VirtualMachine.Stack<Type>
    {
        /// <summary>
        /// バッファー
        /// </summary>
        //[Newtonsoft.Json.JsonProperty]
        protected override Type[] Buffer { get; set; }

        /// <summary>
        /// スタック位置
        /// </summary>
        //[Newtonsoft.Json.JsonProperty]
        protected override int Top { get; set; } = 0;

        /// <summary>
        /// シリアライズデータ
        /// </summary>
        //[Newtonsoft.Json.JsonIgnore]
        public override SerializePack SerializeData { get; set; }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="max">スタック割り当てサイズ</param>
        public CustomStack(int max) : base(max)
        {
        }

    }
}
