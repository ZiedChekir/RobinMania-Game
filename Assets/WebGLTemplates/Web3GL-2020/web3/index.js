// load network.js to get network/chain id
document.body.appendChild(Object.assign(document.createElement("script"), { type: "text/javascript", src: "./network.js" }));
// load web3modal to connect to wallet
document.body.appendChild(Object.assign(document.createElement("script"), { type: "text/javascript", src: "./web3/lib/web3modal.js" }));
// load web3js to create transactions
document.body.appendChild(Object.assign(document.createElement("script"), { type: "text/javascript", src: "./web3/lib/ethers-5.6.4.min.js" }));
document.body.appendChild(Object.assign(document.createElement("script"), { type: "text/javascript", src: "./web3/lib/web3.min.js" }));

// uncomment to enable torus wallet
// document.body.appendChild(Object.assign(document.createElement("script"), { type: "text/javascript", src: "https://unpkg.com/@toruslabs/torus-embed" }));
// uncomment to enable walletconnect
// document.body.appendChild(Object.assign(document.createElement("script"), { type: "text/javascript", src: "https://unpkg.com/@walletconnect/web3-provider@1.2.1/dist/umd/index.min.js" }));

// load web3gl to connect to unity
window.web3gl = {
  networkId: 0,
  connect,
  connectAccount: "",
  signMessage,
  signMessageResponse: "",
  sendTransaction,
  sendTransactionResponse: "",
  sendContract,
  sendContractResponse: "",
  customSignAndSend,
  customSignAndSendResponse:"",
  Call, 
  GetOrders,
  ordersResponse:"",
  

};

// will be defined after connect()
let provider;
let providerWeb3;
let web3;
let signer;
/*
paste this in inspector to connect to wallet:
window.web3gl.connect()
*/

async function connect(){
  provider = new ethers.providers.Web3Provider(window.ethereum)
  await provider.send("eth_requestAccounts", []);
  signer = await  provider.getSigner()
  // const myContract = new ethers.Contract(contractAddress, abi, provider);
  // var x = await myContract.balanceOf("0xf39Fd6e51aad88F6F4ce6aB8827279cffFb92266","2");
const network = await provider.getNetwork();
web3gl.networkId = parseInt(network.chainId);
   // if current network id is not equal to network id, then switch
  if (web3gl.networkId != window.web3ChainId) {
    await window.ethereum
      .request({
        method: "wallet_switchEthereumChain",
        params: [{ chainId: `0x${window.web3ChainId.toString(16)}` }], // chainId must be in hexadecimal numbers
      })
      .catch(() => {
        window.location.reload();
      });
  }
  web3gl.connectAccount = await signer.getAddress();

 
// refresh page if player changes account
  provider.on("accountsChanged", (accounts) => {
    window.location.reload();
  });

  // update if player changes network
  provider.on("chainChanged", (chainId) => {
    web3gl.networkId = parseInt(chainId);
  });


}
async function connectweb3() {
  // uncomment to enable torus and walletconnect
  const providerOptions = {
    // torus: {
    //   package: Torus,
    // },
    // walletconnect: {
    //   package: window.WalletConnectProvider.default,
    //   options: {
    //     infuraId: "00000000000000000000000000000000",
    //   },
    // },
  };

  const web3Modal = new window.Web3Modal.default({
    providerOptions,
  });

  web3Modal.clearCachedProvider();

  // set provider
  provider = new ethers.providers.Web3Provider(window.ethereum)
   provider = await web3Modal.connect();
 web3 = new Web3(provider);

  // set current network id
  web3gl.networkId = parseInt(provider.chainId);

  // if current network id is not equal to network id, then switch
  if (web3gl.networkId != window.web3ChainId) {
    await window.ethereum
      .request({
        method: "wallet_switchEthereumChain",
        params: [{ chainId: `0x${window.web3ChainId.toString(16)}` }], // chainId must be in hexadecimal numbers
      })
      .catch(() => {
        window.location.reload();
      });
  }

  // set current account
  web3gl.connectAccount = provider.selectedAddress;

  // refresh page if player changes account
  provider.on("accountsChanged", (accounts) => {
    window.location.reload();
  });

  // update if player changes network
  provider.on("chainChanged", (chainId) => {
    web3gl.networkId = parseInt(chainId);
  });
}

