using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO ‰¼’u‚«‚È‚Ì‚Å‚ ‚Æ‚Å‚¯‚·
public class returnTitle : MonoBehaviour
{
  public@void Title(bool clear) 
  {
        Supadari.SceneManager.Instance.SceneChange(0);
        UIManager.Instance.CloseALLUI();
  }
}
