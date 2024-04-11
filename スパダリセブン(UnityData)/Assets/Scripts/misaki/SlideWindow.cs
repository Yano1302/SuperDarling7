using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideWindow : MonoBehaviour
{
    RectTransform rectTransform; // ���g��RectTransform�ϐ�
    RectTransform[] rectTransforms; // RectTransform�z��
    public float speacing = 20f; // �I�u�W�F�N�g�Ԃ̃X�y�[�X�̑傫��
    float maxParentLength = 0; // �e�I�u�W�F�N�g�̑S��
    public float speed = 1000f;
    float initialLeft;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>(); // RectTransform����
        rectTransforms = new RectTransform[transform.childCount]; // �q�I�u�W�F�N�g���̔z���p�ӂ���
        // �q�I�u�W�F�N�g��RectTransform���i�[����
        for (int i = 0; i < rectTransforms.Length; i++)
        {
            rectTransforms[i] = transform.GetChild(i).GetComponent<RectTransform>();
            maxParentLength += rectTransforms[i].rect.width + speacing;
        }
        initialLeft = rectTransform.offsetMin.x;
    }
    private void FixedUpdate()
    {
        SlideIn();
    }
    void SlideIn()
    {
        float currentLeft = rectTransform.offsetMin.x;
        float newLeft = currentLeft + speed * Time.deltaTime;
        rectTransform.offsetMin= new Vector2(newLeft, rectTransform.offsetMin.y);

    }
    void SlideOut()
    {

    }
}
