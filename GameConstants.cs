namespace CevarnsOfEvil
{

    public static class GameConstants
    {
        public const string GameName = "Caverns of Evil";

        // Layer IDs
        public const int DefaultLayerLayer   =  0;
        public const int TransparentFXLayer  =  1;
        public const int IgnoreRaycastLayer  =  2;
        public const int WaterLayer          =  4;
        public const int UILayer             =  5;
        public const int PlayerLayer         =  6;
        public const int PlayerAttackLayer   =  7;
        public const int LightingLayer       =  8;
        public const int levelLayer          =  9;
        public const int SubmergedLayer      = 10;
        public const int DamageableeLayer    = 11;
        public const int PostprocessingLayer = 12;
        public const int UnsafeLayer         = 13;
        public const int InteractiveLayer    = 14;
        public const int TriggerLayer        = 15;
        public const int ProjectileLayer     = 16;
        public const int GhostLayer          = 17;
        //TODO/FIXME: Include all layers!

        // Layer Masks
        public const int MobMask = (0x1 << DamageableeLayer) | (0x1 << GhostLayer);
        public const int DamageMask = (0x1 << PlayerLayer) | MobMask;
        public const int LevelMask = 0x1 << levelLayer;
        public const int StaticMask = LevelMask | 1;
        public const int ObjectMask = DamageMask | StaticMask;
        public const int PlayerAttackMask = MobMask | StaticMask;
        public const int MobAttackMask = (0x1 << PlayerLayer) | StaticMask;
        public const int JumpMask = 0x1 << DamageableeLayer | LevelMask | 1;
        public const int InteractMask = LevelMask | 0x1 << InteractiveLayer;

        // Generator
        public const int BaseDoorHeight = 3;



    }
}
