using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.InputSystem;

public class Newspaper : Item
{
    Dictionary<string, string> diaries = new Dictionary<string, string>();
    Dictionary<string, string> news = new Dictionary<string, string>();
    [SerializeField] private TextMeshProUGUI pageText, diaryTitle, diaryContent, newsTitle, newsContent;
    [SerializeField] GameObject canvas;
    PhotonView PV;
    int page = 1;
    private void Start()
    {
        PV = GetComponent<PhotonView>();
        if (!PV.IsMine)
            Destroy(canvas);
    }
    public override void Use()
    {
        SetNewspaperContent();
    }
    public void SetNewspaperContent()
    {
        diaries = PrepareDeathDiaries.GetDiaries();
        news = RandomNews.GetNews(diaries.Count);
        pageText.text = "1/" + diaries.Count;
        ShowPage(page);
    }
    // Update is called once per frame
    void Update()
    {
        if (!PV.IsMine)
            return;
        if (ItemManager.Instance.Item() == gameObject.GetComponent<Newspaper>())
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                ShowPage(page + 1);
            }
            else if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                ShowPage(page - 1);
            }
        }
    }

    private void ShowPage(int _page)
    {
        if (_page > diaries.Count)
            _page = 1;
        else if (_page < 1)
            _page = diaries.Count;
        var diary = diaries.ElementAt(_page - 1);
        var _news = news.ElementAt(_page - 1);
        diaryTitle.text = "DEATH NEWS!\n" + diary.Key + " died yesterday. Someone found " + diary.Key + "'s diary";
        diaryContent.text = diary.Value;
        newsTitle.text = _news.Key;
        newsContent.text = _news.Value;
        pageText.text = _page + "/" + diaries.Count;
        page = _page;
    }
}
