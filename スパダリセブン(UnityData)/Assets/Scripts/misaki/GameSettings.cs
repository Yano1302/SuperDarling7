using UnityEngine;
//�Q�[���̐ݒ�pScriptableObject
[CreateAssetMenu(menuName = "ScriptableObject/GameSettings", fileName = "GameSettings")]
public class GameSettings : ScriptableObject
{
    [Header("�f�o�b�O���O��\���������ꍇ�̓`�F�b�N������")]
    public bool debugLogEnabled; //�f�o�b�O���O��\�����邩
}
