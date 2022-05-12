namespace CevarnsOfEvil
{

    public static class GameConstants
    {
        public const string GameName = "Caverns of Evil";

        // Layer IDs
        public const int levelLayer = 9;

        // Layer Masks
        public const int MobMask = (0x1 << 11) | (0x1 << 17);
        public const int DamageMask = (0x1 << 6) | MobMask;
        public const int LevelMask = 0x1 << levelLayer;
        public const int StaticMask = LevelMask | 1;
        public const int ObjectMask = DamageMask | StaticMask;
        public const int PlayerAttackMask = MobMask | StaticMask;
        public const int MobAttackMask = (0x1 << 6) | StaticMask;
        public const int JumpMask = 0x1 << 11 | LevelMask | 1;
        public const int InteractMask = LevelMask | 0x1 << 14;

        // Generator
        public const int BaseDoorHeight = 3;



    }
}