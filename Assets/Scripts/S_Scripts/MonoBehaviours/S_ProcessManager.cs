using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[ExecuteAlways]
public class S_ProcessManager : MonoBehaviour
{
    public S_CentralAccessor Accessor;

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    //存档文件
    public S_GameSaving m_Saving;

    public void Save(int id,int option,string answer)
    {
        Debug.Log(id + "  " + answer);

        bool exist = false;
        int existIndex = 0;

        //先根据游戏内数据修改m_Saving
        foreach (var str in m_Saving.Choices)
        {
            if (str.ID == id)
            {
                exist = true;
                break;
            }
            existIndex++;
        }

        if (exist)
        {
            if (m_Saving.Choices[existIndex].Answer!=""&&answer == "")
                answer = m_Saving.Choices[existIndex].Answer;

            m_Saving.Choices.RemoveAt(existIndex);
            m_Saving.Choices.Add(new S_ChoiceMade(id, option,answer));
        }
        else
        {
            m_Saving.Choices.Add(new S_ChoiceMade(id, option,answer));
        }
        WriteSaving();

        Accessor.StateManager.CalendarPanel.GetComponent<S_CalendarPanelManager>().InitAllDays();
        //Accessor.StateManager.CalendarPanel.GetComponent<S_CalendarPanelManager>().InitDayButtons();
    }

    public void Load()
    {
        if (ReadSaving())
        {

        }
        else
        {
            Debug.Log("存档读取失败，文件不存在");
        }
    }

    /// <summary>
    /// 暂时没有用武之地
    /// </summary>
    /// <param name="id"></param>
    public void LoadLog(int id = -1)
    {
        Debug.Log(Application.persistentDataPath + "/ApodaSaving/SavingFile.txt");

        if (ReadSaving())
        {
            int idx;
            int day;

            {
                if (id == -1)
                    id = m_Saving.Choices.Last().ID;

                idx = id % 1000;
                day = (id - idx) / 1000;
            }

            Accessor._DioLogueState.ReadToCurrentID(day, idx);
        }
        else
        {
            Debug.Log("存档读取失败，文件不存在");
            Accessor._DioLogueState.ReadToCurrentID(1, -1);
            return;
        }
    }

    //将m_Saving写入硬盘
    private void WriteSaving()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/ApodaSaving"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/ApodaSaving");
        }

        BinaryFormatter formatter = new BinaryFormatter();

        FileStream file = File.Create(Application.persistentDataPath + "/ApodaSaving/SavingFile.txt");

        var json = JsonUtility.ToJson(m_Saving);

        formatter.Serialize(file, json);

        file.Flush();
        file.Close();
        file.Dispose();

        Debug.Log("Save");
    }

    //读取硬盘中的存档并修改m_Saving
    private bool ReadSaving()
    {
        BinaryFormatter bf = new BinaryFormatter();

        if (File.Exists(Application.persistentDataPath + "/ApodaSaving/SavingFile.txt"))
        {
            Debug.Log("Load");

            FileStream file = File.Open(Application.persistentDataPath + "/ApodaSaving/SavingFile.txt", FileMode.Open);

            JsonUtility.FromJsonOverwrite((string)bf.Deserialize(file), m_Saving);

            file.Flush();
            file.Close();
            file.Dispose();

            return true;
        }
        else
        {
            m_Saving.Choices.Clear();
            return false;
        }
    }

    public void DeleteSaving()
    {
        if (File.Exists(Application.persistentDataPath + "/ApodaSaving/SavingFile.txt"))
        {
            File.Delete(Application.persistentDataPath + "/ApodaSaving/SavingFile.txt");
        }
        else
        {
            Debug.Log("存档删除失败，存档文件不存在");
        }
    }
}
