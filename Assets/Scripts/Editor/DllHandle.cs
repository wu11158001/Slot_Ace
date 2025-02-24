using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public class DllHandle : MonoBehaviour
{
    private static readonly IReadOnlyList<string> dllList = new List<string>()
    {
        "Assembly-CSharp.dll",
        "HotFix.dll",
    };

    private static readonly string _dllTargetFolderPath = $"Assets/HotFixAssets/DLL";
    private static readonly string _aotTargetFolderPath = $"Assets/HotFixAssets/AOT";

    private static readonly string _dllSourceFilePath = $"E:/MyUnityProject/Slot Ace File/Slot Ace/HybridCLRData/HotUpdateDlls/Android/";
    private static readonly string _aotSourceFilePath = $"E:/MyUnityProject/Slot Ace File/Slot Ace/HybridCLRData/AssembliesPostIl2CppStrip/Android/";

    [MenuItem("MyTool/DllHandle")]
    private static void DllHandleMethod()
    {
        Handle(_dllTargetFolderPath, _dllSourceFilePath, dllList);
        Handle(_aotTargetFolderPath, _aotSourceFilePath, AOTGenericReferences.PatchedAOTAssemblyList);
        Debug.Log("Dll複製完成。");
    }

    private static void Handle(string targetFolderPath, string sourceFilePath, IReadOnlyList<string> dllList)
    {
        // 清除舊有檔案
        string[] files = Directory.GetFiles(targetFolderPath, "*.bytes");
        foreach (string file in files)
        {
            File.Delete(file);
        }

        foreach (var dll in dllList)
        {
            string path = $"{sourceFilePath}{dll}";

            string targetFilePath = Path.Combine(targetFolderPath, dll);

            // 複製文件
            if (File.Exists(path))
            {
                File.Copy(path, targetFilePath, true);

                // 更改檔名
                if (File.Exists(targetFilePath))
                {
                    string newTargetFileName = $"{dll}.bytes";
                    string newTargetFilePath = Path.Combine(targetFolderPath, newTargetFileName);
                    File.Move(targetFilePath, newTargetFilePath);
                }
                else
                {
                    Debug.LogError($"{dll}:目標文件不存在，檔名更改失敗。");
                }
            }
            else
            {
                Debug.LogError($"{dll}源文件不存在，複製失敗。");
            }

            Debug.Log($"{dll} 已複製。");
        }
    }
}
