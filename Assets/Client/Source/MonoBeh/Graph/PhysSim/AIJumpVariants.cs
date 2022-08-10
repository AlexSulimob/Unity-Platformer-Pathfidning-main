using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Assets/AiJump Variants")]
public class AIJumpVariants: ScriptableObject
{
    public static JumpAi emptyJump = new JumpAi(-1f, -1f);
    public JumpAi[] aiJumpList;
}