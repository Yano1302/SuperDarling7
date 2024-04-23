using System;


/// <summary>
/// IDとJson読み込み用クラスを配置してます。
/// </summary>

//IDを設定　(Jsonファイルと同じ名前にする)
public enum ItemID {
    Dummy = 0,
    ID0　=  1,
    ID1  =  2,
    ID2  =  3,
    ID3  =  4,
    ID4  =  5,
    ID5  =  6,
    ID6  =  7,
    ID7  =  8,
    ID8  =  9,
    ID9  =  10,
    ID10 =  11,
}

//ID名とbool変数名を一致させる
[Serializable]
public class SettingsGetItemFlags {
    public bool ID0;
    public bool ID1;
    public bool ID2;
    public bool ID3;
    public bool ID4;
    public bool ID5;
    public bool ID6;
    public bool ID7;
    public bool ID8;
    public bool ID9;
    public bool ID10;

    public bool GetFlag(ItemID id) {
        switch (id) {
            case ItemID.ID0: return ID0;
            case ItemID.ID1: return ID1;
            case ItemID.ID2: return ID2;
            case ItemID.ID3: return ID3;
            case ItemID.ID4: return ID4;
            case ItemID.ID5: return ID5;
            case ItemID.ID6: return ID6;
            case ItemID.ID7: return ID7;
            case ItemID.ID8: return ID8;
            case ItemID.ID9: return ID9;
            case ItemID.ID10: return ID10;
        }
        UsefulSystem.LogError("アイテムIDに対応する所持フラグがありませんでした。 ID : " + id);
        return false;
    }

    public void SetFlag(ItemID id, bool value) {
        switch (id) {
            case ItemID.ID0: ID0 = value; break;
            case ItemID.ID1: ID1 = value; break;
            case ItemID.ID2: ID2 = value; break;
            case ItemID.ID3: ID3 = value; break;
            case ItemID.ID4: ID4 = value; break;
            case ItemID.ID5: ID5 = value; break;
            case ItemID.ID6: ID6 = value; break;
            case ItemID.ID7: ID7 = value; break;
            case ItemID.ID8: ID8 = value; break;
            case ItemID.ID9: ID9 = value; break;
            case ItemID.ID10: ID10 = value; break;


            default: UsefulSystem.LogError("アイテムIDに対応する所持フラグがありませんでした。 ID : " + id);break;
        }
    }
}