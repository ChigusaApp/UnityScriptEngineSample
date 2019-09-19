//  (C)2019 Chigusa
using Parsing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using VirtualMachine;

namespace ScriptEngine
{
    /// <summary>
    /// スクリプトのロード
    /// </summary>
    public static class Loader
    {

        #region Header and Source
        /// <summary>
        /// 
        /// </summary>
        public static readonly string UnityHelperHeader =
    @"
struct Vector2
{
	float	x;
	float	y;
}
struct Vector3
{
	float	x;
	float	y;
	float	z;
}
struct Vector4
{
	float	x;
	float	y;
	float	z;
	float	w;
}
struct Quaternion
{
	float	x;
	float	y;
	float	z;
	float	w;
}

//  Time.deltaTime（readonly）
extern float DeltaTime;
";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string UnityHelperSource =
    @"
#include ""UnityHelper.ckh""
";
        #endregion


        #region AutoCreate
#if DEBUG && UNITY_EDITOR
        /// <summary>
        /// 定位置のファイルをすべて作成
        /// </summary>
        public static void AutoCreate<TCompiler, TCpu>(string targetFolder, bool forceFlag) where TCompiler : Compiler, new() where TCpu : Cpu, new()
        {
            var commonOutputName = Path.Combine(Application.dataPath, targetFolder, "Resources", "Cks", "CkScripts" + ".bytes");
            Data commonVmLinkData = new Data();
            if (!commonVmLinkData.FileLoad(commonOutputName))
                return;

            var tempFolder = System.IO.Path.Combine(Application.dataPath, targetFolder, "CkScripts");
            //var commonFilesList = Directory.GetFiles(tempFolder, "*.ck", SearchOption.TopDirectoryOnly);
            var folders = Directory.GetDirectories(tempFolder);
            foreach (var folderPath in folders)
            {
                var baseName = Path.GetFileName(folderPath);
                var outputName = Path.Combine(Application.dataPath, targetFolder, "Resources", "Cks", baseName + ".bytes");
                var objectFilesText = new Dictionary<string, string>();
                var filesList = new List<string>();
                //filesList.AddRange(commonFilesList);
                filesList.AddRange(Directory.GetFiles(folderPath, "*.ck", SearchOption.AllDirectories));

                //  日付のチェック
                if (!forceFlag && File.Exists(outputName))
                {
                    var outputDate = File.GetLastWriteTime(outputName);
                    bool alreadyCreated = true;
                    foreach (var filePath in filesList)
                    {
                        if (outputDate < File.GetLastWriteTime(filePath))
                        {
                            alreadyCreated = false;
                            break;
                        }
                    }
                    if (alreadyCreated)
                        continue;
                }


                var result = Loader.FromFilesToFile<TCompiler>(filesList, commonVmLinkData, outputName);
                if (result)
                {
                    Log(outputName + " を作成しました。");
                    try
                    {
                        var tempMachine = Loader.FromLinkedFile<TCpu>(outputName);
                        if (tempMachine != null)
                            Log("作成した " + baseName + " の読み込みに成功しました。");
                        else if (tempMachine == null)
                            LogError("作成した " + baseName + " の読み込みに失敗しました。");
                    }
                    catch
                    {
                        LogError("作成した " + baseName + " の読み込みに失敗しました。");
                    }
                }
                else
                {
                    LogError(baseName + " の出力に失敗しました。");
                }
            }
            UnityEditor.AssetDatabase.Refresh();
        }


