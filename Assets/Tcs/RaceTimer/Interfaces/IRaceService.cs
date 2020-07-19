using System;
using System.Collections.Generic;
using Tcs.RaceTimer.Models;
using Tcs.RaceTimer.ViewModels;

namespace Tcs.RaceTimer.Interfaces
{
    public interface IRaceService
    {
        Race CurrentRace { get; }
        IObservable<Race> OnRaceLoaded { get; }
        IObservable<Race> OnNewRace { get; }
        IObservable<RacePlayerInfo> OnNewRacePlayer { get; }
        IObservable<Player> OnNewPlayer { get; }

        Player CreatePlayer(string name, int age, string email);
        Player CreatePlayer(string id, string name, int age, string email);
        RacePlayerInfo CreateRacePlayer(string raceId, string name, int age, string email, string teamName);
        RacePlayerInfo CreateRacePlayer(string id, string raceId, string name, int age, string email, string teamName, int no);
        Team CreateTeam(string id, string name);
        Team CreateTeam(string name);

        Race CreateRace(string name, long eventDate, int stages, string location);
        Race CreateRace(string id, string name, long eventDate, int stages, string location);
        IEnumerable<Race> GetAllRaces();
        
        void LoadRace(string raceId);
        RacePlayer AddTeamPlayer(string raceId, string teamId, string playerId);
        PlayerTime AddPlayerTime(string raceId, string playerId, TimeType type, LogTime time);
        
        Category CreateCategory(string name);
        Category CreateCategory(string id, string name);

        IEnumerable<RacePlayerInfo> GetAllRacePlayers(string raceId);
        IEnumerable<Player> GetAllPlayers();
        IEnumerable<Category> GetAllCategories();
        IEnumerable<Category> GetAllRaceCategories(string raceId);
        IEnumerable<CategoryPlayer> GetAllRaceCategoryPlayers(string raceId, string categoryId);

    }
}
