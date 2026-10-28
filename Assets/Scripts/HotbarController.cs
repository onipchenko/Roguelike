using UnityEngine;
using UnityEngine.InputSystem;

public class HotbarController : MonoBehaviour
{
    [SerializeField] private GameObject hotbarPanel;
    [SerializeField] private GameObject hotbarSlotPrefab;
    [SerializeField, Min(1)] private int slotCount = 3;

    private Slot[] slots;
    private static readonly Key[] HotbarKeys =
    {
        Key.Digit1, Key.Digit2, Key.Digit3, Key.Digit4, Key.Digit5,
        Key.Digit6, Key.Digit7, Key.Digit8, Key.Digit9, Key.Digit0
    };

    private void Awake()
    {
        if (hotbarPanel == null)
            hotbarPanel = gameObject;

        if (hotbarSlotPrefab == null)
        {
            Debug.LogError("HotbarController: назначьте префаб HotbarSlot в Inspector.", this);
            enabled = false;
            return;
        }

        slotCount = Mathf.Clamp(slotCount, 1, HotbarKeys.Length);
        CreateHotbarSlots();
    }

    private void CreateHotbarSlots()
    {
        slots = new Slot[slotCount];
        int nextSlotIndex = 0;

        // Используем уже существующие прямые дочерние слоты, если они есть.
        for (int i = 0; i < hotbarPanel.transform.childCount && nextSlotIndex < slotCount; i++)
        {
            Slot existingSlot = hotbarPanel.transform.GetChild(i).GetComponent<Slot>();
            if (existingSlot != null)
                slots[nextSlotIndex++] = existingSlot;
        }

        // Недостающие слоты создаём из HotbarSlot.
        while (nextSlotIndex < slotCount)
        {
            GameObject slotObject = Instantiate(hotbarSlotPrefab, hotbarPanel.transform);
            slotObject.name = $"HotbarSlot ({nextSlotIndex + 1})";

            Slot slot = slotObject.GetComponent<Slot>();
            if (slot == null)
            {
                Debug.LogError("В префабе HotbarSlot отсутствует компонент Slot.", slotObject);
                Destroy(slotObject);
                enabled = false;
                return;
            }

            RectTransform rectTransform = slotObject.GetComponent<RectTransform>();
            if (rectTransform != null)
                rectTransform.anchoredPosition = Vector2.zero;

            slots[nextSlotIndex++] = slot;
        }
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        for (int i = 0; i < slots.Length; i++)
        {
            if (Keyboard.current[HotbarKeys[i]].wasPressedThisFrame)
                UseItemInSlot(i);
        }
    }

    private void UseItemInSlot(int index)
    {
        Slot slot = slots[index];
        if (slot == null || slot.currentItem == null)
            return;

        Item item = slot.currentItem.GetComponent<Item>();
        if (item == null)
            item = slot.currentItem.GetComponentInChildren<Item>();

        if (item != null)
            item.UseItem();
        else
            Debug.LogWarning($"В слоте хотбара {index + 1} лежит объект без компонента Item.", slot.currentItem);
    }
}