/*
paste this in inspector to connect to sign message:
window.web3gl.signMessage("hello")
*/
async function signMessage(message) {
  try {
    const from = (await web3.eth.getAccounts())[0];
    const signature = await web3.eth.personal.sign(message, from, "");
    window.web3gl.signMessageResponse = signature;
  } catch (error) {
    window.web3gl.signMessageResponse = error.message;
  }
}

/*
paste this in inspector to send eth:
const to = "0xdD4c825203f97984e7867F11eeCc813A036089D1"
const value = "12300000000000000"
const gasLimit = "21000" // gas limit
const gasPrice = "33333333333"
window.web3gl.sendTransaction(to, value, gasLimit, gasPrice);
*/
async function sendTransaction(to, value, gasLimit, gasPrice) {
  const from = (await web3.eth.getAccounts())[0];
  web3.eth
    .sendTransaction({
      from,
      to,
      value,
      gas: gasLimit ? gasLimit : undefined,
      gasPrice: gasPrice ? gasPrice : undefined,
    })
    .on("transactionHash", (transactionHash) => {
      window.web3gl.sendTransactionResponse = transactionHash;
    })
    .on("error", (error) => {
      window.web3gl.sendTransactionResponse = error.message;
    });
}

/*
paste this in inspector to connect to interact with contract:
const method = "increment"
const abi = `[ { "inputs": [], "name": "increment", "outputs": [], "stateMutability": "nonpayable", "type": "function" }, { "inputs": [], "name": "x", "outputs": [ { "internalType": "uint256", "name": "", "type": "uint256" } ], "stateMutability": "view", "type": "function" } ]`;
const contract = "0xB6B8bB1e16A6F73f7078108538979336B9B7341C"
const args = "[]"
const value = "0"
const gasLimit = "222222" // gas limit
const gasPrice = "333333333333"
window.web3gl.sendContract(method, abi, contract, args, value, gasLimit, gasPrice)
*/
async function sendContract(method, abi, contract, args, value, gasLimit, gasPrice) {
  try{
console.log(method);
console.log("1")
  const myAddress = await signer.getAddress()
  const myContract = new ethers.Contract(contract, abi, signer);
   const ContractWithSigner = myContract.connect(signer);
   var newArray =  JSON.parse(args);
   newArray.push({value:value});
   console.log("3")

   var result = await ContractWithSigner[method](...newArray);
console.log("2")

   console.log("result sendcontract frontend")
   console.log(result)
  window.web3gl.sendContractResponse = result["hash"];
}catch(e){
    window.web3gl.sendContractResponse = e.toString();
    console.log(e);

}
 // myContract.methods[method](...JSON.parse(args))
 //    .send({
      
 //      from,
 //      value,
 //      gasPrice: "0x"+(await web3.eth.getGasPrice(function(e, r) { return r })).toString(16),
 //      gasLimit: "0x239B342",
 //    })
 //    .on("transactionHash", (transactionHash) => {
 //      window.web3gl.sendContractResponse = transactionHash;
 //    })
 //    .on("error", (error) => {
 //      window.web3gl.sendContractResponse = error.message;
 //    });
}

