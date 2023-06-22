using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class S_ProcessManager : MonoBehaviour
{
    public S_CentralAccessor accessor;

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    //大一轮存档文件
    public S_GameSaving m_Saving1;

    //配置文件
    public S_Profile m_Profile;

    //新手教程触发记录
    public S_GuiderSave m_GuiderSave;

    //大一轮存档名字
    [HideInInspector]
    public string m_SavingName1 = "SavingFile.txt";

    [HideInInspector]
    public string m_ProfileName = "Profile.txt";

    [HideInInspector]
    public string m_GuiderName = "GuiderSave.txt";


    /// <summary>
    /// 储存游戏存档
    /// </summary>
    /// <param name="id"></param>
    /// <param name="option"></param>
    /// <param name="answer"></param>
    public void Save(int id, int option, string answer)
    {
        Debug.Log(id + "  " + answer);

        //bool end = false;
        //int endIndex = 0;

        bool exist = false;
        int existIndex = 0;

        //先根据游戏内数据修改m_Saving
        foreach (var str in m_Saving1.Choices)
        {
            if (str.ID == id)
            {
                exist = true;
                break;
            }
            existIndex++;
        }

        //if(option != 1)
        //{
        //    //找到endIndex和是否有末尾
        //    for (int i = 0; i < m_Saving1.Choices.Count; i++)
        //    {
        //        var c = m_Saving1.Choices[i];

        //        if ((int)(id / 1000) == (int)(c.ID / 1000))
        //        {
        //            if (c.Choice == -2)
        //            {
        //                end = true;
        //                endIndex = i;
        //                break;
        //            }
        //        }
        //    }

        //    //如果有末尾的时候
        //    if (end)
        //    {
        //        if(id == m_Saving1.Choices[endIndex].ID)
        //        {
        //            //当当前id和末尾id一致时 进入exist中的情况
        //        }
        //        else if (option == -2||(exist && m_Saving1.Choices[existIndex].Choice != option))
        //        {
        //            m_Saving1.Choices.RemoveAt(endIndex);
        //            if (exist && existIndex > endIndex)
        //                existIndex--;
        //        }

        //    }
        //}

        if (exist)
        {
            if (m_Saving1.Choices[existIndex].Answer != "" && answer == "")
                answer = m_Saving1.Choices[existIndex].Answer;

            m_Saving1.Choices.RemoveAt(existIndex);
            m_Saving1.Choices.Add(new S_ChoiceMade(id, option, answer));
        }
        else
        {
            m_Saving1.Choices.Add(new S_ChoiceMade(id, option, answer));
        }

        WriteFile(m_Saving1, m_SavingName1);

        accessor.StateManager.CalendarPanel.GetComponent<S_CalendarPanelManager>().InitAllDays();
        //Accessor.StateManager.CalendarPanel.GetComponent<S_CalendarPanelManager>().InitDayButtons();
    }

    /// <summary>
    /// 读取游戏存档
    /// </summary>
    public void Load()
    {
        if (ReadFile(m_Saving1, m_SavingName1))
        {

        }
        else
        {
            Debug.Log("存档读取失败，文件不存在");
        }
    }

    public void SaveGuider()
    {
        m_GuiderSave.GuiderList.Clear();
        foreach(var item in accessor.GuiderManager.GuiderList)
        {
            m_GuiderSave.GuiderList.Add(item);
        }

        WriteFile(m_GuiderSave, m_GuiderName);
    }

    public void LoadGuider()
    {
        if (ReadFile(m_GuiderSave, m_GuiderName))
        {
            accessor.GuiderManager.GuiderList.Clear();
            foreach(var item in m_GuiderSave.GuiderList)
            {
                accessor.GuiderManager.GuiderList.Add(item);
            }
        }
        else
        {
            Debug.Log("未检测到教程记录，已重新生成");
            accessor.GuiderManager.GuiderList.Clear();
            m_GuiderSave.GuiderList.Clear();
            for (int i = 0; i < accessor.GuiderManager.GuiderCount; i++)
            {
                accessor.GuiderManager.GuiderList.Add(true);
                m_GuiderSave.GuiderList.Add(true);
            }

            WriteFile(m_GuiderSave, m_GuiderName);
        }
    }

    public void SaveProfile()
    {
        Debug.Log("profileSave");

        //Debug.Log(m_Profile.BGMVolume);
        //Debug.Log(m_Profile.SEVolume);

        //Slider BGMSlider = accessor.StateManager.SettingPanel.transform.GetChild(1).GetChild(0).GetComponent<Slider>();
        //Slider SESlider = accessor.StateManager.SettingPanel.transform.GetChild(1).GetChild(1).GetComponent<Slider>();

        m_Profile.BGMVolume = accessor.AudioManager.BGMPlayer.volume;
        m_Profile.SEVolume = accessor.AudioManager.SEPlayer.volume;

        //Debug.Log(m_Profile.BGMVolume);
        //Debug.Log(m_Profile.SEVolume);

        WriteFile(m_Profile, m_ProfileName);
    }

    public void LoadProfile()
    {
        Slider BGMSlider = accessor.StateManager.SettingPanel.transform.GetChild(1).GetChild(0).GetComponent<Slider>();
        Slider SESlider = accessor.StateManager.SettingPanel.transform.GetChild(1).GetChild(1).GetComponent<Slider>();

        if (ReadFile(m_Profile, m_ProfileName))
        {
            //Debug.Log("读取成功");
            //Debug.Log(accessor.ProcessManager.m_Profile.SEVolume);

            accessor.AudioManager.BGMPlayer.volume = m_Profile.BGMVolume;
            accessor.AudioManager.SEPlayer.volume = m_Profile.SEVolume;

            //Debug.Log(m_Profile.BGMVolume);
            //Debug.Log(m_Profile.SEVolume);

            BGMSlider.value = m_Profile.BGMVolume;

            //Debug.Log(m_Profile.BGMVolume);
            //Debug.Log(m_Profile.SEVolume);

            SESlider.value = m_Profile.SEVolume;

            //Debug.Log(m_Profile.BGMVolume);
            //Debug.Log(m_Profile.SEVolume);

            
            //Debug.Log(accessor.ProcessManager.m_Profile.SEVolume);
        }
        else
        {
            Debug.Log("配置文件不存在，已重新生成");
            accessor.AudioManager.SetBGMVolume(1f);
            accessor.AudioManager.SetSEVolume(1f);

            BGMSlider.value = 1f;
            SESlider.value = 1f;

            m_Profile.BGMVolume = 1f;
            m_Profile.SEVolume = 1f;

            WriteFile(m_Profile, m_ProfileName);
        }
    }

    /// <summary>
    /// 暂时没有用武之地
    /// </summary>
    /// <param name="id"></param>
    public void LoadLog(int id = -1)
        {
            Debug.Log(Application.persistentDataPath + "/ApodaSaving/SavingFile.txt");

            if (ReadFile(m_Saving1, m_SavingName1))
            {
                int idx;
                int day;

                {
                    if (id == -1)
                        id = m_Saving1.Choices.Last().ID;

                    idx = id % 1000;
                    day = (id - idx) / 1000;
                }

                accessor._DioLogueState.ReadToCurrentID(day, idx);
            }
            else
            {
                Debug.Log("存档读取失败，文件不存在");
                accessor._DioLogueState.ReadToCurrentID(1, -1);
                return;
            }
        }

    //将m_Saving写入硬盘
    public void WriteFile(ScriptableObject data, string fileName)
    {
        if (!Directory.Exists(Application.persistentDataPath + "/ApodaSaving"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/ApodaSaving");
        }

        BinaryFormatter formatter = new BinaryFormatter();

        FileStream file = File.Create(Application.persistentDataPath + "/ApodaSaving/" + fileName);

        var json = JsonUtility.ToJson(data);

        formatter.Serialize(file, json);

        file.Flush();
        file.Close();
        file.Dispose();

        Debug.Log("Save");
    }

    //读取硬盘中的存档并修改m_Saving
    private bool ReadFile(ScriptableObject data, string fileName)
    {
        BinaryFormatter bf = new BinaryFormatter();

        if (File.Exists(Application.persistentDataPath + "/ApodaSaving/" + fileName))
        {
            Debug.Log("Load");

            FileStream file = File.Open(Application.persistentDataPath + "/ApodaSaving/" + fileName, FileMode.Open);

            JsonUtility.FromJsonOverwrite((string)bf.Deserialize(file), data);

            file.Flush();
            file.Close();
            file.Dispose();

            return true;
        }
        else
        {
            if (data is S_GameSaving)
            {
                (data as S_GameSaving).Choices.Clear();
            }
            return false;
        }
    }

    //删除存档文件
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

    //通用删除文件
    public void DeleteFile(string FileName)
    {
        if (File.Exists(Application.persistentDataPath + "/ApodaSaving/" + FileName))
        {
            File.Delete(Application.persistentDataPath + "/ApodaSaving/" + FileName);
            Debug.Log("删除成功");
        }
        else
        {
            Debug.Log("删除" + FileName + "失败，文件不存在");
        }
    }
}
