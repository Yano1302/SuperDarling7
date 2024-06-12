using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class InvTextureholder : SingletonMonoBehaviour<InvTextureholder>{ 

    /// <summary>�e�N�X�`�����擾���܂�</summary>
    public Sprite GetSprite(InvType type) { return m_InvSprite[(int)type]; }




    private static Sprite[] m_InvSprite;
    protected override void Awake() {
        base.Awake();
        if(m_InvSprite == null) {
            //�񋓌^��z��Ƃ��Ď擾
            var type = Enum.GetValues(typeof(InvType));  
            //None�̕��̉摜�͖����̂Ŋ܂߂Ȃ�
           int length = type.Length - 1;
           m_InvSprite = new Sprite[length];
            for (int i = 0,index = 0; i < length; i++,index++) {
                InvType inv = (InvType)type.GetValue(index);
                if(inv == InvType.None) {
                    index++;
                    inv = (InvType)type.GetValue(index);
                }
                //Texture2D��Sprite�ɕϊ�
                Texture2D tex = UsefulSystem.ResourcesLoad<Texture2D>(inv.ToString() + ".jpg");
                m_InvSprite[i] = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
                Debug.Assert(m_InvSprite[i] != null, $"{ inv.ToString() + ".jpg" } �̉摜���i�[����Ă��܂���B");           
           }
        }
    }
}
