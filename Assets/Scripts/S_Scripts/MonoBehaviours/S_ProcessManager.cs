using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

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

    //�浵�ļ�
    public S_GameSaving m_Saving;

    public void Save(int id,int option)
    {
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
            m_Saving.Choices.RemoveAt(existIndex);
            m_Saving.Choices.Add(new S_ChoiceMade(id, option));
        }
        else
        {
            m_Saving.Choices.Add(new S_ChoiceMade(id, option));
        }
        WriteSaving();
    }

    public void Load()
    {
        if (ReadSaving())
        {

        }
        else
        {
            Debug.Log("�浵��ȡʧ�ܣ��ļ�������");
        }
    }

    public void LoadLog(int id = -1)
    {
        if (!File.Exists(Application.persistentDataPath + "/ApodaSaving/SavingFile.txt"))
        {
            Accessor._DioLogueState.ReadToCurrentID(1, -1);
            return;
        }

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
            Debug.Log("�浵��ȡʧ�ܣ��ļ�������");
        }
    }



    //��m_Savingд��Ӳ��
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

    //��ȡӲ���еĴ浵���޸�m_Saving
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
            Debug.Log("�浵ɾ��ʧ�ܣ��浵�ļ�������");
        }    
    }
}
