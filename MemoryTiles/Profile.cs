using System;
using System.Collections.Generic;

namespace MemoryTiles
{
    public class Profile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public Tuple<int, int> GamesPlayed { get; set; }
        public int CurentLevel { get; set; }
        public int SaveGameLevel { get; set; }
        public int Timer { get; set; }
        public List<List<int>> SaveGame { get; set; }

        public Profile(int id, string name, string avatar, int gamesPlayed, int gamesWon, int saveGameLevel, int timer, List<List<int>> saveGame = null)
        {
            this.Id = id;
            this.Name = name;
            this.Avatar = avatar;
            this.GamesPlayed = new Tuple<int, int>(gamesPlayed, gamesWon);
            this.CurentLevel = 1;
            this.SaveGameLevel = saveGameLevel;
            this.Timer = timer;
            this.SaveGame = saveGame;
        }

        public static bool operator ==(Profile profile1, Profile profile2)
        {
            return ReferenceEquals(profile1, profile2);
        }

        public static bool operator !=(Profile profile1, Profile profile2)
        {
            return !(ReferenceEquals(profile1, profile2));
        }

    }
}