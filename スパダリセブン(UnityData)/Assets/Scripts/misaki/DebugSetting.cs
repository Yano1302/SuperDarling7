using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���쐬�̃f�o�b�O���O��\�����邩�ǂ��������߂�X�N���v�g�ł�
// ���̃X�N���v�g���p������΁A�p�������X�N���v�g�ɋL�ڂ����f�o�b�O���O��\���̗L����Project��Őݒ�ł��܂�
// Project���ScriptableObjects/GameSettings�̃`�F�b�N�̗L���Ńf�o�b�O���O��\�����邩���Ȃ��������܂�܂�
public class DebugSetting : MonoBehaviour
{
    [SerializeField] private DebugSettings debugSettings;    //�Q�[���̐ݒ�f�[�^(�ǋL)
    // Start is called before the first frame update
    protected virtual void Awake()
    {
        Debug.unityLogger.logEnabled = debugSettings.debugLogEnabled;   //�f�o�b�O���O���\���ɂ���
    }
}