        /// <summary>
        /// ベースファイルをすべて作成
        /// </summary>
        public static void AutoBaseCreate<TCompiler, TCpu>(string targetFolder, bool forceFlag) where TCompiler : Compiler, new() where TCpu : Cpu, new()
        {
            var tempFolder = Path.Combine(Application.dataPath, targetFolder, "CkScripts");
            var commonFilesList = Directory.GetFiles(tempFolder, "*.ck", SearchOption.TopDirectoryOnly);
            {
                var baseName = Path.GetFileName(tempFolder);
                var outputName = Path.Combine(Application.dataPath, targetFolder, "Resources", "Cks", baseName + ".bytes");
                var objectFilesText = new Dictionary<string, string>();
                var filesList = new List<string>();
                filesList.AddRange(commonFilesList);

                //  日付のチェック
                if (!forceFlag && File.Exists(outputName))
                {
                    var outputDate = File.GetLastWriteTime(outputName);
                    bool alreadyCreated = true;
                    foreach (var filePath in filesList)
                    {
                        if (outputDate < File.GetLastWriteTime(filePath))
                        {
                            alreadyCreated = false;
                            break;
                        }
                    }
                    if (alreadyCreated)
                        return;
                }


                var result = Loader.FromFilesToFile<TCompiler>(filesList, null, outputName);
                if (result)
                {
                    Log(outputName + " を作成しました。");
                    try
                    {
                        var tempMachine = Loader.FromLinkedFile<TCpu>(outputName);
                        if (tempMachine != null)
                            Log("作成した " + baseName + " の読み込みに成功しました。");
                        else if (tempMachine == null)
                            LogError("作成した " + baseName + " の読み込みに失敗しました。");
                    }
                    catch
                    {
                        LogError("作成した " + baseName + " の読み込みに失敗しました。");
                    }
                }
                else
                {
                    LogError(baseName + " の出力に失敗しました。");
                }
            }
        }


        /// <summary>
        /// ベースファイルのヘッダーをC#ソースのテキストに埋め込む
        /// </summary>
        public static void AutoBaseHeaderCreate(string targetFolder, bool forceFlag)
        {
            var tempFolder = Path.Combine(Application.dataPath, targetFolder, "CkScripts");
            var commonFilesList = Directory.GetFiles(tempFolder, "*.ckh", SearchOption.TopDirectoryOnly);
            var baseName = Path.GetFileName(tempFolder);
            var outputName = Path.Combine(Application.dataPath, targetFolder, "Resources", "Cks", baseName + "Header" + ".txt");

            //  日付のチェック
            if (!forceFlag && File.Exists(outputName))
            {
                var outputDate = File.GetLastWriteTime(outputName);
                bool alreadyCreated = true;
                foreach (var filePath in commonFilesList)
                {
                    if (outputDate < File.GetLastWriteTime(filePath))
                    {
                        alreadyCreated = false;
                        break;
                    }
                }
                if (alreadyCreated)
                    return;
            }
            var commonFilesDictionary = new Dictionary<string, string>();
            foreach (var commonFile in commonFilesList)
            {
                var commonText = File.ReadAllText(commonFile);
                commonFilesDictionary.Add(Path.GetFileName(commonFile), commonText);
            }
            //var serializedObject = Newtonsoft.Json.JsonConvert.SerializeObject(commonFilesDictionary, Newtonsoft.Json.Formatting.Indented);
            //{
            //    File.WriteAllText(outputName, serializedObject);
            //    Log(outputName + " を作成しました。");
            //}
        }
#endif
        #endregion



        #region Log
        /// <summary>
        /// デバッグ用のログの表示
        /// </summary>
        /// <param name="message">メッセージ</param>
        static void Log(string message)
        {
#if DEBUG
            Debug.Log(message);
#endif
        }
        /// <summary>
        /// デバッグ用の警告ログの表示
        /// </summary>
        /// <param name="message">メッセージ</param>
        static void LogWarning(string message)
        {
#if DEBUG
            Debug.LogWarning(message);
#endif
        }
        /// <summary>
        /// デバッグ用のエラーログの表示
        /// </summary>
        /// <param name="message">メッセージ</param>
        static void LogError(string message,
            [CallerFilePath] string file = "",
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string member = "")
        {
#if DEBUG
            Debug.LogError(message + " : " + file + " : " + line + " : " + member);
#endif
        }
        #endregion



