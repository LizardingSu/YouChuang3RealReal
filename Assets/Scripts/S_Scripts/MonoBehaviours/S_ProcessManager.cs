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

    //�浵�ļ�
    public S_GameSaving m_Saving;

    //�����ļ�
    public S_Profile m_Profile;


    /// <summary>
    /// ������Ϸ�浵
    /// </summary>
    /// <param name="id"></param>
    /// <param name="option"></param>
    /// <param name="answer"></param>
    public void Save(int id, int option, string answer)
    {
        Debug.Log(id + "  " + answer);

        bool exist = false;
        int existIndex = 0;

        //�ȸ�����Ϸ�������޸�m_Saving
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
            if (m_Saving.Choices[existIndex].Answer != "" && answer == "")
                answer = m_Saving.Choices[existIndex].Answer;

            m_Saving.Choices.RemoveAt(existIndex);
            m_Saving.Choices.Add(new S_ChoiceMade(id, option, answer));
        }
        else
        {
            m_Saving.Choices.Add(new S_ChoiceMade(id, option, answer));
        }

        WriteFile(m_Saving, "SavingFile.txt");

        accessor.StateManager.CalendarPanel.GetComponent<S_CalendarPanelManager>().InitAllDays();
        //Accessor.StateManager.CalendarPanel.GetComponent<S_CalendarPanelManager>().InitDayButtons();
    }

    /// <summary>
    /// ��ȡ��Ϸ�浵
    /// </summary>
    public void Load()
    {
        if (ReadFile(m_Saving, "SavingFile.txt"))
        {

        }
        else
        {
            Debug.Log("�浵��ȡʧ�ܣ��ļ�������");
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

        WriteFile(m_Profile, "Profile.txt");
    }

    public void LoadProfile()
    {
        Slider BGMSlider = accessor.StateManager.SettingPanel.transform.GetChild(1).GetChild(0).GetComponent<Slider>();
        Slider SESlider = accessor.StateManager.SettingPanel.transform.GetChild(1).GetChild(1).GetComponent<Slider>();

        if (ReadFile(m_Profile, "Profile.txt"))
        {
            //Debug.Log("��ȡ�ɹ�");
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
            Debug.Log("�����ļ������ڣ�����������");
            accessor.AudioManager.SetBGMVolume(1f);
            accessor.AudioManager.SetSEVolume(1f);

            BGMSlider.value = 1f;
            SESlider.value = 1f;

            m_Profile.BGMVolume = 1f;
            m_Profile.SEVolume = 1f;

            WriteFile(m_Profile, "Profile.txt");
        }
    }

    /// <summary>
    /// ��ʱû������֮��
    /// </summary>
    /// <param name="id"></param>
    public void LoadLog(int id = -1)
        {
            Debug.Log(Application.persistentDataPath + "/ApodaSaving/SavingFile.txt");

            if (ReadFile(m_Saving, "SavingFile.txt"))
            {
                int idx;
                int day;

                {
                    if (id == -1)
                        id = m_Saving.Choices.Last().ID;

                    idx = id % 1000;
                    day = (id - idx) / 1000;
                }

                accessor._DioLogueState.ReadToCurrentID(day, idx);
            }
            else
            {
                Debug.Log("�浵��ȡʧ�ܣ��ļ�������");
                accessor._DioLogueState.ReadToCurrentID(1, -1);
                return;
            }
        }

    //��m_Savingд��Ӳ��
    private void WriteFile(ScriptableObject data, string fileName)
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

    //��ȡӲ���еĴ浵���޸�m_Saving
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

    //ɾ���浵�ļ�
    public void DeleteSaving()
    {
        if (File.Exists(Application.persistentDataPath + "/ApodaSaving/SavingFile.txt"))
        {
            File.Delete(Application.persistentDataPath + "/ApodaSaving/SavingFile.txt");
        }
        else
        {
            Debug.Log("�浵ɾ��ʧ�ܣ��浵�ļ�������");
        }
    }
}
