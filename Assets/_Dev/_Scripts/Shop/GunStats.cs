namespace Game.Shop
{
    [System.Serializable]
    public class GunStats : ItemStats
    {
        public UpgradeTypes UpgradeType;
        public float UpgradeAmount;
        public int GunLevel;
        public int GunPartLevel;
    }
}