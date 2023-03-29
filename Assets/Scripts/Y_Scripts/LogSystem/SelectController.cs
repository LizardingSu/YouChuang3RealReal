using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectContent
{
    public bool isInput;
    public string log;

    public uint nextIdx;

    public SelectContent(bool isInput, string log, uint nextIdx)
    {
        this.isInput = isInput;
        this.log = log;
        this.nextIdx = nextIdx;
    }
}

public class SelectController : MonoBehaviour
{
    // Start is called before the first frame update
    public diologueButtonController selectDioPrefab;
    public InputFieldController inputFieldPrefab;

    public bool isSelect = false;
    public bool isFirstTime = true;

    public void Init(LogEntry logEntry)
    {
        if (!isFirstTime) return;

        isSelect = true;
        isFirstTime = false;
        //先销毁所有子物体（用处在于可以避免重复生成）
        //if (transform.childCount != 0)
        //{
        //    foreach(Transform child in this.transform)
        //    {
        //        Destroy(child.gameObject);
        //    }
        //}

        var selectContents = LogEntryParser.GetSelectContents(logEntry.Log);

        for(int i=0;i<selectContents.Count;i++)
        {
            var item = selectContents[i];

            char p = (char)('A' + i);

            if (!item.isInput)
            {
                var db = Instantiate(selectDioPrefab, transform);
                db.Init(p+": "+item.log,logEntry.Idx,item.nextIdx);
            }
            else
            {
                var dbI = Instantiate(inputFieldPrefab, transform);
                dbI.Init(item.log, p + ": ", logEntry.Idx,item.nextIdx);

            }
        }
    }

}