        /// <summary>
        /// リソースから仮想CPUを取得
        /// </summary>
        /// <typeparam name="TCpu">仮想CPU</typeparam>
        /// <param name="assetName">アセット名</param>
        /// <returns>仮想CPU</returns>
        public static TCpu LoadFromResource<TCpu>(string assetName) where TCpu : Cpu, new()
        {
            //var headerFilesText = new Dictionary<string, string>
            //{
            ////    { "UnityHelper.ckh", Loader.UnityHelperHeader }
            //};
            //    var objectFilesText = new Dictionary<string, string>
            //{
            ////    { "UnityHelper.ck", Loader.UnityHelperSource },
            //    { "test.ck", scriptText }
            //};
            //
            ////Machine = Loader.FromMemories<CustomCpu, CustomCompiler>(objectFilesText, headerFilesText);
            //Machine = Loader.FromFiles<CustomCpu, CustomCompiler>(new List<string> { @"D:/Users/Chigusa/Source/Repos/Prototypes/Assets/Games/CK/C~/test.ck", });
            //if (Machine == null)
            //    LogError("コンパイルエラー");
            //
            //if (!Loader.FromFilesToFile<CustomCompiler>(new List<string> { @"D:/Users/Chigusa/Source/Repos/Prototypes/Assets/Games/CK/C~/test.ck", }, @"D:/Users/Chigusa/Source/Repos/Prototypes/Assets/Games/CK/Resources/CK/test.bytes"))
            //    LogError("コンパイルエラー");
            //
            //Machine = Loader.FromLinkedFile<CustomCpu>(@"D:/Users/Chigusa/Source/Repos/Prototypes/Assets/Games/CK/Resources/CK/test.bytes");
            //if (Machine == null)
            //    LogError("コンパイルエラー");
            ////Loader.FileToFile<CustomCpu, CustomCompiler>(objectFilesText, headerFilesText);

            if (string.IsNullOrWhiteSpace(assetName))
                return null;

            TextAsset bindata = Resources.Load("Cks/" + assetName) as TextAsset;
            if (bindata != null)
            {
                Log("リソースから読み込み");
                var machine = Loader.FromLinkedBytes<TCpu>(bindata.bytes);
                if (machine == null)
                    LogError("コンパイルエラー");
                else
                    Log("Machine作成");
                return machine;
            }
            return null;
        }



        /// <summary>
        /// UnityHelperSourceからコンパイル済みデータを取得
        /// </summary>
        /// <typeparam name="TCompiler">仮想コンパイラー</typeparam>
        /// <returns>コンパイル済みデータ</returns>
        public static Data CreateUnityHelperData<TCompiler>() where TCompiler : Compiler, new()
        {
            Data vmData = new Data();
            var compiler = new TCompiler();
            bool compileResut = compiler.Compile(UnityHelperSource, new Dictionary<string, string> { { "UnityHelper.ckh", UnityHelperHeader } }, vmData);
            if (compileResut)
                Log(compiler.GetDebugDump());
            return vmData;
        }



        /// <summary>
        /// メモリー内のスクリプトからコンパイル済みデータを取得
        /// </summary>
        /// <typeparam name="TCompiler">仮想コンパイラー</typeparam>
        /// <param name="objectFilesText">スクリプトファイル</param>
        /// <param name="headerFilesList">ヘッダーファイル</param>
        /// <returns>コンパイル済みデータ</returns>
        public static List<Data> FromMemories<TCompiler>(Dictionary<string, string> objectFilesText, Dictionary<string, string> headerFilesList) where TCompiler : Compiler, new()
        {
            if (objectFilesText.Count <= 0)
                return null;


            //  コンパイル
            var vmDataList = new List<Data>();
            foreach (var objectFileText in objectFilesText)
            {
                Data vmData = new Data();
                var compiler = new TCompiler();
                bool compileResut = compiler.Compile(objectFileText.Value, headerFilesList, vmData);
                if (compileResut)
                {
                    Log("コンパイルに成功\n" + objectFileText.Key);
                    Log(compiler.GetDebugDump());
                    vmDataList.Add(vmData);
                }
                else
                {
                    LogWarning("コンパイルに失敗\n" + objectFileText.Key);
                    foreach (var errorMessage in compiler.ErrorMessageList)
                    {
                        LogWarning("コンパイル結果：" + errorMessage);
                    }
                    return null;
                }
            }
            return vmDataList;
        }



