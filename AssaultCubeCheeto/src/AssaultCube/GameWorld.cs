using System;
using System.Collections.Generic;
using Cucumba.Cheeto.Core;
using Cucumba.Cheeto.Structs;

namespace Cucumba.Cheeto.AssaultCube
{
    class GameWorld : IDisposable
    {
        private const int ADDRESS_RECOIL_FUNCTION_CALL = 0x63786;
        private readonly byte[] BYTES_RECOIL_FUNCTION_INSTRUCTIONS = new byte[10] { 0x50, 0x8D, 0x4C, 0x24, 0x1C, 0x51, 0x8B, 0xCE, 0xFF, 0xD2 };

        private readonly List<GameMode> TeamGameModes = new List<GameMode>
        { 
            GameMode.TEAMDEATHMATCH,
            GameMode.TEAMSURVIVOR,
            GameMode.CTF,
            GameMode.BOTTEAMDEATHMATCH,
            GameMode.TEAMONESHOTONEKILL,
            GameMode.HUNTTHEFLAG,
            GameMode.TEAMKEEPTHEFLAG,
            GameMode.TEAMPF,
            GameMode.TEAMLSS,
            GameMode.BOTTEAMSURVIVOR,
            GameMode.BOTTEAMONESHOTONEKILL
        };

        private readonly IntPtr _clientAddress;

        public readonly PlayerEntity LocalPlayer;
        public readonly List<PlayerEntity> Players;
        public readonly Camera Camera;

        private bool _isRecoilEnabled = true;
        private GameMode _gameMode;

        public GameMode GameMode
        { 
            get => _gameMode;
            private set
            {
                if (value != _gameMode)
                {
                    _gameMode = value;
                    IsTeamGameMode = TeamGameModes.Contains(_gameMode);
                }
            }
        }

        public string MapName { get; private set; }
        public int GameVersion { get; private set; }

        public bool IsTeamGameMode { get; private set; }

        public GameWorld(IntPtr clientAddress)
        {
            _clientAddress = clientAddress;

            Camera = new Camera();
            LocalPlayer = new PlayerEntity();
            Players = new List<PlayerEntity>(32);

            GameVersion = MemoryContext.Instance.Read<int>(_clientAddress + Offsets.GameVersion);
        }

        public void Dispose()
        {
            if (!_isRecoilEnabled)
            {
                ToggleRecoil(true);
            }
        }

        public void ToggleRecoil()
        {
            _isRecoilEnabled = !_isRecoilEnabled;
            ToggleRecoil(_isRecoilEnabled);
        }

        public void ToggleRecoil(bool active)
        {
            var address = _clientAddress + ADDRESS_RECOIL_FUNCTION_CALL;

            if (active)
            {
                MemoryContext.Instance.WriteBytes(address, BYTES_RECOIL_FUNCTION_INSTRUCTIONS);
            }
            else
            {
                MemoryContext.Instance.NopBytes(address, BYTES_RECOIL_FUNCTION_INSTRUCTIONS.Length);
            }
        }

        public void Update()
        {
            GameMode = (GameMode)MemoryContext.Instance.Read<int>(_clientAddress + Offsets.GameMode);
            MapName = MemoryContext.Instance.ReadStringASCII(_clientAddress + Offsets.MapName, 16);

            UpdateCamera();
            UpdatePlayers();
        }

        private void UpdateCamera()
        {
            Camera.ScreenSize = MemoryContext.Instance.Read<Vector2Int>(_clientAddress + Offsets.WindowSize);

            var viewMatrixBytes = MemoryContext.Instance.ReadBytes(_clientAddress + Offsets.ViewMatrix, 16 * 4);

            for (int i = 0, length = Camera.ViewMatrix.Values.Length; i < length; i++)
            {
                Camera.ViewMatrix.Values[i] = BitConverter.ToSingle(viewMatrixBytes, i * 4);
            }
        }

        private void UpdatePlayers()
        {
            var localPlayerAddress = MemoryContext.Instance.ReadPointer(_clientAddress + Offsets.LocalPlayer);
            if (localPlayerAddress == IntPtr.Zero)
            {
                return;
            }

            LocalPlayer.Update(localPlayerAddress);

            Players.Clear();

            var playersCount = MemoryContext.Instance.Read<int>(_clientAddress + Offsets.EntityListCount);

            var entityListAddress = MemoryContext.Instance.ReadPointer(_clientAddress + Offsets.EntityList);
            if (entityListAddress == IntPtr.Zero)
            {
                return;
            }

            for (int i = 1; i < playersCount; i++)
            {
                var entityAddress = MemoryContext.Instance.ReadPointer(entityListAddress + i * 4);
                if (entityAddress == IntPtr.Zero)
                {
                    continue;
                }

                var entity = new PlayerEntity();
                entity.Update(entityAddress);
                Players.Add(entity);
            }
        }
    }
}