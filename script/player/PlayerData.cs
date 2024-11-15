using System;
using System.Collections.Generic;

using Godot;

namespace INTOnlineCoop.Script.Player
{
    /// <summary>
    /// Class for storing player data over network
    /// </summary>
    public partial class PlayerData : Resource
    {
        /// <summary>
        /// Username of the player
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Number of the player
        /// </summary>
        public int PlayerNumber { get; set; } = -1;

        /// <summary>
        /// Characters selected by the player
        /// </summary>
        public CharacterType[] Characters { get; }

        /// <summary>
        /// Creates new player data
        /// </summary>
        public PlayerData()
        {
            Characters = new[] { CharacterType.None, CharacterType.None, CharacterType.None, CharacterType.None };
        }

        /// <summary>
        /// Returns a selected character by its index
        /// </summary>
        /// <param name="index">Index of the character -> Between 0 and 3</param>
        /// <returns>Selected character type</returns>
        public CharacterType GetCharacterByIndex(int index)
        {
            return Characters[Math.Clamp(index, 0, 3)];
        }

        /// <summary>
        /// Changes the selected character at the given index
        /// </summary>
        /// <param name="index">Index of the character -> Between 0 and 3</param>
        /// <param name="characterType">Type of the character</param>
        public void SetCharacterByIndex(int index, CharacterType characterType)
        {
            Characters[Math.Clamp(index, 0, 3)] = characterType;
        }

        /// <summary>
        /// Updates the current instance with data from another instance
        /// </summary>
        /// <param name="newPlayerData"></param>
        public void UpdateInstance(PlayerData newPlayerData)
        {
            if (Name == "")
            {
                Name = newPlayerData.Name;
            }

            if (PlayerNumber == -1)
            {
                PlayerNumber = newPlayerData.PlayerNumber;
            }

            for (int i = 0; i < 4; i++)
            {
                if (Characters[i] == CharacterType.None)
                {
                    Characters[i] = newPlayerData.Characters[i];
                }
            }
        }

        /// <summary>
        /// Serializes the object into a dictionary
        /// </summary>
        /// <returns></returns>
        public static Godot.Collections.Dictionary<string, Variant> Serialize(PlayerData data)
        {
            Godot.Collections.Dictionary<string, Variant> dict = new() { { "Name", data.Name }, { "PlayerNumber", data.PlayerNumber } };
            for (int i = 0; i < 4; i++)
            {
                dict["Character" + i] = data.Characters[i].Name;
            }

            return dict;
        }

        /// <summary>
        /// Deserializes a dictionary into the current player data
        /// </summary>
        /// <param name="serializedDict"></param>
        public static PlayerData Deserialize(Godot.Collections.Dictionary<string, Variant> serializedDict)
        {
            string name = (string)serializedDict.GetValueOrDefault("Name", "");
            int playerNumber = (int)serializedDict.GetValueOrDefault("PlayerNumber", -1);
            PlayerData data = new()
            {
                Name = name,
                PlayerNumber = playerNumber
            };
            for (int i = 0; i < 4; i++)
            {
                string characterValue = (string)serializedDict.GetValueOrDefault("Character" + i, "None");
                CharacterType characterType = CharacterType.FromName(characterValue);
                data.SetCharacterByIndex(i, characterType);
            }

            return data;
        }
    }
}