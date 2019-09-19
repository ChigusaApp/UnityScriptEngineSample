//  (C)2019 Chigusa
using Parsing;
using System.Collections.Generic;

namespace ScriptEngine
{
    /// <summary>
    /// カスタムコンパイラーのサンプル
    /// （システムコールか構造体をシステムレベルで使用する場合のみ）
    /// </summary>
    public class CustomCompiler : Compiler
    {
        /// <summary>
        /// 追加のシステムコール
        /// </summary>
        public enum CustomSystemCall
        {
            /// <summary>
            /// ランダム値の取得
            /// Random.value
            /// </summary>
            RandomValue = VirtualMachine.SystemCall.MaxCall,
            /// <summary>
            /// ランダム値の範囲取得
            /// Random.Range(float, float)
            /// </summary>
            RandomRangeFloat,
            /// <summary>
            /// ランダム値の範囲取得
            /// Random.Range(int, int)
            /// </summary>
            RandomRangeInt,
            /// <summary>
            /// テキストの置換
            /// string TextReplace(string)
            /// </summary>
            TextReplace,
            /// <summary>
            /// メッセージの取得
            /// string GetMessage(string)
            /// </summary>
            GetMessage,
            /// <summary>
            /// 音楽の再生
            /// Play(int targetType, float specifiedFadeOutTime, float specifiedFadeInTime)
            /// </summary>
            MusicPlay,
            /// <summary>
            /// 効果音の再生
            /// SePlay(int type, bool oneShotFlag, bool loopFlag, float delayTime)
            /// </summary>
            SEPlay,
            /// <summary>
            /// ダイアログの表示
            /// void Show(int buttonType, string captionId, string bodyId)
            /// </summary>
            PopupShowFromId,
            /// <summary>
            /// 最大数
            /// </summary>
            MaxCall,
        };

        /// <summary>
        /// コンパイルの実行
        /// </summary>
        /// <param name="text">スクリプト</param>
        /// <param name="headerFilesList">ヘッダーファイル名のリスト</param>
        /// <param name="data">格納先データ</param>
        /// <returns>結果</returns>
        public override bool Compile(string text, Dictionary<string, string> headerFilesList, VirtualMachine.Data data)
        {
            // システムコールの追加の設定
            AddSystemFunction((int)CustomSystemCall.RandomValue, Types.FLOAT, "float", "RandomValue");
            AddSystemFunction((int)CustomSystemCall.RandomRangeFloat, Types.FLOAT, "float", "RandomRange", new char[2] { 'f', 'f' });
            AddSystemFunction((int)CustomSystemCall.RandomRangeInt, Types.INTEGER, "int", "RandomRange", new char[2] { 'i', 'i' });
            AddSystemFunction((int)CustomSystemCall.TextReplace, Types.STRING, "string", "TextReplace", new char[1] { 's' });
            AddSystemFunction((int)CustomSystemCall.GetMessage, Types.STRING, "string", "GetMessage", new char[1] { 's' });
            AddSystemFunction((int)CustomSystemCall.MusicPlay, Types.VOID, "void", "MusicPlay", new char[3] { 'i', 'f', 'f' });
            AddSystemFunction((int)CustomSystemCall.SEPlay, Types.VOID, "void", "SEPlay", new char[4] { 'i', 'b', 'b', 'f' });
            AddSystemFunction((int)CustomSystemCall.PopupShowFromId, Types.VOID, "void", "PopupShow", new char[3] { 'i', 's', 's' });

            //  構造体テーブルセット
            Structures.Add(new StructureTable());
            //ValueTable tempValueTable = new ValueTable();
            //tempValueTable.Add(Types.FLOAT, "float", "x", 1);
            //tempValueTable.Add(Types.FLOAT, "float", "y", 1);
            //tempValueTable.Add(Types.FLOAT, "float", "z", 1);
            //tempValueTable.Add(Types.FLOAT, "float", "w", 1);
            //Structures.Last().Add("TipVector", tempValueTable);

            return base.Compile(text, headerFilesList, data);
        }
    }

}
