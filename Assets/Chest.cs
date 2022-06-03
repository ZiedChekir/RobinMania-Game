using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

public class Chest : MonoBehaviour
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



    public int ItemID;
    public Sprite Opened;
    public Sprite Closed;
    public GameObject ClaimNftPanel;
    public bool opened;
    private bool canOpen = false; 
    public int ChestID;
    SpriteRenderer SR;
    Transform Player;
    NFTPanel PanelScript;

  

    // Start is called before the first frame update
    async void Start()
    {
        ClaimNftPanel.SetActive(false);
        ChestID = transform.GetSiblingIndex();
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        PanelScript = ClaimNftPanel.gameObject.GetComponent<NFTPanel>();
        SR = GetComponent<SpriteRenderer>();
    }


    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.O))
            PlayerPrefs.DeleteAll();
        
    }
    //// Update is called once per frame
    ///
    ///
    private void OnTriggerEnter2D(Collider2D collision)
    {

        
        if (collision.gameObject.CompareTag("Player") && !opened)
        {
            ClaimNftPanel.SetActive(true);
            PanelScript.InitializePanel(ItemDatabase.ItemList[ItemID].Name, ItemDatabase.ItemList[ItemID].sprite, ItemID);
            PanelScript.button.onClick.AddListener(()=>Claim());
          

        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !opened)
        {
            ClaimNftPanel.SetActive(false);
            PanelScript.button.onClick.RemoveAllListeners();
        }
    }

    async public void Claim()
    {
        try
        {
            var x = await allowToMint(ItemID);

        }
        catch (Exception e)
        {
            print(e);
        }

        try
        {
            string y = await Mint(ItemID);
            if (y.Length == 66)
            {
                print("mint successful");
                InventoryBackend.Instance.AddItem(ItemDatabase.ItemList[ItemID]);
                ToastManager.Instance.AddToast("Item Successfully minted");
                SR.sprite = Opened;
                PlayerPrefs.SetInt("Chest" + ChestID, 1);


            }
            else
            {
                
                print("minting failed");
            }
        }
        catch (Exception e)
        {
            print(e);
        }

    }

    public async Task<string> Mint(int tokenId)
    {
        string args = "[\"" + tokenId + "\"]";
        string getTokenPrice = "verifyPrice";




        SetContractResponse("");
        CallContract(getTokenPrice, ContractData.GameAbi, ContractData.GameAddress, args);
        string response = SendContractResponse();
        while (response == "")
        {
            await new WaitForSeconds(0.3f);
            response = SendContractResponse();
        }
        SetContractResponse("");


        string method = "mint";

        SetContractResponse("");
     
        SendContractJs(method, ContractData.GameAbi, ContractData.GameAddress, args, "0", "12", "23");
        

        response = SendContractResponse();
        while (response == "")
        {
            await new WaitForSeconds(0.3f);
            response = SendContractResponse();
        }
        SetContractResponse("");

        print("minting response");

        print(response);




        return response;


    }


    class obj
    {
        public string playerAddress;
        public int tokenId;
    }
    public async Task<string> allowToMint(int tokenID)
    {
        try
        {

            obj Obj = new obj() { playerAddress = PlayerPrefs.GetString("Account"), tokenId = tokenID };

            string args = JsonConvert.SerializeObject(Obj);
            int gasLimit = 1245000;
            int gasPrice = 80000000;
            SetCustomTransactionResponse("");
    

            customSignAndSend("allowToMint", ContractData.GameAbi, ContractData.GameAddress, args, "0", "0x" + gasLimit.ToString("X"), "0x" + gasPrice.ToString("X"), "a10f6af85c7540e4ac1be6ca74b06b9c56c69eb49b88506dd8b425ae92fe22db");
            string response = SendCustomTransactionResponse();

            while (response == "")
            {
                await new WaitForSeconds(0.3f);
                response = SendCustomTransactionResponse();
            }
            SetCustomTransactionResponse("");
            print(response);

            return response;
        }
        catch (Exception e)
        {
            print(e);
            return "error";
        }
    }
}
