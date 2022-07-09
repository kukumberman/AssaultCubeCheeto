namespace Cucumba.Cheeto.AssaultCube
{
    class Offsets
    {
        public const int WindowSize = 0x110C94;
        public const int GameMode = 0x10F49C;
        public const int MapName = 0x102F78;
        public const int GameVersion = 0x110CF4;

        public const int LocalPlayer = 0x10F4F4;
        public const int EntityList = 0x10F4F8;
        public const int EntityListCount = 0x10F500;
        public const int ViewMatrix = 0x101AE8;

        public const int PlayerEntity_Head = 0x4;
        public const int PlayerEntity_Velocity = 0x10;
        public const int PlayerEntity_Origin = 0x34;
        public const int PlayerEntity_ForceJump = 0x6B;
        public const int PlayerEntity_Grounded = 0x69;

        public const int PlayerEntity_ForceAttack = 0x224;
        public const int PlayerEntity_Name = 0x225;
        public const int PlayerEntity_Health = 0xF8;
        public const int PlayerEntity_Armor = 0xFC;

        public const int PlayerEntity_Yaw = 0x40;   //[0..360]
        public const int PlayerEntity_Pitch = 0x44; //[-90..90]
        public const int PlayerEntity_Roll = 0x48;

        public const int PlayerEntity_KillCount = 0x1FC;
        public const int PlayerEntity_DeathCount = 0x204;
        public const int PlayerEntity_Team = 0x32C;
    }
}
