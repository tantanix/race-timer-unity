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

        public Race Create(string id, string name, long eventDate, int stages, string location)
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
                Stages = stages,
                Location = location
            };

            var json = JsonUtility.ToJson(race, true);
            Debug.Log("CreateRace(" + race.Id + "): " + json);
            PlayerPrefs.SetString(race.Id, json);

            AddToList(race.Id);

            return race;
        }

        public void Delete(string id)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Race id cannot be null or empty or whitespace");

            var data = PlayerPrefs.GetString(id, null);
            if (string.IsNullOrEmpty(data))
                throw new RaceNotFoundException();

            RemoveFromList(id);

            Debug.Log("Delet Race(" + id + ")");
            PlayerPrefs.DeleteKey(id);
        }

        public Race Get(string id)
        {
            if (id == null)
                throw new RaceNotFoundException();

            string data = PlayerPrefs.GetString(id, null);
            if (string.IsNullOrEmpty(data))
                throw new RaceNotFoundException();

            Debug.Log("Get Race(" + id + "): " + data);
            var race = JsonUtility.FromJson<Race>(data);

            return race;
        }

        public IEnumerable<Race> GetAll()
        {
            if (PlayerPrefs.HasKey(RaceListIds))
            {
                var raceListIds = PlayerPrefs.GetString(RaceListIds);
                if (string.IsNullOrEmpty(raceListIds))
                    return new List<Race>();

                var raceList = JsonUtility.FromJson<RaceList>(raceListIds);
                return raceList.Ids.Select(Get);
            }

            return new List<Race>();
        }

        private void AddToList(string raceId)
        {
            if (string.IsNullOrEmpty(raceId))
                return;

            RaceList raceList;

            string raceListIds = PlayerPrefs.GetString(RaceListIds, null);
            if (!string.IsNullOrEmpty(raceListIds))
                raceList = JsonUtility.FromJson<RaceList>(raceListIds);
            else
                raceList = new RaceList();

            var exists = raceList.Ids.Contains(raceId);
            Debug.Log("Adding race data to the list: " + raceId + " - Exists: " + exists);

            if (exists)
                return;

            raceList.Ids.Add(raceId);
            var json = JsonUtility.ToJson(raceList);
            Debug.Log("Race list new: " + json);

            PlayerPrefs.SetString(RaceListIds, json);
        }

        private void RemoveFromList(string raceId)
        {
            if (string.IsNullOrEmpty(raceId))
                return;

            string raceListIds = PlayerPrefs.GetString(RaceListIds, null);
            if (raceListIds == null)
                return;

            var raceList = JsonUtility.FromJson<RaceList>(raceListIds);
            Debug.Log("Before Remove: " + raceListIds);

            if (!raceList.Ids.Contains(raceId))
                return;

            raceList.Ids.Remove(raceId);

            var json = JsonUtility.ToJson(raceList);
            Debug.Log("After Remove: " + raceList);

            PlayerPrefs.SetString(RaceListIds, json);
        }
    }
}