        /// <summary>
        /// スクリプトファイルからコンパイル済みデータを取得
        /// </summary>
        /// <typeparam name="TCompiler">仮想コンパイラー</typeparam>
        /// <param name="filesName">スクリプトのファイル名リスト</param>
        /// <returns>コンパイル済みデータ</returns>
        public static List<Data> FromFiles<TCompiler>(List<string> filesName) where TCompiler : Compiler, new()
        {
            //  引数確認
            if (filesName.Count <= 0)
                return null;

            //  コンパイル
            var vmDataList = new List<Data>();
            foreach (string fileName in filesName)
            {
                Data vmData = new Data();
                var compiler = new TCompiler();
                bool compileResut = compiler.Compile(fileName, vmData);
                if (compileResut)
                {
                    Log("コンパイルに成功\n" + fileName);
                    Log(compiler.GetDebugDump());
                    vmDataList.Add(vmData);
                }
                else
                {
                    LogWarning("コンパイルに失敗\n" + fileName);
                    foreach (var errorMessage in compiler.ErrorMessageList)
                    {
                        LogWarning("コンパイル結果：" + errorMessage);
                    }
                    return null;
                }
            }

            return vmDataList;
        }



        /// <summary>
        /// メモリー内のスクリプトから仮想CPUを取得
        /// </summary>
        /// <typeparam name="TCpu">仮想CPU</typeparam>
        /// <typeparam name="TCompiler">仮想コンパイラー</typeparam>
        /// <param name="objectFilesText">スクリプトファイル</param>
        /// <param name="headerFilesList">ヘッダーファイル</param>
        /// <returns>仮想CPU</returns>
        public static TCpu FromMemories<TCpu, TCompiler>(Dictionary<string, string> objectFilesText, Dictionary<string, string> headerFilesList) where TCpu : Cpu, new() where TCompiler : Compiler, new()
        {
            return FromMemoriesAndFiles<TCpu, TCompiler>(objectFilesText, headerFilesList, null);
        }



        /// <summary>
        /// スクリプトファイルから仮想CPUを取得
        /// </summary>
        /// <typeparam name="TCpu">仮想CPU</typeparam>
        /// <typeparam name="TCompiler">仮想コンパイラー</typeparam>
        /// <param name="filesName">スクリプトのファイル名リスト</param>
        /// <returns>仮想CPU</returns>
        public static TCpu FromFiles<TCpu, TCompiler>(List<string> filesName) where TCpu : Cpu, new() where TCompiler : Compiler, new()
        {
            return FromMemoriesAndFiles<TCpu, TCompiler>(null, null, filesName);
        }



        /// <summary>
        /// メモリー内のスクリプトとスクリプトファイルから仮想CPUを取得
        /// </summary>
        /// <typeparam name="TCpu">仮想CPU</typeparam>
        /// <typeparam name="TCompiler">仮想コンパイラー</typeparam>
        /// <param name="objectFilesText">スクリプトファイル</param>
        /// <param name="headerFilesList">ヘッダーファイル</param>
        /// <param name="filesName">スクリプトのファイル名リスト</param>
        /// <returns>仮想CPU</returns>
        public static TCpu FromMemoriesAndFiles<TCpu, TCompiler>(Dictionary<string, string> objectFilesText, Dictionary<string, string> headerFilesList, List<string> filesName) where TCpu : Cpu, new() where TCompiler : Compiler, new()
        {

            //  コンパイル
            List<Data> vmDataList = new List<Data>();
            List<Data> tempList = null;
            if (objectFilesText?.Count > 0 || headerFilesList != null)
            {
                tempList = FromMemories<TCompiler>(objectFilesText, headerFilesList);
                if (tempList == null)
                    return null;
                vmDataList.AddRange(tempList);
            }
            if (filesName?.Count > 0)
            {
                tempList = FromFiles<TCompiler>(filesName);
                if (tempList == null)
                    return null;
                vmDataList.AddRange(tempList);
            }
            //vmDataList.Add(CreateUnityHelperData<TCompiler>());


            //  リンク
            Linking.Linker linker = new Linking.Linker();
            var linkedData = linker.Execute(vmDataList);
            Log("リンク終了");
            foreach (var errorMessage in linker.ErrorMessageList)
            {
                LogWarning("リンク結果：" + errorMessage);
            }
            Log(linkedData.GetDebugDump());

            //  ロード
            var machine = (TCpu)typeof(TCpu).GetConstructor(new Type[] { typeof(Data) }).Invoke(new object[] { linkedData });

            return machine;
        }



