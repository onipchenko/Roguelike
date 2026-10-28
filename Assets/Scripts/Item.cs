using UnityEngine;

public class Item : MonoBehaviour
{
    public int ID;
    public string itemName;
    
    public virtual void UseItem()
    {
        // Реализация использования предмета
        Debug.Log($"Использован предмет: {itemName}");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void PickUp()
    {
        // Отдельный контроллер UI подбора в проекте пока отсутствует.
        // Сам предмет и хотбар от этого не зависят.
        Debug.Log($"Подобран предмет: {itemName}", this);
    }
}
