using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    public void Save()
    {
        //�ȸ�����Ϸ�������޸�m_Saving

        WriteSaving();
    }

    public void Load()
    {
        if (ReadSaving())
        {
            //Ȼ�����m_Saving�޸���Ϸ������
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
}