        /// <summary>
        /// メモリー内のスクリプトとDataから仮想CPUを取得
        /// </summary>
        /// <typeparam name="TCpu">仮想CPU</typeparam>
        /// <typeparam name="TCompiler">仮想コンパイラー</typeparam>
        /// <param name="objectFilesText">スクリプトファイル</param>
        /// <param name="headerFilesList">ヘッダーファイル</param>
        /// <param name="datas">スクリプトのDataリスト</param>
        /// <returns>仮想CPU</returns>
        public static TCpu FromMemoriesAndDatas<TCpu, TCompiler>(Dictionary<string, string> objectFilesText, Dictionary<string, string> headerFilesList, List<Data> datas) where TCpu : Cpu, new() where TCompiler : Compiler, new()
        {

            //  コンパイル
            List<Data> vmDataList = new List<Data>();
            List<Data> tempList = null;
            if (objectFilesText?.Count > 0 || headerFilesList != null)
            {
                tempList = FromMemories<TCompiler>(objectFilesText, headerFilesList);
                if (tempList == null)
                    return null;
                vmDataList.AddRange(tempList);
            }
            if (datas?.Count > 0)
            {
                vmDataList.AddRange(datas);
            }
            //vmDataList.Add(CreateUnityHelperData<TCompiler>());


            //  リンク
            Linking.Linker linker = new Linking.Linker();
            var linkedData = linker.Execute(vmDataList);
            Log("リンク終了");
            foreach (var errorMessage in linker.ErrorMessageList)
            {
                LogWarning("リンク結果：" + errorMessage);
            }
            Log(linkedData.GetDebugDump());

            //  ロード
            var machine = (TCpu)typeof(TCpu).GetConstructor(new Type[] { typeof(Data) }).Invoke(new object[] { linkedData });

            return machine;
        }



        /// <summary>
        /// スクリプトファイルからリンク済みデータファイルを作成
        /// </summary>
        /// <typeparam name="TCompiler">仮想コンパイラー</typeparam>
        /// <param name="filesName">スクリプトのファイル名リスト</param>
        /// <param name="outputFileName">リンク済みデータファイル名</param>
        /// <returns>結果</returns>
        public static bool FromFilesToFile<TCompiler>(List<string> filesName, Data vmLinkData, string outputFileName) where TCompiler : Compiler, new()
        {
            return FromMemoriesAndFilesToFile<TCompiler>(null, null, filesName, vmLinkData, outputFileName);
        }



        /// <summary>
        /// メモリー内のスクリプトとスクリプトファイルからリンク済みデータファイルを作成
        /// </summary>
        /// <typeparam name="TCompiler">仮想コンパイラー</typeparam>
        /// <param name="objectFilesText">スクリプトファイル</param>
        /// <param name="headerFilesList">ヘッダーファイル</param>
        /// <param name="filesName">スクリプトのファイル名リスト</param>
        /// <param name="outputFileName">リンク済みデータファイル名</param>
        /// <returns>仮想CPU</returns>
        public static bool FromMemoriesAndFilesToFile<TCompiler>(Dictionary<string, string> objectFilesText, Dictionary<string, string> headerFilesList, List<string> filesName, Data vmLinkData, string outputFileName) where TCompiler : Compiler, new()
        {
            //  コンパイル
            List<Data> vmDataList = new List<Data>();
            if (vmLinkData != null)
                vmDataList.Add(vmLinkData);
            List<Data> tempList;
            if (objectFilesText?.Count > 0 || headerFilesList != null)
            {
                tempList = FromMemories<TCompiler>(objectFilesText, headerFilesList);
                if (tempList == null)
                    return false;
                vmDataList.AddRange(tempList);
            }
            if (filesName?.Count > 0)
            {
                tempList = FromFiles<TCompiler>(filesName);
                if (tempList == null)
                    return false;
                vmDataList.AddRange(tempList);
            }
            //vmDataList.Add(CreateUnityHelperData<TCompiler>());


            //  リンク
            Linking.Linker linker = new Linking.Linker();
            var linkedData = linker.Execute(vmDataList);
            Log("リンク終了");
            foreach (var errorMessage in linker.ErrorMessageList)
            {
                LogWarning("リンク結果：" + errorMessage);
            }
            Log(linkedData.GetDebugDump());
            return linkedData.FileSave(outputFileName);
        }



