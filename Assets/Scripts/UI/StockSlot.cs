using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StockSlot : MonoBehaviour
{
    [SerializeField]
    private Town town;

    [SerializeField]
    private int productId;
    public int ProductId { get { return productId; } }

    [SerializeField]
    private int quantity;
    public int Quantity { get { return quantity; } set { quantity = value; } }

    [SerializeField]
    private TMP_Text stockText;
    public TMP_Text StockText { get { return stockText; } set { stockText = value; } }

    [SerializeField]
    private Image image;
    public Image Image { get { return image; } }

    [SerializeField]
    private UIManager uiMgr;

    void Awake()
    {
        uiMgr = UIManager.instance;
    }

    public void stockInit(int i, Town t)
    {
        //Debug.Log($"stock Init:{i}");
        town = t;
        productId = i;
        quantity = town.Warehouse[productId];
        stockText.text = quantity.ToString();
    }

    public void UpdateStockText()
    {
        stockText.text = town.Warehouse[productId].ToString();
    }

    public void UpdateQuantityStock(int n)
    {
        town.Warehouse[productId] += n;
        quantity = town.Warehouse[productId];
        stockText.text = quantity.ToString();
    }
}
