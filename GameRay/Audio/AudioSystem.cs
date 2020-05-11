using GameRay.Elements;
using GameRay.MapData;
using GameRay.MapData.Collision;
using SFML.Audio;
using SFML.System;
using System.Collections.Generic;
using static GameRay.Utils.MathUtils;

namespace GameRay.Audio
{
    public class AudioSystem
    {
        //Read-only properties
        public List<SoundBuffer> Sounds { get; internal set; }
        public Map Map { get; internal set; }
        public RayCaster RayCaster { get; internal set; }
        public int MaxSoundEffects { get; internal set; }

        //Private properties
        private Sound[] inPlaySounds;

        //Standar properties
        public Sprite Listener { get; set; }

        public AudioSystem(int maxSoundEffects, Map map = null, RayCaster rayCaster = null)
        {
            Map = map;
            RayCaster = rayCaster;
            Sounds = new List<SoundBuffer>();
            MaxSoundEffects = maxSoundEffects;
            inPlaySounds = new Sound[MaxSoundEffects];
        }

        //Public interface
        public int LoadSound(string fileName)
        {
            Sounds.Add(new SoundBuffer(fileName));
            return Sounds.Count - 1;
        }

        public (bool succes, int id) PlaySound(int soundNumber, bool loop, Vector2f? soundPosition = null)
        {
            if (RayCaster != null)
            {
                int soundIndex;
                Sound selected;

                for (selected = inPlaySounds[0], soundIndex = 0;
                     soundIndex < inPlaySounds.Length && inPlaySounds[soundIndex] != null;
                     selected = inPlaySounds[soundIndex++]) ;

                if (selected == null)
                {
                    Sound sound = new Sound(Sounds[soundNumber])
                    {
                        RelativeToListener = !soundPosition.HasValue,
                        Position = soundPosition.HasValue ? new Vector3f(soundPosition.Value.X, 0, soundPosition.Value.Y) : new Vector3f(0, 0, 0),
                        Loop = loop,
                        MinDistance = RayCaster.CellSize / 2
                    };

                    inPlaySounds[soundIndex] = sound;
                    sound.Play();

                    return (true, soundIndex);
                }
            }
            return (false, -1);
        }

        public void StopSound(int id)
        {
            if (inPlaySounds[id] != null)
            {
                inPlaySounds[id].Stop();
                inPlaySounds[id].Dispose();
                inPlaySounds[id] = null;
            }
        }

        public void UpdateAudio()
        {
            SFML.Audio.Listener.Position = new Vector3f(Listener.Position.X, 0, Listener.Position.Y);
            SFML.Audio.Listener.Direction = new Vector3f(CosD(Listener.Angle), 0, SinD(Listener.Angle));

            for (int i = 0; i < inPlaySounds.Length; i++)
                if (inPlaySounds[i] != null && inPlaySounds[i].Status == SoundStatus.Stopped)
                {
                    inPlaySounds[i].Dispose();
                    inPlaySounds[i] = null;
                }
        }
    }
}
