using UnityEngine;

namespace Untility
{
    public class TextColor
    {
        public static Color soulColor = new Color(255f / 255f, 163f / 255f, 254f / 255f);             //魂的文本颜色
        public static Color damageColor = Color.red;                                                                    //伤害的 文本颜色
        public static Color hpColor = new Color(204f / 255f, 255f / 255f, 68f / 255f);                 //生命的文本颜色
        public static Color mpColor = new Color(163f / 255f, 250f / 255f, 255f / 255f);           //魔法的文本颜色
        public static Color warningColor = new Color(255f / 255f, 53f / 255f, 53f / 255f);           //警告的文本颜色

        //public static Color monsterSkillColor = new Color(0.88f, 0.31f, 0.21f);             //怪物使用技能
        public static Color monsterSkillColor = new Color(255f / 255f, 250f / 255f, 228f / 255f);            //怪物使用技能
        public static Color systemTipsColor = new Color(255f / 255f, 250f / 255f, 228f / 255f); //系统提示

        public static string paramColorHead = "<color=#FFD25AFF>";          //天赋、技能、词缀的参数颜色
        public static string invalidParamColorHead = "<color=#4A412CFF>";          //天赋、技能、词缀的参数颜色

        public static string warningColorHead = "<color=#FF3535FF>";            //警告的文本颜色
        public static string critDamageColorHead = "<color=#D824FFFF>";       //触发暴击时掉落的文本颜色
        public static string triggerPayerSkillColorHead = "<color=#00FFBAFF>";//触发了玩家技能的文本角色
        public static string boostPropertyColorHead = "<color=#CCFF44FF>";
        public static string deboostPropertyColorHead = "<color=#FF9090FF>";

        public static string whiteColorHead = "<color=#E5E5E5FF>";
        public static string greenColorHead = "<color=#B5FF6CFF>";
        public static string grayColorHead = "<color=#999999>";

        public static string colorEnd = "</color>";

        static public Color D_Color = new Color(229f / 255f, 229f / 255f, 229f / 255f);
        static public Color C_Color = new Color(181 / 255f, 255 / 255f, 108 / 255f);
        static public Color B_Color = new Color(150 / 255f, 222f / 255f, 255f / 255f);
        static public Color A_Color = new Color(255f / 255f, 164 / 255f, 254f / 255f);

       
    }
}

