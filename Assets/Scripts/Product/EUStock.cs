using UnityEngine;

[System.Serializable]
public class EUStock
{
    [SerializeField]
    private int id;

    [SerializeField]
    private int bidPrice; //market's buying price
    public int BidPrice { get { return bidPrice; } }

    [SerializeField]
    private int askPrice; //market's selling price
    public int AskPrice { get { return askPrice; } }

    [SerializeField]
    private int quantity;
    public int Quantity { get { return quantity; } set { quantity = value; } }

    public EUStock(int i, int bid, int ask, int q)
    {
        id = i;
        bidPrice = bid;
        askPrice = ask;
        quantity = q;
    }

    public void UpdatePrice(int increment)
    {
        bidPrice += increment;
        if (bidPrice < 0)
            bidPrice = 0;

        askPrice += increment;
        if (askPrice < 0)
            askPrice = 0;
    }
}
