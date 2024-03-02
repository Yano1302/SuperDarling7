using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSetting : MonoBehaviour
{
    [SerializeField] private GameSettings gameSettings;    //�Q�[���̐ݒ�f�[�^(�ǋL)
    // Start is called before the first frame update
    protected virtual void Awake()
    {
        Debug.unityLogger.logEnabled = gameSettings.debugLogEnabled;   //�f�o�b�O���O���\���ɂ���
    }
}
