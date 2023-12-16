using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KorYmeLibrary
{
    public static class EncryptionUtilities
    {
        const string ENCRYPTION_STRING = "Yq3t6w9z$C&F)J@NcRfUjWnZr4u7x!A%D*G-KaPdSgVkYp2s5v8y/B?E(H+MbQeThWmZq4t6w9z$C&F)J@NcRfUjXn2r5u8x!A%D*G-KaPdSgVkYp3s6v9y$B?E(H+MbQeThWmZq4t7w!z%C*F)J@NcRfUjXn2r5u8x/A?D(G+KaPdSgVkYp3s6v9y$B&E)H@McQeThWmZq4t7w!z%C*F-JaNdRgUjXn2r5u8x/A?D(G+KbPeShVmYp3s6v9y$B&E)H@McQfTjWnZr4t7w!z%C*F-JaNdRgUkXp2s5v8x/A?D(G+KbPeShVmYq3t6w9z$B&E)H@McQfTjWnZr4u7x!A%D*F-JaNdRgUkXp2s5v8y/B?E(H+KbPeShVmYq3t6w9z$C&F)J@NcQfTjWnZr4u7x!A%D*G-KaPdSgUkXp2s5v8y/B?E(H+MbQeThWmYq3t6w9z$C&F)J@NcRfUjXn2r4u7x!A%D*G-KaPdSgVkYp3s6v8y/B?E(H+MbQeThW";

        //A mettre dans la class Utility ou dans une nouvelle classe
        public enum EncryptionType
        {
            None,
            XOR,
        }

        public static string Encrypt(string data, EncryptionType encryptionType, bool isEncrypting, string encrytionString = "")
        {
            if (encrytionString == "")
            {
                encrytionString = ENCRYPTION_STRING;
            }
            switch (encryptionType)
            {
                case EncryptionType.None:
                    return data;
                case EncryptionType.XOR:
                    return XOREncrypting(data, encrytionString);
                default:
                    return "";
            }
        }

        public static string XOREncrypting(string data, string encrytionString)
        {
            string modifiedData = "";
            for (int i = 0; i < data.Length; i++)
            {
                modifiedData += (char)(data[i] ^ encrytionString[i % encrytionString.Length]);
            }
            return modifiedData;
        }
    }
}