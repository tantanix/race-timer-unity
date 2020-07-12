using Tcs.RaceTimer.Exceptions;
using Tcs.RaceTimer.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace Tcs.RaceTimer.Repository
{
    public class TeamRepository
    {
        public const string TeamListIds = "teamListIds";

        public Team Create(string id, string name)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Team id cannot be null or empty or whitespace");

            if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Team name cannot be null or empty or whitespace");

            var team = new Team
            {
                Id = id,
                Name = name
            };

            var json = JsonUtility.ToJson(team, true);
            Debug.Log("Create Team(" + team.Id + "): " + json);
            PlayerPrefs.SetString(team.Id, json);

            AddToList(team.Id);

            return team;
        }

        public Team Get(string id)
        {
            if (id == null)
                throw new TeamNotFoundException();

            string data = PlayerPrefs.GetString(id, null);
            if (string.IsNullOrEmpty(data))
                throw new TeamNotFoundException();

            Debug.Log("Get Team(" + id + "): " + data);
            var team = JsonUtility.FromJson<Team>(data);

            return team;
        }

        public IEnumerable<Team> GetAll()
        {
            if (PlayerPrefs.HasKey(TeamListIds))
            {
                var teamListIds = PlayerPrefs.GetString(TeamListIds);
                if (string.IsNullOrEmpty(teamListIds))
                    return new List<Team>();

                var teamList = JsonUtility.FromJson<TeamList>(teamListIds);
                return teamList.Ids.Select(Get);
            }

            return new List<Team>();
        }

        public Team FindByName(string teamName)
        {
            var teams = GetAll();
            foreach (var team in teams)
            {
                if (team.Name == teamName)
                    return team;
            }

            return null;
        }

        private void AddToList(string teamId)
        {
            if (string.IsNullOrEmpty(teamId))
                return;

            TeamList teamList;

            string teamListIds = PlayerPrefs.GetString(TeamListIds, null);
            if (!string.IsNullOrEmpty(teamListIds))
                teamList = JsonUtility.FromJson<TeamList>(teamListIds);
            else
                teamList = new TeamList();

            var exists = teamList.Ids.Contains(teamId);
            Debug.Log("Adding team data to the list: " + teamId + " - Exists: " + exists);

            if (exists)
                return;

            teamList.Ids.Add(teamId);
            var json = JsonUtility.ToJson(teamList);
            Debug.Log("Team list new: " + json);

            PlayerPrefs.SetString(TeamListIds, json);
        }
    }
}
