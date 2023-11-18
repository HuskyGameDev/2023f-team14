using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team : ushort
{
    Team0 = 1,
    Team1 = 2,
    Team2 = 4,
    Team3 = 8,
    Team4 = 16,
    Team5 = 32,
    Team6 = 64,
    Team7 = 128,
    NoTeam = 0
}
public class TeamUtils
{
    static readonly Dictionary<Team, Color> teamColors = new()
    {
        { Team.NoTeam, Color.grey },
        { Team.Team1, Color.blue },
        { Team.Team2, Color.yellow },
        { Team.Team3, Color.cyan },
        { Team.Team4, Color.green },
        { Team.Team5, Color.magenta },
        { Team.Team6, Color.white },
        { Team.Team0, Color.red },
        { Team.Team7, Color.black },
    };

    public static int GetTeamNumber(Team team)
    {
        return (int)Math.Log((double)team, 2);
    }
    public static Color GetTeamColor(Team team)
    {
        return teamColors[team];
    }

    public static Team TeamNumberToTeam(int teamNumber)
    {
        return (Team)Math.Pow(2, teamNumber);
    }
}