using UnityEngine;
using System.Collections.Generic;

public enum Channel
{
    normal,
    mafia,
    dead
}
public enum SubType
{
    subscribe,
    unsubscribe
}
public class ChatChannel : MonoBehaviour
{
    public static ChatChannel Instance;
    private List<Channel> channels = new List<Channel>() { Channel.normal }; 
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    public void SubOrUnsubChannel(Channel c, SubType s)
    {
        if (s == SubType.subscribe)
            channels.Add(c);
        else if (s == SubType.unsubscribe)
            channels.Remove(c);
    }
    public bool isSubscribed(Channel c)
    {
        if (channels.Contains(c))
            return true;
        return false;
    }
}
