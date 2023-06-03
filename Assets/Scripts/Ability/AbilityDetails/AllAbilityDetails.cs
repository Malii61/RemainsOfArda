using System;
using System.Collections.Generic;
public class AllAbilityDetails 
{
    public Dictionary<string, Tuple<string, string, string>> abilityDetails = new Dictionary<string, Tuple<string, string, string>>()
    {
        //Role
            //Passive Detail
            //Q Detail
            //R Detail

        {Roles.Watchman.ToString() ,new Tuple<string, string, string>(
            "none",
            "Gains invulnerability for 1/8 of the night. Also, if he is attacked during this time, he will kill the attacker.",
            "Allows self and a chosen player to be invulnerable for 1/4 of the night. Also, if they are attacked during this time, the attacker dies. If he chooses himself again, he doubles the time.")
        },
        {Roles.Murderer.ToString(),new Tuple<string, string, string>(
            "You have a weapon. It is waiting to be used in the item slot at night. The sound of gunfire can be heard throughout the village.",
            "He attaches a silencer on his weapon during 1/6 of the night.",
            "Adds 1 bullet to your weapon.")
        },
        {Roles.Imam.ToString(),new Tuple<string, string, string>(
            "none",
            "He sees the heart of a player by choosing a player he comes across around the mosque. He finds out if he is on the Town side.",
            "Casts a spell that lasts 1/3 of the night on the player he chooses. This player is haunted by demons. If the player is a member of the mob and his spell is still active at the end of the timer, he commits suicide.")
        },
        {Roles.Gravedigger.ToString(),new Tuple<string, string, string>(
            "Can see the direction of the graves of players who died at night.",
            "During 1/6 of the night, he can be included in the chat of the dead and get info.",
            "Can dig up the dead player's grave and revive him.")
        },
        {Roles.Necromancer.ToString(),new Tuple<string, string, string>(
            "none",
            "none",
            "He sees dead players and can resurrect his chosen player globally (from anywhere)")
        },
        {Roles.Schemer.ToString(),new Tuple<string, string, string>(
            "Schemer can also keep a diary during the day.",
            "If a player of his choice dies at night, he replaces his diary with her own.",
            "Makes the chosen player's role (because of his side) look different. He chooses one of 3 random roles and the role of the person chosen that night appears in that role.")
        },
        {Roles.Greedy.ToString(),new Tuple<string, string, string> (
            "Passive 1: If he reaches the gold amount determined for him (determined by the number of players), he wins the game.\nPassive 2: A sac of gold drops from a dead player and Gredy can smell it. The direction of the gold sack appears on the screen.",
            "Steals money when near the deceased player.",
            "During 1/3 of the night, the amount of gold received from all sources (player bags, quests, etc.) is doubled.")
        },
        {Roles.Belladonna.ToString(),new Tuple<string, string, string> (
            "none",
            "none",
            "none")
        },
        {Roles.Lucky.ToString(),new Tuple<string, string, string> (
            "Has a 25 percent chance to dodge all negative effects during the night.",
            "Doubles the chance of recovering from all negative effects for 1/3 of the night.",
            "Proves that he is lucky if he is selected to be executed in the voting on the morning of the night this skill is active. The guillotine breaks and the player does not die.")
        },
        {Roles.Slayer.ToString(),new Tuple<string, string, string> (
            "Slayer has a knife that he can use at night. When he stabs someone, the knife remains on the deceased. When he kills someone, he gains movement speed. If he kills it, he can reuse the Q.",
            "Slayer becomes invisible for 1/4 of the night. The ability is canceled when he wants to attack.",
            "During 1/3 of the night, the nickname of the selected player is replaced with the nickname of the Slayer.")
        }, 
    
    };

}
