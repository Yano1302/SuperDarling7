using UnityEngine;
//�Q�[���̐ݒ�pScriptableObject
[CreateAssetMenu(menuName = "ScriptableObject/DebugSettings", fileName = "DebugSettings")]
public class DebugSettings : ScriptableObject
{
    [Header("�f�o�b�O���O��\���������ꍇ�̓`�F�b�N������")]
    public bool debugLogEnabled; //�f�o�b�O���O��\�����邩
}
