//  (C)2019 Chigusa
using ScriptEngine;
using System.Collections.Generic;
using UnityEngine;
using VirtualMachine;

public class CKTest : MonoBehaviour
{


    CustomCpu Machine { get; set; }



    public void OnDebugClick(GameObject target)
    {
        Machine.Initialize();

        Machine.FunctionCall(ArgVariable.CreateFunctionName("Vector3", "Move", "Vector3", "Vector3", "float"),
            CustomArgVariable.Create(new Vector3(0.0f, 0.0f, 0.0f)),
            CustomArgVariable.Create(new Vector3(10.0f, 1.0f, 1.0f)),
            CustomArgVariable.Create(1.0f));
        StartCoroutine(Machine.Routin((cpu) => PreAction(cpu), (cpu) => Move(cpu, target), (cpu) => Move(cpu, target)));
    }

    void PreAction(CustomCpu _)
    {

    }

    void Move(CustomCpu cpu, GameObject target)
    {
        var result = cpu.GetResult<Vector3>();
        target.transform.localPosition = result;
    }



    void Start()
    {
#if DEBUG && UNITY_EDITOR
        Loader.AutoCreate<CustomCompiler, CustomCpu>("CK", false);
#endif
        try
        {
            Machine = Loader.LoadFromResource<CustomCpu>("test");
        }
        catch
        {
            Debug.LogError("ロードエラー");
        }
        if (Machine == null)
            Debug.LogError("コンパイルエラー");
        else
            Debug.Log("Machine作成");

        Machine.Run();

    }


}
