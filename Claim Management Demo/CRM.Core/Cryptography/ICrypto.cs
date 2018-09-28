namespace CRM.Core.Cryptography
{
    public interface ICrypto
    {
        /// <summary>
        /// Encrypt the given string
        /// </summary>
        /// <param name="strToEncrypt"></param>
        /// <returns></returns>
        string Encrypt(string strToEncrypt) ;
        /// <summary>
        /// Decrypt the given encrypted text
        /// </summary>
        /// <param name="strToDecrypt"></param>
        /// <returns></returns>
        string Decrypt(string strToDecrypt);
    }
}
