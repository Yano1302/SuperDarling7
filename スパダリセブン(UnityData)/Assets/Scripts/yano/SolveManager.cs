using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// �}�l�[�W���[�p�I�u�W�F�N�g�ɃA�^�b�`���Ďg�p����
/// TODO �S�̂̍\����������
/// </summary>
public class SolveManager : MonoBehaviour
{
//public variables
    public static SolveManager Instance { get { return m_instance; }}

    public static bool CheckScene { get { return SceneManager.GetActiveScene().name == "SolveScene"; } }

    //public functions
    public void choice(ItemID id) {
        if(m_playerAnswer != null)
            m_obj[(int)m_playerAnswer].SetActive(false);

        m_playerAnswer = id;
        m_obj[(int)id].SetActive(true);
        m_answerDecisionBtn.SetActive(true);
    }

    //Attach variables
    //���u��
    [SerializeField] private GameObject m_answerDecisionBtn;
    [SerializeField] private ItemID m_Answer = ItemID.ID0; 
    [SerializeField]private GameObject[] m_obj;

    //Attach function
    //���u��
    public void Btn__CheckingAnswer() {
        Destroy(m_answerDecisionBtn);
        m_obj[(int)m_playerAnswer].SetActive(false);
        bool check = m_Answer == m_playerAnswer;
        UIType type = check ? UIType.Clear : UIType.miss;
        UIManager.Instance.OpenUI(type);
    }

    public void Btn__RetrunTitle() { Supadari.SceneManager.Instance.SceneChange(0); }

    //private
    private static SolveManager m_instance;
    private ItemID? m_playerAnswer = null;
    


    private void Awake() {
        m_instance = GetComponent<SolveManager>();
        m_answerDecisionBtn.SetActive (false);
    }


}
