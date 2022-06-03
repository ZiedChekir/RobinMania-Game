using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.InteropServices;
using System.Numerics;

public class MarketplaceItem : MonoBehaviour
{
    // public TMPro.TMP_Text title;
    public Image sprite;
    public TMPro.TMP_Text price;
    public Button buyButton;


    [DllImport("__Internal")]
    private static extern void SendContractJs(string method, string abi, string contract, string args, string value, string gasLimit, string gasPrice);

    [DllImport("__Internal")]
    private static extern string SendContractResponse();

    [DllImport("__Internal")]
    private static extern void SetContractResponse(string value);

    [DllImport("__Internal")]
    private static extern void SendTransactionJs(string to, string value, string gasLimit, string gasPrice);

    [DllImport("__Internal")]
    private static extern string SendTransactionResponse();

    [DllImport("__Internal")]
    private static extern void SetTransactionResponse(string value);

    [DllImport("__Internal")]
    private static extern void SignMessage(string value);

    [DllImport("__Internal")]
    private static extern string SignMessageResponse();

    [DllImport("__Internal")]
    private static extern void SetSignMessageResponse(string value);

    [DllImport("__Internal")]
    private static extern int GetNetwork();
    [DllImport("__Internal")]
    private static extern void Web3Connect();

    [DllImport("__Internal")]
    private static extern string ConnectAccount();

    [DllImport("__Internal")]
    private static extern void SetConnectAccount(string value);
    [DllImport("__Internal")]
    private static extern void customSignAndSend(string method, string abi, string contract, string args, string value, string gasLimit, string gasPrice, string privateKey);
    [DllImport("__Internal")]
    private static extern string SendCustomTransactionResponse();
    [DllImport("__Internal")]
    private static extern void SetCustomTransactionResponse(string a);
    [DllImport("__Internal")]
    private static extern void CallContract(string method, string abi, string contract, string args);



    public void populateItem(string price, string tokenID, string index)
    {
        // this.title.text = title;
        sprite.sprite = ItemDatabase.ItemList[Int32.Parse(tokenID)].sprite;
        double priceDouble = double.Parse(price) / 1000000000000000000;

        this.price.text = priceDouble.ToString() + " KAI";

        //convert kai to wei


        buyButton.onClick.AddListener(() => buy(tokenID, index, price));
    }

    async void buy(string tokenID, string index, string price)
    {


        //maybe check if approved before approve all



        string method = "setApprovalForAll";



        SetContractResponse("");
        string args1 = "[\"" + PlayerPrefs.GetString("Account") + "\",\"" + ContractData.MarketplaceAddress + "\"]";
        CallContract("isApprovedForAll", ContractData.GameAbi, ContractData.GameAddress, args1);
        string response = SendContractResponse();
        while (response == "")
        {
            await new WaitForSeconds(0.3f);
            response = SendContractResponse();
        }
        SetContractResponse("");
        print("call result");
        print(response);
        string args;
        if (response == "false")
        {


            args = "[\"" + ContractData.MarketplaceAddress + "\",true]";
            SendContractJs(method, ContractData.GameAbi, ContractData.GameAddress, args, "0", "12", "23");

            response = SendContractResponse();
            while (response == "")
            {
                await new WaitForSeconds(0.3f);
                response = SendContractResponse();
            }
            SetContractResponse("");

        }
        method = "buyItem";
        args = "[\"" + ContractData.GameAddress + "\",\"" + tokenID + "\"," + index + "]";
        SendContractJs(method, ContractData.MarketplaceAbi, ContractData.MarketplaceAddress, args, price, "12", "23");

        response = SendContractResponse();
        while (response == "")
        {
            await new WaitForSeconds(0.3f);
            response = SendContractResponse();
        }
        SetContractResponse("");

        if (response.Length == 66)
        {
            print("buy success");
            InventoryBackend.Instance.AddItem(ItemDatabase.ItemList[Int32.Parse(tokenID)]);

        }
        else
        {
            print("buy failed");
        }



    }
}
