using System;
using System.Collections;
using System.Numerics;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

public class InventoryBackend : MonoBehaviour
{
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


    private const int InventorySize = 9;

    public Transform[] Slots = new Transform[InventorySize];


    public Transform InventoryUI;
    public GameObject InventorySlot;

    public static InventoryBackend Instance;
  

    public  InventoryEvents inventoryEvent;





    private void Start()
    {

        InventoryUI = InventoryUI.transform.GetChild(0);
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
        InitializeInventory();
        
        AddItem(ItemDatabase.ItemList[5]);

        PopulateInventory();

        print(ContractData.GameAddress);



 
    }

    public async Task<string> BalanceOf(string account, int token)
    {

        SetContractResponse("");
        string[] obj = { account, token.ToString() };
        string args = JsonConvert.SerializeObject(obj);

        CallContract("balanceOf", ContractData.GameAbi, ContractData.GameAddress, args);
        string response = SendContractResponse();
        while (response == "")
        {
            await new WaitForSeconds(0.3f);
            response = SendContractResponse();
        }
        SetContractResponse("");

        print(response);
        return response;

    }


    async private void PopulateInventory()
    {
        foreach( var item in ItemDatabase.ItemList)
        {


            int tokenID = item.Key;
            int balanceOf = Int32.Parse(await BalanceOf(PlayerPrefs.GetString("Account"),tokenID));
            print("inventory balance is ");
            print(balanceOf);
            for (int i = 0; i < balanceOf; i++)
            {
                AddItem(ItemDatabase.ItemList[tokenID]);
            }  
            //string rpc = "https://dev-1.kardiachain.io/";
            //print("bef");
            //BigInteger balanceOf = await ERC1155.BalanceOf(chain, network, "0x584cb58df81ea75795b7043d906d6ce3adb0139c", PlayerPrefs.GetString("Account"), tokenID.ToString(),rpc);
            //print("balance of " + tokenID + " is " + balanceOf);
            //for (int i = 0; i < balanceOf; i++)
            //{
            //    AddItem(ItemDatabase.ItemList[tokenID]);
            //}    
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            OpenCloseInventory();
    }

    public bool hasItem(int itemID)
    {

        foreach (var item in Slots)
        {
            if (!item.GetComponent<InventorySlot>().isSlotEmpty() && item.GetComponent<InventorySlot>().item.id == itemID)
                return true;
        }

        return false;
       
    }




    void InitializeInventory()
    {
        //open inventory to initialize slots because u cant init them when it s not active
        OpenCloseInventory(1);
        for (int i = 0; i < InventorySize; i++)
        {
            Slots[i] = Instantiate(InventorySlot, InventoryUI.transform).transform;
        }
        //closes the inventory
  
        OpenCloseInventory(0);
    }





    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //+++++++++++++++++++++++ ADD ITEM ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++



    //add item at nearst slot
    public void AddItem(Item item)
    {
        //loop through all the slots and find the nearst emtpy slot and then assign the new item to that slot
        for (int i = 0; i < InventorySize; i++)
        {
            if (Slots[i].GetComponent<InventorySlot>().isSlotEmpty())
            {
                Slots[i].GetComponent<InventorySlot>().UpdateSlot(item);
                return;
            }
        }
        print("full inventory");
        return;
    }
     void AddItemByID(int itemID)
    {
        //loop through all the slots and find the nearst emtpy slot and then assign the new item to that slot
        for (int i = 0; i < InventorySize; i++)
        {
            if (Slots[i].GetComponent<InventorySlot>().isSlotEmpty())
            {

                Slots[i].GetComponent<InventorySlot>().UpdateSlot(ItemDatabase.ItemList[itemID]);

                return;
            }

        }
        print("full inventory");
        return;
    }
    public void AddItemTest(string s)
    {
        print(s);
        int x = Int32.Parse(s);

        AddItemByID(x);
    }

    void AddItemAtSlot(Item item, int SlotIndex)
    {
        if (SlotIndex < InventorySize && SlotIndex >= 0 )
        {

            Slots[SlotIndex].GetComponent<InventorySlot>().UpdateSlot(item);
            
        }
    }

    



    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //+++++++++++++++++++++++ Remove ITEM ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++




    Item RemoveItem(int SlotIndex)
    {
        Item temp = Slots[SlotIndex].GetComponent<InventorySlot>().item;
        if (Slots[SlotIndex].GetComponent<InventorySlot>().isSlotEmpty())
            return null;

        Slots[SlotIndex].GetComponent<Slot>().UpdateSlot(null);
        
        return temp;
    }


    //removes an item but returns nothing used for the event system
    void RemoveItemNoReturn(int SlotIndex)
    {
        RemoveItem(SlotIndex);
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //+++++++++++++++++++++++ Utils ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


    // status 0 close inventory 
    //status 1 open inventory 
    //status -1 close or open invenotry based on the inventoryUI state
   public  void OpenCloseInventory(int status = -1)
    {
        if( status == 0)
        {
            InventoryUI.parent.gameObject.SetActive(false);
        }
        else if (status == 1)
        {
            InventoryUI.parent.gameObject.SetActive(true);
        }
        else
        {
            InventoryUI.parent.gameObject.SetActive(!InventoryUI.parent.gameObject.activeSelf);
        }
        
    }
    //INVENTORY EVENTS



    private void OnEnable()
    {
        inventoryEvent.ItemRemovedEvent += RemoveItemNoReturn;
    }
    private void OnDisable()
    {
        inventoryEvent.ItemRemovedEvent -= RemoveItemNoReturn;
    }
}
