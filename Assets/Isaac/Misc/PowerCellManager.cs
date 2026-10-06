using UnityEngine;
using UnityEngine.Events;

public class PowerCellManager : MonoBehaviour
{
    public static PowerCellManager Instance { get; private set; }

    [Header("Door / Barrier Reference")]
    [SerializeField] private DoorController targetDoor;

    [Header("Events (Optional Visual UI)")]
    public UnityEvent<int, int> onCellCountChanged; // Current destroyed, Total required

    private int totalCells;
    private int destroyedCells = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void RegisterCell()
    {
        totalCells++;
    }

    public void OnCellDestroyed()
    {
        destroyedCells++;
        Debug.Log($"[PowerCellManager] Cell destroyed! ({destroyedCells}/{totalCells})");

        onCellCountChanged?.Invoke(destroyedCells, totalCells);

        if (destroyedCells >= totalCells)
        {
            Debug.Log("[PowerCellManager] All cells destroyed Opening door");
            if (targetDoor != null)
            {
                targetDoor.OpenDoor();
            }
        }
    }
}