using System.Collections.Generic;
using UnityEngine;

public enum Roles
{
    Imam,
    Watchman,
    Gravedigger,
    Lucky,
    Necromancer,
    Schemer,
    Murderer,
    Greedy,
    Belladonna,
    Slayer,

}
public class AllRoles : ScriptableObject
{
    internal readonly Dictionary<string, string> allRoles = new Dictionary<string, string>()
    {
        {  Roles.Imam.ToString() , "You are the " + Roles.Imam.ToString() + " of " + Side.town.ToUpperInvariant() },
        {  Roles.Watchman.ToString() , "You have a dangerous shield. Use it wisely" },
        {  Roles.Gravedigger.ToString() , "Talk to the dead and even revive them.You are from " + Side.town.ToUpperInvariant() },
        {  Roles.Lucky.ToString() , "You are the luckiest player. You may dodge everything if you are lucky enough"},
        {  Roles.Necromancer.ToString() , "You are the spirit master and the reviver in the " + Side.mafia.ToUpperInvariant()+"."},
        {  Roles.Schemer.ToString() , "Change the diaries, show the role of the players differently. Trick others to win with " + Side.mafia.ToUpperInvariant()},
        {  Roles.Murderer.ToString() , "You are the murderer of " + Side.mafia.ToUpperInvariant()+". You have a pistol and limited bullets."},
        {  Roles.Greedy.ToString() , "Collect and steal money to win"},
        {  Roles.Belladonna.ToString() , "Poison and kill everyone to win"},
        {  Roles.Slayer.ToString(), "Become invisible and kill everyone silently with your sharp knife to win."},
    };
    internal readonly Dictionary<string, string> townRoles = new Dictionary<string, string>()
    {
        {  Roles.Gravedigger.ToString() , "Talk to the dead and even revive them.You are from " + Side.town.ToUpperInvariant() },
        {  Roles.Watchman.ToString() , "You have a dangerous shield. Use it wisely" },
        {  Roles.Imam.ToString() , "You are the " +  Roles.Imam.ToString() + " of " + Side.town.ToUpperInvariant() },
        {  Roles.Lucky.ToString() , "You are the luckiest player. You may dodge everything if you are lucky enough"},
    };
    internal readonly Dictionary<string, string> mafiaRoles = new Dictionary<string, string>()
    {
        {  Roles.Necromancer.ToString() , "You are the spirit master and the reviver in the " + Side.mafia.ToUpperInvariant()+"."},
        {  Roles.Schemer.ToString() , "Change the diaries, show the role of the players differently. Trick others to win with " + Side.mafia.ToUpperInvariant()},
        {  Roles.Belladonna.ToString() , "Poison and kill everyone to win"},
        {  Roles.Murderer.ToString() , "You are the murderer of " + Side.mafia.ToUpperInvariant()+". You have a pistol and limited bullets."},
    };
    internal readonly Dictionary<string, string> otherRoles = new Dictionary<string, string>()
    {
        {  Roles.Greedy.ToString() , "Collect and steal money to win"},
        {  Roles.Belladonna.ToString(), "Poison and kill everyone to win"},
        {  Roles.Slayer.ToString(), "Become invisible and kill everyone silently with your sharp knife to win."},
    };
}
