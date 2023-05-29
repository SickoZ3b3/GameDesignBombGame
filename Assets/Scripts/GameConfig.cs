using UnityEngine;

namespace StarterAssets
{
    [CreateAssetMenu(fileName = "FILENAME", menuName = "游戏配置", order = 0)]
    public class GameConfig : ScriptableObject
    {
        [Header("炸弹倒计时")]
        public float BombExploreTime = 10;
        
        [Header("炸弹扔出后爆炸倒计时")]
        public float BombTrowExploreTime = 5;
        
        [Header("炸弹爆炸后重置倒计时")]
        public float BombRebornTime = 3;

        [Header("玩家重生倒计时")]
        public float PlayerRebornTime = 3;
    }
}