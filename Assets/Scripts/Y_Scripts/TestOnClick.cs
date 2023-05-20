using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestOnClick : MonoBehaviour
{
    // Start is called before the first frame update
    public Button testButton;
    public TMP_Text tmp;

    public float speed = 5;

    private void Awake()
    {
        testButton.onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        testButton.onClick.RemoveListener(OnClick);
    }

    private void OnClick()
    {
        tmp.maxVisibleCharacters = 0;
        StartCoroutine(TypeWriter(tmp,speed));
    }

    IEnumerator TypeWriter(TMP_Text textComponent, float speed)
    {
        textComponent.ForceMeshUpdate();
        Debug.Log("Click");
        TMP_TextInfo textInfo = textComponent.textInfo;
        int total = textInfo.characterCount;
        bool complete = false;
        int current = 0;

        while (!complete)
        {
            if(current > total)
            {
                current = total;
                yield return new WaitForSeconds(1/speed);
                complete = true;
            }

            textComponent.maxVisibleCharacters = current;
            current += 1;

            yield return new WaitForSeconds(1 / speed);
        }

        yield return null;
    }

    IEnumerator PlayPrinterEffect(TMP_Text textComponent, float speed)
    {
        if (textComponent == null)
        {
            Debug.LogError("Input component is null!!!");
            yield break;
        }
        if (speed <= 0)
        {
            Debug.LogError("Input speed should be larger than 0!!!");
            speed = 1;
        }

        TMPro.TMP_TextInfo textInfo = textComponent.textInfo;

        Debug.Log(textInfo.characterCount);
        Debug.Log(textInfo.meshInfo.Length);

        // ��ʼ���Ŵ�ӡ��Ч�����������������������
        for (int i = 0; i < textInfo.materialCount; ++i)
        {
            textInfo.meshInfo[i].Clear();
        }
        textComponent.UpdateVertexData(TMPro.TMP_VertexDataUpdateFlags.Vertices);

        float timer = 0;
        float duration = 1 / speed;
        for (int i = 0; i < textInfo.characterCount; ++i)
        {
            while (timer < duration)
            {
                yield return null;
                timer += Time.deltaTime;
            }

            timer -= duration;

            TMPro.TMP_CharacterInfo characterInfo = textInfo.characterInfo[i];

            int materialIndex = characterInfo.materialReferenceIndex;
            int verticeIndex = characterInfo.vertexIndex;
            if (characterInfo.elementType == TMPro.TMP_TextElementType.Sprite)
            {
                verticeIndex = characterInfo.spriteIndex;
            }
            if (characterInfo.isVisible)
            {
                textInfo.meshInfo[materialIndex].vertices[0 + verticeIndex] = characterInfo.vertex_BL.position;
                textInfo.meshInfo[materialIndex].vertices[1 + verticeIndex] = characterInfo.vertex_TL.position;
                textInfo.meshInfo[materialIndex].vertices[2 + verticeIndex] = characterInfo.vertex_TR.position;
                textInfo.meshInfo[materialIndex].vertices[3 + verticeIndex] = characterInfo.vertex_BR.position;
                textComponent.UpdateVertexData(TMPro.TMP_VertexDataUpdateFlags.Vertices);
            }

        }
    }

}
