using Supadari;
using UnityEngine;

public class SlideWindow : MonoBehaviour
{
    RectTransform rectTransform; // ���g��RectTransform�ϐ�
    RectTransform[] rectTransforms; // RectTransform�z��
    [SerializeField] UIManager uiManager; // UIManager�ϐ�
    [SerializeField] AudioManager audioManager; // AudioManager�ϐ�
    [SerializeField] SceneManager sceneManager; // SceneManager�ϐ�
    public float speacing = 20f; // �I�u�W�F�N�g�Ԃ̃X�y�[�X�̑傫��
    float maxParentLength = 0; // �e�I�u�W�F�N�g�̑S��
    float parentOffsetMin_x = 0; // �e�I�u�W�F�N�g��left�����l
    public float speed = 1000f;
    float[] phase; // ���t�F�[�Y�ɐi�߂�ڕW�l
    float phasePoint = 0; // ���t�F�[�Y�ɐi�ނ��ǂ����̔���Ŏg���t���[�g�l
    SLIDESTATE slideState; // SLIDESTATE�ϐ�
    bool openMenu = false; // ���j���[���J���Ă��邩�ǂ���
    public bool OpenCheck() => openMenu; // openMenu�Q�b�^�[�֐�

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>(); // RectTransform����
        parentOffsetMin_x = rectTransform.offsetMin.x; // �e�I�u�W�F�N�g
        rectTransforms = new RectTransform[transform.childCount]; // �q�I�u�W�F�N�g���̔z���p�ӂ���
        // �q�I�u�W�F�N�g��RectTransform���i�[���A�q�I�u�W�F�N�g�̉�����S�đ����Đe�I�u�W�F�N�g�̑S�����Z�o����
        for (int i = 0; i < rectTransforms.Length; i++)
        {
            rectTransforms[i] = transform.GetChild(i).GetComponent<RectTransform>();
            maxParentLength += rectTransforms[i].rect.width + speacing;
        }
        phase = new float[transform.childCount]; // �t�F�[�Y�̗v�f�����q�I�u�W�F�N�g���ɂ���
        for (int i = 0; i < phase.Length; i++)
        {
            // ��ƂȂ�t�F�[�Y�ڕW�l���Z�o����@�e�I�u�W�F�N�g�̑S��/�q�I�u�W�F�N�g��
            if (i == 0)
            {
                phase[0] = maxParentLength / transform.childCount;
                continue;
            }
            // �v�f��1�ȍ~��phase[0]��(i+1)�{���đ������ �� phase[4]�̏ꍇ��phase[0]*5�{�ƂȂ�
            phase[i] = phase[0] * (i + 1);
        }
        slideState = SLIDESTATE.DEFAULT; // slideState���f�t�H���g�ɂ���
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>(); // AudioManager����
    }

    private void FixedUpdate()
    {
        if (!openMenu) SlideIn(); // ���j���[�����܂��ꍇ
        else if (openMenu) SlideOut(); // ���j���[���o���ꍇ
    }
    /// <summary>
    /// ���j���[�����܂��֐�
    /// </summary>
    void SlideIn()
    {
        // �t�F�[�Y�|�C���g���e�I�u�W�F�N�g�̑S���ȏ�ɂȂ��� �܂��� �X���C�h�X�e�[�^�X���f�t�H���g�̏ꍇ
        if (phasePoint >= maxParentLength || slideState == SLIDESTATE.DEFAULT)
        {
            if (slideState == SLIDESTATE.DEFAULT) return; // �f�t�H���g�̏ꍇ�̓��^�[�����Ċ֐��𓮂����Ȃ�
            // �e�I�u�W�F�N�g�̑S�����ŏ��ɕύX
            rectTransform.offsetMin = new Vector2(rectTransform.offsetMax.x  , rectTransform.offsetMin.y);
            for (int i = 0; i < rectTransforms.Length; i++)
            {
                rectTransforms[i].position = new Vector2(rectTransform.position.x, rectTransforms[i].position.y);
            }
            phasePoint = default; // �t�F�[�Y�|�C���g�����Z�b�g
            slideState = SLIDESTATE.DEFAULT; // �f�t�H���g�ɂ���
            return;
        }
        float currentLeft = rectTransform.offsetMin.x; // �e�I�u�W�F�N�g�̌��݂�Left�l����
        float move = speed * Time.deltaTime; // �ړ��l���Z�o
        float newLeft = currentLeft + move; // �ړ��l�𑫂���Left�l���Z�o
        phasePoint += move; // �t�F�[�Y�|�C���g�Ɉړ��l�����Z
        // �t�F�[�Y�|�C���g�ɂ���Ďq�I�u�W�F�N�g�𓮂���
        PhaseSlide(move);
        // �e�I�u�W�F�N�g��Left��ύX���đS�����k�߂�
        rectTransform.offsetMin = new Vector2(newLeft, rectTransform.offsetMin.y);
    }
    /// <summary>
    /// ���j���[���o���֐�
    /// </summary>
    void SlideOut()
    {
        // �t�F�[�Y�|�C���g���e�I�u�W�F�N�g�̑S���ȏ�ɂȂ��� �܂��� �X���C�h�X�e�[�^�X���f�t�H���g�̏ꍇ
        if (phasePoint >= maxParentLength || slideState == SLIDESTATE.DEFAULT)
        {
            if (slideState == SLIDESTATE.DEFAULT) return; // �f�t�H���g�̏ꍇ�̓��^�[�����Ċ֐��𓮂����Ȃ�
            // �e�I�u�W�F�N�g�̑S�����ő�(�S�q�I�u�W�F�N�g�̕����v�l)�ɕύX
            rectTransform.offsetMin = new Vector2(-(maxParentLength - rectTransform.offsetMax.x), rectTransform.offsetMin.y);
            for (int i = 0; i < rectTransforms.Length; i++)
            {
                if (i == 0)
                {
                    rectTransforms[i].position = new Vector2(rectTransform.position.x + phase[2], rectTransforms[i].position.y);
                }
                else if(i == 1)
                {
                    rectTransforms[i].position = new Vector2(rectTransform.position.x + phase[1], rectTransforms[i].position.y);
                }
                else if (i == 2)
                {
                    rectTransforms[i].position = new Vector2(rectTransform.position.x + phase[0], rectTransforms[i].position.y);
                }
                else if (i == 3)
                {
                    rectTransforms[i].position = new Vector2(rectTransform.position.x, rectTransforms[i].position.y);
                }
            }
            phasePoint = default; // �t�F�[�Y�|�C���g�����Z�b�g
            slideState = SLIDESTATE.DEFAULT; // �f�t�H���g�ɂ���
            return; 
        }
        float currentLeft = rectTransform.offsetMin.x; // �e�I�u�W�F�N�g�̌��݂�Left�l����
        float move = speed * Time.deltaTime; // �ړ��l���Z�o
        float newLeft = currentLeft - move; // �ړ��l�𑫂���Left�l���Z�o
        phasePoint += move; // �t�F�[�Y�|�C���g�Ɉړ��l�����Z
        // �t�F�[�Y�|�C���g�ɂ���Ďq�I�u�W�F�N�g�𓮂���
        PhaseSlide(move);
        // �e�I�u�W�F�N�g��Left��ύX���đS�����k�߂�
        rectTransform.offsetMin = new Vector2(newLeft, rectTransform.offsetMin.y);
    }
    /// <summary>
    /// �q�I�u�W�F�N�g���X���C�h�C��������֐�
    /// </summary>
    /// <param name="value">�����������q�I�u�W�F�N�g�̗v�f��</param>
    /// <param name="move">�ړ��l</param>
    void SlideChildren(int value, float move)
    {
        // value�ȉ��̎q�I�u�W�F�N�g���X���C�h������
        for (int i = -1; i < value; i++)
        {
            if (!openMenu) // ���j���[���J���ꍇ
            {
                rectTransforms[i + 1].position = new Vector2(rectTransforms[i + 1].position.x - move, rectTransforms[i + 1].position.y);
            }
            else // ���j���[�����܂��ꍇ
            {
                rectTransforms[i + 1].position = new Vector2(rectTransforms[i + 1].position.x + move, rectTransforms[i + 1].position.y);
            }
        }
    }
    /// <summary>
    /// �q�I�u�W�F�N�g���X���C�h�C�������邩�̃`�F�b�N�֐�
    /// </summary>
    /// <param name="move">�ړ��l</param>
    void PhaseSlide(float move)
    {
        // �t�F�[�Y�|�C���g���ǂ̃t�F�[�Y�܂œ��B���Ă��邩�̃`�F�b�N
        for (int i = phase.Length - 1; i >= 0; i--)
        {
            if (phasePoint > phase[i])
            {
                // �`�F�b�N���ʂ�����t�F�[�Y�z��̗v�f�����̎q�I�u�W�F�N�g�𓮂���
                SlideChildren(i, move);
                return;
            }
        }
    }
    /// <summary>
    /// �X�g�[���[���j���[���J���܂��͕���֐�
    /// </summary>
    public void StoryMenuButton()
    {
        // �X���C�h�X�e�[�^�X���f�t�H���g�����`�F�b�N
        if (slideState == SLIDESTATE.DEFAULT)
        {
            openMenu = !openMenu; // ���]���Ċ֐��𓮂���
            slideState = SLIDESTATE.SLIDE; // �X���C�h���ɕύX
        }
    }
    /// <summary>
    /// ���j���[�̃{�^���z�u������������֐�
    /// </summary>
    public void InitializeStoryMenu()
    {
        if (!openMenu) return; // ���j���[���J���Ă��Ȃ��Ȃ烊�^�[��
        // �J���Ă��Ȃ���Ԃɂ���
        openMenu = false;
        rectTransform.offsetMin = new Vector2(parentOffsetMin_x, rectTransform.offsetMin.y);
        
        for (int i = 0; i < rectTransforms.Length; i++)
        {
            rectTransforms[i].localPosition = new Vector2(rectTransforms[i].localPosition.x - rectTransforms[i].localPosition.x, rectTransforms[i].localPosition.y);
        }

    }
    /// <summary>
    /// �Z�[�u�X���b�g���J���֐�
    /// </summary>
    public void SaveButton()
    {
        audioManager.SE_Play("SE_click", sceneManager.enviromentalData.TInstance.volumeSE); // SE��炷
        uiManager.OpenUI(UIType.SaveSlot); // �Z�[�u�X���b�g��\��
    }
    /// <summary>
    /// ���[�h�X���b�g���J���֐�
    /// </summary>
    public void LoadButton()
    {
        audioManager.SE_Play("SE_click", sceneManager.enviromentalData.TInstance.volumeSE); // SE��炷
        uiManager.OpenUI(UIType.LoadSlot); // ���[�h�X���b�g��\��
    }
}
