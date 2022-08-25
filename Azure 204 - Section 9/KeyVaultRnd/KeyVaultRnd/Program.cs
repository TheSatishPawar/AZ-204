//Encryption/Decryption using keyvault

/* Azure - 
1. open Azure Active Directory
2. click on App registrations
3. click on New registration
4. Add application name and save
5. copy clientId and teneantId from overview screen
6. click on Certificates & secrets and add description and save
7. copy value for client secret
8. copy Vault URI from created key value overview (e.g. Resource group -> select Key Vault from list -> Overview)
9. copy keyName from Key vault keys (e.g. Resource group -> select Key Vault from list -> keys->Name);
10. provide access to key vault:
a) Resource group -> select Key Vault from list -> Access policies
b) click on Add access policy
c) For Key Permissions : Key Management Operations(Select Get),Cryptographic Operations(Select Encrypt and Decrypt
d) click on principal (None Selected)
e) search your Key vault and select
11. click on Add
12. click on save

*/
// install Azure.Security.KeyVault.Keys nuget package
// install Azure.Identity nuget package

using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using System.Text;

string teneantId = "--teneantId--";
string clinetId = "--clinetId--";
string clientSecret = "--clientSecret--";

ClientSecretCredential clientSecretCredential = new ClientSecretCredential(teneantId,clinetId,clientSecret);
string keyValutUri = "--keyValutUri--";
string keyName = "--keyName--";
string textToEncrypt = "satish pawar";

KeyClient keyClient = new KeyClient(new Uri(keyValutUri), clientSecretCredential);

var key = keyClient.GetKey(keyName);
var cryptoClient = new CryptographyClient(key.Value.Id, clientSecretCredential);

byte[] textToBytes = Encoding.UTF8.GetBytes(textToEncrypt);

EncryptResult result = cryptoClient.Encrypt(EncryptionAlgorithm.RsaOaep, textToBytes);

Console.WriteLine("Encrypted string");
Console.WriteLine(Convert.ToBase64String(result.Ciphertext));


//Decryption

byte[] ciperToBytes = result.Ciphertext;
DecryptResult decryptResult = cryptoClient.Decrypt(EncryptionAlgorithm.RsaOaep, ciperToBytes);

Console.WriteLine("Decrypted string");
Console.WriteLine(Encoding.UTF8.GetString(decryptResult.Plaintext));
