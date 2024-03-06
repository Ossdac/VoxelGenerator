using UnityEngine;

[System.Serializable]
public abstract class GameOfLife : MonoBehaviour
{
    [SerializeField]protected World world;

    [SerializeField] protected int size = 50;

    [SerializeField] protected Vector2Int birthRange = new Vector2Int(3, 3);
    [SerializeField] protected Vector2Int deathRange = new Vector2Int(1, 4);

    [SerializeField] protected float saturationValue = 0.5f;

    protected int lowBirth;
    protected int highBirth;
    protected int lowDeath;
    protected int highDeath;

    private void InitializeState()
    {
        lowBirth = birthRange.x; highBirth = birthRange.y;
        lowDeath = deathRange.x; highDeath = deathRange.y; 
    }

    public void PerformUpdate()
    {
        InitializeState();
        CustomUpdateState();
    }

    public abstract void InitializePattern();
    public abstract void CustomUpdateState();

    public abstract void RenderWorld();
}