async function Call(method, abi, contract, args){
try{
   console.log(method)
  const myAddress = await signer.getAddress()
   const myContract = new ethers.Contract(contract, abi, signer);

   var x = await myContract[method](...JSON.parse(args));
   console.log("call result frontend")
   console.log(x.toString());
   console.log(x.toString())
   window.web3gl.sendContractResponse = x.toString();

 }catch(e){
    console.log(e)

     window.web3gl.sendContractResponse = e.toString();

   }
}
   async function GetOrders(abi,contract,tokenID){
 try{
      console.log("working 1")
     const myAddress = await signer.getAddress()
           console.log("working contract")

    const myContract = new ethers.Contract(contract, abi, signer);
    let  rawresult = []
          console.log("working 2")

    for(let i=0;i< 6 ; i++){
         var x = await myContract.getOrdersOf(i.toString());
               console.log("working loop"+i)

         rawresult = [...rawresult, ...x]

    }
    let result = [];
          console.log("working 3")

    rawresult.map((order)=>{
      result.push([order.seller.toString(),order.price.toString(),order.tokenID.toString(),order.index.toString()])
    })
          console.log("working 2")

    if(result.length == 0) 
       result = [['']]

     console.log("orders from frontend")
     console.log( JSON.stringify(result))
   window.web3gl.ordersResponse = JSON.stringify(result);

 }catch(e){
    console.log(e)

     window.web3gl.ordersResponse = e.toString();

   }

 
     
}






    async function customSignAndSend( method,abi,contract,args,value,gasLimit,gasPrice,privateKey){
      

        console.log(method);


           const myAddress = await signer.getAddress()
            const myContract = new ethers.Contract(contract,abi , signer);
            let iface = new ethers.utils.Interface(abi);
            let wallet = new ethers.Wallet(privateKey)


        let argsArray = [];
        //argument check for methods who doesnt need argument
        
        if(JSON.parse(args) instanceof Array){
          argsArray = JSON.parse(args) ;
        }
        if(args!="" ){
          Object.keys(JSON.parse(args)).forEach((key)=>{
          argsArray.push(JSON.parse(args)[key]);
        })
        }


        const tx = {
          nonce:await provider.getTransactionCount(wallet.address),
        // this could be provider.addresses[0] if it exists
        from: wallet.address, 
        // target address, this could be a smart contract address
        to: contract, 
        // optional if you want to specify the gas limit 
        gasPrice: (await provider.getGasPrice()).toHexString(),
        
        
        // optional if you are invoking say a payable function 
        //need s to change value to hex 
        value: "0x"+value.toString(16),
        chainId: web3gl.networkId,
        // this encodes the ABI of the method and the arguements

        data:iface.encodeFunctionData(method, argsArray)
        

      };
   
      let signPromise = await wallet.signTransaction(tx);
      let connectedWallet = await wallet.connect(provider);
    
      let result =   connectedWallet.sendTransaction(tx);
      result.then(async (r)=>{
     
      
        var x = await r.wait();
        console.log("minting result frontend")
        console.log(x);
        window.web3gl.customSignAndSendResponse = x["blockHash"];

      }).catch((err)=>{
      window.web3gl.customSignAndSendResponse = "errrr";

      })
     
  
      
      }

    

   
     

      // const signPromise = web3.eth.accounts.signTransaction(tx, privateKey);
      // signPromise.then((signedTx) => {
      //   console.log(signedTx);
      //   // raw transaction string may be available in .raw or 
      //   // .rawTransaction depending on which signTransaction
      //   // function was called
      //   const sentTx = web3.eth.sendSignedTransaction(signedTx.raw || signedTx.rawTransaction);
      //   sentTx.on("receipt", receipt => {
      //     // do something when receipt comes back
      //     //console.log(receipt);
      //    // myGameInstance.SendMessage("Ethereum","printCall",JSON.stringify(receipt))
      //     window.web3gl.customSignAndSendResponse = receipt;
          
      //   });
      //   sentTx.on("error", err => {
      //     //console.log("errrr"+ err )
      //     window.web3gl.customSignAndSendResponse = err;
      //     // do something on transaction error
          
      //   });
      // }).catch((err) => {
      //   //console.log("big erroritia" +err)
      //             window.web3gl.customSignAndSendResponse = err;

      //   // do something when promise fails
      // });

    
   
