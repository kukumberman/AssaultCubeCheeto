using System;
using Cucumba.Cheeto.Core;
using Cucumba.Cheeto.Structs;

namespace Cucumba.Cheeto.AssaultCube
{
    class PlayerEntity
    {
        private IntPtr _baseAddress = IntPtr.Zero;

        public IntPtr BaseAddress => _baseAddress;

        public int Health { get; private set; }
        public int Armor { get; private set; }
        public int Team { get; private set; }
        public string Name { get; private set; }
        public Vector3 HeadPosition { get; private set; }
        public Vector3 Origin { get; private set; }

        public void Update(IntPtr baseAddress)
        {
            _baseAddress = baseAddress;

            Health = MemoryContext.Instance.Read<int>(_baseAddress + Offsets.PlayerEntity_Health);
            Armor = MemoryContext.Instance.Read<int>(_baseAddress + Offsets.PlayerEntity_Armor);
            Team = MemoryContext.Instance.Read<int>(_baseAddress + Offsets.PlayerEntity_Team);

            Name = MemoryContext.Instance.ReadStringASCII(_baseAddress + Offsets.PlayerEntity_Name, 16);

            HeadPosition = MemoryContext.Instance.Read<Vector3>(_baseAddress + Offsets.PlayerEntity_Head);
            Origin = MemoryContext.Instance.Read<Vector3>(_baseAddress + Offsets.PlayerEntity_Origin);
        }

        public void TryForceJump()
        {
            if (Health <= 0)
            {
                return;
            }

            bool grounded = MemoryContext.Instance.Read<bool>(_baseAddress + Offsets.PlayerEntity_Grounded);

            if (grounded)
            {
                MemoryContext.Instance.Write(_baseAddress + Offsets.PlayerEntity_ForceJump, true);
            }
        }
    }
}