        /// <summary>
        /// スクリプトファイルから実行ファイルを作成
        /// </summary>
        /// <typeparam name="TCompiler">仮想コンパイラー</typeparam>
        /// <param name="filesName">スクリプトのファイル名リスト</param>
        /// <param name="outputFileName">出力する実行ファイル名</param>
        /// <returns>結果</returns>
        public static bool FileToFile<TCompiler>(List<string> filesName, string outputFileName) where TCompiler : Compiler, new()
        {
            //  引数確認
            if (filesName.Count <= 0 || string.IsNullOrEmpty(outputFileName))
                return false;

            //  コンパイル
            var objFilesName = new List<string>();
            foreach (string fileName in filesName)
            {
                Data vmData = new Data();
                var compiler = new TCompiler();
                bool compileResut = compiler.Compile(fileName, vmData);
                if (compileResut)
                {
                    Log("コンパイルに成功\n" + fileName);
                    Log(compiler.GetDebugDump());
                    vmData.FileSave(fileName + ".obj");
                    vmData.FileLoad(fileName + ".obj");
                    objFilesName.Add(fileName + ".obj");
                }
                else
                {
                    LogWarning("コンパイルに失敗\n" + fileName);
                    foreach (var errorMessage in compiler.ErrorMessageList)
                    {
                        LogWarning("コンパイル結果：" + errorMessage);
                    }
                    return false;
                }
            }


            //  リンク
            var linker = new Linking.Linker();
            var result = linker.Execute(objFilesName, outputFileName);
            Log("リンク終了\n" + outputFileName);
            foreach (var errorMessage in linker.ErrorMessageList)
            {
                LogWarning("リンク結果：" + errorMessage);
            }
            return result;
        }



        /// <summary>
        /// リンク済みデータから仮想CPUを取得
        /// </summary>
        /// <typeparam name="TCpu">仮想CPU</typeparam>
        /// <param name="bytes">バイト配列</param>
        /// <returns>仮想CPU</returns>
        public static TCpu FromLinkedBytes<TCpu>(byte[] bytes) where TCpu : Cpu, new()
        {
            //  ロード
            Data vmLinkData = new Data();
            if (!vmLinkData.BytesLoad(bytes))
                return null;
            var machine = (TCpu)typeof(TCpu).GetConstructor(new Type[] { typeof(Data) }).Invoke(new object[] { vmLinkData });

            return machine;
        }



        /// <summary>
        /// リンク済みデータファイルから仮想CPUを取得
        /// </summary>
        /// <typeparam name="TCpu">仮想CPU</typeparam>
        /// <param name="linkedFileName">実行ファイル名</param>
        /// <returns>仮想CPU</returns>
        public static TCpu FromLinkedFile<TCpu>(string linkedFileName) where TCpu : Cpu, new()
        {
            //  ロード
            Data vmLinkData = new Data();
            if (!vmLinkData.FileLoad(linkedFileName))
                return null;
            var machine = (TCpu)typeof(TCpu).GetConstructor(new Type[] { typeof(Data) }).Invoke(new object[] { vmLinkData });

            return machine;
        }

    }

}
