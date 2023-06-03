using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomNews : MonoBehaviour
{
    static Dictionary<string, string> news = new Dictionary<string, string>();
    private void Start()
    {
        if (news.Count == 0)
            CreateNews();
    }

    private void CreateNews()
    {
        news.Add("News1", "News1 Content");
        news.Add("News2", "News2 Content");
        news.Add("News3", "News3 Content");
        news.Add("News4", "News4 Content");
        news.Add("News5", "News5 Content");
        news.Add("News6", "News6 Content");
        news.Add("News7", "News7 Content");
    }

    public static Dictionary<string, string> GetNews(int newsCount)
    {
        Dictionary<string, string> tempNews = new Dictionary<string, string>();
        while (tempNews.Count < newsCount)
        {
            int newsOrder = Random.Range(0, news.Count - 1);
            if (!tempNews.ContainsKey(news.ElementAt(newsOrder).Key))
                tempNews.Add(news.ElementAt(newsOrder).Key, news.ElementAt(newsOrder).Value);

        }
        return tempNews;
    }
}
