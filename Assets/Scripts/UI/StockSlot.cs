using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StockSlot : MonoBehaviour, IDropHandler
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
    private StockDrag stockDrag;

    [SerializeField]
    private UIManager uiMgr;

    [SerializeField]
    private GameManager gameMgr;

    [SerializeField]
    private EuropeManager EUMgr;

    void Awake()
    {
        uiMgr = UIManager.instance;
        gameMgr = GameManager.instance;
        EUMgr = EuropeManager.instance;
    }

    public void stockInit(int i, Town t, bool Europe)
    {
        //Debug.Log($"stock Init:{i}");
        town = t;
        productId = i;
        image.sprite = gameMgr.ProductData[i].icons[0];
        quantity = town.Warehouse[productId];

        if (Europe)
            stockText.text = $"{EUMgr.EuropeStocks[i].BidPrice}/{EUMgr.EuropeStocks[i].AskPrice}";
        else
            stockText.text = quantity.ToString();

        if (stockDrag == null)
        {
            stockDrag = GenerateStockDrag(productId, this);
        }
    }

    public void stockInitEurope(int i)
    {
        //Debug.Log($"stock Init Europe:{i}");
        productId = i;
        image.sprite = gameMgr.ProductData[i].icons[0];

        quantity = EUMgr.EuropeStocks[productId].Quantity;
        stockText.text =
            $"{EUMgr.EuropeStocks[productId].BidPrice}/{EUMgr.EuropeStocks[productId].AskPrice}";

        if (stockDrag == null)
        {
            stockDrag = GenerateStockDrag(productId, this);
        }
    }

    public void UpdateStockText()
    {
        stockText.text = town.Warehouse[productId].ToString();
    }

    public void UpdateQuantityStock(int n)
    {
        if (uiMgr.InEurope)
        {
            EUMgr.EuropeStocks[productId].Quantity += n;
            quantity = EUMgr.EuropeStocks[productId].Quantity;

            stockText.text =
                $"{EUMgr.EuropeStocks[productId].BidPrice}/{EUMgr.EuropeStocks[productId].AskPrice}";
        }
        else
        {
            town.Warehouse[productId] += n;
            quantity = town.Warehouse[productId];
            stockText.text = quantity.ToString();
        }
    }

    public StockDrag GenerateStockDrag(int id, StockSlot slot)
    {
        //Debug.Log($"Generate Cargo Init:{id}");

        GameObject stockDragObj =
            Instantiate(uiMgr.StockDragPrefab, slot.Image.transform.position, Quaternion.identity, slot.transform);

        StockDrag stockDrag = stockDragObj.GetComponent<StockDrag>();
        stockDrag.StockDragInit(slot);

        return stockDrag;
    }

    public void ToggleRayCastStockDrag(bool flag)
    {
        if (stockDrag != null)
            stockDrag.Image.raycastTarget = flag;
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject obj = eventData.pointerDrag;
        CargoDrag cargoDrag = obj.GetComponent<CargoDrag>();

        if (cargoDrag == null)
            return;

        if (cargoDrag.Cargo.ProductID != productId)
            return;

        UpdateQuantityStock(cargoDrag.Cargo.Quantity);
        cargoDrag.RemoveCargoListFromShip();

        if (uiMgr.InEurope)
        {
            uiMgr.UpdateCargoSlots(cargoDrag.Ship, uiMgr.CargoSlotsEurope);
            //Sell to market
            gameMgr.PlayerFaction.Money +=
                cargoDrag.Cargo.Quantity * EUMgr.EuropeStocks[productId].BidPrice;

            uiMgr.UpdateMoneyEuropeText();
        }
        else
        {
            uiMgr.UpdateCargoSlots(cargoDrag.Ship, uiMgr.CargoSlots);
        }

        //Debug.Log("CargoDrag - On Drop - on StockSlot");
        uiMgr.ToggleStockDragRaycast(true);

        Destroy(obj);
    }
}
