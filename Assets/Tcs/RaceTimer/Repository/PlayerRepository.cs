using System;
using System.Collections.Generic;
using System.Linq;
using Tcs.RaceTimer.Exceptions;
using Tcs.RaceTimer.Models;
using UnityEngine;

namespace Tcs.RaceTimer.Repository
{
    public class PlayerRepository
    {
        public const string PlayerListIds = "playerListIds";
        
        public Player Create(string id, string name, int age, string email)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Race id cannot be null or empty or whitespace.");

            if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or empty or whitespace.");

            if (age <= 0)
                throw new ArgumentException("Age cannot be zero or less.");

            if (string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty or whitespace.");

            var player = new Player
            {
                Id = id,
                Name = name,
                Age = age,
                Email = email
            };

            var json = JsonUtility.ToJson(player, true);
            Debug.Log("Create Player(" + player.Id + "): " + json);
            PlayerPrefs.SetString(player.Id, json);

            AddToList(player.Id);

            return player;
        }

        private void AddToList(string playerId)
        {
            if (string.IsNullOrEmpty(playerId))
                return;

            PlayerList playerList;
            
            string playerListIds = PlayerPrefs.GetString(PlayerListIds, null);
            if (!string.IsNullOrEmpty(playerListIds))
                playerList = JsonUtility.FromJson<PlayerList>(playerListIds);
            else
                playerList = new PlayerList();

            var exists = playerList.Ids.Contains(playerId);
            Debug.Log("Adding player data to the list: " + playerId + " - Exists: " + exists);

            if (exists)
                return;

            playerList.Ids.Add(playerId);
            var json = JsonUtility.ToJson(playerList);
            Debug.Log("Player list new: " + json);

            PlayerPrefs.SetString(PlayerListIds, json);
        }

        public Player Get(string id)
        {
            if (id == null)
                throw new PlayerNotFoundException();

            string data = PlayerPrefs.GetString(id, null);
            if (string.IsNullOrEmpty(data))
                throw new PlayerNotFoundException();

            Debug.Log("Get Player(" + id + "): " + data);
            var player = JsonUtility.FromJson<Player>(data);

            return player;
        }

        public IEnumerable<Player> GetAll()
        {
            if (PlayerPrefs.HasKey(PlayerListIds))
            {
                var playerListIds = PlayerPrefs.GetString(PlayerListIds);
                if (string.IsNullOrEmpty(playerListIds))
                    return new List<Player>();

                var playerList = JsonUtility.FromJson<PlayerList>(playerListIds);
                return playerList.Ids.Select(Get);
            }

            return new List<Player>();
        }

        public Player FindByName(string name)
        {
            var players = GetAll();
            foreach (var player in players)
            {
                if (player.Name == name)
                    return player;
            }

            return null;
        }
    }
}
