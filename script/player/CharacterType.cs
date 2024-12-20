using System;
using System.Collections.Generic;
using System.Linq;

using Godot;

namespace INTOnlineCoop.Script.Player
{
    /// <summary>
    /// Contains name + textures of available characters
    /// </summary>
    public partial class CharacterType : RefCounted
    {
        /// <summary> Character that is not available </summary>
        public static readonly CharacterType None = new("None", 0);

        /// <summary> Amara (Borderlands 3) </summary>
        public static readonly CharacterType Amara = new("Amara", 10);

        /// <summary> Athena (Borderlands Pre-Sequel) </summary>
        public static readonly CharacterType Athena = new("Athena", 16);

        /// <summary> Axton (Borderlands 2) </summary>
        public static readonly CharacterType Axton = new("Axton", 11);

        /// <summary> Gaige (Borderlands 2) </summary>
        public static readonly CharacterType Gaige = new("Gaige", 13);

        /// <summary> Krieg (Borderlands 2) </summary>
        public static readonly CharacterType Krieg = new("Krieg", 17);

        /// <summary> Maja (Borderlands 2) </summary>
        public static readonly CharacterType Maja = new("Maja", 13);

        /// <summary> Moze (Borderlands 3) </summary>
        public static readonly CharacterType Moze = new("Moze", 19);

        /// <summary> Nisha (Borderlands Pre-Sequel) </summary>
        public static readonly CharacterType Nisha = new("Nisha", 10);

        /// <summary> Salvador (Borderlands 2) </summary>
        public static readonly CharacterType Salvador = new("Salvador", 13);

        /// <summary> Wilhelm (Borderlands Pre-Sequel) </summary>
        public static readonly CharacterType Wilhelm = new("Wilhelm", 12);

        /// <summary> Zero (Borderlands 2) </summary>
        public static readonly CharacterType Zero = new("Zero", 14);

        /// <summary>
        /// List of all available characters
        /// </summary>
        public static IEnumerable<CharacterType> Values
        {
            get
            {
                yield return Amara;
                yield return Athena;
                yield return Axton;
                yield return Gaige;
                yield return Krieg;
                yield return Maja;
                yield return Moze;
                yield return Nisha;
                yield return Salvador;
                yield return Wilhelm;
                yield return Zero;
            }
        }

        /// <summary>
        /// Name of the character
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Index of the last idle frame
        /// </summary>
        public int LastIdleFrame { get; private set; }

        /// <summary>
        /// Head texture of the character
        /// </summary>
        public Texture2D HeadTexture { get; private set; }

        /// <summary>
        /// Body texture of the character
        /// </summary>
        public Texture2D BodyTexture { get; private set; }

        /// <summary>
        /// Sprite frames of the character
        /// </summary>
        public SpriteFrames SpriteFrames { get; private set; }

        private CharacterType(string name, int lastIdleFrame)
        {
            Name = name;
            LastIdleFrame = lastIdleFrame;
            if (name == "None")
            {
                return;
            }

            HeadTexture = GD.Load<Texture2D>($"res://assets/sprites/game_figure/{name.ToLower()}/head.png");
            BodyTexture = GD.Load<Texture2D>($"res://assets/sprites/game_figure/{name.ToLower()}/body.png");
            SpriteFrames = GD.Load<SpriteFrames>($"res://assets/sprites/game_figure/{name.ToLower()}/sprite.res");
        }

        /// <summary>
        /// Returns the character object with the given name
        /// </summary>
        /// <param name="name">Name of the character</param>
        /// <returns>Character object</returns>
        public static CharacterType FromName(string name)
        {
            try
            {
                return Values.Single(character => character.Name == name);
            }
            catch (Exception)
            {
                GD.PrintErr($"Couldn't convert Character {name} to CharacterType!");
                return None;
            }
        }

        /// <summary>
        /// Converts the type to a string
        /// </summary>
        /// <returns>Converted string</returns>
        public override string ToString()
        {
            return "CharacterType: " + Name;
        }
    }
}