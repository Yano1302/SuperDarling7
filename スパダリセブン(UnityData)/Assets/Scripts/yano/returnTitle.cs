using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO ���u���Ȃ̂ł��Ƃł���
public class returnTitle : MonoBehaviour
{
  public�@void Title(bool clear) 
  {
        Supadari.SceneManager.Instance.SceneChange(0);
        UIManager.Instance.CloseALLUI();
  }
}
