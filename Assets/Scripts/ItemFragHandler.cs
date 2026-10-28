using UnityEngine;
using UnityEngine.EventSystems;

public class ItemFragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform originalParent;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Canvas canvas;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();

        // Находим корневой Canvas для правильного расчета координат
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;

        // Переносим в корень канваса, чтобы предмет был поверх всех остальных UI элементов
        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();

        canvasGroup.blocksRaycasts = false; // Отключаем лучи, чтобы попадать под мышь не по самому себе, а по слотам под ним
        canvasGroup.alpha = 0.6f;          // Полупрозрачность при перетаскивании
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Корректное перемещение для UI в зависимости от масштаба Canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out Vector2 localPoint
        );
        rectTransform.anchoredPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true; // Возвращаем кликабельность

        Slot originalSlot = originalParent.GetComponent<Slot>();

        // 1. Ищем слот под курсором (даже если попали прямо в другой предмет, найдем его родительский слот)
        Slot dropSlot = null;
        if (eventData.pointerEnter != null)
        {
            dropSlot = eventData.pointerEnter.GetComponent<Slot>();
            if (dropSlot == null)
            {
                dropSlot = eventData.pointerEnter.GetComponentInParent<Slot>();
            }
        }

        // Если бросили вообще мимо любого слота
        if (dropSlot == null)
        {
            transform.SetParent(originalParent);
            rectTransform.anchoredPosition = Vector2.zero;
            return;
        }

        // 2. Логика перемещения
        if (dropSlot == originalSlot)
        {
            // Вернули в тот же слот
            transform.SetParent(originalParent);
        }
        else if (dropSlot.currentItem != null)
        {
            // В целевом слоте УЖЕ ЕСТЬ предмет — меняем их местами!
            GameObject otherItemObj = dropSlot.currentItem;
            Transform otherItemTransform = otherItemObj.transform;

            // Старый предмет отправляем в наш исходный слот
            otherItemTransform.SetParent(originalSlot.transform);
            otherItemTransform.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            originalSlot.currentItem = otherItemObj;

            // Наш перетаскиваемый предмет ставим в целевой слот
            transform.SetParent(dropSlot.transform);
            dropSlot.currentItem = gameObject;
        }
        else
        {
            // Целевой слот пустой
            originalSlot.currentItem = null; // Очищаем старый слот

            transform.SetParent(dropSlot.transform);
            dropSlot.currentItem = gameObject; // Занимаем новый слот
        }

        rectTransform.anchoredPosition = Vector2.zero;
    }
}