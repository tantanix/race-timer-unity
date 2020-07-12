using System;
using System.Collections.Generic;
using System.Linq;
using Tcs.RaceTimer.Exceptions;
using Tcs.RaceTimer.Models;
using UnityEngine;

namespace Tcs.RaceTimer.Repository
{
    public class RacePlayerRepository
    {
        public const string RacePlayerListPrefix = "List-";

        public RacePlayer Create(string id, string raceId, string teamId, string playerId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Id cannot be null or empty or whitespace");

            if (string.IsNullOrEmpty(raceId) || string.IsNullOrWhiteSpace(raceId))
                throw new ArgumentException("Race id cannot be null or empty or whitespace");

            if (string.IsNullOrEmpty(teamId) || string.IsNullOrWhiteSpace(teamId))
                throw new ArgumentException("Team id cannot be null or empty or whitespace");

            if (string.IsNullOrEmpty(playerId) || string.IsNullOrWhiteSpace(playerId))
                throw new ArgumentException("Player id cannot be null or empty or whitespace");

            var racePlayer = new RacePlayer
            {
                Id = id,
                RaceId = raceId,
                TeamId = teamId,
                PlayerId = playerId
            };

            var json = JsonUtility.ToJson(racePlayer, true);
            Debug.Log("Create Race Player(" + racePlayer.Id + "): " + json);
            PlayerPrefs.SetString(racePlayer.Id, json);

            AddToList(raceId, racePlayer.Id);

            return racePlayer;
        }

        public RacePlayer Get(string id)
        {
            if (id == null)
                throw new RacePlayerNotFoundException();

            string data = PlayerPrefs.GetString(id, null);
            if (string.IsNullOrEmpty(data))
                throw new RacePlayerNotFoundException();

            Debug.Log("Get Race Player(" + id + "): " + data);
            var racePlayer = JsonUtility.FromJson<RacePlayer>(data);

            return racePlayer;
        }

        public IEnumerable<RacePlayer> GetAll(string raceId)
        {
            var key = GetRacePlayerListKey(raceId);
            if (PlayerPrefs.HasKey(key))
            {
                var racePlayerListIds = PlayerPrefs.GetString(key);
                if (string.IsNullOrEmpty(racePlayerListIds))
                    return new List<RacePlayer>();

                var racePlayerList = JsonUtility.FromJson<RacePlayerList>(racePlayerListIds);
                return racePlayerList.Ids.Select(Get);
            }

            return new List<RacePlayer>();
        }

        public RacePlayer Find(string raceId, string teamId, string playerId)
        {
            var racePlayers = GetAll(raceId);
            foreach (var racePlayer in racePlayers)
            {
                if (racePlayer.TeamId == teamId && racePlayer.PlayerId == playerId)
                    return racePlayer;
            }
            return null;
        }

        private string GetRacePlayerListKey(string raceId)
        {
            return RacePlayerListPrefix + raceId;
        }

        private void AddToList(string raceId, string id)
        {
            RacePlayerList racePlayerList;
            var key = GetRacePlayerListKey(raceId);

            string racePlayerListIds = PlayerPrefs.GetString(key, null);
            if (!string.IsNullOrEmpty(racePlayerListIds))
                racePlayerList = JsonUtility.FromJson<RacePlayerList>(racePlayerListIds);
            else
                racePlayerList = new RacePlayerList();

            var exists = racePlayerList.Ids.Contains(id);
            Debug.Log("Adding player id: " + id + " to the race: " + raceId + " - Exists: " + exists);

            if (exists)
                return;

            racePlayerList.Ids.Add(id);
            var json = JsonUtility.ToJson(racePlayerList);
            Debug.Log("Race player list new: " + json);

            PlayerPrefs.SetString(key, json);
        }
    }
}
