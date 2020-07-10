using Tcs.RaceTimer.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Tcs.RaceTimer.Models;

namespace Tcs.RaceTimer.Repository
{
    public class RaceRepository
    {
        public const string RaceListIds = "raceListIds";

        public Race CreateRace(string name, long eventDate, int stages)
        {
            var id = Guid.NewGuid().ToString();
            return CreateRace(id, name, eventDate, stages);
        }

        public Race CreateRace(string id, string name, long eventDate, int stages)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Race id cannot be null or empty or whitespace");

            if (stages <= 0)
                throw new ArgumentException("Stage cannot be zero or less.");

            var race = new Race
            {
                Id = id,
                Name = name,
                EventDate = eventDate,
                Stages = stages
            };

            var json = JsonUtility.ToJson(race, true);
            Debug.Log("CreateRace(" + race.Id + "): " + json);
            PlayerPrefs.SetString(race.Id, json);

            AddToList(race.Id);

            return race;
        }

        public void DeleteRace(string id)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Race id cannot be null or empty or whitespace");

            var data = PlayerPrefs.GetString(id, null);
            if (string.IsNullOrEmpty(data))
                throw new RaceNotFoundException();

            RemoveFromList(id);

            Debug.Log("DeleteRace(" + id + ")");
            PlayerPrefs.DeleteKey(id);
        }

        public Race GetRace(string id)
        {
            if (id == null)
                throw new RaceNotFoundException();

            string data = PlayerPrefs.GetString(id, null);
            if (string.IsNullOrEmpty(data))
                throw new RaceNotFoundException();

            Debug.Log("GetMovieData(" + id + "): " + data);
            var race = JsonUtility.FromJson<Race>(data);

            // Remove race if it was faulty saved
            if (string.IsNullOrEmpty(race.Id))
            {
                DeleteRace(id);
                return null;
            }

            return race;
        }

        public IReadOnlyList<Race> GetAll()
        {
            if (PlayerPrefs.HasKey(RaceListIds))
            {
                var raceListIds = PlayerPrefs.GetString(RaceListIds);
                if (string.IsNullOrEmpty(raceListIds))
                    return new List<Race>();

                var arrList = raceListIds.Split(',');
                return arrList.Select(GetRace).ToList().AsReadOnly();
            }

            return new List<Race>();
        }

        private void AddToList(string raceId)
        {
            if (string.IsNullOrEmpty(raceId))
                return;

            string raceListIds = PlayerPrefs.GetString(RaceListIds, null);
            var exists = raceListIds.IndexOf(raceId, StringComparison.InvariantCultureIgnoreCase) != -1;
            Debug.Log("Adding race data to the list: " + raceId + " - Exists: " + exists);

            if (exists)
                return;

            if (!string.IsNullOrEmpty(raceListIds))
                raceListIds += ",";

            raceListIds += raceId;

            PlayerPrefs.SetString(RaceListIds, raceListIds);
        }

        public void RemoveFromList(string raceId)
        {
            if (string.IsNullOrEmpty(raceId))
                return;

            string raceListIds = PlayerPrefs.GetString(RaceListIds, null);
            Debug.Log("Before Remove: " + raceListIds);
            var list = raceListIds.Split(',');
            string newList = "";
            foreach (var id in list)
            {
                // Exclude the raceId
                if (id != raceId)
                {
                    if (!string.IsNullOrEmpty(newList))
                        newList += ",";

                    newList += id;
                }
            }
            Debug.Log("After Remove: " + newList);
            PlayerPrefs.SetString(RaceListIds, newList);
        }
    }
}
