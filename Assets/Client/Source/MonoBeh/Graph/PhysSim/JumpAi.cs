[System.Serializable]
public struct JumpAi 
{
    public float RunSpeed;
    public float JumpForce; 
    
    public JumpAi(float RunSpeed, float JumpForce)
    {
        this.RunSpeed = RunSpeed;
        this.JumpForce = JumpForce;
    }
}
